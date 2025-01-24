using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Tuteurs
{
    public class TuteursViewOnlyVM
    {
        public List<Tuteur> listeTuteursVM { get; set; }

        public List<CoursMaitrise> listeCoursMaitriseVM { get; set; }
    }
}