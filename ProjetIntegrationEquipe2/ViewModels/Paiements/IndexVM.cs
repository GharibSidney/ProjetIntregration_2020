using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Paiements
{
    public class IndexVM
    {
        public dynamic ClePublic { get; set; }

        public double PrixTotal { get; set; }

        public int TutoratId { get; set; }

        public string Courriel { get; set; }

        public double Heure { get; set; } 

        public double Tarif { get; set; }

        public List<PlageHoraire> ListePlageHoraire { get; set; }
    }
}