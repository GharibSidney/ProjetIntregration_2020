using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Tutores
{
    public class LoginPartialViewModel
    {
        [Display(Name = "Photo")]
        public string CheminPhoto { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }

        [StringLength(255)]
        [Display(Name = "Intérêt")]
        public string Interet { get; set; }

        [StringLength(255)]
        public string Force { get; set; }
                
        [Display(Name = "Téléphone")]
        public string Telephone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        

    }
}