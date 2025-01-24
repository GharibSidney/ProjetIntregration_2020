using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.ApprouverCours
{
    public class ApprouverCoursIndexVM
    {
        public List<Tuteur> TuteurList { get; set; }
        public List<CoursMaitrise> CoursMaitriseList { get; set; }
        public List<string> ListeStatusCoursMaitrise { get; set; }
    }
}