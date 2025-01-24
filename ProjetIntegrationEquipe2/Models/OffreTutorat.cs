using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.Models
{
    public class OffreTutorat
    {
        public OffreTutorat()
        {
            ListeDeCours = new List<Cours>();
            ListePlageHoraire = new List<PlageHoraire>();
        }

        [Key]
        public int OffreId { get; set; }

        public string Titre { get; set; }

        [ForeignKey("Tuteur")]
        [Required]
        public string TuteurId { get; set; }

        public Tuteur Tuteur { get; set; }

        public List<Cours> ListeDeCours { get; set; }

        public List<PlageHoraire> ListePlageHoraire { get; set; }

    }
}
