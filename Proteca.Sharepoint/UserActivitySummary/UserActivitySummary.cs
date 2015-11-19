using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Proteca.Sharepoint.Utilities;
using System.Linq;

namespace Proteca.Sharepoint.UserActivitySummary
{
    [ToolboxItemAttribute(false)]
    public class UserActivitySummary : WebPart
    {
        class UnlockingRequest
        {
            public int Key;
            public string Label;
            public string UserName;
            public DateTime Date;
        }

        class ActionToPlan
        {
            public int key;
            public string ActionNum;
            public string Label;
            public string Creator;
            public DateTime Date;
        };

        List<ActionToPlan> _actionsToPlan = new List<ActionToPlan>();
        List<UnlockingRequest> _unlockingRequests = new List<UnlockingRequest>();
        int _delais = 0;
        int _nb_eq_to_validate = 0;

        private void FetchData()
        {
            SqlConnection conn = new SqlConnection(SecurityStoreHelper.GetDBConnectionString());
            SqlDataReader reader = null;
            try
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[SP_GET_VISITE_TO_VALIDATE]";
                cmd.Parameters.Add("@identifiant", SqlDbType.VarChar, 10).Value = SPContext.Current.Web.CurrentUser.LoginName.Split(new string[]{"\\"}, StringSplitOptions.None)[1];
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    _delais = (int)reader["DELAIS"];
                }
                reader.Close();
                ////
                cmd.CommandText = "[dbo].[SP_GET_DDE_DEVERROUILLAGE]";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    _unlockingRequests.Add(new UnlockingRequest
                    {
                        Key = (int)reader["CLE_PP"],
                        Label = (string)reader["LIBELLE"],
                        UserName = (string)reader["USERNAME"],
                        Date = (DateTime)reader["DATE_DDE_DEVERROUILLAGE_COORD_GPS"]
                    });
                }
                reader.Close();
                cmd.CommandText = "[dbo].[SP_GET_ACTION_TO_PLAN]";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    _actionsToPlan.Add(new ActionToPlan
                    {
                        key = (int)reader["CLE_ACTION"],
                        ActionNum = (string)reader["NUM_ACTION_PC"],
                        Label = (string)reader["LIBELLE"],
                        Creator = (string)reader["USERNAME"],
                        Date = (DateTime)reader["DATE_CREATION"]
                    });
                }
                reader.Close();

                cmd.CommandText = "[dbo].[SP_GET_EQUIPEMENT_TO_VALIDATE]";
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    _nb_eq_to_validate = (int)reader["NB_EQ"];
                }
                reader.Close();
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }


        protected override void RenderContents(HtmlTextWriter writer)
        {
            FetchData();
            writer.Write("<div class='user_act_sum'>");
            if (_delais > 0)
            {
                writer.Write("<div class='section_title'>");
                writer.Write(string.Format("<a href='/Pages/proteca.aspx#/Visite/ValidationVisite'>Vous avez des visites à valider.</a><br/>A défaut de validation sous {0} jours les données seront validées d’office.<br/><br/>", _delais));
                writer.Write("</div>");
            }
            ////////
            if (_nb_eq_to_validate > 0)
            {
                writer.Write("<div class='section_title'>");
                writer.Write(string.Format("<a href='/Pages/proteca.aspx#/Visite/ValidationEquipement'>Vous avez des équipements à valider.</a><br/><br/>", _nb_eq_to_validate));
                writer.Write("</div>");
            }
            ////////
            writer.Write("<div class='section_title'>");
            if (_unlockingRequests.Any())
            {
                writer.Write("Demande de déverrouillage :");
            }
            else
            {
                writer.Write("Aucune demande de déverrouillage.<br/><br/>");
            }
            writer.Write("</div>");

            if (_unlockingRequests.Any())
            {
                writer.Write("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                writer.Write("<th>PP</th>");
                writer.Write("<th>Demandé par</th>");
                writer.Write("<th>Date</th>");
                int index = 0;
                foreach (UnlockingRequest req in _unlockingRequests)
                {
                    String ligneClass = "ligne" + (index++ % 2).ToString();
                    writer.Write(string.Format("<tr class=\"{0}\">", ligneClass));
                    writer.Write(string.Format("<td><a href='/Pages/proteca.aspx#/ouvrages/Equipement/PP/Id={0}'>{1}</a></td>", req.Key, req.Label));
                    writer.Write(string.Format("<td>{0}</td>", req.UserName));
                    writer.Write(string.Format("<td>{0}</td>", req.Date.ToString("dd/MM/yyyy")));
                    writer.Write("</tr>");
                }
                writer.Write("</table><br/><br/>");
            }

            ////////
            
            writer.Write("<div class='section_title'>");
            if (_actionsToPlan.Any())
            {
                writer.Write("Actions à prendre en compte :");
            }
            else
            {
                writer.Write("Aucune action à planifier.<br/><br/>");
            }            
            writer.Write("</div>");

            if (_actionsToPlan.Any())
            {
                writer.Write("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                writer.Write("<th>Action n°</th>");
                writer.Write("<th>Créée par</th>");
                writer.Write("<th>Date</th>");
                int index = 0;
                foreach (ActionToPlan act in _actionsToPlan)
                {
                    String ligneClass = "ligne" + (index++ % 2).ToString();
                    writer.Write(string.Format("<tr class=\"{0}\">", ligneClass));
                    writer.Write(string.Format("<td><a href='/Pages/proteca.aspx#/Visite/FicheAction/Id={0}'>{1}</a></td>", act.key, act.ActionNum));
                    writer.Write(string.Format("<td>{0}</td>", act.Creator));
                    writer.Write(string.Format("<td>{0}</td>", act.Date.ToString("dd/MM/yyyy")));
                    writer.Write("</tr>");
                }
                writer.Write("</table><br/><br/>");                
            }

            writer.Write("</div>");
        }

    }
}
