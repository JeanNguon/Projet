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
using System.Windows.Data;
using Proteca.Silverlight.Enums;
using System.ComponentModel;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class RestitutionButton : UserControl, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Texte du bouton
        /// </summary>
        public int ButtonWidth
        {
            get { return (int)GetValue(ButtonWidthProperty); }
            set { SetValue(ButtonWidthProperty, value); }
        }

        public static readonly DependencyProperty ButtonWidthProperty =
                               DependencyProperty.Register("ButtonWidth", typeof(int), typeof(RestitutionButton), new PropertyMetadata(250, null));

        /// <summary>
        /// Texte du bouton
        /// </summary>
        public string ButtonText 
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty =
                               DependencyProperty.Register("ButtonText", typeof(string), typeof(RestitutionButton), new PropertyMetadata("", null));

        /// <summary>
        /// Type du bouton
        /// </summary>
        public RestitutionEnum ButtonType
        {
            get { return (RestitutionEnum)GetValue(ButtonTypeProperty); }
            set { SetValue(ButtonTypeProperty, value); }
        }

        public static readonly DependencyProperty ButtonTypeProperty =
                               DependencyProperty.Register("ButtonType", typeof(RestitutionEnum), typeof(RestitutionButton), new PropertyMetadata(RestitutionEnum.Restitution, OnButtonTypeChanged));

        private static void OnButtonTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArg)
        {
            if (d is RestitutionButton)
            {
                (d as RestitutionButton).OnPropertyChanged("ButtonType");
                (d as RestitutionButton).OnPropertyChanged("IsRestitution");
                (d as RestitutionButton).OnPropertyChanged("IsGraphique");
                (d as RestitutionButton).OnPropertyChanged("IsBilan");
                (d as RestitutionButton).OnPropertyChanged("IsPersonnalise");
            }
        }

        public Visibility IsRestitution
        {
            get { return (ButtonType == RestitutionEnum.Restitution) ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility IsGraphique
        {
            get { return (ButtonType == RestitutionEnum.Graphique) ? Visibility.Visible : Visibility.Collapsed;}
        }

        public Visibility IsBilan
        {
            get { return (ButtonType == RestitutionEnum.Bilan) ? Visibility.Visible : Visibility.Collapsed;}
        }

        public Visibility IsPersonnalise
        {
            get { return (ButtonType == RestitutionEnum.Personnalise) ? Visibility.Visible : Visibility.Collapsed;}
        }

        /// <summary>
        /// Commande de restitution
        /// </summary>
        public ICommand ButtonCommand
        {
            get { return (ICommand)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }

        public static readonly DependencyProperty ButtonCommandProperty =
                               DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(RestitutionButton), new PropertyMetadata(null, null));

        /// <summary>
        /// Paramètre de la commande
        /// </summary>
        public String CommandParameter
        {
            get { return (String)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
                               DependencyProperty.Register("CommandParameter", typeof(String), typeof(RestitutionButton), new PropertyMetadata(null, null));


        #endregion Properties

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public RestitutionButton()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #endregion Events
    }
}
