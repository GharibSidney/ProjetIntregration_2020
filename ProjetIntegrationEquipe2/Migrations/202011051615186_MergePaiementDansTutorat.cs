namespace ProjetIntegrationEquipe2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MergePaiementDansTutorat : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Paiements", "StatutPaiementId", "dbo.StatutPaiements");
            DropIndex("dbo.Paiements", new[] { "StatutPaiementId" });
            AddColumn("dbo.Tutorats", "NumeroFacture", c => c.String());
            AddColumn("dbo.Tutorats", "NombreRefus", c => c.Int(nullable: false));
            AddColumn("dbo.Tutorats", "StatutPaiementId", c => c.Int(nullable: false));
            CreateIndex("dbo.Tutorats", "StatutPaiementId");
            AddForeignKey("dbo.Tutorats", "StatutPaiementId", "dbo.StatutPaiements", "StatutPaiementId", cascadeDelete: true);
            DropTable("dbo.Paiements");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Paiements",
                c => new
                    {
                        PaiementId = c.Int(nullable: false, identity: true),
                        NumeroFacture = c.String(nullable: false),
                        NombreRefus = c.Int(nullable: false),
                        StatutPaiementId = c.Int(nullable: false),
                        Prix = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.PaiementId);
            
            DropForeignKey("dbo.Tutorats", "StatutPaiementId", "dbo.StatutPaiements");
            DropIndex("dbo.Tutorats", new[] { "StatutPaiementId" });
            DropColumn("dbo.Tutorats", "StatutPaiementId");
            DropColumn("dbo.Tutorats", "NombreRefus");
            DropColumn("dbo.Tutorats", "NumeroFacture");
            CreateIndex("dbo.Paiements", "StatutPaiementId");
            AddForeignKey("dbo.Paiements", "StatutPaiementId", "dbo.StatutPaiements", "StatutPaiementId", cascadeDelete: true);
        }
    }
}
