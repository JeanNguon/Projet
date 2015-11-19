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

namespace Proteca.Silverlight.Helpers
{
    public class CustomLocalizationManager : LocalizationManager
    {
        public override string GetStringOverride(string key)
        {
            switch (key)
            {
                case "Documents_RadRichTextBox_HtmlPrintPreview_Print":
                    return "Imprimer";
                case "Documents_RadRichTextBox_HtmlPrintPreview_Close":
                    return "Fermer";
            }
            return base.GetStringOverride(key);
        }
    }
}
