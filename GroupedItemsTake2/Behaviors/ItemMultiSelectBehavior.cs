using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using Telerik.Windows.Controls;

namespace GroupedItemsTake2.Behaviors
{
    public class ItemMultiSelectBehavior
    {
        private readonly RadTreeListView _grid;
        private readonly INotifyCollectionChanged _selectedItems;
        private Boolean _isSubscribedToEvents;
        private static Boolean _isAttached;

        public static readonly DependencyProperty SelectedItemsProperty
            = DependencyProperty.RegisterAttached("SelectedItems", typeof(INotifyCollectionChanged), typeof(ItemMultiSelectBehavior),
                new PropertyMetadata(OnSelectedItemsPropertyChanged));

        public static void SetSelectedItems(DependencyObject dependencyObject, INotifyCollectionChanged selectedItems)
        {
            dependencyObject.SetValue(SelectedItemsProperty, selectedItems);
        }

        public static INotifyCollectionChanged GetSelectedItems(DependencyObject dependencyObject)
        {
            return (INotifyCollectionChanged)dependencyObject.GetValue(SelectedItemsProperty);
        }

        private static void OnSelectedItemsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var grid = dependencyObject as RadTreeListView;
            var selectedItems = e.NewValue as INotifyCollectionChanged;

            if (grid == null || selectedItems == null || _isAttached) return;
            var behavior = new ItemMultiSelectBehavior(grid, selectedItems);
            behavior.Attach();
            _isAttached = true;
        }

        private void Attach()
        {
            if (_grid == null || _selectedItems == null) return;
            Transfer(GetSelectedItems(_grid) as IList, _grid.SelectedItems);
            SubscribeToEvents();
        }

        public ItemMultiSelectBehavior(RadTreeListView grid, INotifyCollectionChanged selectedItems)
        {
            _grid = grid;
            _selectedItems = selectedItems;
        }

        void ContextSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UnsubscribeFromEvents();

            Transfer(GetSelectedItems(_grid) as IList, _grid.SelectedItems);

            SubscribeToEvents();
        }

        void GridSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UnsubscribeFromEvents();

            Transfer(_grid.SelectedItems, GetSelectedItems(_grid) as IList);

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            if (_isSubscribedToEvents) return;
            _grid.SelectedItems.CollectionChanged += GridSelectedItemsCollectionChanged;

            if (GetSelectedItems(_grid) != null)
            {
                GetSelectedItems(_grid).CollectionChanged += ContextSelectedItemsCollectionChanged;
            }
            _isSubscribedToEvents = true;
        }

        private void UnsubscribeFromEvents()
        {
            if (!_isSubscribedToEvents) return;
            _grid.SelectedItems.CollectionChanged -= GridSelectedItemsCollectionChanged;

            if (GetSelectedItems(_grid) != null)
            {
                GetSelectedItems(_grid).CollectionChanged -= ContextSelectedItemsCollectionChanged;
            }
            _isSubscribedToEvents = false;
        }

        public static void Transfer(IList source, IList target)
        {
            if (source == null || target == null)
                return;

            target.Clear();

            foreach (var o in source)
            {
                target.Add(o);
            }
        }
    }
}



