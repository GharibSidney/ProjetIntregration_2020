using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Statistiques
{
    public class StatistiqueGeneraleVM
    {
        public int NbTuteurs { get; set; }

        public int NbTutores { get; set; }

        public int NbProgramme { get; set; }

        public int NbCours { get; set; }

        public int NbDemandes { get; set; }

        public int NbOffres { get; set; }

        public int NbTutorats { get; set; }

        public int NbTutoratsActif { get; set; }

    }
}