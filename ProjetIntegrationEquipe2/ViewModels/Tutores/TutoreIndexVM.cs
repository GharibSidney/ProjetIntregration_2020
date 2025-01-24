using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Tutores
{
    public class TutoreIndexVM
    {
        public List<Tutore> listeTutore { get; set; }
    }
}