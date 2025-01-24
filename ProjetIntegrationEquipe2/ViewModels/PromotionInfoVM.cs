using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjetIntegrationEquipe2.Models;

namespace ProjetIntegrationEquipe2.ViewModels­
{
    public class PromotionInfoVM
    {
        public int PromotionId { get; set; }

        [StringLength(200)]
        public string Description { get; set; }
        [StringLength(40)]
        public string Titre { get; set; }

        public string CheminImage { get; set; }

    }
}