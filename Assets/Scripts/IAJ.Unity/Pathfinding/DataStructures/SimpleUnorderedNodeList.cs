﻿using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    //very simple (and unefficient) implementation of the open/closed sets
    public class SimpleUnorderedNodeList : IOpenSet, IClosedSet
    {
        private List<NodeRecord> NodeRecords { get; set; }

        public SimpleUnorderedNodeList()
        {
            this.NodeRecords = new List<NodeRecord>();
        }

        public void Initialize()
        {
            this.NodeRecords.Clear(); 
        }

        public int CountOpen()
        {
            return this.NodeRecords.Count;
        }

        public void AddToClosed(NodeRecord nodeRecord)
        {
            this.NodeRecords.Add(nodeRecord);
        }

        public void RemoveFromClosed(NodeRecord nodeRecord)
        {
            this.NodeRecords.Remove(nodeRecord);
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            //here I cannot use the == comparer because the nodeRecord will likely be a different computational object
            //and therefore pointer comparison will not work, we need to use Equals
            //LINQ with a lambda expression
            return this.NodeRecords.FirstOrDefault(n => n.Equals(nodeRecord));
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            this.NodeRecords.Add(nodeRecord);
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            this.NodeRecords.Remove(nodeRecord);
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
            //here I cannot use the == comparer because the nodeRecord will likely be a different computational object
            //and therefore pointer comparison will not work, we need to use Equals
            //LINQ with a lambda expression
            return this.NodeRecords.FirstOrDefault(n => n.Equals(nodeRecord));
        }

        public ICollection<NodeRecord> All()
        {
            return this.NodeRecords;
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            //since the list is not ordered we do not need to remove the node and add the new one, just copy the different values
            //remember that if NodeRecord is a struct, for this to work we need to receive a reference
            nodeToBeReplaced.parent = nodeToReplace.parent;
            nodeToBeReplaced.fValue = nodeToReplace.fValue;
            nodeToBeReplaced.gValue = nodeToReplace.gValue;
            nodeToBeReplaced.hValue = nodeToReplace.hValue;
        }

        public NodeRecord GetBestAndRemove()
        {
            var best = this.PeekBest();
            this.NodeRecords.Remove(best);
            return best;
        }

        public NodeRecord PeekBest()
        {
            //welcome to LINQ guys, for those of you that remember LISP from the AI course, the LINQ Aggregate method is the same as lisp's Reduce method
            //so here I'm just using a lambda that compares the first element with the second and returns the lowest
            //by applying this to the whole list, I'm returning the node with the lowest F value.
            return this.NodeRecords.Aggregate((nodeRecord1, nodeRecord2) => nodeRecord1.fValue < nodeRecord2.fValue ? nodeRecord1 : nodeRecord2);
        }
    }
}
