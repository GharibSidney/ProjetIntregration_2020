using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjetIntegrationEquipe2.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Adresse courriel")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Display(Name = "Mémoriser le mot de passe ?")]
        public bool RememberMe { get; set; }
    }

  

}

  
