namespace ProjetIntegrationEquipe2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaiementDateHeure : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tutorats", "DatePaiement", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tutorats", "DatePaiement");
        }
    }
}
