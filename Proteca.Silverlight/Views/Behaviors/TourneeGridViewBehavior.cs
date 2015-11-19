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
using Telerik.Windows.DragDrop;
using Telerik.Windows.DragDrop.Behaviors;
using System.Collections;
using Proteca.Web.Models;
using System.Linq;
using System.Windows.Data;

namespace Proteca.Silverlight.Views.Behaviors
{


    public static class TourneeGridViewBehavior
    {


        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {

            obj.SetValue(IsEnabledProperty, value);

        }

        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(RadGridView),
            new PropertyMetadata(false, OnIsEnabledPropertyChanged));

        public static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            InitializeDragDropGrid((RadGridView)dependencyObject, (bool)e.NewValue);
        }

        #region IsAllowDropPortion

        public static readonly DependencyProperty IsAllowDropPortionProperty =
            DependencyProperty.RegisterAttached("IsAllowDropPortion", typeof(bool), typeof(TourneeGridViewBehavior),
            new PropertyMetadata(false, OnIsAllowDropPortionPropertyChanged));

        public static void OnIsAllowDropPortionPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            InitializeDragDropGrid((RadGridView)dependencyObject, (bool)e.NewValue);
        }

        public static bool GetIsAllowDropPortion(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAllowDropPortionProperty);
        }

        public static void SetIsAllowDropPortion(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAllowDropPortionProperty, value);
        }

        #endregion

        #region IsAllowDropOuvrage

        public static readonly DependencyProperty IsAllowDropOuvrageProperty =
            DependencyProperty.RegisterAttached("IsAllowDropOuvrage", typeof(bool), typeof(TourneeGridViewBehavior),
            new PropertyMetadata(new PropertyChangedCallback(OnIsAllowDropOuvragePropertyChanged)));

        public static void OnIsAllowDropOuvragePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            InitializeDragDropGrid((RadGridView)dependencyObject, (bool)e.NewValue);
        }

        public static bool GetIsAllowDropOuvrage(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAllowDropOuvrageProperty);
        }

        public static void SetIsAllowDropOuvrage(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAllowDropOuvrageProperty, value);
        }

        #endregion




        private static void InitializeDragDropGrid(RadGridView grid, bool attach)
        {
            DragDropManager.RemoveDragInitializeHandler(grid, OnDragInitialize);
            DragDropManager.RemoveGiveFeedbackHandler(grid, OnGiveFeedback);
            DragDropManager.RemoveDropHandler(grid, OnDrop);
            DragDropManager.RemoveDragOverHandler(grid, OnDragOver);
            grid.RowLoaded -= grid_RowLoaded;
            if (attach)
            {
                DragDropManager.AddDragInitializeHandler(grid, OnDragInitialize);
                DragDropManager.AddGiveFeedbackHandler(grid, OnGiveFeedback);
                DragDropManager.AddDropHandler(grid, OnDrop);
                DragDropManager.AddDragOverHandler(grid, OnDragOver);
                grid.RowLoaded += grid_RowLoaded;
            }
        }


        static void grid_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            if (e.Row is GridViewHeaderRow || e.Row is GridViewNewRow || e.Row is GridViewFooterRow)
                return;

            GridViewRow row = e.Row as GridViewRow;
            DragDropManager.RemoveDragOverHandler(row, OnRowDragOver);
            DragDropManager.AddDragOverHandler(row, OnRowDragOver);

        }


        private static void OnDragInitialize(object sender, DragInitializeEventArgs e)
        {
            RadGridView grid = (RadGridView)sender;
            var source = e.OriginalSource as FrameworkElement;
            if (source != null && source.Name != "PART_RowResizer")
            {
                DragDropIndicationDetail details = new DragDropIndicationDetail();
                var item = (sender as RadGridView).SelectedItem;
                details.CurrentDraggedItem = item;

                IDragPayload dragPayload = DragDropPayloadManager.GeneratePayload(null);

                dragPayload.SetData("DraggedItem", item);
                dragPayload.SetData("DropDetails", details);

                e.Data = dragPayload;

                e.DragVisual = new DragVisual()
                {
                    Content = details,
                    ContentTemplate = grid.Resources["DraggedItemTemplate"] as DataTemplate
                };
                e.DragVisualOffset = e.RelativeStartPoint;
                e.AllowedEffects = DragDropEffects.All;
            }
        }

        private static void OnGiveFeedback(object sender, Telerik.Windows.DragDrop.GiveFeedbackEventArgs e)
        {
            RadGridView grid = (RadGridView)sender;
            e.SetCursor(Cursors.Arrow);
            e.Handled = true;
        }

        private static void OnDrop(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            RadGridView grid = (RadGridView)sender;
            var draggedItem = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedItem");
            if (draggedItem == null)
            {
                draggedItem = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData");
            }
            var details = DragDropPayloadManager.GetDataFromObject(e.Data, "DropDetails") as DragDropIndicationDetail;

            if (details == null || draggedItem == null || (draggedItem is IList && (draggedItem as IList).Count == 0))
            {
                return;
            }

            if (sender is RadGridView && (e.Effects == DragDropEffects.Move || e.Effects == DragDropEffects.All))
            {
                ((sender as RadGridView).ItemsSource as IList).Remove(draggedItem);
            }

            if (e.Effects != DragDropEffects.None)
            {
                var collection = grid.ItemsSource as IList;
                int index = details.DropIndex < 0 ? 0 : details.DropIndex;
                index = details.DropIndex > collection.Count - 1 ? collection.Count : index;

                if (draggedItem is IList && (draggedItem as IList).Count > 0 && (draggedItem as IList)[0] is IOuvrage && collection.GetType().GetGenericArguments()[0] == typeof(Composition))
                {
                    foreach (var item in draggedItem as IList)
                    {
                        Composition co = new Composition();
                        if (item is EqEquipement)
                        {
                            co.EqEquipement = item as EqEquipement;
                        }
                        else if (item is Pp)
                        {
                            co.Pp = item as Pp;
                        }
                        collection.Insert(index++, co);
                    }
                }
                else if (draggedItem is IList && (draggedItem as IList).Count > 0 && (draggedItem as IList)[0] is GeoEnsElecPortion && collection.GetType().GetGenericArguments()[0] == typeof(Composition))
                {
                    foreach (var item in draggedItem as IList)
                    {
                        Composition co = new Composition();
                        co.ClePortion = (item as GeoEnsElecPortion).ClePortion;
                        collection.Insert(index++, co);
                    }
                }
                else
                {
                    collection.Insert(index, draggedItem);
                }
            }
        }

        private static void OnDragOver(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            RadGridView grid = (RadGridView)sender;
            var collection = grid.ItemsSource as IList;

            var draggedItem = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedItem");
            if (draggedItem == null)
            {
                draggedItem = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData");
            }
            Type draggedType;
            if (draggedItem is IList && (draggedItem as IList).Count > 0)
            {
                draggedItem = (draggedItem as IList)[0];
            }

            draggedType = draggedItem.GetType();

            BindingExpression binding = grid.GetBindingExpression(RadGridView.ItemsSourceProperty);

            var itemsType = (grid.ItemsSource as IList).AsQueryable().ElementType;

            if (draggedType != itemsType && !(((draggedItem is IOuvrage && GetIsAllowDropOuvrage(grid)) || (draggedItem is GeoEnsElecPortion && GetIsAllowDropPortion(grid))) && collection.GetType().GetGenericArguments()[0] == typeof(Composition)))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
            // interdit le drop de composition Ouvrage
            else if (draggedItem is Composition && ((draggedItem as Composition).CleEnsElectrique.HasValue || (draggedItem as Composition).ClePp.HasValue) && !GetIsAllowDropOuvrage(grid))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }

            //e.Handled = true;
        }

        private static void OnRowDragOver(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {

            var row = sender as GridViewRow;

            var grid = row.GetVisualParent<RadGridView>();

            var details = DragDropPayloadManager.GetDataFromObject(e.Data, "DropDetails") as DragDropIndicationDetail;

            if (details == null || row == null)
            {
                return;
            }

            details.CurrentDraggedOverItem = row.DataContext;

            if (details.CurrentDraggedItem == details.CurrentDraggedOverItem)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            details.CurrentDropPosition = GetDropPositionFromPoint(e.GetPosition(row), row);
            int dropIndex = (grid.Items as IList).IndexOf(row.DataContext) + 1;
            int draggedItemIdex = (grid.Items as IList).IndexOf(DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedItem"));

            if (dropIndex >= row.GridViewDataControl.Items.Count - 1 && details.CurrentDropPosition == DropPosition.After)
            {
                details.DropIndex = dropIndex;
                return;
            }

            dropIndex = draggedItemIdex > dropIndex ? dropIndex : dropIndex - 1;
            details.DropIndex = details.CurrentDropPosition == DropPosition.Before ? dropIndex : dropIndex + 1;
        }

        private static DropPosition GetDropPositionFromPoint(Point absoluteMousePosition, GridViewRow row)
        {
            if (row != null)
            {
                return absoluteMousePosition.Y < row.ActualHeight / 2 ? DropPosition.Before : DropPosition.After;
            }

            return DropPosition.Inside;
        }
    }
}
