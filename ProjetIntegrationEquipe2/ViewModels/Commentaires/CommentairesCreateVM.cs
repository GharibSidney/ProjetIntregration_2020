using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Commentaires
{
    public class CommentairesCreateVM
    {
        [StringLength(75)]
        [Display(Name ="*Titre")]
        [Required(ErrorMessage ="Veuillez entrer un titre")]
        public string Titre { get; set; }

        [Display(Name = "*Texte")]
        [Required(ErrorMessage ="Veuillez entrer du texte")]
        public string Texte { get; set; }

        [Display(Name = "*Cote")]
        [Required(ErrorMessage ="Veuillez laisser une cote")]
        public double Cote { get; set; }

        public string IdTutore { get; set; }

        [Required(ErrorMessage ="Indiquez le tuteur concerné par ce commentaire svp!")]
        public string IdTuteur { get; set; }

        public List<Tutorat> ListeSeances { get; set; }

        public IEnumerable<Tuteur> ListeTuteurs { get; set; }

        [Display(Name = "*Tuteur")]
        public Tuteur Tuteur { get; set; }
    }
}