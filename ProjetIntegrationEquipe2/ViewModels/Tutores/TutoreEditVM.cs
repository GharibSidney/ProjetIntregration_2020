using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Tutores
{
    public class TutoreEditVM
    {

        public string Id { get; set; }

        [Display(Name = "La photo qui sera sur votre profil")]
        public string CheminPhoto { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        [Required]
        [Display(Name = "*Nom")]
        [StringLength(35)]
        public string Nom { get; set; }

        [Required]
        [StringLength(35)]
        [Display(Name = "*Prénom")]
        public string Prenom { get; set; }

        [StringLength(50)]
        [Display(Name = "Intérêt(s)")]
        public string Interet { get; set; }

        [StringLength(50)]
        [Display(Name = "Force(s)")]
        public string Force { get; set; }

        [Required(ErrorMessage = "Le champs Telephone est requis.")]
        [RegularExpression(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}", ErrorMessage = "Le champs Telephone doit être de format « 999-999-9999 » uniquement.")]
        [Display(Name = "*Téléphone")]
        public string Telephone { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Le champs Courriel est requis.")]
        [RegularExpression(@"([a-z0-9_\.-]+\@cegepgranby.qc.ca)", ErrorMessage = "Le champs Courriel doit être un format de courriel valide, c’est-à-dire « xxxxxx@cegepgranby.qc.ca ».")]
        [Display(Name = "*Courrier électronique")]
        public string Email { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "*Confirmer le courriel ")]
        [Compare("Email", ErrorMessage = "Le courriel et le courriel de confirmation ne correspondent pas.")]
        public string ConfirmEmail { get; set; }
    }
}