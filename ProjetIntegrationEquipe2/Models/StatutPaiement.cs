using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.Models
{
    public class StatutPaiement
    {
        [Key]
        public int StatutPaiementId { get; set; }

        [Required]
        public string Statut { get; set; }
    }
}
