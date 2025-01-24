using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.Models
{
    public class PlageHoraire
    {
        [Key]
        public int PlageHoraireId { get; set; }

        [Required]
        public DateTime DateHeureDebut { get; set; }

        [Required]
        public DateTime DateHeureFin { get; set; }

        //Optionnel, car j'ai une collection de PlageHoraire dans Tutorat !
        [ForeignKey("Tutorat")]
        public int? TutoratId { get; set; }

        public Tutorat Tutorat { get; set; }

        //Optionnel, car j'ai une collection de PlageHoraire dans OffreTutorat !
        [ForeignKey("OffreTutorat")]
        public int? OffreId { get; set; }

        public OffreTutorat OffreTutorat { get; set; }

        //Optionnel, car j'ai une collection de PlageHoraire dans DemandeTutorat !
        [ForeignKey("DemandeTutorat")] //Mes 2 champs représente la même chose !
        public int? DemandeTutoratId { get; set; }

        public DemandeTutorat DemandeTutorat { get; set; }
    }
}
