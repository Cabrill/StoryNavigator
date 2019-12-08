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

        public MenuItemRuntime AddMenuItem(string menuItemText, FlatRedBall.Gui.WindowEvent menuItemClickEvent, bool shouldRightAlign = false)
        {
            var menuItem = new MenuItemRuntime();
            menuItem.ItemText = menuItemText;
            menuItem.Click += menuItemClickEvent;

            if (shouldRightAlign)
                menuItem.CurrentAlignmentState = MenuItemRuntime.Alignment.Right;
            else
                menuItem.CurrentAlignmentState = MenuItemRuntime.Alignment.Left;
            MenuItemContainer.Children.Add(menuItem);

            return menuItem;
        }

        public void ClearItems()
        {
            var itemCount = MenuItemContainer.Children?.Count();
            if (itemCount > 0)
            {
                for (var i = 0; i < itemCount; i++)
                {
                    var item = MenuItemContainer.Children[i] as MenuItemRuntime;
                    MenuItemContainer.Children.Remove(item);
                    item.RemoveFromManagers();
                    item.Destroy();
                }
            }
        }
    }
}
