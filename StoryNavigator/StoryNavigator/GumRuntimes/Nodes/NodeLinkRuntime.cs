using System;
using System.Collections.Generic;
using System.Linq;
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
            XPriorToDrag = X;
            YPriorToDrag = Y;
            ZPriorToDrag = Z;
            //Parent.Children.Remove(this);
        }

        private void ReLinkToFormerParent()
        {
            ResetPositionToParent();
            //_parentRunTime.Children.Add(this);
            //Parent.Children.Add(this);
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

        private void ResetPositionToParent()
        {
            X = XPriorToDrag;
            Y = YPriorToDrag;
            Z = ZPriorToDrag;
        }
    }
}
