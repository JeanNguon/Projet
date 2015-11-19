using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Proteca.Web.Models;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class ListParametre : UserControl
    {
        public ListParametre()
        {
            InitializeComponent();
        }

        #region Properties

        public Boolean IsEditMode
        {
            get { return (Boolean)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }

        public ObservableCollection<RefParametre> ListParams
        {
            get { return (ObservableCollection<RefParametre>)GetValue(ListParamsProperty); }
            set { SetValue(ListParamsProperty, value); }
        }

        public ObservableCollection<MesModeleMesure> ListMesModeleMesure
        {
            get { return (ObservableCollection<MesModeleMesure>)GetValue(ListMesModeleMesureProperty); }
            set { SetValue(ListMesModeleMesureProperty, value); }
        }

        #endregion Properties

        #region DependencyProperties

        public static readonly DependencyProperty IsEditModeProperty =
            DependencyProperty.Register("IsEditMode", typeof(Boolean), typeof(ListParametre), new PropertyMetadata(null));

        public static readonly DependencyProperty ListParamsProperty =
            DependencyProperty.Register("ListParams", typeof(ObservableCollection<RefParametre>), typeof(ListParametre), new PropertyMetadata(null));

        public static readonly DependencyProperty ListMesModeleMesureProperty =
           DependencyProperty.Register("ListMesModeleMesure", typeof(ObservableCollection<MesModeleMesure>), typeof(ListParametre), new PropertyMetadata(null));

        #endregion DependencyProperties

    }
}
