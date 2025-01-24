using ProjetIntegrationEquipe2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.ViewModels.Matieres
{
    public class MatieresIndexVM
    {
        public List<Matiere> MatieresList { get; set; }
    }
}