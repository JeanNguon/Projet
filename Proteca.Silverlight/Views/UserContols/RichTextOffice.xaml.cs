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
using Proteca.Silverlight.Helpers;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections;
using Telerik.Windows.Controls;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class RichTextOffice : UserControl, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Public Properties

        /// <summary>
        /// Retourne le texte
        /// </summary>
        public string RichText
        {
            get { return (string)GetValue(RichTextProperty); }
            set { SetValue(RichTextProperty, value); }
        }


        private string _errorMessage = String.Empty;
        /// <summary>
        /// Retourne le texte
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage;}
            set 
            { 
                _errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty RichTextProperty =
            DependencyProperty.Register("RichText", typeof(string), typeof(RichTextOffice), new PropertyMetadata(null, OnIsEditModeChanged));

        private static void OnIsEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArg)
        {
            ((RichTextOffice)d).NotifyErrorsChanged("RichText");
        }

        #endregion

        #region Constructeur

        public RichTextOffice()
        {
            InitializeComponent();
            this.BindingValidationError += RichTextOffice_BindingValidationError;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            IEnumerable returnValue = null;

            if (propertyName == "RichText")
            {
                if (Validation.GetErrors(this).Count == 0)
                {
                    ErrorMessage = String.Empty;
                }
                else
                {
                    ErrorMessage = Validation.GetErrors(this).First().ErrorContent.ToString();
                }

                if (String.IsNullOrEmpty(ErrorMessage))
                {
                    returnValue = null;
                }
                else
                {
                    returnValue = new List<String> { ErrorMessage };
                }
            }

            return returnValue;
        }

        public bool HasErrors
        {
            get { return Validation.GetErrors(this).Any(); }
        }

        public void NotifyErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, new System.ComponentModel.DataErrorsChangedEventArgs(propertyName));
            }
        }

        void RichTextOffice_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            this.NotifyErrorsChanged("RichText");
        }

        #endregion Events
    }
}
