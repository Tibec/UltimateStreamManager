using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace UltimateStreamMgr.ViewModel
{
    public class LayoutUpdateStrategy : ILayoutUpdateStrategy
    {
        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
           // throw new NotImplementedException();
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {
           // throw new NotImplementedException();
        }

        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            /*
            LayoutAnchorablePane destPane = destinationContainer as LayoutAnchorablePane;
            if (destinationContainer != null &&
                destinationContainer.FindParent<LayoutFloatingWindow>() != null)
                return false;
            */
            var toBeAddedPane = layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(d => d.Content.ToString() == anchorableToShow.Content.ToString());
            if (toBeAddedPane != null)
            {
                if(toBeAddedPane.IsHidden)
                {
                    return false;
                }
               // destinationContainer
                return true;
            }
            return false;
        }

        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return true;
        }
    }

    
}
