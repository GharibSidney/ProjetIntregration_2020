using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Demandes
{
    public class DemandeCreateVM
    {
        public List<Cours> ListeDeTousLesCours { get; set; }
        [Required]
        [Display(Name = "*Cours demandé(s)")]
        public List<int> ListeCoursSelectionnesId { get; set; }

        [Display(Name = "Matière(s)")]
        public List<int> ListeMatiereSelectionnesId { get; set; }

        public List<Matiere> ListeDeToutesLesMatieres { get; set; }

        [StringLength(35)]
        public string Titre { get; set; }

        [DatetimeValidation]
        public List<DateTime> ListeDateTimes { get; set; }
    }
}