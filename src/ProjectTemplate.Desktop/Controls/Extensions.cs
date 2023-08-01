using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace ProjectTemplate.Desktop.Controls
{
    public static class Extensions
    {
        private static readonly PropertyInfo InheritanceContextProperty;
        public static readonly DependencyProperty AutoFocusProperty;
        public static readonly DependencyProperty SubmitButtonProperty;
        public static readonly DependencyProperty CancelButtonProperty;
        public static readonly DependencyProperty AutoSortProperty;
        public static readonly DependencyProperty SortByProperty;
        public static readonly DependencyProperty GroupByProperty;
        public static readonly DependencyProperty InitialSortByProperty;
        public static readonly DependencyProperty DataGridColumnsProperty;
        public static readonly DependencyProperty GridViewColumnsProperty;

        static Extensions()
        {
            InheritanceContextProperty = typeof(DependencyObject).GetProperty("InheritanceContext", BindingFlags.NonPublic | BindingFlags.Instance);
            AutoFocusProperty = DependencyProperty.RegisterAttached("AutoFocus", typeof(bool), typeof(Extensions), new PropertyMetadata(false, OnAutoFocusPropertyChange));
            SubmitButtonProperty = DependencyProperty.RegisterAttached("SubmitButton", typeof(Button), typeof(Extensions), new PropertyMetadata(OnSubmitButtonPropertyChange));
            CancelButtonProperty = DependencyProperty.RegisterAttached("CancelButton", typeof(Button), typeof(Extensions), new PropertyMetadata(OnCancelButtonPropertyChange));
            AutoSortProperty = DependencyProperty.RegisterAttached("AutoSort", typeof(bool), typeof(Extensions), new UIPropertyMetadata(false, OnAutoSortPropertyChanged));
            SortByProperty = DependencyProperty.RegisterAttached("SortBy", typeof(string), typeof(Extensions), new UIPropertyMetadata(null));
            GroupByProperty = DependencyProperty.RegisterAttached("GroupBy", typeof(string), typeof(Extensions), new UIPropertyMetadata(null, OnGroupByPropertyPropertyChanged));
            InitialSortByProperty = DependencyProperty.RegisterAttached("InitialSortBy", typeof(string), typeof(Extensions), new UIPropertyMetadata(null, OnInitialSortByPropertyPropertyChanged));
            DataGridColumnsProperty = DependencyProperty.RegisterAttached("DataGridColumns", typeof(IEnumerable<string>), typeof(Extensions), new UIPropertyMetadata(Enumerable.Empty<string>(), OnDataGridColumnsPropertyChanged));
            GridViewColumnsProperty = DependencyProperty.RegisterAttached("GridViewColumns", typeof(IEnumerable<string>), typeof(Extensions), new UIPropertyMetadata(Enumerable.Empty<string>(), OnGridViewColumnsPropertyChanged));
        }

        #region AutoFocus

        public static bool GetAutoFocus(DependencyObject o)
        {
            return (bool)o.GetValue(AutoFocusProperty);
        }

        public static void SetAutoFocus(DependencyObject o, bool value)
        {
            o.SetValue(AutoFocusProperty, value);
        }

        private static void OnAutoFocusPropertyChange(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement sourceElement = o as FrameworkElement;
            if (sourceElement == null)
            {
                return;
            }
            if (sourceElement.IsLoaded)
            {
                bool autoFocus = (bool)e.NewValue;
                if (autoFocus)
                {
                    sourceElement.Focus();
                }
            }
            else
            {
                sourceElement.Loaded += OnSourceElementLoaded;
            }
        }

        private static void OnSourceElementLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement sourceElement = sender as FrameworkElement;
            if (sourceElement != null)
            {
                sourceElement.Loaded -= OnSourceElementLoaded;
                bool autoFocus = GetAutoFocus(sourceElement);
                if (autoFocus)
                {
                    sourceElement.Focus();
                }
            }
        }

        #endregion

        #region SubmitButton

        public static Button GetSubmitButton(DependencyObject o)
        {
            return (Button)o.GetValue(SubmitButtonProperty);
        }

        public static void SetSubmitButton(DependencyObject o, Button value)
        {
            o.SetValue(SubmitButtonProperty, value);
        }

        private static void OnSubmitButtonPropertyChange(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UIElement sourceElement = o as UIElement;
            if (sourceElement == null)
            {
                return;
            }
            sourceElement.KeyDown -= OnSubmitSourceElementKeyDown;
            Button targetButton = e.NewValue as Button;
            if (targetButton == null)
            {
                return;
            }
            sourceElement.KeyDown += OnSubmitSourceElementKeyDown;
        }

        private static void OnSubmitSourceElementKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Button targetButton = GetSubmitButton((DependencyObject)sender);
                if (targetButton == null)
                {
                    return;
                }
                targetButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        #endregion

        #region CancelButton

        public static Button GetCancelButton(DependencyObject o)
        {
            return (Button)o.GetValue(SubmitButtonProperty);
        }

        public static void SetCancelButton(DependencyObject o, Button value)
        {
            o.SetValue(SubmitButtonProperty, value);
        }

        private static void OnCancelButtonPropertyChange(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UIElement sourceElement = o as UIElement;
            if (sourceElement == null)
            {
                return;
            }
            sourceElement.KeyDown -= OnCancelSourceElementKeyDown;
            Button targetButton = e.NewValue as Button;
            if (targetButton == null)
            {
                return;
            }
            sourceElement.KeyDown += OnCancelSourceElementKeyDown;
        }

        private static void OnCancelSourceElementKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Button targetButton = GetSubmitButton((DependencyObject)sender);
                if (targetButton == null)
                {
                    return;
                }
                targetButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        #endregion

        #region AutoSortProperty

        public static bool GetAutoSort(DependencyObject o)
        {
            return (bool)o.GetValue(AutoSortProperty);
        }

        public static void SetAutoSort(DependencyObject o, bool value)
        {
            o.SetValue(AutoSortProperty, value);
        }

        private static void OnAutoSortPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView != null)
            {
                if ((bool)e.OldValue)
                {
                    listView.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnGridColumnHeaderClick));
                }
                if ((bool)e.NewValue)
                {
                    listView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnGridColumnHeaderClick));
                }
            }
        }

        private static void OnGridColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader columnHeader = e.OriginalSource as GridViewColumnHeader;
            if (columnHeader != null)
            {
                string sortByPropertyName = GetSortBy(columnHeader.Column);
                if (!string.IsNullOrEmpty(sortByPropertyName))
                {
                    ListView listView = columnHeader.GetParent<ListView>();
                    if (listView != null)
                    {
                        string groupByPropertyName = GetGroupBy(listView);
                        SortCollection(listView.Items, sortByPropertyName, groupByPropertyName);
                    }
                }
            }
        }

        private static void SortCollection(ICollectionView view, string sortByPropertyName, string groupByPropertyName)
        {
            ListSortDirection direction = ListSortDirection.Ascending;
            SortDescription currentSort = view.SortDescriptions.FirstOrDefault(x => string.Equals(x.PropertyName, sortByPropertyName, StringComparison.Ordinal));
            if (!default(SortDescription).Equals(currentSort))
            {
                direction = currentSort.Direction == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            view.SortDescriptions.Clear();
            if (!string.IsNullOrEmpty(groupByPropertyName))
            {
                view.SortDescriptions.Add(new SortDescription(groupByPropertyName, ListSortDirection.Ascending));
            }
            if (!string.IsNullOrEmpty(sortByPropertyName))
            {
                view.SortDescriptions.Add(new SortDescription(sortByPropertyName, direction));
            }
        }

        #endregion

        #region SortBy

        public static string GetSortBy(DependencyObject o)
        {
            return (string)o.GetValue(SortByProperty);
        }

        public static void SetSortBy(DependencyObject o, string value)
        {
            o.SetValue(SortByProperty, value);
        }

        #endregion

        #region GroupBy

        public static string GetGroupBy(DependencyObject o)
        {
            return (string)o.GetValue(GroupByProperty);
        }

        public static void SetGroupBy(DependencyObject o, string value)
        {
            o.SetValue(GroupByProperty, value);
        }

        public static void OnGroupByPropertyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView != null)
            {
                ICollectionView view = listView.Items;
                string oldPropertyName = e.OldValue as string;
                if (!string.IsNullOrEmpty(oldPropertyName))
                {
                    SortDescription? sortDescription = view.SortDescriptions.FirstOrDefault(x => string.Equals(x.PropertyName, oldPropertyName, StringComparison.Ordinal));
                    if (sortDescription != null)
                    {
                        view.SortDescriptions.Remove(sortDescription.Value);
                    }
                }
                view.GroupDescriptions.Clear();
                string newPropertyName = e.NewValue as string;
                if (!string.IsNullOrEmpty(newPropertyName))
                {
                    view.GroupDescriptions.Add(new PropertyGroupDescription(newPropertyName));
                    view.SortDescriptions.Insert(0, new SortDescription(newPropertyName, ListSortDirection.Ascending));
                }
            }
        }

        #endregion

        #region InitialSortBy

        public static string GetInitialSortBy(DependencyObject o)
        {
            return (string)o.GetValue(InitialSortByProperty);
        }

        public static void SetInitialSortBy(DependencyObject o, string value)
        {
            o.SetValue(InitialSortByProperty, value);
        }

        public static void OnInitialSortByPropertyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //ListView? listView = sender as ListView;
            //if (listView != null)
            //{
            //    if (!string.IsNullOrEmpty(e.OldValue?.ToString()))
            //    {
            //        TypeDescriptor.GetProperties(listView)["ItemsSource"].RemoveValueChanged(listView, OnInitialSortByListViewDataContextChanged);
            //    }
            //    if (!string.IsNullOrEmpty(e.NewValue?.ToString()))
            //    {
            //        TypeDescriptor.GetProperties(listView)["ItemsSource"].AddValueChanged(listView, OnInitialSortByListViewDataContextChanged);
            //    }
            //}
        }

        private static void OnInitialSortByListViewDataContextChanged(object sender, EventArgs e)
        {
            //ListView? listView = sender as ListView;
            //if (listView != null)
            //{
            //    string propertyName = GetInitialSortBy(listView);
            //    if (!string.IsNullOrEmpty(propertyName))
            //    {
            //        SortCollection(listView.Items, propertyName);
            //    }
            //}
        }

        #endregion

        #region DataGridColumns

        public static IEnumerable<string> GetDataGridColumns(DependencyObject o)
        {
            return (IEnumerable<string>)o.GetValue(DataGridColumnsProperty);
        }

        public static void SetDataGridColumns(DependencyObject o, IEnumerable<string> value)
        {
            o.SetValue(DataGridColumnsProperty, value);
        }

        private static void OnDataGridColumnsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                IEnumerable<string> oldColumnNames = e.OldValue as IEnumerable<string>;
                if (oldColumnNames != null && oldColumnNames.Any())
                {
                    foreach (string oldColumnName in oldColumnNames)
                    {
                        DataGridColumn oldColumn = dataGrid.Columns.FirstOrDefault(x => string.Equals((string)x.Header, oldColumnName, StringComparison.Ordinal));
                        dataGrid.Columns.Remove(oldColumn);
                    }
                }
                IEnumerable<string> newColumnNames = e.NewValue as IEnumerable<string>;
                if (newColumnNames != null && newColumnNames.Any())
                {
                    foreach (string newColumnName in newColumnNames)
                    {
                        DataGridColumn newColumn = new DataGridTextColumn()
                        {
                            Header = newColumnName,
                            Binding = new Binding($"[{newColumnName}]"),
                            IsReadOnly = true
                        };
                        dataGrid.Columns.Add(newColumn);
                    }
                }
            }
        }

        #endregion

        #region DataGridColumns

        public static IEnumerable<string> GetGridViewColumns(DependencyObject o)
        {
            return (IEnumerable<string>)o.GetValue(GridViewColumnsProperty);
        }

        public static void SetGridViewColumns(DependencyObject o, IEnumerable<string> value)
        {
            o.SetValue(GridViewColumnsProperty, value);
        }

        private static void OnGridViewColumnsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gridView = listView?.View as GridView;
            if (gridView != null)
            {
                IEnumerable<string> oldColumnNames = e.OldValue as IEnumerable<string>;
                if (oldColumnNames != null && oldColumnNames.Any())
                {
                    foreach (string oldColumnName in oldColumnNames)
                    {
                        GridViewColumn oldColumn = gridView.Columns.FirstOrDefault(x => string.Equals((string)x.Header, oldColumnName, StringComparison.Ordinal));
                        if (oldColumn != null)
                        {
                            gridView.Columns.Remove(oldColumn);
                        }
                    }
                }
                IEnumerable<string> newColumnNames = e.NewValue as IEnumerable<string>;
                if (newColumnNames != null && newColumnNames.Any())
                {
                    foreach (string newColumnName in newColumnNames)
                    {
                        GridViewColumn newColumn = new GridViewColumn()
                        {
                            Header = newColumnName,
                            DisplayMemberBinding = new Binding($"[{newColumnName}]"),
                        };
                        gridView.Columns.Add(newColumn);
                    }
                }
            }
        }

        #endregion

        #region GetParent

        public static T GetParent<T>(this DependencyObject child)
        {
            DependencyObject parent = GetDirectParent(child);
            while (parent != null && !(parent is T))
            {
                parent = GetDirectParent(parent);
            }
            return (T)(parent as object);
        }

        private static DependencyObject GetDirectParent(this DependencyObject child)
        {
            DependencyObject parent = LogicalTreeHelper.GetParent(child);
            if (parent == null)
            {
                if (child is FrameworkElement)
                {
                    parent = VisualTreeHelper.GetParent(child);
                }
                if (parent == null && child is ContentElement)
                {
                    parent = ContentOperations.GetParent((ContentElement)child);
                }
                if (parent == null)
                {
                    parent = InheritanceContextProperty?.GetValue(child, null) as DependencyObject;
                }
            }
            return parent;
        }

        #endregion

        public static bool IsUserVisible(this FrameworkElement element, FrameworkElement container)
        {
            if (element == null || !element.IsVisible)
            {
                return false;
            }

            Rect bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
        }
    }
}
