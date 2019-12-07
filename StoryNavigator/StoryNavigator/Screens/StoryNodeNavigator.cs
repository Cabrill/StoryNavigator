using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Localization;
using StoryNavigator.GumRuntimes.Nodes;
using FlatRedBall.Gui;

namespace StoryNavigator.Screens
{
    public partial class StoryNodeNavigator
    {
        private bool isGrabbed = false;
        NodeDisplayRuntime currentNode;

        void CustomInitialize()
        {
            currentNode = new NodeDisplayRuntime();
            currentNode.AddToManagers();
            currentNode.MoveToFrbLayer(NodeLayer, NodeLayerGum);
            currentNode.X = 200;
            currentNode.Y = 300;
        }

        void CustomActivity(bool firstTimeCalled)
        {
            var cursor = GuiManager.Cursor;
            if (isGrabbed)
            {
                this.currentNode.X += cursor.ScreenXChange;
                this.currentNode.Y += cursor.ScreenYChange;
            }
            if (cursor.PrimaryPush && cursor.WindowOver == currentNode)
            {
                isGrabbed = true;
            }

            if (cursor.PrimaryClick)
            {
                isGrabbed = false;
            }
        }

        void CustomDestroy()
        {


        }

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

    }
}
