using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Seances
{
    public class SeanceCreateVM
    {
        public SeanceCreateVM()
        {
            ListeDateTimes = new List<DateTime>();
            ListeTutores = new List<Tutore>();
        }
        [Required]
        [Display(Name = "*Séance(s) prévue(s)")]
        [DateTimePickerSeanceValidations]
        public List<DateTime> ListeDateTimes { get; set; }

        [Display(Name = "*Les tutorés")]
        public List<Tutore> ListeTutores { get; set; }
        [Required]
        [Display(Name = "Le tutoré")]
        public string TutoreId { get; set; } 

    }
}