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
using static StoryNavigator.DataTypes.DialogTreeRaw;
using StoryNavigator.Entities;

namespace StoryNavigator.Screens
{
    public partial class StoryNodeNavigator
    {
#if DEBUG
        private StringBuilder debugStringBuilder = new StringBuilder();
#endif

        #region Properties
        //Screen-specific properties
        private bool nodeIsGrabbed = false;
        private float incrementalNodeZ = 0f;

        //Story data properties
        private StoryData currentStoryData;
        private bool isStoryLoaded => currentStoryData != null;

        //Node properties
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
            CreateNodeDisplayForEachStoryPassage();
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
            TopMenuBar.AddMenuItem("+Node", HandleAddNodeClicked);
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
        private void HandleAddNodeClicked(IWindow window)
        {
            var newPassage = currentStoryData.AddNewPassage();
            CreateNewNodeDisplayForPassage(newPassage);
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
        }

        private void CreateNewStory()
        {
            DestroyAllNodes();

            currentStoryData = new StoryData();
            //Create initial passage
            currentStoryData.AddNewPassage();
        }

        private void CreateNodeDisplayForEachStoryPassage()
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

        #region Story node display & interaction
        
        private void CreateNewNodeDisplayForPassage(Passage passage)
        {
            var newNode = new NodeDisplayRuntime();
            newNode.SetPassage(passage);
            NodeDisplays.Add(newNode);
            newNode.AddToManagers();
            newNode.MoveToFrbLayer(NodeLayer, NodeLayerGum);
            newNode.AssignPosition(GetSuitablePositionForNewNode());
        }

        private Position GetSuitablePositionForNewNode()
        {
            var pos = new Position();
            pos.x = 300;
            pos.y = 300;

            //TODO:  Algorithm to find space for node on current display
            //given current camera/nodes, and return best position
            //TODO+ handle camera move/zoom if no space present

            return pos;
        }

        #endregion

        #region Activity
        void CustomActivity(bool firstTimeCalled)
        {
            HandleInputActivity();
#if !DEBUG
        }
            
#else 
            PerformDebugActivity();
        }

        private void PerformDebugActivity()
        {
            if (DebugVariables.DisplayDebuggingConsole)
            {
                FlatRedBall.Debugging.Debugger.Write(debugStringBuilder.ToString());
            }
            debugStringBuilder.Clear();
            debugStringBuilder.AppendLine("\n\n\n");
        }
#endif

        private void HandleInputActivity()
        {
            var cursor = GuiManager.Cursor;

#if DEBUG
            if (cursor.ScreenXChange > 0 || cursor.ScreenYChange > 0)
            {
                //But these are never greater than zero, why?
                int m = 3;
            }


            if (nodeIsGrabbed && currentDraggedNode is IWindow nodeWindow)
            {
                //This is is hit
                nodeWindow.X += cursor.ScreenXChange;
                nodeWindow.Y += cursor.ScreenYChange;
                debugStringBuilder.Append($"NodeX: {nodeWindow.X}\nNodeY:{nodeWindow.Y}");
            }

            if (cursor.PrimaryPush && currentDraggedNode == null && cursor.WindowOver is NodeDisplayRuntime nodeDisplay)
            {
                nodeIsGrabbed = true;
                currentDraggedNode = nodeDisplay;
                currentDraggedNode.Z = incrementalNodeZ;
                //This should ensure objects are always drawn in the order they were last interacted with
                //...but it doesn't :(
                //They are drawn to the NodeLayer, which is set to order by Z, but setting Z on our
                //NodeDisplayGumRuntime instances does not change their draw order
                incrementalNodeZ = incrementalNodeZ + float.MinValue;
            }
            else if (!cursor.PrimaryButton.IsDown)
            {
                nodeIsGrabbed = false;
                currentDraggedNode = null;
            }
#endif
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
