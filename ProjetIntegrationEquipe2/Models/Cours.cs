using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.Models
{
    public class Cours
    {
        [Key]
        public int CoursId { get; set; }

        [Required]
        [Display(Name = "Nom du cours")]
        public string Nom { get; set; }

        [Required]
        [Display(Name = "Numéro du cours")]
        public string NumeroCours { get; set; }

        [ForeignKey("Matiere")]
        [Display(Name = "Matière")]
        [Required]
        public int MatiereId { get; set; }

        public Matiere Matiere { get; set; }

        //À ajouter, car il s'agit d'un lien plusieurs à plusieurs avec la propriété de la liste des cours de la table "OffreTutorat" !
        public List<OffreTutorat> ListeOffres { get; set; }

        //À ajouter, car il s'agit d'un lien plusieurs à plusieurs avec la propriété de la liste des cours de la table "DemandeTutorat" !
        public List<DemandeTutorat> ListeDemandes { get; set; }
    }
}
