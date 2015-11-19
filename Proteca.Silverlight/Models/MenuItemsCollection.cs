using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Linq;

namespace Proteca.Silverlight.Models
{
    public class MenuItemsCollection : ObservableCollection<MenuItem>
    {
        private MenuItem parent;

        public MenuItemsCollection()
            : this(null)
        {
        }

        public MenuItemsCollection(MenuItem parent)
        {
            this.parent = parent;
        }

        public MenuItem Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
                foreach (MenuItem item in base.Items)
                {
                    item.Parent = value;
                }
            }
        }

        public MenuItem findByURL(String Url)
        {
            MenuItem mi = null;

            foreach (MenuItem item in base.Items)
            {
                if (item.URL.Equals(Url, StringComparison.InvariantCultureIgnoreCase))
                {
                    return item;
                }
                else
                {
                    mi = item.Items.findByURL(Url);
                    if (mi != null)
                    {
                        return mi;
                    }

                    if ((item.URL.Length > 1 && item.URL.StartsWith(Url, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        return item;
                    }
                }                
            }

            return mi;
        }

        public MenuItem ItemSelected
        {
            get
            {
                MenuItem mi = null;
                if (base.Items.Any(i=>i.IsSelected == true))
                {
                    mi = base.Items.Single(i=>i.IsSelected == true);
                    if (mi.Items.ItemSelected != null)
                    {
                        return mi.Items.ItemSelected;
                    }
                }                
                return mi;
            }
        }

        protected override void InsertItem(int index, MenuItem item)
        {
            item.Parent = this.Parent;
            base.InsertItem(index, item);
        }
    }
}
