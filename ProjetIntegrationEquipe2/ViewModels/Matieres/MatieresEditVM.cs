using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Matieres
{
    public class MatieresEditVM
    {
        [Display(Name = "Nom de la matière")]
        [Required(ErrorMessage = "Le Nom est requis.")]
        [StringLength(30)]
        public string Nom { get; set; }

        public int MatiereId { get; set; }
    }
}