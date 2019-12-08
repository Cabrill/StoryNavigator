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
using StoryNavigator.DataTypes;

namespace StoryNavigator.Screens
{
    public partial class StoryNodeNavigator
    {
        #region Properties
        private bool nodeIsGrabbed = false;
        private StoryData currentStoryData;
        private bool isStoryLoaded => currentStoryData != null;

        private List<NodeDisplayRuntime> NodeDisplays = new List<NodeDisplayRuntime>();
        NodeDisplayRuntime currentDraggedNode;
        #endregion

        #region Initialize
        void CustomInitialize()
        {
            InitializeCamera();
            AttemptLoadLastSavedStoryOrCreateNew();
            CreateNodesForStoryPassages();


        }
        private void InitializeCamera()
        {
            //TODO
        }

        #endregion

        #region Story Create/Load/Save
        private void AttemptLoadLastSavedStoryOrCreateNew()
        {
            //TODO
            //LoadStoryLocally();
            if (!isStoryLoaded)
            {
                CreateNewStory();
            }
            CreateNodesForStoryPassages();
        }

        private void CreateNewStory()
        {
            currentStoryData = new StoryData();
            //Create initial passage
            currentStoryData.AddNewPassage();
        }



        private void CreateNodesForStoryPassages()
        {
            foreach (var passage in currentStoryData.Passages)
            {
                var newNode = new NodeDisplayRuntime();
                newNode.SetPassage(passage);
                NodeDisplays.Add(newNode);
                newNode.AddToManagers();
                newNode.MoveToFrbLayer(NodeLayer, NodeLayerGum);
            }
        }

        #endregion

        #region Activity
        void CustomActivity(bool firstTimeCalled)
        {
            var cursor = GuiManager.Cursor;
            if (nodeIsGrabbed)
            {
                currentDraggedNode.X += cursor.ScreenXChange;
                currentDraggedNode.Y += cursor.ScreenYChange;
            }

            if (cursor.PrimaryPush && currentDraggedNode == null && cursor.WindowOver is NodeDisplayRuntime nodeDisplay)
            {
                nodeIsGrabbed = true;
                currentDraggedNode = nodeDisplay;
            }
            else if (!cursor.PrimaryPush)
            {
                nodeIsGrabbed = false;
                currentDraggedNode = null;
            }
        }

        #endregion

        #region Destroy/Unload
        void CustomDestroy()
        {
            

        }

        #endregion

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

    }
}
