using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.Models
{
    public class Tutorat
    {

        public Tutorat()
        {
        }

        public Tutorat(string tuteurId, string tutoreId, int statutPaiementId)
        {
            TutoreId = tutoreId;
            TuteurId = tuteurId;
            StatutPaiementId = statutPaiementId;
        }

        [Key]
        public int TutoratId { get; set; }

        [NotMapped]
        public double NombreHeureTotal { get; set; }

        [NotMapped]
        public double PrixTotal { get; set; }

        [ForeignKey("Tutore")]
        [Required]
        public string TutoreId { get; set; } 

        public Tutore Tutore { get; set; }

        [ForeignKey("Tuteur")]
        [Required]
        public string TuteurId { get; set; }

        public Tuteur Tuteur { get; set; }

        public List<PlageHoraire> ListePlageHoraire { get; set; }

        public string NumeroFacture { get; set; }

        [Display(Name = "Nombre de refus")]
        public int NombreRefus { get; set; }

        [ForeignKey("StatutPaiement")]
        [Required]
        public int StatutPaiementId { get; set; }

        public StatutPaiement StatutPaiement { get; set; }

        public DateTime DatePaiement { get; set; } 
    }
}
