using RAIN.Navigation.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class NodeRecordArray : IOpenSet, IClosedSet
    {
        private NodeRecord[] NodeRecords { get; set; }
        private List<NodeRecord> SpecialCaseNodes { get; set; }
        private NodePriorityHeap Open { get; set; }

        public NodeRecordArray(List<NavigationGraphNode> nodes)
        {
            //this method creates and initializes the NodeRecordArray for all nodes in the Navigation Graph
            this.NodeRecords = new NodeRecord[nodes.Count];

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                node.NodeIndex = i; //we're setting the node Index because RAIN does not do this automatically
                this.NodeRecords[i] = new NodeRecord { node = node, status = NodeStatus.Unvisited };
            }

            this.SpecialCaseNodes = new List<NodeRecord>();

            this.Open = new NodePriorityHeap();
        }

        public NodeRecord GetNodeRecord(NavigationGraphNode node)
        {
            //do not change this method
            //here we have the "special case" node handling
            if (node.NodeIndex == -1)
            {
                for (int i = 0; i < this.SpecialCaseNodes.Count; i++)
                {
                    if (node == this.SpecialCaseNodes[i].node)
                    {
                        return this.SpecialCaseNodes[i];
                    }
                }
                return null;
            }
            else
            {
                return this.NodeRecords[node.NodeIndex];
            }
        }

        public void AddSpecialCaseNode(NodeRecord node)
        {
            this.SpecialCaseNodes.Add(node);
        }

        void IOpenSet.Initialize()
        {
            this.Open.Initialize();
            //we want this to be very efficient (that's why we use for)
            for (int i = 0; i < this.NodeRecords.Length; i++)
            {
                this.NodeRecords[i].status = NodeStatus.Unvisited;
            }

            this.SpecialCaseNodes.Clear();
        }

        void IClosedSet.Initialize()
        {
            // not needed
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            Open.AddToOpen(nodeRecord);

            NodeRecords[nodeRecord.node.NodeIndex].status = NodeStatus.Open;
        }

        public void AddToClosed(NodeRecord nodeRecord)
        {
            NodeRecords[nodeRecord.node.NodeIndex].status = NodeStatus.Closed;
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
            if (NodeRecords[nodeRecord.node.NodeIndex].status == NodeStatus.Open)
            {
                return NodeRecords[nodeRecord.node.NodeIndex];
            }
            else
            {
                return null;
            }
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            if (NodeRecords[nodeRecord.node.NodeIndex].status == NodeStatus.Closed)
            {
                return NodeRecords[nodeRecord.node.NodeIndex];
            }
            else
            {
                return null;
            }
        }

        public NodeRecord GetBestAndRemove()
        {
            NodeRecord best = Open.GetBestAndRemove();
            NodeRecords[best.node.NodeIndex].status = NodeStatus.Closed;
            return best;
        }

        public NodeRecord PeekBest()
        {
            return Open.PeekBest();
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            Open.Replace(nodeToBeReplaced, nodeToReplace);
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            Open.RemoveFromOpen(nodeRecord);
            NodeRecords[nodeRecord.node.NodeIndex].status = NodeStatus.Unvisited;
        }

        public void RemoveFromClosed(NodeRecord nodeRecord)
        {
            NodeRecords[nodeRecord.node.NodeIndex].status = NodeStatus.Unvisited;
        }

        ICollection<NodeRecord> IOpenSet.All()
        {
            return Open.All();
        }

        ICollection<NodeRecord> IClosedSet.All()
        {
            //return NodeRecords?.Where(nodeRecord => nodeRecord.status.Equals(NodeStatus.Closed)) as ICollection<NodeRecord>;

            List<NodeRecord> closedRecords = new List<NodeRecord>();

            for (int record = 0; record < NodeRecords.Length; record++)
            {
                if (NodeRecords[record].status == NodeStatus.Closed)
                {
                    closedRecords.Add(NodeRecords[record]);
                }
            }

            return closedRecords;
        }

        public int CountOpen()
        {
            return Open.CountOpen();
        }
    }
}