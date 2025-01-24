using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Offres
{
    public class EditOffreVM
    {
        [Required]
        public int Id { get; set; }
        [StringLength(35)]
        public string Titre { get; set; }

        public List<Cours> ListeCoursOfferts { get; set; }

        public List<CoursMaitrise> ListeCoursMaitrises { get; set; }

        [Display(Name = "Cours offerts")]
        public List<int> ListeCoursSelectionnesId { get; set; }

        //Sert à faire le "binding" sur les cours maîtrisés (checkbox) !
        [Display(Name = "Cours maîtrisé(s)")]
        public List<int> ListeCoursMaitrisesId { get; set; }

        [DateTimePickerValidations]
        public List<DateTime> ListeDateTimes { get; set; }
    }
}
