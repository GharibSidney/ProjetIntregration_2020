using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Windows.Media.Imaging;

namespace ProjetIntegrationEquipe2.Models
{
    [Table("Tutore")]
    public class Tutore : ApplicationUser
    {
        [Required]
        [Display(Name ="Photo")]
        public string CheminPhoto { get; set; }

        public string Interet { get; set; }

        public string Force { get; set; }
    }
}