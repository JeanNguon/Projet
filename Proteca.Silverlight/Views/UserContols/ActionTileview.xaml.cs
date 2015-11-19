using Proteca.Silverlight.Enums;
using Proteca.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class ActionTileview : UserControl, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Liste des actions
        /// </summary>
        public List<AnAction> ListActions
        {
            get { return (List<AnAction>)GetValue(ListActionsProperty); }
            set { SetValue(ListActionsProperty, value); }
        }

        #endregion

        #region DependancyProperty

        public static readonly DependencyProperty ListActionsProperty = DependencyProperty.Register("ListActions", typeof(List<AnAction>),
                       typeof(ActionTileview), new PropertyMetadata(null));

        #endregion

        #region Constructor

        public ActionTileview()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion Events
    }
}
