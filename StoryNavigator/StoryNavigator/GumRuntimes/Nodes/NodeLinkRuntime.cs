using FlatRedBall.Gui;
using FlatRedBall.Math.Splines;
using Gum.Wireframe;
using RenderingLibrary;
using RenderingLibrary.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static StoryNavigator.DataTypes.DialogTreeRaw;

namespace StoryNavigator.GumRuntimes.Nodes
{
    public partial class NodeLinkRuntime
    {
        #region Properties
        public bool IsSuccessfulLinkToOtherNode => CurrentConnectionStateState != ConnectionState.Add && 
                                PassageLink != null &&
                                PassageLink.pid != 0 &&
                                PassageLink.pid != ParentNode.NodePassage.pid;

        public Line LineToDestinationNode { get; private set; }

        //Used in detaching from parent display during drag event
        private float XPriorToDrag = 0f;
        private float YPriorToDrag = 0f;
        private float ZPriorToDrag = 0f;
        private NodeDisplayRuntime _parentRunTime;
        private ContainerRuntime _parentContainer;
        public NodeDisplayRuntime ParentNode => _parentRunTime;
        private NodeDisplayRuntime _linkedNodeRuntime;
        public NodeDisplayRuntime LinkedNode => _linkedNodeRuntime;
        public int? LinkedPid => LinkedNode?.Pid;
        public Link PassageLink { get; protected set; }
        
        public GraphicalUiElement ParentContainer;

        public SplinePoint LinkSplineStartPosition {
            get 
            {
                var thisAsIpso = this as IPositionedSizedObject;
                var splinePoint = new SplinePoint();
                splinePoint.Position.X = WorldUnitX;
                splinePoint.Position.Y = WorldUnitY + thisAsIpso.Height / 2;
                if ((this.Parent as GraphicalUiElement).Visible)
                {
                    splinePoint.Position.X += thisAsIpso.Width;
                }
                return splinePoint;
            }
        
        }

        #endregion

        partial void CustomInitialize () 
        {

        }

        internal void SetPassageLink(Link passageLink)
        {
            PassageLink = passageLink;

            PassageLinkNameText = passageLink.name;
            LinkText = passageLink.link;
            PassageLinkNumberText = passageLink.pid.ToString();
            CurrentConnectionStateState = ConnectionState.DisplayExistingWithEdit;
        }


        public void UpdatePassageFromDisplay()
        {
            PassageLink.name = PassageLinkNameText;
            PassageLink.link = LinkText;

            if (PassageLinkNumberText.IsNumeric())
            {
                PassageLink.pid = int.Parse(PassageLinkNumberText);
            }
            else
            {
                PassageLinkNumberText = PassageLink.pid.ToString();
            }
        }

        internal void HandleBeingDragged()
        {
            RecordParent();
            CurrentDragStatusState = DragStatus.Dragged;
            Z = 5;
        }

        public void RecordParent()
        {
            if (Parent != null && Parent is ContainerRuntime nodeLinkContainer)
            {
                _parentContainer = nodeLinkContainer;
                if (Parent?.Parent != null && Parent.Parent is NodeDisplayRuntime nodeDisplay)
                {
                    _parentRunTime = nodeDisplay;
                }
            }
        }

        internal void HandleDraggingStopped()
        {
            CurrentDragStatusState = DragStatus.NotDragged;
            if (IsSuccessfulLinkToOtherNode)
            {
                CurrentConnectionStateState = ConnectionState.DisplayExistingWithEdit;
            }
            else
            {
                CurrentConnectionStateState = ConnectionState.Add;
            }

        }

        private void ResetToAbsolutePositionPriorBeginDragging()
        {
            X = XPriorToDrag;
            Y = YPriorToDrag;
            Z = ZPriorToDrag;
        }

        internal void SetLine(global::RenderingLibrary.Math.Geometry.Line line)
        {
            LineToDestinationNode = line;
        }

        internal void UpdateLine()
        {
            var currentLineOrigination = LinkSplineStartPosition;
            LineToDestinationNode.X = currentLineOrigination.Position.X;
            LineToDestinationNode.Y = currentLineOrigination.Position.Y;

            var currentLineDestination = LinkedNode.NodeSplineEndPosition;
            var relativeDifference = currentLineDestination.Position - currentLineOrigination.Position;
            LineToDestinationNode.RelativePoint.X = relativeDifference.X;
            LineToDestinationNode.RelativePoint.Y = relativeDifference.Y;
        }

        public void SetLinkedNode(NodeDisplayRuntime linkedNode)
        {
            _linkedNodeRuntime = linkedNode;
        }
    }
}
