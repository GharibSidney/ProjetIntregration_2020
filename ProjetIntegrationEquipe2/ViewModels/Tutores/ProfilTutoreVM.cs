using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels
{
    public class ProfilTutoreVM
    {
        public string Id { get; set; }

        [Display(Name = "Nom:")]
        public string Nom { get; set; }

        [Display(Name ="Prénom:")]
        public string Prenom { get; set; }

        public string CheminPhoto { get; set; }

        [Display(Name ="Courriel:")]
        public string Courriel { get; set; }

        [Display(Name ="Numéro de téléphone:")]
        public string Telephone { get; set; }

        [Display(Name ="Intérêts:")]
        public string Interets { get; set; }

        [Display(Name ="Forces:")]
        public string Forces { get; set; }

        public List<CoursMaitrise> ListeCoursMaitrises { get; set; }
    }
}