using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfCommon.Views.Components
{
    public class MultipleItemsListView : ListView
    {
        public static DependencyProperty CustomSelectedItemsProperty = DependencyProperty.Register("CustomSelectedItems", typeof(IList), typeof(MultipleItemsListView), new FrameworkPropertyMetadata(null));

        public MultipleItemsListView()
        {
            this.SelectionChanged += MultipleItemsListView_SelectionChanged;
            //CustomSelectedItems = SelectedItems;
        }

        private void MultipleItemsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //CustomSelectedItems = SelectedItems;
            if (CustomSelectedItems != null)
            {
                CustomSelectedItems.Clear();

            }
            else
                CustomSelectedItems = new List<object>();

            foreach (var a in SelectedItems)
            {
                CustomSelectedItems.Add(a);
            }
            CommandManager.InvalidateRequerySuggested();
        }

        public IList CustomSelectedItems
        {
            get
            {
                return (IList)GetValue(CustomSelectedItemsProperty);
            }
            set
            {
                SetValue(CustomSelectedItemsProperty, value);
            }
        }
    }
}
