using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ProjetIntegrationEquipe2.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Commentaire> Commentaires { get; set; }

        public DbSet<Cours> Cours { get; set; }

        public DbSet<CoursMaitrise> CoursMaitrises { get; set; }

        public DbSet<DemandeTutorat> DemandeTutorats { get; set; }

        public DbSet<Matiere> Matieres { get; set; }

        public DbSet<OffreTutorat> OffreTutorats { get; set; }

        public DbSet<PlageHoraire> PlageHoraires { get; set; }

        public DbSet<Promotion> Promotions { get; set; }

        public DbSet<StatutCoursMaitrise> StatutCoursMaitrises { get; set; }

        public DbSet<StatutPaiement> StatutPaiements { get; set; }

        public DbSet<Tuteur> Tuteurs { get; set; }

        public DbSet<Tutorat> Tutorats { get; set; }

        public DbSet<Tutore> Tutores { get; set; }
    }
}
