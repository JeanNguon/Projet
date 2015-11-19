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
using System.Collections.ObjectModel;
using Proteca.Web.Models;
using Jounce.Core.Command;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class NiveauProtection : UserControl
    {
        public NiveauProtection()
        {
            InitializeComponent();
        }

        #region Properties

        public Boolean IsEditMode
        {
            get { return (Boolean)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }

        public Boolean IsTypeEqVisible
        {
            get { return (Boolean)GetValue(IsTypeEqVisibleProperty); }
            set { SetValue(IsTypeEqVisibleProperty, value); }
        }

        public Boolean CanAdd
        {
            get { return (Boolean)GetValue(CanAddProperty); }
            set { SetValue(CanAddProperty, value); }
        }

        public Boolean CanDelete
        {
            get { return (Boolean)GetValue(CanDeleteProperty); }
            set { SetValue(CanDeleteProperty, value); }
        }

        public ObservableCollection<MesNiveauProtection> ListNiveauProtection
        {
            get { return (ObservableCollection<MesNiveauProtection>)GetValue(ListNiveauProtectionProperty); }
            set { SetValue(ListNiveauProtectionProperty, value); }
        }
        
        public IActionCommand AddNiveauProtectionCommand
        {
            get { return (IActionCommand)GetValue(AddNiveauProtectionCommandProperty); }
            set { SetValue(AddNiveauProtectionCommandProperty, value); }
        }
        
        public IActionCommand DeleteNiveauProtectionCommand
        {
            get { return (IActionCommand)GetValue(DeleteNiveauProtectionCommandProperty); }
            set { SetValue(DeleteNiveauProtectionCommandProperty, value); }
        }

        public ObservableCollection<TypeEquipement> ListTypeEq 
        {
            get { return (ObservableCollection<TypeEquipement>)GetValue(ListTypeEqProperty); }
            set { SetValue(ListTypeEqProperty, value); }
        }

        #endregion Properties

        #region Dependency Properties

        public static readonly DependencyProperty IsEditModeProperty =
            DependencyProperty.Register("IsEditMode", typeof(Boolean), typeof(NiveauProtection), new PropertyMetadata(null));

        public static readonly DependencyProperty IsTypeEqVisibleProperty =
            DependencyProperty.Register("IsTypeEqVisible", typeof(Boolean), typeof(NiveauProtection), new PropertyMetadata(null));

        public static readonly DependencyProperty CanAddProperty =
            DependencyProperty.Register("CanAdd", typeof(Boolean), typeof(NiveauProtection), new PropertyMetadata(null));

        public static readonly DependencyProperty CanDeleteProperty =
            DependencyProperty.Register("CanDelete", typeof(Boolean), typeof(NiveauProtection), new PropertyMetadata(null));

        public static readonly DependencyProperty ListNiveauProtectionProperty =
            DependencyProperty.Register("ListNiveauProtection", typeof(ObservableCollection<MesNiveauProtection>), typeof(NiveauProtection), new PropertyMetadata(null));

        public static readonly DependencyProperty AddNiveauProtectionCommandProperty =
            DependencyProperty.Register("AddNiveauProtectionCommand", typeof(IActionCommand), typeof(NiveauProtection), new PropertyMetadata(null));

        public static readonly DependencyProperty DeleteNiveauProtectionCommandProperty =
            DependencyProperty.Register("DeleteNiveauProtectionCommand", typeof(IActionCommand), typeof(NiveauProtection), new PropertyMetadata(null));

        public static readonly DependencyProperty ListTypeEqProperty =
            DependencyProperty.Register("ListTypeEq", typeof(ObservableCollection<TypeEquipement>), typeof(NiveauProtection), new PropertyMetadata(null));

        #endregion Dependency Properties
    }
}
