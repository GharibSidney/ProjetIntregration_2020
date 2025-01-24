using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.Models
{
    public class Promotion
    {
        [Key]
        public int PromotionId { get; set; }

        public string CheminImage { get; set; }

        public string Titre { get; set; }

        [Required]
        public string Description { get; set; }
    }
}