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

namespace Proteca.Silverlight.ViewModels
{
    public partial class DetailPPViewModel
    {
        partial void SetCleUtiDdeDeverrouillage()
        {
            if (SelectedEntity.LastVisite.CleUtilisateurMesure != 0)
            {
                SelectedEntity.CleUtiDdeDeverrouillage = SelectedEntity.LastVisite.CleUtilisateurMesure;
            }
            else
            {
                //MANTIS-21144 - Correction pour éviter le plantage lorsque l'utilisateur de la denière visite est un utilisateur externe (et donc avec une clé = 0)
                SelectedEntity.UsrUtilisateur1 = SelectedEntity.LastVisite.UsrUtilisateur2;
            }
        }

        partial void SetNeedCheckUser()
        {
            NeedCheckUser = false;
        }
    }
}
