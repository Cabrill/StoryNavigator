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
using FlatRedBall.Math.Splines;
using StoryNavigator.UtilityClasses;

namespace StoryNavigator.Screens
{
    public partial class StoryNodeNavigator
    {
#if DEBUG
        private StringBuilder debugStringBuilder = new StringBuilder();
#endif

        #region Properties
        //Screen-specific properties
        private bool nodeIsGrabbed => currentDraggedNode != null;
        private bool linkIsGrabbed => currentDraggedLink != null;
        private float incrementalZ = 0f;

        //Story data properties
        private string currentDataFileLocation = "StoryData.json";
        private StoryData currentStoryData;
        private bool isStoryLoaded => currentStoryData != null;

        //Node properties
        private List<NodeDisplayRuntime> NodeDisplays = new List<NodeDisplayRuntime>();
        NodeDisplayRuntime currentDraggedNode;
        NodeLinkRuntime currentDraggedLink;

        //Splines for links between nodes
        private List<Spline> SplinesForNodeLinks = new List<Spline>();

        List<global::RenderingLibrary.Math.Geometry.Line> LinesFromLinkToDestinationNodes = new List<RenderingLibrary.Math.Geometry.Line>();
        #endregion

        #region Initialize
        void CustomInitialize()
        {
            NodeDisplayRuntime.FrbLayer = NodeLayer;
            NodeDisplayRuntime.GumLayer = NodeLayerGum;

            InitializeCamera();
            InitializeTopMenu();
            AttemptLoadLastSavedStoryOrCreateNew();

            CreateNodeDisplayForEachStoryPassage();

            Container.Set(new Finder(NodeDisplays));
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
            //TopMenuBar.AddMenuItem("File", HandleFileMenuItemClicked);
            TopMenuBar.AddMenuItem("Save", HandleSaveMenuItemClicked);
            TopMenuBar.AddMenuItem("Load", HandleLoadMenuItemClicked);
            TopMenuBar.AddMenuItem("+Node", HandleAddNodeClicked);
            //TODO: More menu buttons
            //Current project settings, view, search, file and replace, node options, etc.

            TopMenuBar.AddMenuItem("Exit", (IWindow notUsed) => { FlatRedBallServices.Game.Exit(); }, shouldRightAlign: true);
        }

        #endregion

        #region Menu Interaction
        private void HandleFileMenuItemClicked(IWindow window)
        {
            //Todo:  Open a vertical menu of New/Load/Save
        }
        private void HandleSaveMenuItemClicked(IWindow window)
        {
            currentStoryData.SaveData(currentDataFileLocation);
        }

        private void HandleLoadMenuItemClicked(IWindow window)
        {
            DestroyAllNodes();
            currentStoryData.LoadData(currentDataFileLocation);
            CreateNodeDisplayForEachStoryPassage();
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
            DestroyAllNodes();
            currentStoryData = new StoryData();

            if (!currentStoryData.LoadData(currentDataFileLocation))
            {
                //New data, create initial node
                currentStoryData.AddNewPassage();
            }
        }

        private void CreateNodeDisplayForEachStoryPassage()
        {
            foreach (var passage in currentStoryData.Passages)
            {
                var newNode = new NodeDisplayRuntime();
                newNode.SetPassage(passage);
                newNode.AddToManagers();
                newNode.MoveToFrbLayer(NodeLayer, NodeLayerGum);

                NodeDisplays.Add(newNode);
            }
            foreach (var node in NodeDisplays)
            {
                foreach (var link in node.NodeLinks)
                {
                    if (link.IsSuccessfulLinkToOtherNode && link.LinkedNode == null)
                    {
                        NodeDisplayRuntime linkedNode = null;
                        try
                        {
                            
                            linkedNode = Container.Get<Finder>().GetNodeByPid(link.PassageLink.pid);
                            if (linkedNode == null) throw new NullReferenceException("Node with valid link can't find other node");

                        } catch (Exception e) {
#if DEBUG
                            //Failed to create a link to a node for a link determined valid
                            //This indicates corruption of the file because every
#endif
                        }
                        link.SetLinkedNode(linkedNode);
                    }
                }
            }
            CreateLinesForAllLinks();
        }

