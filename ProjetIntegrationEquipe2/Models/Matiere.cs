using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.Models
{
    public class Matiere
    {
        [Key]
        public int MatiereId { get; set; }

        [Required]
        public string Nom { get; set; }
    }
}
