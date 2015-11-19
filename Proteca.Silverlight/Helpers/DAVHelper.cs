using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Browser;
using System.Security;

namespace Proteca.Silverlight.Helpers
{
    public class AsyncCallbackParameters
    {
        public AsyncCallbackParameters ()
	    {
	    }
        public Action<Exception> Completed { get; set; }
        public HttpWebRequest Request { get; set; }
        public byte[] FileContent { get; set; }
    }


    public class DAVHelper
    {
        /// <summary>
        /// Fonction de téléchargement vers un SharePoint d'un fichier depuis Silverlight peu importe sa taille
        /// </summary>
        /// <param name="url">Url complète d'accès au fichier voulu (url du Sharepoint + chemin de dossiers + nom du fichier et extension)</param>
        /// <param name="fileContent">Contenu du fichier</param>
        /// <param name="completed">Action de CallBack</param>
        static public void UploadFile(string url, byte[] fileContent, Action<Exception> completed)
        {
            WebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);

            HttpWebRequest request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(new Uri(url));
            request.Method = "PUT";

            /* Make an asynchronous call for the request stream. The callback method will be called on a background thread. */
            IAsyncResult asyncResult = request.BeginGetRequestStream(
                new AsyncCallback(RequestStreamCallback), 
                new AsyncCallbackParameters() 
                { 
                    Request = request, 
                    FileContent = fileContent, 
                    Completed = completed 
                });
        }

        static private void RequestStreamCallback(IAsyncResult ar)
        {
            if (ar.AsyncState is AsyncCallbackParameters)
            {
                HttpWebRequest request = ((AsyncCallbackParameters)ar.AsyncState).Request as HttpWebRequest;
                Stream requestStream = request.EndGetRequestStream(ar);
                BinaryWriter streamWriter = new BinaryWriter(requestStream);

                // Write your file here.
                streamWriter.Write(((AsyncCallbackParameters)ar.AsyncState).FileContent);

                // Close the stream.
                streamWriter.Close();

                /* Make an asynchronous call for the response. The callback method will be called on a background thread. */
                request.BeginGetResponse(new AsyncCallback(ResponseCallback), ar.AsyncState);
            }
        }

        static private void ResponseCallback(IAsyncResult ar)
        {
            if (ar.AsyncState is AsyncCallbackParameters)
            {
                HttpWebRequest request = ((AsyncCallbackParameters)ar.AsyncState).Request as HttpWebRequest;
                WebResponse response = null;
                Exception exception = null;

                try
                {
                    response = request.EndGetResponse(ar);
                }
                catch (WebException webEx)
                {
                    exception = (Exception)webEx;
                }
                catch (SecurityException secEx)
                {
                    exception = (Exception)secEx;
                }

                finally
                {
                    // You may need to analyze the response to see if it succeeded.
                    ((AsyncCallbackParameters)ar.AsyncState).Completed(exception);
                }
            }
        }

    }
}
