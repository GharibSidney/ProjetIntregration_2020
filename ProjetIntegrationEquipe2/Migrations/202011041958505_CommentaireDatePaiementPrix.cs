namespace ProjetIntegrationEquipe2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentaireDatePaiementPrix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Commentaires", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Paiements", "Prix", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Paiements", "Prix");
            DropColumn("dbo.Commentaires", "Date");
        }
    }
}
