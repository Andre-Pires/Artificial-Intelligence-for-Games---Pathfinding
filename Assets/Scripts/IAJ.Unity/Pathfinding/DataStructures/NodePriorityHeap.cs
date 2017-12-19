using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Utils;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class NodePriorityHeap : IOpenSet
    {
        PriorityHeap<NodeRecord> OpenHeap { get; set; }

        public NodePriorityHeap()
        {
            this.OpenHeap = new PriorityHeap<NodeRecord>();
        }
 
        public void Initialize()
        {
            this.OpenHeap.Clear();
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            this.OpenHeap.Remove(nodeToBeReplaced);
            this.OpenHeap.Enqueue(nodeToReplace);
        }

        public NodeRecord GetBestAndRemove()
        {
            return this.OpenHeap.Dequeue();
        }

        public NodeRecord PeekBest()
        {
            return this.OpenHeap.Peek();
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            this.OpenHeap.Enqueue(nodeRecord);
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            this.OpenHeap.Remove(nodeRecord);
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
            return this.OpenHeap.SearchForEqual(nodeRecord);
        }

        public ICollection<NodeRecord> All()
        {
            return this.OpenHeap;
        }

        public int CountOpen()
        {
            return this.OpenHeap.Count;
        }
    }
}
