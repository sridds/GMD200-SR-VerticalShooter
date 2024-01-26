using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

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

        public MoveMode Mode;

        [AllowNesting]
        [ShowIf(nameof(Mode), MoveMode.Steps)]
        public Vector2 MoveSteps;

        [AllowNesting]
        [ShowIf(nameof(Mode), MoveMode.ToPoint)]
        public Vector2 Point;

        public float MoveTime;
        public float Delay;
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
        Debug.Log("filling");
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
            Vector2 target = node.Mode == MoveNode.MoveMode.Steps ? initial + node.MoveSteps : node.Point;

            while (elapsed < node.MoveTime)
            {
                // lerp position over time
                transform.position = Vector2.Lerp(initial, target, elapsed / node.MoveTime);

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
