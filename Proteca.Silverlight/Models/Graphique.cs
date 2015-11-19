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
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Jounce.Core.Model;
using Proteca.Silverlight.Resources;

namespace Proteca.Web.Models
{
    public class Graphique : BaseNotify
    {
        public String NoData 
        {
            get
            {
                return this.TitreY + " : " + Resource.Graphique_NoData.ToLower();
            }
        }

        private const int stepNumberY = 5;

        private List<SelectPortionGraphique_Result> _sourceMesures;
        public List<SelectPortionGraphique_Result> SourceMesures
        {
            get
            {
                if (_sourceMesures == null)
                {
                    _sourceMesures = new List<SelectPortionGraphique_Result>();
                }
                return _sourceMesures;
            }
            set
            {
                _sourceMesures = value;
                RaisePropertyChanged(() => this.SourceMesures);
                RaisePropertyChanged(() => this.IsOnlyOne);
                this.ReCalculY();
                RaisePropertyChanged(() => this.Maxi);
                RaisePropertyChanged(() => this.Moyen);
                RaisePropertyChanged(() => this.Mini);
            }
        }

        public Boolean IsOnlyOne
        {
            get
            {
                return SourceMesures.Select(s => s.PkTruncated).Distinct().Count() < 2;
            }
        }

        public List<Decimal> Abscisses 
        { 
            get
            {
                return SourceMesures.Select(s => s.PkTruncated).Distinct().OrderBy(d => d).ToList();
            }
        }

        private string _titreY;
        public String TitreY 
        { 
            get
            {
                return this._titreY;
            }
            set
            {
                this._titreY = value;
                RaisePropertyChanged(() => this.TitreY);
                RaisePropertyChanged(() => this.NoData);
            }
        }

        private void ReCalculY()
        {
            //Calcul des maximum et minimum de Y
            Double maxY = 0;
            Double minY = 0;
            Double stepY = 0;

            if (SourceMesures.Count > 0)
            {
                // Calcul du MaximumY
                maxY = Decimal.ToDouble(SourceMesures.Max(s => s.VALEUR));

                if (maxY != 0)
                {
                    int puissance = (int)(Math.Log10(Math.Abs(maxY)));

                    Double step = Math.Pow(10, puissance - 1);

                    if (step != 0)
                    {
                        Double reste = maxY % step;

                        maxY = (maxY > 0) ? maxY - reste + step : maxY - reste;
                    }
                }
                
                // Calcul du MinimumY
                minY = Decimal.ToDouble(SourceMesures.Min(s => s.VALEUR));

                if (minY != 0)
                {
                    int puissance = (int)(Math.Log10(Math.Abs(minY)));

                    Double step = Math.Pow(10, puissance - 1);

                    if (step != 0)
                    {
                        Double reste = minY % step;

                        minY = (minY < 0) ? minY - reste - step : minY - reste;
                    }
                }

                //Calcul du StepY
                if (maxY <= 0)
                {
                    stepY = Math.Abs((Math.Abs(maxY) - Math.Abs(minY)) / stepNumberY);
                }
                else
                {
                    stepY = (maxY - minY) / stepNumberY;
                }
            }
            MaximumY = maxY;
            MinimumY = minY;
            StepY = stepY;
        }

        private Double _maximumY;
        public Double MaximumY
        {
            get
            {
                return _maximumY;
            }
            set
            {
                _maximumY = value;
                RaisePropertyChanged(() => this.MaximumY);
            }
        }

        private Double _minimumY;
        public Double MinimumY
        {
            get
            {
                return _minimumY;
            }
            set
            {
                _minimumY = value;
                RaisePropertyChanged(() => this.MinimumY);
            }
        }

        private Double _stepY;
        public Double StepY
        {
            get
            {
                return _stepY;
            }
            set
            {
                _stepY = value;
                RaisePropertyChanged(() => this.StepY);
            }
        }

        public List<PointGraphique> Maxi
        {
            get
            {
                return this.InitPointGraphique("Maxi");
            }
        }

        public List<PointGraphique> Moyen
        {
            get
            {
                return this.InitPointGraphique("Moyen");
            }
        }

        public List<PointGraphique> Mini
        {
            get
            {
                return this.InitPointGraphique("Mini");
            }
        }

        private List<PointGraphique> InitPointGraphique(String ligne)
        {
            List<PointGraphique> listeRetour = new List<PointGraphique>();

            foreach (Decimal x in Abscisses)
            {
                PointGraphique p = new PointGraphique(){X = x};
                SelectPortionGraphique_Result element = SourceMesures.Where(t => t.NIVEAU == ligne && t.PkTruncated == x).FirstOrDefault();
                if (element != null)
                {
                    p.Y = element.VALEUR;
                }
                listeRetour.Add(p);
            }

            return listeRetour;
        }
    }
}
