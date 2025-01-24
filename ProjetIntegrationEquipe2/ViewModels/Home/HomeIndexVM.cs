using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels
{
    public class HomeIndexVM
    {
        public List<Tuteur> listeTuteursVM { get; set; }
        public List<string> listeIntrosTuteur { get; set; }
    }
}
