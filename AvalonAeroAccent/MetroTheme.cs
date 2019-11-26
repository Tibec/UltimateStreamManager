﻿/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using Xceed.Wpf.AvalonDock.Themes;

namespace AvalonMetroAccent
{
    public class MetroTheme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri(
                "/AvalonMetroAccent;component/Theme.xaml", 
                UriKind.Relative);  
        }
    }
}
