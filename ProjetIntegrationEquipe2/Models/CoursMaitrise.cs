using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.Models
{
    public class CoursMaitrise
    {
        public CoursMaitrise()
        {
        }

        public CoursMaitrise(int coursId, string tuteurId, int statutCoursMaitriseId)
        {
            CoursId = coursId;
            TuteurId = tuteurId;
            StatutCoursMaitriseId = statutCoursMaitriseId;
        }

        [Key]
        public int CoursMaitriseId { get; set; }

        [ForeignKey("Cours")]
        public int CoursId { get; set; }

        public Cours Cours { get; set; }

        [ForeignKey("Tuteur")]
        [Required]
        public string TuteurId { get; set; }

        public Tuteur Tuteur { get; set; }

        [Required]
        [ForeignKey("StatutCoursMaitrise")]
        public int StatutCoursMaitriseId { get; set; }

        public StatutCoursMaitrise StatutCoursMaitrise { get; set; }
    }
}
