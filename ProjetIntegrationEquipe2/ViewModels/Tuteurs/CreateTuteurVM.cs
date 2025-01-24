using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Tuteurs
{
    public class CreateTuteurVM
    {
        [Display(Name = "La photo qui sera sur votre profil")]
        public string CheminPhoto { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner une photo !")]
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

        [Display(Name = "Intérêt")]
        [StringLength(50)]
        public string Interet { get; set; }
        [StringLength(50)]
        public string Force { get; set; }

        [Required]
        [Range(0.00, 100.00, ErrorMessage = "Le salaire doit être entre 0 et 100")]
        [Display(Name = "*Tarif voulu")]
        public double Tarif { get; set; }

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#$%^&*])[0-9a-zA-Z!@#$%^&*0-9]{6,}$",
            ErrorMessage = "Le mot de passe doit avoir minimalement 6 caractères et doit contenir au moins un caractère spécial, un chiffre, une minuscule et une majuscule")]
        [DataType(DataType.Password)]
        [Display(Name = "*Mot de passe")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "*Confirmer le mot de passe")]
        [Compare("Password", ErrorMessage = "Le mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        public List<Matiere> ListeMatieres { get; set; }

        [Display(Name = "Matière(s)")]
        public List<int> ListeMatieresSelectionnesId { get; set; }

        public List<Cours> ListeCoursDemandes { get; set; }

        [Required]
        [Display(Name = "*Cours demandé(s)")]
        public List<int> ListeCoursSelectionnesId { get; set; }
    }
}
