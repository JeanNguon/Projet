using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Proteca.Sharepoint.SilverlightWebPart
{
    public partial class SilverlightWebPartUserControl : UserControl
    {
        /// <summary>
        /// Edition du InitParams du composant Silverlight
        /// </summary>
        public string InitParams { get; set; }
        
        /// <summary>
        /// Chemin vers le fichier XAP
        /// </summary>
        public string XAPFilePath { get; set; }

        /// <summary>
        /// Hauteur du XAP
        /// </summary>
        public string SLHeight { get; set; }

        /// <summary>
        /// Largeur du XAP
        /// </summary>
        public string SLWidth { get; set; }

        /// <summary>
        /// Chargement du UserControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
