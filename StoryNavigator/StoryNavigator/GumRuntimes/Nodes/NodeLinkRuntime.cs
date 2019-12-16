using FlatRedBall.Math.Splines;
using Gum.Wireframe;
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
        //Used in detaching from parent display during drag event
        private float XPriorToDrag = 0f;
        private float YPriorToDrag = 0f;
        private float ZPriorToDrag = 0f;
        private NodeDisplayRuntime _parentRunTime;

        public Link PassageLink { get; protected set; }
        public NodeDisplayRuntime ParentNode => _parentRunTime;
        public GraphicalUiElement ParentContainer;

        public SplinePoint LinkSplineStartPosition {
            get 
            {
                //var thisAsGraphicalUiElement = this as GraphicalUiElement;
                var splinePoint = new SplinePoint();
                splinePoint.Position.X = WorldUnitX + Width;
                splinePoint.Position.Y = WorldUnitY - Height/2;
                return splinePoint;
            }
        
        }

        #endregion

        partial void CustomInitialize () 
        {

        }

        public void SetPassageLink(Link passageLink)
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
            UnlinkParent();
            CurrentConnectionStateState = ConnectionState.Dragged;
            Z = 5;
        }

        private void UnlinkParent()
        {
            _parentRunTime = this.Parent.Parent as NodeDisplayRuntime;
            XPriorToDrag = this.AbsoluteX;
            YPriorToDrag = this.AbsoluteY;
            ZPriorToDrag = this.Z;
            ParentContainer = Parent as GraphicalUiElement;
            Parent.Children.Remove(this);
            Parent = null;
            AddToManagers();
            ResetToAbsolutePositionPriorBeginDragging();
        }

        private void ReLinkToFormerParent()
        {
            RemoveFromManagers();
            _parentRunTime.Children.Add(this);
            ParentContainer.Children.Add(this);
            Parent = ParentContainer;
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        internal void HandleDraggingStopped()
        {
            if (_parentRunTime != null)
            {
                ReLinkToFormerParent();
            }
#if DEBUG
            else
            {
                throw new NullReferenceException($"{nameof(_parentRunTime)} is null");
            }
#endif
        }

        private void ResetToAbsolutePositionPriorBeginDragging()
        {
            X = XPriorToDrag;
            Y = YPriorToDrag;
            Z = ZPriorToDrag;
        }
    }
}
