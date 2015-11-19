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
using Telerik.Windows.Controls;
using System.Collections.Generic;
using Telerik.Windows.Controls.GridView;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Markup;
using Proteca.Silverlight.Views.UserContols.Providers;

namespace Proteca.Silverlight.Views.UserContols
{
    public class CustomGridView : RadGridView
    {
        #region Commands

        /// <summary>
        /// Commande indiquant des modifications sur des éléments du tableau
        /// </summary>
        public ICommand SelectedCellChangedCommand
        {
            get { return (ICommand)GetValue(SelectedCellChangedCommandProperty); }
            set { SetValue(SelectedCellChangedCommandProperty, value); }
        }

        public static readonly DependencyProperty SelectedCellChangedCommandProperty =
                               DependencyProperty.Register("SelectedCellChangedCommand", typeof(ICommand),
                               typeof(CustomGridView), new PropertyMetadata(null, null));

        #endregion Commands

        #region Constructor

        public CustomGridView() : base()
        {
            this.Sorting += new EventHandler<GridViewSortingEventArgs>(CustomGridView_Sorting);
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(CustomGridView_DataContextChanged);
            this.CellEditEnded += new EventHandler<GridViewCellEditEndedEventArgs>(CustomGridView_CellEditEnded);
            this.Loaded += CustomGridView_Loaded;
            this.KeyboardCommandProvider = new CustomGridViewKeyboardCommandProvider(this);
            this.ValidatesOnDataErrors = GridViewValidationMode.InViewMode;
            //this.DataLoaded += CustomGridView_DataLoaded;
            //Abandon de la généralisation du set du langage à cause de la répercution sur les colonnes éditables en numéric textbox
            //this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);
        }

        //void CustomGridView_DataLoaded(object sender, EventArgs e)
        //{
            
        //}

        void CustomGridView_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.Columns)
            {
                if (item is GridViewBoundColumnBase)
                {
                    ((GridViewBoundColumnBase)item).ValidatesOnDataErrors = GridViewValidationMode.None;
                }
            }
        }
 
        protected override void OnItemsSourceChanged(object oldValue, object newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (oldValue is IList && newValue is IList)
            {
                if (((IList)oldValue).Count > 0
                && ((IList)oldValue)[0] != null && ((IList)oldValue)[0] is Entity
                && ((Entity)((IList)oldValue)[0]).EntityState == EntityState.Detached)
                {
                    // remonter en haut de liste en rechargement
                    if (Items.Count > 0)
                    {
                        this.ScrollIntoViewAsync(Items[0], (fe) =>
                        {
                            this.CollapseAllGroups();
                        }
                        );
                    } 
                }
                else
                {
                    // En cas d'ajout => remonter en haut de la liste
                    if (((IList)oldValue).Count < ((IList)newValue).Count)
                    {
                        if (Items.Count > 0)
	                    {
                            this.ScrollIntoViewAsync(Items[0], (fe) =>
                                {
                                    this.CollapseAllGroups();
                                }
                            );
	                    }                        
                    }
                }
            }
        }

        #endregion Constructor

        #region Events

        void CustomGridView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Columns["IsNew"] == null && this.ItemsSource != null
            && this.ItemsSource.GetType().GetGenericArguments()[0].BaseType == typeof(Entity)
            && this.ItemsSource.GetType().GetGenericArguments()[0].GetProperties().Any(p=>p.Name == "IsNew"))
            {
                this.Columns.Insert(0, new GridViewDataColumn() { DataMemberBinding = new System.Windows.Data.Binding("IsNew"), IsVisible=false, IsCustomSortingEnabled=true });

                this.SortDescriptors.Add(new ColumnSortDescriptor()
                {
                    Column = this.Columns["IsNew"],
                    SortDirection = ListSortDirection.Descending,
                });
                this.Columns["IsNew"].SortingState = SortingState.Descending;
            }
        }

        void CustomGridView_Sorting(object sender, GridViewSortingEventArgs e)
        {
            if (this.Columns["IsNew"] != null)
            {
                e.Cancel = true;

                this.SortDescriptors.Clear();

                this.SortDescriptors.Add(new ColumnSortDescriptor()
                {
                    Column = this.Columns["IsNew"],
                    SortDirection = ListSortDirection.Descending,
                });
                this.Columns["IsNew"].SortingState = SortingState.Descending;

                this.SortDescriptors.Add(new ColumnSortDescriptor()
                {
                    Column = e.Column,
                    SortDirection = e.NewSortingState == SortingState.Descending ? ListSortDirection.Descending : ListSortDirection.Ascending,
                });
            }
            else if (e.NewSortingState == SortingState.None)
            {
                e.NewSortingState = SortingState.Ascending;
            }
        }

        /// <summary>
        /// Détecte la modification d'une cellule en édition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomGridView_CellEditEnded(object sender, EventArgs e)
        {
            if (SelectedCellChangedCommand != null)
            {
                SelectedCellChangedCommand.Execute(null);
            }
        }

        #endregion Events
    }
}
