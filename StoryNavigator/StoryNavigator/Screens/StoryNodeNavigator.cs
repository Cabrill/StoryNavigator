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
            InitializeTopMenu();
            AttemptLoadLastSavedStoryOrCreateNew();

            //Last step
            CreateNodesForStoryPassages();
        }
        private void InitializeCamera()
        {
            //TODO: Load zoom level, and camera X/Y value from last save
            //Set min/max camera levels and tie camera to 
            //cursor movement on edge of screen
            //and setup middle mouse wheel roll to zoom in/out
        }

        private void InitializeTopMenu()
        {
            TopMenuBar.AddMenuItem("File", HandleFileMenuItemClicked);
            //TODO: More menu buttons
            //Current project settings, view, search, file and replace, node options, etc.

            TopMenuBar.AddMenuItem("Exit", (IWindow notUsed) => { FlatRedBallServices.Game.Exit(); }, shouldRightAlign:true);
        }

        #endregion

        #region Menu Interaction
        private void HandleFileMenuItemClicked(IWindow window)
        {
            //Todo:  Open a vertical menu of New/Load/Save
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
            DestroyAllNodes();

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
            
            if (nodeIsGrabbed && currentDraggedNode is IWindow nodeWindow)
            {
                //This is is hit
                nodeWindow.X += cursor.ScreenXChange;
                nodeWindow.Y += cursor.ScreenYChange;
                
                if (cursor.ScreenXChange != 0 || cursor.ScreenYChange != 0)
                {
                    //But these are never greater than zero, why?
                    int m = 3;
                }
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

        private void DestroyAllNodes()
        {
            var nodeCount = NodeDisplays?.Count();
            if (nodeCount > 0)
            {
                for (var i = 0; i < nodeCount; i++)
                {
                    NodeDisplays[i].RemoveFromManagers();
                    NodeDisplays[i].Destroy();
                }
            }
            
        }
        void CustomDestroy()
        {
            DestroyAllNodes();
            TopMenuBar.ClearItems();
        }

        #endregion

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

    }
}
