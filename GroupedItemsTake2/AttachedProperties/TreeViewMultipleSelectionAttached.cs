using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GroupedItemsTake2.Controls;

namespace GroupedItemsTake2.AttachedProperties
{
    public class TreeListViewMultipleSelectionAttached
    {
        public static readonly DependencyProperty IsMultipleSelectionProperty =
        DependencyProperty.RegisterAttached(
            "IsMultipleSelection",
            typeof(Boolean),
            typeof(TreeListViewMultipleSelectionAttached),
            new PropertyMetadata(false, OnMultipleSelectionPropertyChanged));

        public static bool GetIsMultipleSelection(TreeListView element)
        {
            return (bool)element.GetValue(IsMultipleSelectionProperty);
        }

        public static void SetIsMultipleSelection(TreeListView element, Boolean value)
        {
            element.SetValue(IsMultipleSelectionProperty, value);
        }

        private static void OnMultipleSelectionPropertyChanged(DependencyObject d,
                                         DependencyPropertyChangedEventArgs e)
        {
            var TreeListView = d as TreeListView;

            if (TreeListView != null)
            {
                if (e.NewValue is bool)
                {
                    if ((bool)e.NewValue)
                    {
                        TreeListView.AddHandler(TreeListViewItem.MouseLeftButtonDownEvent,
                          new MouseButtonEventHandler(OnTreeListViewItemClicked), true);
                    }
                    else
                    {
                        TreeListView.RemoveHandler(TreeListViewItem.MouseLeftButtonDownEvent,
                             new MouseButtonEventHandler(OnTreeListViewItemClicked));
                    }
                }
            }
        }

        public static readonly DependencyProperty IsItemSelectedProperty =
        DependencyProperty.RegisterAttached(
            "IsItemSelected",
            typeof(Boolean),
            typeof(TreeListViewMultipleSelectionAttached),
            new PropertyMetadata(false, OnIsItemSelectedPropertyChanged));

        public static bool GetIsItemSelected(TreeListViewItem element)
        {
            return (bool)element.GetValue(IsItemSelectedProperty);
        }

