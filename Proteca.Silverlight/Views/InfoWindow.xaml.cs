namespace Proteca.Silverlight
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Net;
    using Proteca.Silverlight.Resources;
    
    /// <summary>
    /// <see cref="ChildWindow"/> class that displays errors to the user.
    /// </summary>
    public partial class InfoWindow : ChildWindow
    {
        /// <summary>
        /// Creates a new <see cref="InfoWindow"/> instance.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        /// <param name="errorDetails">Extra information about the error.</param>
        protected InfoWindow(string message)
        {
            InitializeComponent();
            IntroductoryText.Text = message;
        }
        
        #region Factory Methods
        /// <summary>
        /// All other factory methods will result in a call to this one.
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="stackTrace">The associated stack trace</param>
        /// <param name="policy">The situations in which the stack trace should be appended to the message</param>
        /// <param name="type">Type window to display</param>
        public static InfoWindow CreateNew(string message)
        {

            InfoWindow window = new InfoWindow(message);
            window.Show();
            return window;
        }
        #endregion

        #region Factory Helpers

        /// <summary>
        /// Creates a user-friendly message given an Exception. 
        /// Currently this method returns the Exception.Message value. 
        /// You can modify this method to treat certain Exception classes differently.
        /// </summary>
        /// <param name="e">The exception to convert.</param>
        private static string ConvertExceptionToMessage(Exception e)
        {
            return e.Message;
        }
        #endregion

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}