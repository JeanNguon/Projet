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

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class AddButton : UserControl
    {
        #region Properties

        /// <summary>
        /// Texte du bouton
        /// </summary>
        public string ButtonText 
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty =
                               DependencyProperty.Register("ButtonText", typeof(string), typeof(AddButton), new PropertyMetadata("", null));

        /// <summary>
        /// Commande d'ajout
        /// </summary>
        public ICommand ButtonCommand
        {
            get { return (ICommand)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }

        public static readonly DependencyProperty ButtonCommandProperty =
                               DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(AddButton), new PropertyMetadata(null, null));

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public AddButton()
        {
            InitializeComponent();
        }

        #endregion Constructor
    }
}
