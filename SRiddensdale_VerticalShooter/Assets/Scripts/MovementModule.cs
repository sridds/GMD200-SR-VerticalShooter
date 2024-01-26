using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class MovementModule : MonoBehaviour
{
    [System.Serializable]
    public struct MoveNode
    {
        public enum MoveMode
        {
            Steps,
            ToPoint
        }

        public float Delay;
        public MoveMode Mode;

        [AllowNesting]
        [ShowIf(nameof(Mode), MoveMode.Steps)]
        public Vector2 MoveSteps;

        [AllowNesting]
        [ShowIf(nameof(Mode), MoveMode.ToPoint)]
        public Vector2 Point;

        public float MoveTime;
        public UnityEvent OnReachNode;
    }

    [SerializeField]
    private MoveNode[] _moveNodes;

    [SerializeField]
    private bool _chooseRandomNodes;

    // different movement types - node based
    private Queue<MoveNode> moveQueue = new Queue<MoveNode>();

    private void Start() => FillQueue();

    private void Update()
    {
        // refill
        if (moveQueue.Count == 0) FillQueue();
    }

    /// <summary>
    /// Fill up the queue either randomly or sequentially
    /// </summary>
    private void FillQueue()
    {
        if(_moveNodes.Length == 0) {
            Debug.LogWarning("Could not execute move nodes. No movement nodes added");
            return;
        }

        // fill queue randomly
        if (_chooseRandomNodes) {
            MoveNode[] moveNodes = _moveNodes;

            int count = moveNodes.Length;
            int last = count - 1;

            // randomize list
            for(int i = 0; i < last; ++i) {
                int r = Random.Range(i, count);
                MoveNode temp = moveNodes[i];
                moveNodes[i] = moveNodes[r];
                moveNodes[r] = temp;
            }
        }

        // fill queue
        foreach (MoveNode node in _moveNodes) {
            moveQueue.Enqueue(node);
        }

        StartCoroutine(HandleQueue());
    }

    private System.Collections.IEnumerator HandleQueue()
    {
        // loop through each node
        int count = moveQueue.Count;
        for(int i = 0; i < count; i++)
        {
            // dequeue the node
            MoveNode node = moveQueue.Dequeue();

            // wait for delay (if there is delay)
            yield return new WaitForSeconds(node.Delay);

            float elapsed = 0.0f;
            Vector2 initial = transform.position;
            Vector2 target = GetTarget(node);

            while (elapsed < node.MoveTime)
            {
                // lerp position over time
                transform.position = Vector2.Lerp(initial, target, elapsed / node.MoveTime);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // set to target position
            transform.position = target;
            node.OnReachNode?.Invoke();
        }
    }

    private Vector2 GetTarget(MoveNode node)
    {
        Vector2 target = target = node.Mode == MoveNode.MoveMode.Steps ? new Vector2(transform.position.x, transform.position.y) + node.MoveSteps : node.Point;

        return target;
    }
}
