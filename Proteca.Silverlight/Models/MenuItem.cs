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
using Jounce.Core.ViewModel;
using Jounce.Framework.Command;
using Jounce.Core.Command;

namespace Proteca.Silverlight.Models
{
    public class MenuItem : BaseViewModel
    {
        public MenuItem(String name, String url)
        {
            Name = name;
            URL = "/" + url;
            _originalUrl = "/" + url;
            Items = new MenuItemsCollection(this);
            MenuCommand = new ActionCommand<object>(
obj => Navigate(), obj => true);
        }

        public MenuItem(String name, String url, MenuItemsCollection items)
        {
            Name = name;
            URL = "/" + url;
            _originalUrl = "/" + url;
            Items = items;
            Items.Parent = this;
        }

        public IActionCommand MenuCommand { get; set; }

        public String Name { get; set; }

        private String _originalUrl;

        public String URL { get; set; }

        private Boolean _isSelected;
        public Boolean IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (Parent != null)
                {
                    Parent.IsSelected = value;
                }
                RaisePropertyChanged("IsSelected");
            }
        }

        private Boolean _isSubmenuOpen;
        public Boolean IsSubmenuOpen
        {
            get { return _isSubmenuOpen; }
            set
            {
                _isSubmenuOpen = value;
                ToggleSubmenu(value);
            }
        }

        public Boolean HasChildren
        {
            get
            {
                return Items.Count > 0;
            }
        }

        private MenuItem _parent;
        public MenuItem Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                if (_parent != null)
                {
                    URL = _parent.URL + _originalUrl;
                }
                else
                {
                    URL = _originalUrl;
                }
            }
        }

        public MenuItemsCollection Items { get; set; }

        public void Navigate()
        {
            // Navigation autorisée uniquement sur les éléments de second niveau
            // Sauf pour l'accueil
            if (this.Parent != null || this.URL.Equals("/Accueil", StringComparison.InvariantCultureIgnoreCase))
            {
                ((MainPage)Application.Current.RootVisual).ContentFrame.Navigate(new Uri(this.URL, UriKind.Relative));
            }
        }

        private void ToggleSubmenu(bool expand)
        {
            if (System.Windows.Browser.HtmlPage.IsEnabled)
            {
                System.Windows.Browser.HtmlPage.Window.Invoke("ToggleMenu", expand);
            }
        }

    }
}
