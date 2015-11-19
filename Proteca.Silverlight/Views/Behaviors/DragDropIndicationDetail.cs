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
using System.Collections;

namespace Proteca.Silverlight.Views.Behaviors
{
    public class DragDropIndicationDetail : ViewModelBase
    {
        private object currentDraggedItem;
        private DropPosition currentDropPosition;
        private object currentDraggedOverItem;

        public object CurrentDraggedOverItem
        {
            get
            {
                return currentDraggedOverItem;
            }
            set
            {
                if (this.currentDraggedOverItem != value)
                {
                    currentDraggedOverItem = value;
                    OnPropertyChanged("CurrentDraggedOverItem");
                }
            }
        }

        public int DropIndex { get; set; }

        public DropPosition CurrentDropPosition
        {
            get
            {
                return this.currentDropPosition;
            }
            set
            {
                if (this.currentDropPosition != value)
                {
                    this.currentDropPosition = value;
                    OnPropertyChanged("CurrentDropPosition");
                    OnPropertyChanged("CurrentDropPositionStr");
                }
            }
        }

        public String CurrentDropPositionStr
        {
            get
            {
                switch (CurrentDropPosition)
                {
                    case DropPosition.After:
                        return "Après";
                    case DropPosition.Before:
                        return "Avant";
                    case DropPosition.Inside:
                        return "Dans";
                    default:
                        return "";
                }
            }
        }

        public object CurrentDraggedItem
        {
            get
            {
                return this.currentDraggedItem;
            }
            set
            {
                if (this.currentDraggedItem != value)
                {
                    this.currentDraggedItem = value;
                    OnPropertyChanged("CurrentDraggedItem");
                    OnPropertyChanged("CurrentDraggedItemStr");
                }
            }
        }

        public object CurrentDraggedItemStr
        {
            get
            {
                if (this.currentDraggedItem is IList)
                {
                    String retour = String.Empty;
                    foreach (object item in (this.currentDraggedItem as IList))
                    {
                        if (String.IsNullOrEmpty(retour))
                        {
                            retour += ", ";
                        }
                        retour += item.ToString();
                    }

                    return retour;
                }
                return this.currentDraggedItem.ToString();
            }
        }
    }
}
