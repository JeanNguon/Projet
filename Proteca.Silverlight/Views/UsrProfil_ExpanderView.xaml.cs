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
using System.Windows.Navigation;
using Jounce.Core.View;
using Jounce.Regions.Core;
using Jounce.Core.ViewModel;
using System.ComponentModel.Composition;
using System.ComponentModel;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("UsrProfil_Expander")]
    [ExportViewToRegion("UsrProfil_Expander", "ExpanderContainer")]
    public partial class UsrProfil_ExpanderView : Page, INotifyPropertyChanged
    {
        public Double MaxDropHeight
        {
            get { return ProfilExpanderGrid.RenderSize.Height - 60; }
        }

        public UsrProfil_ExpanderView()
        {
            InitializeComponent();
            ProfilExpanderGrid.SizeChanged += new SizeChangedEventHandler(ProfilExpanderGrid_SizeChanged);
        }

        void ProfilExpanderGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RaisePropertyChanged("MaxDropHeight");
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("UsrProfil", "UsrProfil_Expander");
            }
        }


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
