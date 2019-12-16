using FlatRedBall.Gui;
using StoryNavigator.GumRuntimes.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryNavigator.UtilityClasses
{
    public class Finder
    {
        public List<NodeDisplayRuntime> AllNodes { get; private set; }
        
        public Finder(List<NodeDisplayRuntime> nodeList)
        {
            AllNodes = nodeList;
        }

        public NodeDisplayRuntime GetNodeCursorIsCurrentlyOver(FlatRedBall.Gui.Cursor cursor)
        {
            NodeDisplayRuntime returnNode = null;
            foreach (var node in AllNodes)
            {
                if (cursor.IsOnWindowOrFloatingChildren(node as IWindow))
                    returnNode = node;
            }
            return returnNode;
        }

        public NodeDisplayRuntime GetNodeByPid(int pid)
        {
            NodeDisplayRuntime returnNode = AllNodes.SingleOrDefault(n => n.NodePassage.pid == pid);
            return returnNode;
        }

        public List<NodeDisplayRuntime> GetNodesWithLinksToNode(NodeDisplayRuntime node)
        {
            return GetNodesWithLinksToNodeByPid(node.NodePassage.pid);
        }

        public List<NodeDisplayRuntime> GetNodesWithLinksToNodeByPid(int pid)
        {
            var returnNodes = AllNodes.Where(n => n.Pid != pid && n.NodeLinks.Any(nl => nl.IsSuccessfulLinkToOtherNode && nl.PassageLink.pid == pid)).ToList();
            return returnNodes;
        }

        public List<NodeLinkRuntime> GetLinksToThisNode(NodeDisplayRuntime node)
        {
            return GetLinksToThisNodePid(node.Pid);
        }

        public List<NodeLinkRuntime> GetLinksToThisNodePid(int pid)
        {
            var linksFound = AllNodes.Where(n => n.Pid != pid).SelectMany(n => n.NodeLinks.Where(l => l.IsSuccessfulLinkToOtherNode && l.LinkedPid == pid)).ToList();
            return linksFound;
        }

    }
}
