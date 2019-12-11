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
using StoryNavigator.GumRuntimes.DefaultForms;

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
        private bool linkIsGrabbed = false;
        private float incrementalZ = 0f;

        //Story data properties
        private StoryData currentStoryData;
        private bool isStoryLoaded => currentStoryData != null;

        //Node properties
        private List<NodeDisplayRuntime> NodeDisplays = new List<NodeDisplayRuntime>();
        NodeDisplayRuntime currentDraggedNode;
        ButtonRuntime currentDraggedLink;
        #endregion

        #region Initialize
        void CustomInitialize()
        {
            NodeDisplayRuntime.FrbLayer = NodeLayer;
            NodeDisplayRuntime.GumLayer = NodeLayerGum;

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
            currentDraggedNode?.RespondToLosingActiveStatus();
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
        
        private NodeDisplayRuntime CreateNewNodeDisplayForPassage(Passage passage)
        {
            var newNode = new NodeDisplayRuntime();
            newNode.SetPassage(passage);
            NodeDisplays.Add(newNode);
            newNode.AddToManagers();
            newNode.MoveToFrbLayer(NodeLayer, NodeLayerGum);
            newNode.AssignPosition(GetSuitablePositionForNewNode());

            return newNode;
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
            HandleNodeDraggingActivity();
            HandleLinkDraggingActivity();
            HandleMouseWheelCameraZoomActivity();
        }

        private void HandleNodeDraggingActivity()
        {
            var cursor = GuiManager.Cursor;

            debugStringBuilder.AppendLine(cursor.WindowOver?.ToString());

            if (nodeIsGrabbed && currentDraggedNode is IWindow nodeWindow)
            {
                nodeWindow.X += cursor.ScreenXChange;
                nodeWindow.Y += cursor.ScreenYChange;
            }
            if (cursor.PrimaryPush && !nodeIsGrabbed && cursor.WindowOver is NodeDisplayRuntime nodeDisplay)
            {
                currentDraggedNode?.RespondToLosingActiveStatus();
                nodeIsGrabbed = true;
                currentDraggedNode = nodeDisplay;
                currentDraggedNode.Z = incrementalZ;

                currentDraggedNode.HandleBeingDragged();

                //TODO
                //This should ensure objects are always drawn in the order they were last interacted with
                //...but it doesn't :(
                //They are drawn to the NodeLayer, which is set to order by Z, but setting Z on our
                //NodeDisplayGumRuntime instances does not change their draw order
                incrementalZ += float.MinValue;
            }
            else if (!cursor.PrimaryButton.IsDown && nodeIsGrabbed == true)
            {
                currentDraggedNode?.HandleDraggingStopped();
                nodeIsGrabbed = false;
            }
        }

        private void HandleLinkDraggingActivity()
        {
            var cursor = GuiManager.Cursor;

            if (linkIsGrabbed && currentDraggedLink is IWindow nodeLinkAsIWindow)
            {
                nodeLinkAsIWindow.X += cursor.ScreenXChange;
                nodeLinkAsIWindow.Y += cursor.ScreenYChange;
            }

            if (cursor.PrimaryPush && !linkIsGrabbed && cursor.WindowOver is ButtonRuntime newLinkButton && newLinkButton.Parent is NodeLinkRuntime newLinkDisplay)
            {
                //currentDraggedLink?.RespondToLosingActiveStatus();
                linkIsGrabbed = true;
                currentDraggedLink = newLinkButton;
                currentDraggedLink.Z = incrementalZ;

                newLinkDisplay.HandleBeingDragged();
            }
            else if (!cursor.PrimaryButton.IsDown && linkIsGrabbed)
            {
                NodeDisplayRuntime nodeLinkIsOver = null;
                foreach (var node in NodeDisplays)
                {
                    if (cursor.IsOnWindowOrFloatingChildren(node as IWindow))
                        nodeLinkIsOver = node;
                }
                if (nodeLinkIsOver != null && currentDraggedLink.Parent is NodeLinkRuntime linkingNode)
                {
                    var newLink = new DialogTreeRaw.Link();
                    newLink.pid = nodeLinkIsOver.NodePassage.pid;
                    newLink.name = "Link name";
                    newLink.link = "Link text";
                    linkingNode.SetPassageLink(newLink);
                }
                //currentDraggedLink?.HandleDraggingStopped();
                linkIsGrabbed = false;
            }
        }

        private void HandleMouseWheelCameraZoomActivity()
        {
            //TODO:
            //Trying to zoom the nodes when middle-mouse is used
            if (InputManager.Mouse.ScrollWheel.Velocity != 0)
            {
                var gumCamera = RenderingLibrary.SystemManagers.Default.Renderer.Camera;
                gumCamera.Zoom += InputManager.Mouse.ScrollWheel.Velocity;

                //This just makes the screen go black
                //RenderingLibrary.SystemManagers.Default.Renderer.Camera.Zoom += InputManager.Mouse.ScrollWheel.Value;
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
