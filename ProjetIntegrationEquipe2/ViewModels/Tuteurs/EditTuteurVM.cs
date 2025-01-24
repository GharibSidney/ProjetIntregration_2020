using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Tuteurs
{
    public class EditTuteurVM
    {
        [Required]
        public string Id { get; set; }

        [Display(Name = "La photo qui sera sur votre profil")]
        public string CheminPhoto { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        [Required]
        [StringLength(35)]
        [Display(Name = "*Prénom")]
        public string Prenom { get; set; }

        [Required]
        [StringLength(35)]
        [Display(Name = "*Nom")]
        public string Nom { get; set; }

        [Required]
        [Display(Name = "*Numéro de téléphone")]
        [RegularExpression(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}", ErrorMessage = "Le champs Numéro de téléphone doit être de format « 999-999-9999 » uniquement.")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "*Courrier électronique")]
        [RegularExpression(@"([a-z0-9_\.-]+\@cegepgranby.qc.ca)", ErrorMessage = "Le champs Courrier électronique doit être un format de courriel valide, c’est-à-dire « xxxxxx@cegepgranby.qc.ca ».")]
        public string Email { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "*Confirmer le courriel ")]
        [Compare("Email", ErrorMessage = "Le courriel et le courriel de confirmation ne correspondent pas.")]
        public string ConfirmEmail { get; set; }

        [StringLength(50)]
        [Display(Name = "Intérêt")]
        public string Interet { get; set; }
        [StringLength(50)]
        public string Force { get; set; }

        [Required]
        [Range(0.00, 100.00, ErrorMessage = "Le salaire doit être entre 0 et 100")]
        [Display(Name = "*Tarif voulu")]
        public double Tarif { get; set; }

        public List<Matiere> ListeMatieres { get; set; }

        [Display(Name = "Matière(s)")]
        public List<int> ListeMatieresSelectionnesId { get; set; }

        public List<Cours> ListeCoursDemandes { get; set; }

        public List<CoursMaitrise> ListeCoursMaitrises { get; set; }

        [Display(Name = "Cours demandé(s)")]
        public List<int> ListeCoursSelectionnesId { get; set; }
    }
}
