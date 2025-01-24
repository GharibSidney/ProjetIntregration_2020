namespace ProjetIntegrationEquipe2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreationMigrationInitiale2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Commentaires",
                c => new
                    {
                        CommentaireId = c.Int(nullable: false, identity: true),
                        Titre = c.String(nullable: false),
                        CommentaireTexte = c.String(nullable: false),
                        Cote = c.Double(nullable: false),
                        TutoreId = c.String(nullable: false, maxLength: 128),
                        TuteurId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.CommentaireId)
                .ForeignKey("dbo.Tuteur", t => t.TuteurId)
                .ForeignKey("dbo.Tutore", t => t.TutoreId)
                .Index(t => t.TutoreId)
                .Index(t => t.TuteurId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Prenom = c.String(nullable: false, maxLength: 100),
                        Nom = c.String(nullable: false, maxLength: 100),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.CoursMaitrises",
                c => new
                    {
                        CoursMaitriseId = c.Int(nullable: false, identity: true),
                        CoursId = c.Int(nullable: false),
                        TuteurId = c.String(nullable: false, maxLength: 128),
                        StatutCoursMaitriseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CoursMaitriseId)
                .ForeignKey("dbo.Cours", t => t.CoursId, cascadeDelete: true)
                .ForeignKey("dbo.StatutCoursMaitrises", t => t.StatutCoursMaitriseId, cascadeDelete: true)
                .ForeignKey("dbo.Tuteur", t => t.TuteurId)
                .Index(t => t.CoursId)
                .Index(t => t.TuteurId)
                .Index(t => t.StatutCoursMaitriseId);
            
            CreateTable(
                "dbo.Cours",
                c => new
                    {
                        CoursId = c.Int(nullable: false, identity: true),
                        Nom = c.String(nullable: false),
                        NumeroCours = c.String(nullable: false),
                        MatiereId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CoursId)
                .ForeignKey("dbo.Matieres", t => t.MatiereId, cascadeDelete: true)
                .Index(t => t.MatiereId);
            
            CreateTable(
                "dbo.DemandeTutorats",
                c => new
                    {
                        DemandeTutoratId = c.Int(nullable: false, identity: true),
                        Titre = c.String(),
                        TutoreId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.DemandeTutoratId)
                .ForeignKey("dbo.Tutore", t => t.TutoreId)
                .Index(t => t.TutoreId);
            
            CreateTable(
                "dbo.PlageHoraires",
                c => new
                    {
                        PlageHoraireId = c.Int(nullable: false, identity: true),
                        DateHeureDebut = c.DateTime(nullable: false),
                        DateHeureFin = c.DateTime(nullable: false),
                        TutoratId = c.Int(),
                        OffreId = c.Int(),
                        DemandeTutoratId = c.Int(),
                    })
                .PrimaryKey(t => t.PlageHoraireId)
                .ForeignKey("dbo.DemandeTutorats", t => t.DemandeTutoratId)
                .ForeignKey("dbo.OffreTutorats", t => t.OffreId)
                .ForeignKey("dbo.Tutorats", t => t.TutoratId)
                .Index(t => t.TutoratId)
                .Index(t => t.OffreId)
                .Index(t => t.DemandeTutoratId);
            
            CreateTable(
                "dbo.OffreTutorats",
                c => new
                    {
                        OffreId = c.Int(nullable: false, identity: true),
                        Titre = c.String(),
                        TuteurId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.OffreId)
                .ForeignKey("dbo.Tuteur", t => t.TuteurId)
                .Index(t => t.TuteurId);
            
            CreateTable(
                "dbo.Tutorats",
                c => new
                    {
                        TutoratId = c.Int(nullable: false, identity: true),
                        TutoreId = c.String(nullable: false, maxLength: 128),
                        TuteurId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.TutoratId)
                .ForeignKey("dbo.Tuteur", t => t.TuteurId)
                .ForeignKey("dbo.Tutore", t => t.TutoreId)
                .Index(t => t.TutoreId)
                .Index(t => t.TuteurId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Matieres",
                c => new
                    {
                        MatiereId = c.Int(nullable: false, identity: true),
                        Nom = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.MatiereId);
            
            CreateTable(
                "dbo.StatutCoursMaitrises",
                c => new
                    {
                        StatutCoursMaitriseId = c.Int(nullable: false, identity: true),
                        Statut = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.StatutCoursMaitriseId);
            
            CreateTable(
                "dbo.Paiements",
                c => new
                    {
                        PaiementId = c.Int(nullable: false, identity: true),
                        NumeroFacture = c.String(nullable: false),
                        NombreRefus = c.Int(nullable: false),
                        StatutPaiementId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PaiementId)
                .ForeignKey("dbo.StatutPaiements", t => t.StatutPaiementId, cascadeDelete: true)
                .Index(t => t.StatutPaiementId);
            
            CreateTable(
                "dbo.StatutPaiements",
                c => new
                    {
                        StatutPaiementId = c.Int(nullable: false, identity: true),
                        Statut = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.StatutPaiementId);
            
            CreateTable(
                "dbo.Promotions",
                c => new
                    {
                        PromotionId = c.Int(nullable: false, identity: true),
                        CheminImage = c.String(),
                        Titre = c.String(),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.PromotionId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.DemandeTutoratCours",
                c => new
                    {
                        DemandeTutorat_DemandeTutoratId = c.Int(nullable: false),
                        Cours_CoursId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DemandeTutorat_DemandeTutoratId, t.Cours_CoursId })
                .ForeignKey("dbo.DemandeTutorats", t => t.DemandeTutorat_DemandeTutoratId, cascadeDelete: true)
                .ForeignKey("dbo.Cours", t => t.Cours_CoursId, cascadeDelete: true)
                .Index(t => t.DemandeTutorat_DemandeTutoratId)
                .Index(t => t.Cours_CoursId);
            
            CreateTable(
                "dbo.OffreTutoratCours",
                c => new
                    {
                        OffreTutorat_OffreId = c.Int(nullable: false),
                        Cours_CoursId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OffreTutorat_OffreId, t.Cours_CoursId })
                .ForeignKey("dbo.OffreTutorats", t => t.OffreTutorat_OffreId, cascadeDelete: true)
                .ForeignKey("dbo.Cours", t => t.Cours_CoursId, cascadeDelete: true)
                .Index(t => t.OffreTutorat_OffreId)
                .Index(t => t.Cours_CoursId);
            
            CreateTable(
                "dbo.Tutore",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CheminPhoto = c.String(nullable: false),
                        Interet = c.String(),
                        Force = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Tuteur",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Tarif = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tutore", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tuteur", "Id", "dbo.Tutore");
            DropForeignKey("dbo.Tutore", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Paiements", "StatutPaiementId", "dbo.StatutPaiements");
            DropForeignKey("dbo.Commentaires", "TutoreId", "dbo.Tutore");
            DropForeignKey("dbo.Commentaires", "TuteurId", "dbo.Tuteur");
            DropForeignKey("dbo.CoursMaitrises", "TuteurId", "dbo.Tuteur");
            DropForeignKey("dbo.CoursMaitrises", "StatutCoursMaitriseId", "dbo.StatutCoursMaitrises");
            DropForeignKey("dbo.CoursMaitrises", "CoursId", "dbo.Cours");
            DropForeignKey("dbo.Cours", "MatiereId", "dbo.Matieres");
            DropForeignKey("dbo.DemandeTutorats", "TutoreId", "dbo.Tutore");
            DropForeignKey("dbo.PlageHoraires", "TutoratId", "dbo.Tutorats");
            DropForeignKey("dbo.Tutorats", "TutoreId", "dbo.Tutore");
            DropForeignKey("dbo.Tutorats", "TuteurId", "dbo.Tuteur");
            DropForeignKey("dbo.PlageHoraires", "OffreId", "dbo.OffreTutorats");
            DropForeignKey("dbo.OffreTutorats", "TuteurId", "dbo.Tuteur");
            DropForeignKey("dbo.OffreTutoratCours", "Cours_CoursId", "dbo.Cours");
            DropForeignKey("dbo.OffreTutoratCours", "OffreTutorat_OffreId", "dbo.OffreTutorats");
            DropForeignKey("dbo.PlageHoraires", "DemandeTutoratId", "dbo.DemandeTutorats");
            DropForeignKey("dbo.DemandeTutoratCours", "Cours_CoursId", "dbo.Cours");
            DropForeignKey("dbo.DemandeTutoratCours", "DemandeTutorat_DemandeTutoratId", "dbo.DemandeTutorats");
            DropIndex("dbo.Tuteur", new[] { "Id" });
            DropIndex("dbo.Tutore", new[] { "Id" });
            DropIndex("dbo.OffreTutoratCours", new[] { "Cours_CoursId" });
            DropIndex("dbo.OffreTutoratCours", new[] { "OffreTutorat_OffreId" });
            DropIndex("dbo.DemandeTutoratCours", new[] { "Cours_CoursId" });
            DropIndex("dbo.DemandeTutoratCours", new[] { "DemandeTutorat_DemandeTutoratId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Paiements", new[] { "StatutPaiementId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Tutorats", new[] { "TuteurId" });
            DropIndex("dbo.Tutorats", new[] { "TutoreId" });
            DropIndex("dbo.OffreTutorats", new[] { "TuteurId" });
            DropIndex("dbo.PlageHoraires", new[] { "DemandeTutoratId" });
            DropIndex("dbo.PlageHoraires", new[] { "OffreId" });
            DropIndex("dbo.PlageHoraires", new[] { "TutoratId" });
            DropIndex("dbo.DemandeTutorats", new[] { "TutoreId" });
            DropIndex("dbo.Cours", new[] { "MatiereId" });
            DropIndex("dbo.CoursMaitrises", new[] { "StatutCoursMaitriseId" });
            DropIndex("dbo.CoursMaitrises", new[] { "TuteurId" });
            DropIndex("dbo.CoursMaitrises", new[] { "CoursId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Commentaires", new[] { "TuteurId" });
            DropIndex("dbo.Commentaires", new[] { "TutoreId" });
            DropTable("dbo.Tuteur");
            DropTable("dbo.Tutore");
            DropTable("dbo.OffreTutoratCours");
            DropTable("dbo.DemandeTutoratCours");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Promotions");
            DropTable("dbo.StatutPaiements");
            DropTable("dbo.Paiements");
            DropTable("dbo.StatutCoursMaitrises");
            DropTable("dbo.Matieres");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Tutorats");
            DropTable("dbo.OffreTutorats");
            DropTable("dbo.PlageHoraires");
            DropTable("dbo.DemandeTutorats");
            DropTable("dbo.Cours");
            DropTable("dbo.CoursMaitrises");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Commentaires");
        }
    }
}
