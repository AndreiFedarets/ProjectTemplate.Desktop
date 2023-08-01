using Microsoft.Xaml.Behaviors;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProjectTemplate.Desktop.Controls
{
    public class ListBoxSelectedItemsBehaviour : Behavior<ListBox>
    {
        private bool Selecting { get; set; }

        public IEnumerable<object> SelectedItems
        {
            get { return (IEnumerable<object>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IEnumerable<object>), typeof(ListBoxSelectedItemsBehaviour), new UIPropertyMetadata(null, OnSelectedItemsChanged));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnAssociatedObjectSelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
            {
                AssociatedObject.SelectionChanged -= OnAssociatedObjectSelectionChanged;
            }
        }

        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ListBoxSelectedItemsBehaviour behaviour = (ListBoxSelectedItemsBehaviour)sender;
            if (behaviour.Selecting)
            {
                return;
            }
            behaviour.Selecting = true;
            try
            {
                ListBox listBox = behaviour.AssociatedObject;
                listBox.SelectedItems.Clear();
                IEnumerable<object> selectedItems = e.NewValue as IEnumerable<object>;
                if (selectedItems != null)
                {
                    foreach (object item in selectedItems)
                    {
                        listBox.SelectedItems.Add(item);
                    }
                }
            }
            finally
            {
                behaviour.Selecting = false;
            }
        }

        private void OnAssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Selecting)
            {
                return;
            }
            Selecting = true;
            try
            {
                ListBox listBox = sender as ListBox;
                if (listBox != null)
                {
                    SelectedItems = listBox.SelectedItems.OfType<object>().ToArray();
                }
            }
            finally
            {
                Selecting = false;
            }
        }
    }
}
