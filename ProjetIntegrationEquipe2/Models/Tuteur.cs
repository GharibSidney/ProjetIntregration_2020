using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.Models
{
    [Table("Tuteur")]
    public class Tuteur : Tutore 
    {
        [Required]
        public double Tarif { get; set; }

        [NotMapped]
        public double CoteTotal { get; set; }

        public List<CoursMaitrise> ListeCoursMaitrise { get; set; }
    }
}
