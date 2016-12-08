using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace IV_Play
{
    public class ScrollAndFocusBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += new SelectionChangedEventHandler(AssociatedObject_SelectionChanged);
            
        }

        void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView)
            {
                ListView listView = (sender as ListView);
               
                if (listView.SelectedItem != null)
                {
                    
                    Action action = delegate
                    {
                        listView.UpdateLayout();
                        listView.ScrollIntoView(listView.SelectedItem);
                        var item = listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem) as ListBoxItem;
                        item.Focus();
                        item.BringIntoView();
                    };
                    listView.Dispatcher.BeginInvoke(action);
                }
            }
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectionChanged -=
                new SelectionChangedEventHandler(AssociatedObject_SelectionChanged);
        }
    }
}
