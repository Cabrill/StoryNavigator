using FlatRedBall.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using static StoryNavigator.DataTypes.DialogTreeRaw;

namespace StoryNavigator.GumRuntimes.Nodes
{
    public partial class NodeDisplayRuntime
    {
        public Passage NodePassage { get; protected set; }
        partial void CustomInitialize () 
        {
            
        }

        public void SetPassage(Passage nodePassage)
        {
            this.NodePassage = nodePassage;
            X = nodePassage.position.x;
            Y = nodePassage.position.y;
            NodeInfoInstance.PassageNameText = nodePassage.name;
            NodeInfoInstance.PassagePidText = nodePassage.pid.ToString();

            //TODO:  
            //Add links to NodeDisplayRunTime, and update them here
        }

        public void UpdatePassageFromDisplay()
        {
            NodePassage.position.x = (int)X;
            NodePassage.position.y = (int)Y;
            NodePassage.name = NodeInfoInstance.PassageNameText;

            if (NodeInfoInstance.PassagePidText.IsNumeric())
            {
                NodePassage.pid = int.Parse(NodeInfoInstance.PassagePidText);
            }
            else
            {
                NodeInfoInstance.PassagePidText = NodePassage.pid.ToString();
            }
        }

        public void AssignPosition(Position pos)
        {
            NodePassage.position.x = pos.x;
            NodePassage.position.y = pos.y;

            UpdatePassageFromDisplay();
        }
    }
}
