using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro.Controls;

//
// From: http://msdn.microsoft.com/en-us/library/bb613579%28v=vs.110%29.aspx
//

namespace RockBot2
{
	public partial class MainWindow : MetroWindow
	{
		private ChildItem FindVisualChild<ChildItem>(DependencyObject obj) where ChildItem : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is ChildItem)
				{
					return (ChildItem)child;
				}

				else
				{
					ChildItem childofchild = FindVisualChild<ChildItem>(child);
					if (childofchild != null)
					{
						return childofchild;
					}
				}
			}

			return null;
		}

		private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
		{
			DependencyObject parentObject = VisualTreeHelper.GetParent(child);

			if (parentObject == null)
			{
				return null;
			}

			T parent = parentObject as T;
			if (parent != null)
			{
				return parent;
			}

			return FindVisualParent<T>(parentObject);
		}
	}
}
