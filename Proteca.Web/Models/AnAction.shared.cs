using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Proteca.Web.Models
{
    public partial class AnAction
    {

        public static UsrUtilisateur CurrentUser { get; set; }

        
        /// <summary>
        /// Ce champ est utilisé lors de l'enregistrement de l'action
        /// </summary>
        public string CodeRegion
        {
            get;
            set;
        }

        /// <summary>
        /// Ce champ doit être calculé et renseigné au chargement de l'action
        /// </summary>
        private Nullable<int> _CleRegion;

        [RequiredCustomAction]
        [DataMember()]
        public Nullable<int> CleRegion
        {
            get
            {
                return _CleRegion;
            }
            set
            {
                Nullable<int> previous = _CleRegion;
                if ((previous != value))
                {
                    #if SILVERLIGHT
                    this.ValidateProperty("CleRegion", value);
                    #else
                    #endif

                    _CleRegion = value;

                     #if SILVERLIGHT
                    this.RaisePropertyChanged("CleRegion");
                    #else
                    #endif
                }
            }
        }

        private Nullable<int> _CleEnsembleElec;
        [DataMember()]
        [RequiredCustomAction]
        public Nullable<int> CleEnsembleElec
        {
            get
            {
                return _CleEnsembleElec;
            }
            set
            {
                Nullable<int> previous = _CleEnsembleElec;
              
                if (previous != value)
                {
#if SILVERLIGHT
                    this.ValidateProperty("CleEnsembleElec", value);
#else
#endif

                    _CleEnsembleElec = value;

#if SILVERLIGHT
                    this.RaisePropertyChanged("CleEnsembleElec");
#else
#endif
                }
            }
        }

        public void ForceValidationOnCleEnsembleElec(int? value)
        {
            #if SILVERLIGHT
            this.ValidateProperty("CleEnsembleElec", value);
            #else
            #endif
        }
    }
}