using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels
{
    public class PromotionCreateVM
    {
        public int PromotionId { get; set; }

        [StringLength(200, ErrorMessage = "La description est trop longue!")]
        [Required(ErrorMessage = "Entrez une description!")]
        [Display(Name = "*Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Entrez un Titre!")]
        [Display(Name = "*Titre")]
        [StringLength(40, ErrorMessage = "Le titre est trop long!")]
        public string Titre { get; set; }

        [Display(Name = "Image")]
        public string CheminImage { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }
}