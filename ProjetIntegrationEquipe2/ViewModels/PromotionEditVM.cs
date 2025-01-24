using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels
{
    public class PromotionEditVM
    {
        public int PromotionId { get; set; }

        [StringLength(200,ErrorMessage ="La description est trop longue!")]
        [Display(Name = "*Description")]
        [Required(ErrorMessage = "Entrez une description!")]
        public string Description { get; set; }

        [Display(Name = "*Titre")]
        [Required(ErrorMessage = "Entrez un Titre!")]
        [StringLength(40, ErrorMessage = "Le titre est trop long!")]
        public string Titre { get; set; }

        [Display(Name = "Image")]
        public string CheminImage { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }
}