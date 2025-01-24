using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Offres
{
    public class CreateOffreVM
    {
        [StringLength(35)]
        public string Titre { get; set; }

        public List<CoursMaitrise> ListeCoursMaitrises { get; set; }

        [Display(Name = "*Cours offerts")]
        public List<int> ListeCoursSelectionnesId { get; set; }

        [DateTimePickerValidations]
        public List<DateTime> ListeDateTimes { get; set; }
    }
}
