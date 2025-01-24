using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.ViewModels.Statistiques;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetIntegrationEquipe2.Controllers
{
    [Authorize(Roles =RolesNames.Administrateur)]
    public class StatistiquesController : Controller
    {
        public ActionResult StatsPaiements()
        {
            StatsPaiementsVM vm = new StatsPaiementsVM();
            using(ApplicationDbContext context=new ApplicationDbContext())
            {
                vm.nbPaiementsAttente = context.Tutorats
                    .Where(seance => seance.StatutPaiementId == context.StatutPaiements.FirstOrDefault(p => p.Statut == StatutFacture.AttentePaiement).StatutPaiementId).Count();
                vm.nbPaiementsRefuses= context.Tutorats
                    .Where(seance => seance.StatutPaiementId == context.StatutPaiements.FirstOrDefault(p => p.Statut == StatutFacture.Refuse).StatutPaiementId).Count();
                vm.nbPaiementsValides= context.Tutorats
                    .Where(seance => seance.StatutPaiementId == context.StatutPaiements.FirstOrDefault(p => p.Statut == StatutFacture.Valide).StatutPaiementId).Count();
            }
            return View(vm);
        }
        public ActionResult StatsGeneral()
        {
            StatistiqueGeneraleVM vm = new StatistiqueGeneraleVM();

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                vm.NbTuteurs = context.Tuteurs.Count();
                vm.NbTutores = context.Tutores.Count();
                vm.NbProgramme = context.Matieres.Count();
                vm.NbCours = context.Cours.Count();
                vm.NbDemandes = context.DemandeTutorats.Count();
                vm.NbOffres = context.OffreTutorats.Count();
                vm.NbTutorats = context.Tutorats.Count(t => t.StatutPaiementId != context.StatutPaiements.FirstOrDefault(s => s.Statut == StatutFacture.Attente).StatutPaiementId);
                vm.NbTutoratsActif = context.Tutorats.Where(t => t.StatutPaiementId ==
                context.StatutPaiements.FirstOrDefault(s => s.Statut == StatutFacture.Attente).StatutPaiementId).Count();
            }

            return View("StatistiqueGenerale", vm);

        }
    }
}
