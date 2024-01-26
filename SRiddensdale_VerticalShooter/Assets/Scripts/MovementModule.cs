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

        public float Speed;
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
    }
}