        private void CreateLinesForAllLinks()
        {
            foreach (var node in NodeDisplays)
            {
                foreach (var link in node.NodeLinks)
                {
                    if (link.CurrentConnectionStateState != NodeLinkRuntime.ConnectionState.Add)
                    {
                        //var newSpline = CreateASplineBetweenNodes(link, node);
                        var newLine = CreateALineBetweenNodes(link, node);
                        
                        //SplinesForNodeLinks.Add(newSpline);
                        LinesFromLinkToDestinationNodes.Add(newLine);
                    }
                }
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
            pos.x = 300 + (int)FlatRedBallServices.Random.Between(-100, 100);
            pos.y = 300 + (int)FlatRedBallServices.Random.Between(-100, 100);

            //TODO:  Algorithm to find space for node on current display
            //given current camera/nodes, and return best position
            //TODO+ handle camera move/zoom if no space present

            return pos;
        }

        //private Spline CreateASplineBetweenNodes(NodeLinkRuntime linkOrigin, NodeDisplayRuntime nodeDestination)
        //{
        //    var nodeLinkToNodeSpline = new Spline();

        //    nodeLinkToNodeSpline.Add(linkOrigin.LinkSplineStartPosition);
        //    nodeLinkToNodeSpline.Add(nodeDestination.NodeSplineEndPosition);
        //    nodeLinkToNodeSpline.CalculateVelocities();
        //    nodeLinkToNodeSpline.CalculateAccelerations();
        //    nodeLinkToNodeSpline.Visible = true;

        //    SplinesForNodeLinks.Add(nodeLinkToNodeSpline);

        //    return nodeLinkToNodeSpline;
        //}

        private RenderingLibrary.Math.Geometry.Line CreateALineBetweenNodes(NodeLinkRuntime linkOrigin, NodeDisplayRuntime nodeDestination)
        {
            var line = new RenderingLibrary.Math.Geometry.Line(RenderingLibrary.SystemManagers.Default);
            RenderingLibrary.SystemManagers.Default.ShapeManager.Add(line);
            line.X = linkOrigin.LinkSplineStartPosition.Position.X;
            line.Y = linkOrigin.LinkSplineStartPosition.Position.Y;

            var relativeDifference =
                nodeDestination.NodeSplineEndPosition.Position - linkOrigin.LinkSplineStartPosition.Position;
            line.RelativePoint.X = relativeDifference.X;
            line.RelativePoint.Y = relativeDifference.Y;

            linkOrigin.SetLine(line);

            return line;
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
            HandleMiddleMouseDragActivity();
            HandleMouseWheelCameraZoomActivity();
        }

        private void HandleMiddleMouseDragActivity()
        {
            var cursor = GuiManager.Cursor;
            if (!nodeIsGrabbed && !linkIsGrabbed && cursor.MiddleDown)
            {

                var gumCamera = RenderingLibrary.SystemManagers.Default.Renderer.Camera;
                gumCamera.X -= cursor.ScreenXChange;
                gumCamera.Y -= cursor.ScreenYChange;
                //Move the TopMenuBar so it stays at the top of the screen
                //TODO:  Find a way to attach TopMenuBar or GuiLayer to camera
                TopMenuBar.X -= cursor.ScreenXChange;
                TopMenuBar.Y -= cursor.ScreenYChange;

            }
            else
            {

            }
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
            if (cursor.PrimaryPush && !nodeIsGrabbed && cursor.WindowOver is NodeDisplayRuntime nodeDisplayWithCursorOverIt)
            {
                currentDraggedNode?.RespondToLosingActiveStatus();
                currentDraggedNode = nodeDisplayWithCursorOverIt;
                currentDraggedNode.Z = incrementalZ;

                currentDraggedNode.HandleBeingDragged();

                //TODO
                //This should ensure objects are always drawn in the order they were last interacted with
                //...but it doesn't :(
                //They are drawn to the NodeLayer, which is set to order by Z, but setting Z on our
                //NodeDisplayGumRuntime instances does not change their draw order
                incrementalZ += float.MinValue;
            }
            else if (cursor.PrimaryButton.IsDown && nodeIsGrabbed)
            {
                currentDraggedNode.HandleBeingDragged();
            }
            else if (!cursor.PrimaryButton.IsDown && nodeIsGrabbed == true)
            {
                currentDraggedNode?.HandleDraggingStopped();
                currentDraggedNode = null;
            }
        }

        #region Link drag/drop activity
        private void HandleLinkDraggingActivity()
        {
            var cursor = GuiManager.Cursor;

            if (linkIsGrabbed && currentDraggedLink is IWindow nodeLinkAsIWindow)
            {
                nodeLinkAsIWindow.X += cursor.ScreenXChange;
                nodeLinkAsIWindow.Y += cursor.ScreenYChange;
                var nodeCursorIsOver = Container.Get<Finder>().GetNodeCursorIsCurrentlyOver(cursor);
                if (nodeCursorIsOver != null)
                {
                    currentDraggedLink.CurrentDragStatusState = NodeLinkRuntime.DragStatus.DraggedOverValidNode;
                }
                else
                {
                    currentDraggedLink.CurrentDragStatusState = NodeLinkRuntime.DragStatus.Dragged;
                }
            }

            if (cursor.PrimaryDown && !linkIsGrabbed)
            {
                if (cursor.WindowOver is ButtonRuntime linkButton 
                && linkButton.Parent is NodeLinkRuntime linkDisplay)
                {
                    currentDraggedLink = linkDisplay;

                    linkDisplay.HandleBeingDragged();
                }
            }
            else if (!cursor.PrimaryButton.IsDown && linkIsGrabbed)
            {
                HandleDraggedLinkIsDropped();
            }
        }

        private void HandleDraggedLinkIsDropped()
        {
            currentDraggedLink.HandleDraggingStopped();

            var cursor = GuiManager.Cursor;
            var nodeLinkIsOver = Container.Get<Finder>().GetNodeCursorIsCurrentlyOver(cursor);

            if (nodeLinkIsOver != null)
            {
                currentDraggedLink.ParentNode?.HandleLinkEstablishedWithNode(nodeLinkIsOver, ref currentDraggedLink);
                var newLine =  CreateALineBetweenNodes(currentDraggedLink, nodeLinkIsOver);
                LinesFromLinkToDestinationNodes.Add(newLine);
            }
            else
            {
                currentDraggedLink.ParentNode.HandleFailedNodeLink(currentDraggedLink);
            }
            
            currentDraggedLink = null;
        }

        #endregion

        private void HandleMouseWheelCameraZoomActivity()
        {
            //TODO:
            //Trying to zoom the nodes when middle-mouse is used
            if (InputManager.Mouse.ScrollWheel.Velocity != 0)
            {
                
                var gumCamera = RenderingLibrary.SystemManagers.Default.Renderer.Camera;

                //This zooms ALL gum layers
                //gumCamera.Zoom += InputManager.Mouse.ScrollWheel.Velocity;

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
                    NodeDisplays[i].CustomDispose();
                    NodeDisplays[i].RemoveFromManagers();
                    NodeDisplays[i].Destroy();
                }
            }

            var splineCount = SplinesForNodeLinks?.Count();
            if (splineCount > 0)
            {
                for (var i = 0; i < splineCount; i++)
                {
                    //No need to remove?
                }
            }

            var lineCount = LinesFromLinkToDestinationNodes?.Count();
            if (lineCount > 0)
            {
                for (var i = 0; i < lineCount; i++)
                {
                    var line = LinesFromLinkToDestinationNodes[i];
                    RenderingLibrary.SystemManagers.Default.ShapeManager.Remove(line);
                    LinesFromLinkToDestinationNodes.Remove(line);
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
