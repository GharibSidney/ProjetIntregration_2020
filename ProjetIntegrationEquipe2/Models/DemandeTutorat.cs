using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.Models
{
    public class DemandeTutorat
    {
        public DemandeTutorat()
        {
            ListeDeCours = new List<Cours>();
            ListePlageHoraire = new List<PlageHoraire>();
        }
        [Key]
        public int DemandeTutoratId { get; set; }

        public string Titre { get; set; }

        [ForeignKey("Tutore")]
        [Required]
        public string TutoreId { get; set; }

        public Tutore Tutore { get; set; }

        public List<Cours> ListeDeCours { get; set; }

        public List<PlageHoraire> ListePlageHoraire { get; set; }

    }
}