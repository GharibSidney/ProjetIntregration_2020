using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.Models
{
    public class Commentaire
    {
        [Key]
        public int CommentaireId { get; set; }

        [Required]
        public string Titre { get; set; }

        [Required]
        public string CommentaireTexte { get; set; }

        [Required]
        public double Cote { get; set; }

        [ForeignKey("Tutore")]
        [Required]
        public string TutoreId { get; set; }

        public Tutore Tutore { get; set; }

        [ForeignKey("Tuteur")]
        [Required]
        public string TuteurId { get; set; }

        public Tuteur Tuteur { get; set; }

        public DateTime Date { get; set; }
    }
}
