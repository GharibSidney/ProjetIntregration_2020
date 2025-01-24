using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.AjouterCours
{
    public class AjouterCoursEditVM
    {
        public List<Matiere> MatiereList { get; set; }

        [Display(Name = "Matière")]
        public int MatiereID { get; set; }

        [Display(Name = "Nom du cours")]
        [Required(ErrorMessage = "Le Nom est requis.")]
        [StringLength(60)]
        public string Nom { get; set; }

        [Display(Name = "Le numéro du cours")]
        [Required(ErrorMessage = "Le Numéro du cours est requis.")]
        [StringLength(35)]
        public string NumeroCours { get; set; }

        public int CoursId { get; set; }
    }
}