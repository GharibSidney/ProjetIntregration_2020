using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Commentaires
{
    public class CommentairesTuteurVM
    {
        public List<Commentaire> ListeCommentaires { get; set; }

        public string Prenom { get; set; }

        public string Nom { get; set; }
    }
}