        public static void SetIsItemSelected(TreeListViewItem element, Boolean value)
        {
            element.SetValue(IsItemSelectedProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.RegisterAttached(
            "SelectedItems",
            typeof(IList),
            typeof(TreeListViewMultipleSelectionAttached),
            new PropertyMetadata());

        public static IList GetSelectedItems(TreeListView element)
        {
            return (IList)element.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(TreeListView element, IList value)
        {
            element.SetValue(SelectedItemsProperty, value);
        }

        private static readonly DependencyProperty StartItemProperty =
        DependencyProperty.RegisterAttached(
            "StartItem",
            typeof(TreeListViewItem),
            typeof(TreeListViewMultipleSelectionAttached),
            new PropertyMetadata());

        private static TreeListViewItem GetStartItem(TreeListView element)
        {
            return (TreeListViewItem)element.GetValue(StartItemProperty);
        }

        private static void SetStartItem(TreeListView element, TreeListViewItem value)
        {
            element.SetValue(StartItemProperty, value);
        }

        private static void SelectMultipleItemsRandomly(TreeListView TreeListView,
                                                    TreeListViewItem TreeListViewItem)
        {
            SetIsItemSelected(TreeListViewItem, !GetIsItemSelected(TreeListViewItem));
            if (GetStartItem(TreeListView) == null)
            {
                if (GetIsItemSelected(TreeListViewItem))
                {
                    SetStartItem(TreeListView, TreeListViewItem);
                }
            }
            else
            {
                if (GetSelectedItems(TreeListView).Count == 0)
                {
                    SetStartItem(TreeListView, null);
                }
            }
        }

        private static void SelectMultipleItemsContinuously(TreeListView TreeListView,
                                                    TreeListViewItem TreeListViewItem)
        {
            TreeListViewItem startItem = GetStartItem(TreeListView);
            if (startItem != null)
            {
                if (startItem == TreeListViewItem)
                {
                    SelectSingleItem(TreeListView, TreeListViewItem);
                    return;
                }

                ICollection<TreeListViewItem> allItems = new List<TreeListViewItem>();
                GetAllItems(TreeListView, null, allItems);
                DeSelectAllItems(TreeListView, null);
                bool isBetween = false;
                foreach (var item in allItems)
                {
                    if (item == TreeListViewItem || item == startItem)
                    {
                        // toggle to true if first element is found and
                        // back to false if last element is found
                        isBetween = !isBetween;

                        // set boundary element
                        SetIsItemSelected(item, true);
                        continue;
                    }

                    if (isBetween)
                    {
                        SetIsItemSelected(item, true);
                    }
                }
            }
        }

        private static void GetAllItems(TreeListView TreeListView, TreeListViewItem TreeListViewItem,
                                    ICollection<TreeListViewItem> allItems)
        {
            if (TreeListView != null)
            {
                for (int i = 0; i < TreeListView.Items.Count; i++)
                {
                    TreeListViewItem item = TreeListView.ItemContainerGenerator.
                                               ContainerFromIndex(i) as TreeListViewItem;
                    if (item != null)
                    {
                        allItems.Add(item);
                        GetAllItems(null, item, allItems);
                    }
                }
            }
            else
            {
                for (int i = 0; i < TreeListViewItem.Items.Count; i++)
                {
                    TreeListViewItem item = TreeListViewItem.ItemContainerGenerator.
                                               ContainerFromIndex(i) as TreeListViewItem;
                    if (item != null)
                    {
                        allItems.Add(item);
                        GetAllItems(null, item, allItems);
                    }
                }
            }
        }

        private static void OnIsItemSelectedPropertyChanged(DependencyObject d,
                                           DependencyPropertyChangedEventArgs e)
        {
            TreeListViewItem TreeListViewItem = d as TreeListViewItem;
            TreeListView TreeListView = FindTreeListView(TreeListViewItem);
            if (TreeListViewItem != null && TreeListView != null)
            {
                var selectedItems = GetSelectedItems(TreeListView);
                if (selectedItems != null)
                {
                    if (GetIsItemSelected(TreeListViewItem))
                    {
                        selectedItems.Add(TreeListViewItem.Header);
                    }
                    else
                    {
                        selectedItems.Remove(TreeListViewItem.Header);
                    }
                }
            }
        }

        private static TreeListView FindTreeListView(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                return null;
            }

            TreeListView TreeListView = dependencyObject as TreeListView;
            if (TreeListView != null)
            {
                return TreeListView;
            }

            return FindTreeListView(VisualTreeHelper.GetParent(dependencyObject));
        }

        private static void OnTreeListViewItemClicked(object sender, MouseButtonEventArgs e)
        {
            TreeListViewItem TreeListViewItem = FindTreeListViewItem(
                                            e.OriginalSource as DependencyObject);
            TreeListView TreeListView = sender as TreeListView;

            if (TreeListViewItem != null && TreeListView != null)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    SelectMultipleItemsRandomly(TreeListView, TreeListViewItem);
                }
                else if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    SelectMultipleItemsContinuously(TreeListView, TreeListViewItem);
                }
                else
                {
                    SelectSingleItem(TreeListView, TreeListViewItem);
                }
            }
        }

        private static TreeListViewItem FindTreeListViewItem(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                return null;
            }

            TreeListViewItem TreeListViewItem = dependencyObject as TreeListViewItem;
            if (TreeListViewItem != null)
            {
                return TreeListViewItem;
            }

            return FindTreeListViewItem(VisualTreeHelper.GetParent(dependencyObject));
        }

        private static void SelectSingleItem(TreeListView TreeListView,
                                                    TreeListViewItem TreeListViewItem)
        {
            // first deselect all items
            DeSelectAllItems(TreeListView, null);
            SetIsItemSelected(TreeListViewItem, true);
            SetStartItem(TreeListView, TreeListViewItem);
        }

        private static void DeSelectAllItems(TreeListView TreeListView,
                                                 TreeListViewItem TreeListViewItem)
        {
            if (TreeListView != null)
            {
                for (int i = 0; i < TreeListView.Items.Count; i++)
                {
                    TreeListViewItem item = TreeListView.ItemContainerGenerator.
                                               ContainerFromIndex(i) as TreeListViewItem;
                    if (item != null)
                    {
                        SetIsItemSelected(item, false);
                        DeSelectAllItems(null, item);
                    }
                }
            }
            else
            {
                for (int i = 0; i < TreeListViewItem.Items.Count; i++)
                {
                    TreeListViewItem item = TreeListViewItem.ItemContainerGenerator.
                                               ContainerFromIndex(i) as TreeListViewItem;
                    if (item != null)
                    {
                        SetIsItemSelected(item, false);
                        DeSelectAllItems(null, item);
                    }
                }
            }
        }
    }
}
