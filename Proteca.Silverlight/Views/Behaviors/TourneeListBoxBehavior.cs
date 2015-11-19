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
using System.Collections.Generic;
using Telerik.Windows.DragDrop;
using Telerik.Windows.DragDrop.Behaviors;
using Telerik.Windows.Controls;
using System.Collections;
using System.Linq;

namespace Proteca.Silverlight.Views.Behaviors
{
    public class TourneeListBoxBehavior
    {
        private RadListBox _associatedObject;
		/// <summary>
		/// AssociatedObject Property
		/// </summary>
        public RadListBox AssociatedObject
		{
			get
			{
				return _associatedObject;
			}
			set
			{
				_associatedObject = value;
			}
		}

        private static Dictionary<RadListBox, TourneeListBoxBehavior> instances = new Dictionary<RadListBox, TourneeListBoxBehavior>();


		public static bool GetIsEnabled(DependencyObject obj)
		{
		    var res = false;
		    if (obj != null)
		    {
		        res = (bool) obj.GetValue(IsEnabledProperty);
		    }
		    return res;
		}

		public static void SetIsEnabled(DependencyObject obj, bool value)
		{
		    var listBox = obj as RadListBox;
		    if (obj != null)
		    {
		        TourneeListBoxBehavior behavior = GetAttachedBehavior(listBox);

		        behavior.AssociatedObject = listBox;

		        if (value)
		        {
		            behavior.Initialize();
		        }
		        else
		        {
		            behavior.CleanUp();
		        }
		        obj.SetValue(IsEnabledProperty, value);
		    }
		}

		// Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(TourneeListBoxBehavior),
				new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));

		public static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			SetIsEnabled(dependencyObject, (bool)e.NewValue);
		}

        //Création d'une DependencyProperty pour afficher ou non les infos du DragAndDrop
        public static readonly DependencyProperty InfosHiddenProperty =
            DependencyProperty.RegisterAttached("InfosHidden", typeof(bool), typeof(TourneeListBoxBehavior),
                new PropertyMetadata(false, new PropertyChangedCallback(OnInfosHiddenPropertyChanged)));

        private static void OnInfosHiddenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetInfosHidden(d, (bool)e.NewValue);
        }

        public static void SetInfosHidden(DependencyObject obj, bool value)
        {
            obj.SetValue(InfosHiddenProperty, value);
        }

        public static bool GetInfosHidden(DependencyObject obj)
        {
            return (bool)obj.GetValue(InfosHiddenProperty);
        }

        private static TourneeListBoxBehavior GetAttachedBehavior(RadListBox listBox)
		{
			if (!instances.ContainsKey(listBox))
			{
				instances[listBox] = new TourneeListBoxBehavior();
				instances[listBox].AssociatedObject = listBox;
			}

			return instances[listBox];
		}

		protected virtual void Initialize()
		{
			this.UnsubscribeFromDragDropEvents();
			this.SubscribeToDragDropEvents();
		}

		protected virtual void CleanUp()
		{
			this.UnsubscribeFromDragDropEvents();
		}

		private void SubscribeToDragDropEvents()
		{
			DragDropManager.AddDragInitializeHandler(this.AssociatedObject, OnDragInitialize);
			DragDropManager.AddGiveFeedbackHandler(this.AssociatedObject, OnGiveFeedback);
			DragDropManager.AddDropHandler(this.AssociatedObject, OnDrop);
			DragDropManager.AddDragDropCompletedHandler(this.AssociatedObject, OnDragDropCompleted);
			DragDropManager.AddDragOverHandler(this.AssociatedObject, OnDragOver);
		}

		private void UnsubscribeFromDragDropEvents()
		{
			DragDropManager.RemoveDragInitializeHandler(this.AssociatedObject, OnDragInitialize);
			DragDropManager.RemoveGiveFeedbackHandler(this.AssociatedObject, OnGiveFeedback);
			DragDropManager.RemoveDropHandler(this.AssociatedObject, OnDrop);
			DragDropManager.RemoveDragDropCompletedHandler(this.AssociatedObject, OnDragDropCompleted);
			DragDropManager.RemoveDragOverHandler(this.AssociatedObject, OnDragOver);
		}

		private void OnDragInitialize(object sender, DragInitializeEventArgs e)
		{
			DragDropIndicationDetail details = new DragDropIndicationDetail();
            var items = (sender as RadListBox).SelectedItems;
			details.CurrentDraggedItem = items;

			IDragPayload dragPayload = DragDropPayloadManager.GeneratePayload(null);

            dragPayload.SetData("DraggedData", items);
			dragPayload.SetData("DropDetails", details);

			e.Data = dragPayload;

            //Si les infos du drag&drop ne doivent pas être cachées
            if (GetInfosHidden(sender as DependencyObject).Equals(false))
            {
                e.DragVisual = new DragVisual()
                {
                    Content = details,
                    ContentTemplate = this.AssociatedObject.Resources["DraggedItemTemplate"] as DataTemplate
                };
            }
            else 
            {
                e.DragVisual = new DragVisual();
            }
            
            e.DragVisualOffset = new Point(e.RelativeStartPoint.X + 20, e.RelativeStartPoint.Y + 20);
            e.AllowedEffects = DragDropEffects.All;
		}

		private void OnGiveFeedback(object sender, Telerik.Windows.DragDrop.GiveFeedbackEventArgs e)
		{
			e.SetCursor(Cursors.Arrow);
			e.Handled = true;
		}

		private void OnDragDropCompleted(object sender, Telerik.Windows.DragDrop.DragDropCompletedEventArgs e)
		{
			var draggedItem = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData");

			if (e.Effects != DragDropEffects.None)
			{
                var collection = (sender as RadListBox).ItemsSource as IList;

                var items = new List<object>(draggedItem as IEnumerable<object>);

                foreach (var item in items)
                {
                    collection.Remove(item);
                }		
			}
		}

		private void OnDrop(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
		{
			var draggedItem = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData");
			var details = DragDropPayloadManager.GetDataFromObject(e.Data, "DropDetails") as DragDropIndicationDetail;

			if (details == null || draggedItem == null)
			{
				return;
			}

			if (e.Effects != DragDropEffects.None)
			{
                var collection = (sender as RadListBox).ItemsSource as IList;
				collection.Add(draggedItem);			
			}
		}

		private void OnDragOver(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
		{
			var draggedItem = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData");
            var draggedType = (draggedItem as IList).AsQueryable().ElementType;
			var itemsType = (this.AssociatedObject.ItemsSource as IList).AsQueryable().ElementType;


            if (draggedType != itemsType)
			{
				e.Effects = DragDropEffects.None;
			}

			var dropDetails = DragDropPayloadManager.GetDataFromObject(e.Data, "DropDetails") as DragDropIndicationDetail;
			dropDetails.CurrentDraggedOverItem = this.AssociatedObject;
			dropDetails.CurrentDropPosition = DropPosition.Inside;

			e.Handled = true;
		}
    }
}
