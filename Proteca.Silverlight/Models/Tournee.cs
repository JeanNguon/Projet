using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;
using Proteca.Silverlight.Resources;
using System.Collections.Generic;
using System;

namespace Proteca.Web.Models
{
    public partial class Tournee
    {
        #region Gestion de la propagation des modifications des EntityCollection

        public Boolean HasChildChanges
        {
            get { return ChildChanges.Any(); }
        }

        private List<String> _childChanges;
        public List<String> ChildChanges
        {
            get
            {
                if (_childChanges == null)
                {
                    _childChanges = new List<String>();
                }
                return _childChanges;
            }
            set { _childChanges = value; }
        }

        public void RaiseAnyDataMemberChanged(string prop)
        {
            if (prop == "Compositions")
            {
                if (!ChildChanges.Contains(prop))
                {
                    ChildChanges.Add(prop);
                    //this.RaiseDataMemberChanged(prop);
                }
            }
        }

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "EntityState" && this.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Unmodified)
            {
                ChildChanges.Clear();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// 
        /// </summary>
        public NotifyCollectionChangedEventHandler OnCompositionPortionChanged;

        /// <summary>
        /// 
        /// </summary>
        public NotifyCollectionChangedEventHandler OnCompositionEqChanged;

        #endregion

        /// <summary>
        /// Retourne les compositions d'ensemble électrique associés
        /// </summary>
        public ObservableCollection<Composition> CompositionEEs
        {
            get
            {
                return new ObservableCollection<Composition>(this.Compositions.Where(c => c.CleEnsElectrique.HasValue));
            }
        }

        public void ForceRaiseCompositionEEs()
        {
            this.RaisePropertyChanged("CompositionEEs");
        }

        /// <summary>
        /// Si la tournée a été supprimée on affiche un message sinon rien
        /// </summary>
        public string InfosTournee
        {
            get
            {
                if (this.Supprime)
                {
                    return Resource.Tournee_Supprimee;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Retourne les compositions de portion associées dans son état ou il a été appelé précédemment
        /// </summary>
        public ObservableCollection<Composition> BkpCompositionPortions { get; set; }

        /// <summary>
        /// Retourne les compositions de portion associées
        /// </summary>
        public ObservableCollection<Composition> CompositionPortions
        {
            get
            {
                if (BkpCompositionPortions != null)
                {
                    BkpCompositionPortions.CollectionChanged -= new NotifyCollectionChangedEventHandler(BkpCompositionPortions_CollectionChanged);
                }

                BkpCompositionPortions = new ObservableCollection<Composition>(this.Compositions.Where(c => c.ClePortion.HasValue).OrderBy(c => c.PortionIntegrite.Libelle));
                BkpCompositionPortions.CollectionChanged += new NotifyCollectionChangedEventHandler(BkpCompositionPortions_CollectionChanged);

                return BkpCompositionPortions;
            }
        }

        public void ForceRaiseCompositionPortions()
        {
            this.RaisePropertyChanged("CompositionPortions");
        }

        private void BkpCompositionPortions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int index = 0;
            foreach (Composition co in BkpCompositionPortions)
            {
                co.NumeroOrdre = index++;
            }

            RaisePropertyChanged("Cout");
            RaisePropertyChanged("NbHeures");

            if (OnCompositionPortionChanged != null)
            {
                OnCompositionPortionChanged(sender, e);
            };
        }

        ///// <summary>
        ///// Retourne les compositions d'équipement associés dans son état ou il a été appelé précédemment
        ///// </summary>
        //public ObservableCollection<Composition> BkpCompositionEqs { get; set; }


        private ObservableCollection<Composition> _CompositionEqs;
        /// <summary>
        /// Retourne les compositions d'équipement associés
        /// </summary>
        public ObservableCollection<Composition> CompositionEqs
        {
            get
            {
                if (_CompositionEqs == null)
                {
                    ResetCompositionEqsByOrder();
                }
                return _CompositionEqs;
                //BkpCompositionEqs = new ObservableCollection<Composition>(this.Compositions.Where(c => !c.HasDeleted && (c.ClePp.HasValue || c.CleEquipement.HasValue)).OrderBy(c=>c.NumeroOrdre));
                //BkpCompositionEqs.CollectionChanged += new NotifyCollectionChangedEventHandler(BkpCompositionEqs_CollectionChanged);
                //return BkpCompositionEqs;
            }
            set
            {
                if (_CompositionEqs != null)
                    _CompositionEqs.CollectionChanged -= new NotifyCollectionChangedEventHandler(CompositionEqs_CollectionChanged);
                _CompositionEqs = value;
                if (_CompositionEqs != null)
                    _CompositionEqs.CollectionChanged += new NotifyCollectionChangedEventHandler(CompositionEqs_CollectionChanged);
            }
        }
        public void ResetCompositionEqsByOrder()
        {
            CompositionEqs = new ObservableCollection<Composition>(this.Compositions.Where(c => !c.HasDeleted && (c.ClePp.HasValue || c.CleEquipement.HasValue)).OrderBy(c => c.NumeroOrdre));
        }
        public bool MovingItems { get; set; }
        private void CompositionEqs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!MovingItems)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems)
                    {
                        this.Compositions.Add(item as Composition);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in e.OldItems)
                    {
                        this.Compositions.Remove(item as Composition);
                    }
                }

                int index = 0;
                foreach (Composition co in _CompositionEqs)
                {
                    co.NumeroOrdre = index++;
                }

                if (OnCompositionEqChanged != null)
                {
                    OnCompositionEqChanged(sender, e);
                };
            }
        }

        public void ForceRaiseCompositionEqs()
        {
            ResetCompositionEqsByOrder();
            this.RaisePropertyChanged("CompositionEqs");
        }

        /// <summary>
        /// Url de l'élément courant
        /// </summary>
        public string NaviagtionUrl
        {
            get
            {
                return string.Format("/{0}/{1}/Id={2}",
                   MainNavigation.Visite.GetStringValue(),
                   VisiteNavigation.Tournee.GetStringValue(),
                   CleTournee);
            }
        }

    }
}
