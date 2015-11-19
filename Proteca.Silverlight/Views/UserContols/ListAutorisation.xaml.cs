using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Proteca.Web.Models;
using System.Collections.ObjectModel;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class ListAutorisation : UserControl, INotifyPropertyChanged
    {
        public ListAutorisation()
        {
            InitializeComponent();
        }

        #region Properties

        public int ACleGroupe
        {
            get { return (int)GetValue(CleGroupeProperty); }
            set { SetValue(CleGroupeProperty, value); }
        }

        public IEnumerable<UsrRole> ListRole
        {
            get { return (IEnumerable<UsrRole>)GetValue(ListRoleProperty); }
            set { SetValue(ListRoleProperty, value); }
        }

        public ObservableCollection<RefUsrPortee> ListPortee
        {
            get { return (ObservableCollection<RefUsrPortee>)GetValue(ListPorteeProperty); }
            set { SetValue(ListPorteeProperty, value); }
        }

        public IEnumerable<UsrRole> RoleFilter
        {
            get
            {
                return ListRole != null ? ListRole.Where(r => r.RefUsrAutorisation.CleGroupe == ACleGroupe) : null;
            }
        }

        #endregion

        #region DependencyProperty

        public static readonly DependencyProperty CleGroupeProperty =
            DependencyProperty.Register("ACleGroupe", typeof(int), typeof(ListAutorisation), new PropertyMetadata(null));

        public static readonly DependencyProperty ListRoleProperty =
            DependencyProperty.Register("ListRole", typeof(IEnumerable<UsrRole>), typeof(ListAutorisation), new PropertyMetadata(null, OnListRolePropertyChanged));

        public static readonly DependencyProperty ListPorteeProperty =
            DependencyProperty.Register("ListPortee", typeof(ObservableCollection<RefUsrPortee>), typeof(ListAutorisation), new PropertyMetadata(null));

        private static void OnListRolePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ListAutorisation la = (ListAutorisation)d;
            la.RaisePropertyChanged("ListRole");
            la.RaisePropertyChanged("RoleFilter");
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
