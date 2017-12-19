using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public interface IOpenSet
    {
        void Initialize();
        void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace);
        NodeRecord GetBestAndRemove();
        NodeRecord PeekBest();
        void AddToOpen(NodeRecord nodeRecord);
        void RemoveFromOpen(NodeRecord nodeRecord);
        //should return null if the node is not found
        NodeRecord SearchInOpen(NodeRecord nodeRecord);
        ICollection<NodeRecord> All();
        int CountOpen();
    }
}
