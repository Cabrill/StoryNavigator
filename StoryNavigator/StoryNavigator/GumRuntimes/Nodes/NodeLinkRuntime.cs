using System;
using System.Collections.Generic;
using System.Linq;
using static StoryNavigator.DataTypes.DialogTreeRaw;

namespace StoryNavigator.GumRuntimes.Nodes
{
    public partial class NodeLinkRuntime
    {
        public Link PassageLink { get; protected set; }
        partial void CustomInitialize () 
        {

        }

        public void SetPassageLink(Link passageLink)
        {
            PassageLink = passageLink;

            PassageLinkNameText = passageLink.name;
            LinkText = passageLink.link;
            PassageLinkNumberText = passageLink.pid.ToString();
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
    }
}
