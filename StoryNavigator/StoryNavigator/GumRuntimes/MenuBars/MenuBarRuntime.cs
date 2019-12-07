using System;
using System.Collections.Generic;
using System.Linq;

namespace StoryNavigator.GumRuntimes.MenuBars
{
    public partial class MenuBarRuntime
    {
        partial void CustomInitialize () 
        {
        }

        public void AddMenuItem(MenuItemRuntime menuItemToAdd)
        {
            this.ContainerInstance.Children.Add(menuItemToAdd);
        }

        public void AddMenuItem(string menuItemText, FlatRedBall.Gui.WindowEvent menuItemClickEvent)
        {
            var menuItem = new MenuItemRuntime();
            menuItem.ItemText = menuItemText;
            menuItem.Click += menuItemClickEvent;
        }
    }
}
