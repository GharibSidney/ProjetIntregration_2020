using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Statistiques
{
    public class StatsPaiementsVM
    {
        public int nbPaiementsAttente { get; set;}

        public int nbPaiementsValides { get; set; }

        public int nbPaiementsRefuses { get; set; }
    }
}