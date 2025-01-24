using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Tuteurs
{
    public class TutoreVersTuteurVM
    {

        [Required]
        [Range(0.00, 100.00, ErrorMessage = "Le salaire doit être entre 0 et 100")]
        [Display(Name = "*Tarif voulu")]
        public double Tarif { get; set; }

        public List<Matiere> ListeMatieres { get; set; }

        [Display(Name = "Matières offertes")]
        public List<int> ListeMatieresSelectionnesId { get; set; }

        public List<Cours> ListeCoursDemandes { get; set; }

        //REQUIRED DEMANDE POUR NE PAS CREER UN TUTEUR SANS COURS CHOISI
        [Required]
        [Display(Name = "*Cours demandé(s)")]
        public List<int> ListeCoursSelectionnesId { get; set; }
    }
}
