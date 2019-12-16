using FlatRedBall.Gui;
using FlatRedBall.Math.Splines;
using Gum.Wireframe;
using StoryNavigator.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static StoryNavigator.DataTypes.DialogTreeRaw;

namespace StoryNavigator.GumRuntimes.Nodes
{
    public partial class NodeDisplayRuntime
    {
        public static FlatRedBall.Graphics.Layer FrbLayer;
        public static RenderingLibrary.Graphics.Layer GumLayer;

        public List<NodeLinkRuntime> NodeLinks = new List<NodeLinkRuntime>();

        public Passage NodePassage { get; protected set; }
        public SplinePoint NodeSplineEndPosition {
            get {
                //var thisAsGraphicalUiElement = this as GraphicalUiElement;
                var splinePoint = new SplinePoint();
                splinePoint.Position.X = WorldUnitX;
                splinePoint.Position.Y = WorldUnitY - Height/2;

                splinePoint.Time = 1;

                return splinePoint;
            }
        }

        partial void CustomInitialize () 
        {
            this.TextBoxInstance.CurrentLineModeCategoryState = DefaultForms.TextBoxRuntime.LineModeCategory.Multi;
            TextBoxInstance.FormsControl.TextWrapping = FlatRedBall.Forms.TextWrapping.Wrap;
            TextBoxInstance.FormsControl.TextChanged += FormsControl_TextChanged;
        }

        private void FormsControl_TextChanged(object sender, EventArgs e)
        {
            //TODO
            //if (validation) {
            this.NodePassage.text = TextBoxInstance.Text;
            //}
        }

        public void HandleBeingDragged()
        {
            NodeLinkContainer.Visible = false;
        }

        public void RespondToLosingActiveStatus()
        {
            foreach (var linkDisplay in NodeLinkContainer.Children)
            {
                if (linkDisplay is NodeLinkRuntime nodeLinkDisplay)
                {
                    if (nodeLinkDisplay.CurrentConnectionStateState == NodeLinkRuntime.ConnectionState.Add)
                    {
                        nodeLinkDisplay.Visible = false;
                    }
                    else
                    {
                        nodeLinkDisplay.CurrentConnectionStateState = NodeLinkRuntime.ConnectionState.DisplayExistingWithoutEdit;
                    }
                }
            }
        }

        public void HandleDraggingStopped()
        {
            foreach (var linkDisplay in NodeLinkContainer.Children)
            {
                if (linkDisplay is NodeLinkRuntime nodeLinkDisplay)
                {
                    if (nodeLinkDisplay.CurrentConnectionStateState == NodeLinkRuntime.ConnectionState.Add)
                    {
                        nodeLinkDisplay.Visible = true;
                    }
                    else
                    {
                        nodeLinkDisplay.CurrentConnectionStateState = NodeLinkRuntime.ConnectionState.DisplayExistingWithEdit;
                    }
                }
            }
            UpdateNodePassPositionData();
            NodeLinkContainer.Visible = true;
        }

        public void SetPassage(Passage nodePassage)
        {
            this.NodePassage = nodePassage;

            this.PassageText = nodePassage.text;
            NodeInfoInstance.PassageNameText = nodePassage.name;

            NodeInfoInstance.PassagePidText = nodePassage.pid.ToString();

            if (this is IWindow thisAsIWindow)
            {
                thisAsIWindow.X = nodePassage.position.x;
                thisAsIWindow.Y = nodePassage.position.y;
                thisAsIWindow.UpdateDependencies();
            }

            SetLinks();
        }

        private void SetLinks()
        {
            ClearNodeLinks();
            foreach (var passageLink in NodePassage.links)
            {
                CreateLinkDisplayForPassageLink(passageLink);
            }

            CreateAddNewLinkButton();
        }

        private void CreateLinkDisplayForPassageLink(Link passageLink)
        {
            var linkDisplay = new NodeLinkRuntime();
            linkDisplay.PassageLinkNameText = passageLink.name;
            linkDisplay.LinkText = passageLink.link;
            linkDisplay.PassageLinkNumberText = passageLink.pid.ToString();
            linkDisplay.MoveToFrbLayer(FrbLayer, GumLayer);
            linkDisplay.AddToManagers();
            NodeLinkContainer.Children.Add(linkDisplay);

            NodeLinks.Add(linkDisplay);
        }


        private void CreateAddNewLinkButton()
        {
            var linkDisplay = new NodeLinkRuntime();
            linkDisplay.CurrentConnectionStateState = NodeLinkRuntime.ConnectionState.Add;
            linkDisplay.OpenLinkButtonClick += LinkDisplay_OpenLinkButtonClick;
            linkDisplay.MoveToFrbLayer(FrbLayer, GumLayer);
            linkDisplay.AddToManagers();
            NodeLinkContainer.Children.Add(linkDisplay);

            NodeLinks.Add(linkDisplay);
        }

        private void LinkDisplay_OpenLinkButtonClick(IWindow window)
        {
            if (window is NodeLinkRuntime nodeLinkDisplay)
            {
                var newPassageLink = AddNewLinkToPassage();

                nodeLinkDisplay.CurrentConnectionStateState = NodeLinkRuntime.ConnectionState.DisplayExistingWithEdit;
                nodeLinkDisplay.PassageLinkNameText = newPassageLink.name;
                nodeLinkDisplay.LinkText = newPassageLink.link;
                nodeLinkDisplay.PassageLinkNumberText = newPassageLink.pid.ToString();
            }
        }

        private Link AddNewLinkToPassage()
        {
            var newLink = new Link();
            newLink.name = "NewLink";
            newLink.link = "[YourLink]";

            var currentLinks = NodePassage.links.ToList();
            currentLinks.Add(newLink);
            NodePassage.links = currentLinks.ToArray();

            return newLink;
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

        private void UpdateNodePassPositionData()
        {
            NodePassage.position.x = (int)X;
            NodePassage.position.y = (int)Y;
        }

        private void ClearNodeLinks()
        {
            var linkCount = NodeLinks.Count();
            for (var i = linkCount-1; i >= 0; i--)
            {
                var nodeLink = NodeLinks[i] as NodeLinkRuntime;
                NodeLinkContainer.Children.Remove(nodeLink);
                NodeLinkContainer.RemoveFromManagers();
                nodeLink.Destroy();
            }
        }

        public void CustomDispose()
        {
            ClearNodeLinks();
        }

        internal void HandleLinkEstablishedWithNode(NodeDisplayRuntime nodeLinkIsOver, NodeLinkRuntime nodeLink)
        {
            if (NodePassage != null)
            {
                nodeLink.HandleDraggingStopped();

                var newLink = new DialogTreeRaw.Link();
                newLink.pid = nodeLinkIsOver.NodePassage.pid;
                newLink.name = $"Link to {newLink.pid }";
                newLink.link = "Link text";

                var currentLinks = NodePassage.links.ToList();
                currentLinks.Add(newLink);
                NodePassage.links = currentLinks.ToArray();

                nodeLink.SetPassageLink(newLink);
                

                //Create a new +Link button to this node
                CreateAddNewLinkButton();
            }
        }
    }
}
