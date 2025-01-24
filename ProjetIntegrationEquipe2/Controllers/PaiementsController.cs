using Microsoft.AspNet.Identity;
using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.ViewModels.Paiements;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetIntegrationEquipe2.Controllers
{
    public class PaiementsController : Controller
    {
        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult ListePaiementsRefuse(string rechercherPar, string rechercher, DateTime? dateDebut, DateTime? dateFin)
        {
            List<Tutorat> listeTutorats = new List<Tutorat>();
 

            Tutorat unTutorat = new Tutorat();
            Tuteur unTuteur = new Tuteur();

            ListePaiementsRefuseVM vm = new ListePaiementsRefuseVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                if (rechercherPar == "Tuteur")
                {
                    listeTutorats = dbContext.Tutorats.Include("Tuteur").Include("Tutore").Include("ListePlageHoraire").Include("StatutPaiement")
                        .Where(r => string.Concat(r.Tuteur.Prenom.Trim(), " ", r.Tuteur.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim()))
                        .Where(s => s.StatutPaiement.StatutPaiementId == dbContext.StatutPaiements.FirstOrDefault(stat =>
                        stat.Statut == StatutFacture.Refuse).StatutPaiementId).ToList();
                }
                else if (rechercherPar == "Tutore")
                {
                    listeTutorats = dbContext.Tutorats.Include("Tutore").Include("ListePlageHoraire").Include("StatutPaiement")
                        .Where(r => string.Concat(r.Tutore.Prenom.Trim(), " ", r.Tutore.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim()))
                        .Where(s => s.StatutPaiement.StatutPaiementId == dbContext.StatutPaiements.FirstOrDefault(stat =>
                        stat.Statut == StatutFacture.Refuse).StatutPaiementId).ToList();
                }
                else
                {
                    listeTutorats = dbContext.Tutorats.Include("Tutore").Include("ListePlageHoraire").Include("StatutPaiement")
                        .Where(r => r.ListePlageHoraire.Any(p => p.DateHeureDebut >= dateDebut && p.DateHeureFin <= dateFin) || (dateDebut == null && dateFin == null))
                        .Where(s => s.StatutPaiement.StatutPaiementId == dbContext.StatutPaiements.FirstOrDefault(stat =>
                        stat.Statut == StatutFacture.Refuse).StatutPaiementId).ToList();
                }

                foreach (Tutorat tutorat in listeTutorats)
                {
                    unTutorat = dbContext.Tutorats.Single(t => t.TutoratId == tutorat.TutoratId);
                    unTuteur = dbContext.Tuteurs.Single(tuteur => tuteur.Id == unTutorat.TuteurId);

                    double heuresTotal = 0;
                    foreach (PlageHoraire plageHoraire in unTutorat.ListePlageHoraire)
                    {
                        TimeSpan difference = plageHoraire.DateHeureFin.Subtract(plageHoraire.DateHeureDebut);
                        heuresTotal += difference.TotalHours;
                        unTutorat.PrixTotal = Math.Round(heuresTotal, 2) * unTuteur.Tarif;
                        unTutorat.NombreHeureTotal = Math.Round(heuresTotal, 2);
                    }
                }
            }
            vm.ListeTutorats = listeTutorats;


            return View(vm);
        }

        // GET: Paiements
        [Authorize(Roles = RolesNames.Tutore)]
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Tutorat unTutorat = new Tutorat();
            Tuteur unTuteur = new Tuteur();
            Tutore unTutore = new Tutore();
            try
            {

                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    unTutorat = context.Tutorats.Include("ListePlageHoraire").Single(tutorat => tutorat.TutoratId == (int)id);


                    if (unTutorat.StatutPaiementId != context.StatutPaiements.Single(statut => statut.Statut == StatutFacture.AttentePaiement).StatutPaiementId)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    unTutore = context.Tutores.Single(t => t.Id == unTutorat.TutoreId);

                    unTuteur = context.Tuteurs.Single(tuteur => tuteur.Id == unTutorat.TuteurId);
                }
            }
            catch (Exception)
            {
                return View("Error");
            }

            if (unTutorat.TutoreId != User.Identity.GetUserId())
            {
                return View("Error");
            }

            IndexVM vm = new IndexVM();

            double heuresTotal = 0;

            foreach (PlageHoraire plageHoraire in unTutorat.ListePlageHoraire)
            {
                TimeSpan difference = plageHoraire.DateHeureFin.Subtract(plageHoraire.DateHeureDebut);
                heuresTotal += difference.TotalHours;
            }
            vm.ListePlageHoraire = unTutorat.ListePlageHoraire;
            vm.TutoratId = (int)id;
            vm.Heure = Math.Round(heuresTotal, 2);
            vm.Tarif = unTuteur.Tarif;
            vm.Courriel = unTutore.Email;
            vm.PrixTotal = Math.Round(heuresTotal, 2) * unTuteur.Tarif;
            vm.ClePublic = ConfigurationManager.AppSettings["stripePublishableKey"];

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public ActionResult Index(string stripeToken, string stripeEmail, IndexVM vm)
        {
            Stripe.StripeConfiguration.SetApiKey("pk_test_51HjUU8FozCvQdUsGMiNGtNvBmq8x2t9PQ1PzTKDgCTvA892EdcTp6PIhX5fPlj1AOwWbh39yTkCLq4TXYnSzKWyY00szvtNdfX");
            Stripe.StripeConfiguration.ApiKey = "sk_test_51HjUU8FozCvQdUsG8ZCdCjaMhevPjnWhc9hdkE9kSXUdjEOM9XKpY7a6auYmxnX6mmT1o32jhXCNdaEBm7GDb4KF00KXgOe9Ph";

            var myCharge = new Stripe.ChargeCreateOptions();

            //Toujours setter ces propriétés !
            myCharge.Amount = Convert.ToInt64(100 * vm.PrixTotal);
            myCharge.Currency = "CAD";
            myCharge.ReceiptEmail = stripeEmail;
            myCharge.Description = "Un simple essai";
            myCharge.Source = stripeToken;
            myCharge.Capture = true;
            var chargeService = new Stripe.ChargeService();

            try
            {
                Charge stripeCharge = chargeService.Create(myCharge);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Il y a une erreur avec votre carte, veuillez contacter votre institution bancaire.");

                Tutorat leTutorat = new Tutorat();
                Tuteur leTuteur = new Tuteur();

                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    leTutorat = context.Tutorats.Include("ListePlageHoraire").Single(tutorat => tutorat.TutoratId == vm.TutoratId);

                    if (leTutorat.StatutPaiementId != context.StatutPaiements.Single(statut => statut.Statut == StatutFacture.AttentePaiement).StatutPaiementId)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    double heuresTotal = 0;
                    foreach (PlageHoraire plageHoraire in leTutorat.ListePlageHoraire)
                    {
                        TimeSpan difference = plageHoraire.DateHeureFin.Subtract(plageHoraire.DateHeureDebut);
                        heuresTotal += difference.TotalHours;
                    }

                    leTuteur = context.Tuteurs.Single(tuteur => tuteur.Id == leTutorat.TuteurId);
                    vm.ClePublic = ConfigurationManager.AppSettings["stripePublishableKey"];
                    vm.Heure = heuresTotal;
                    vm.Tarif = leTuteur.Tarif;
                    vm.ListePlageHoraire = new List<PlageHoraire>();
                    vm.ListePlageHoraire = leTutorat.ListePlageHoraire;

                    return View(vm);
                }
            }

            Tutorat unTutorat = new Tutorat();

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                unTutorat = context.Tutorats.Single(tutorat => tutorat.TutoratId == vm.TutoratId);
                unTutorat.StatutPaiementId = context.StatutPaiements.Single(statut => statut.Statut == StatutFacture.Valide).StatutPaiementId;
                unTutorat.DatePaiement = DateTime.Now; //Setter la date du paiement
                context.SaveChanges();
            }
            return PartialView("_PageValide", "Paiements");
        }

    [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Refuser(int? id)
        {
            //On met "int? id" pour que le id puisse être null, afin de faire une redirection !
            //Si on ne le met pas nullable, la page web tombe en erreur, car l'exception n'est pas gérée !
            if (id == null)
            {
                return View("Error");
            }

            Tutorat unTutorat = new Tutorat();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                //Doit avoir la propriété "TutoratId" de setter dans la méthode "Index" (Get), afin que le id ne soit pas null !
                unTutorat = dbContext.Tutorats.Include("StatutPaiement").Single(t => t.TutoratId == id);

                if (unTutorat.NombreRefus < 2 && (unTutorat.StatutPaiement.Statut == StatutFacture.AttentePaiement))
                {
                    unTutorat.StatutPaiementId = dbContext.StatutPaiements.Single(s => s.Statut == StatutFacture.Attente).StatutPaiementId;
                    unTutorat.NombreRefus += 1;
                }

                if (unTutorat.NombreRefus == 2 && unTutorat.StatutPaiement.Statut == StatutFacture.AttentePaiement)
                {
                    unTutorat.StatutPaiementId = dbContext.StatutPaiements.Single(s => s.Statut == StatutFacture.Refuse).StatutPaiementId;
                }
                dbContext.SaveChanges();

                return RedirectToAction("PageRefusValide","Paiements",new { Id = unTutorat.TutoratId });
            }
        }

        public ActionResult PageRefusValide(int id)
        {
            RefusPaiementVM vm = new RefusPaiementVM();
            Tutorat paiement = new Tutorat();
            using(ApplicationDbContext dbContext=new ApplicationDbContext())
            {
                paiement = dbContext.Tutorats.Single(p=>p.TutoratId==id);
            }
            vm.PaiementId = paiement.TutoratId;
            vm.NombreRefus = paiement.NombreRefus;
            return View(vm);
        }

        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult ListePaiements(string rechercherPar, string rechercher, DateTime? dateDebut, DateTime? dateFin)
        {
            List<Tutorat> listeTutorats = new List<Tutorat>();


            ListePaiementsVM vm = new ListePaiementsVM();

            Tutorat unTutorat = new Tutorat();
            Tuteur unTuteur = new Tuteur();

            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                if (rechercherPar == "Tuteur")
                {
                    listeTutorats = context.Tutorats.Include("Tuteur").Include("Tutore").Include("ListePlageHoraire").Include("StatutPaiement")
                        .Where(r => string.Concat(r.Tuteur.Prenom.Trim(), " ", r.Tuteur.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).
                        Where(s => s.StatutPaiement.StatutPaiementId == context.StatutPaiements.FirstOrDefault(stat =>
                        stat.Statut == StatutFacture.AttentePaiement).StatutPaiementId).ToList();
                }
                else if (rechercherPar == "Tutore")
                {
                    listeTutorats = context.Tutorats.Include("Tutore").Include("ListePlageHoraire").Include("StatutPaiement")
                        .Where(r => string.Concat(r.Tutore.Prenom.Trim(), " ", r.Tutore.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim()))
                        .Where(s => s.StatutPaiement.StatutPaiementId == context.StatutPaiements.FirstOrDefault(stat =>
                        stat.Statut == StatutFacture.AttentePaiement).StatutPaiementId).ToList();
                }
                else
                {
                    listeTutorats = context.Tutorats.Include("Tutore").Include("ListePlageHoraire").Include("StatutPaiement")
                        .Where(r => r.ListePlageHoraire.Any(p => p.DateHeureDebut >= dateDebut && p.DateHeureFin <= dateFin) || (dateDebut == null && dateFin == null))
                        .Where(s => s.StatutPaiement.StatutPaiementId == context.StatutPaiements.FirstOrDefault(stat =>
                        stat.Statut == StatutFacture.AttentePaiement).StatutPaiementId).ToList();
                }

                foreach (Tutorat tutorat in listeTutorats)
                {
                    unTutorat = context.Tutorats.Single(t => t.TutoratId == tutorat.TutoratId);
                    unTuteur = context.Tuteurs.Single(tuteur => tuteur.Id == unTutorat.TuteurId);

                    double heuresTotal = 0;
                    foreach (PlageHoraire plageHoraire in unTutorat.ListePlageHoraire)
                    {
                        TimeSpan difference = plageHoraire.DateHeureFin.Subtract(plageHoraire.DateHeureDebut);
                        heuresTotal += difference.TotalHours;
                        unTutorat.PrixTotal = Math.Round(heuresTotal, 2) * unTuteur.Tarif;
                        unTutorat.NombreHeureTotal = Math.Round(heuresTotal, 2);
                    }
                }
            }
            vm.ListeSeances = listeTutorats;


            return View(vm);
        }

        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult ListePaiementsValide(string rechercherPar, string rechercher, DateTime? dateDebut, DateTime? dateFin)
        {
            List<Tutorat> listeTutorats = new List<Tutorat>();

            ListePaiementsValideVM vm = new ListePaiementsValideVM();

            Tutorat unTutorat = new Tutorat();
            Tuteur unTuteur = new Tuteur();

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                if (rechercherPar == "Tuteur")
                {
                    listeTutorats = context.Tutorats.Include("Tuteur").Include("Tutore").Include("ListePlageHoraire").Include("StatutPaiement")
                        .Where(r => string.Concat(r.Tuteur.Prenom.Trim(), " ", r.Tuteur.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim()))
                        .Where(s => s.StatutPaiement.StatutPaiementId == context.StatutPaiements.FirstOrDefault(stat =>
                        stat.Statut == StatutFacture.Valide).StatutPaiementId).ToList();
                }
                else if (rechercherPar == "Tutore")
                {
                    listeTutorats = context.Tutorats.Include("Tutore").Include("ListePlageHoraire").Include("StatutPaiement")
                        .Where(r => string.Concat(r.Tutore.Prenom.Trim(), " ", r.Tutore.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim()))
                        .Where(s => s.StatutPaiement.StatutPaiementId == context.StatutPaiements.FirstOrDefault(stat =>
                        stat.Statut == StatutFacture.Valide).StatutPaiementId).ToList();
                }
                else if (rechercherPar == "DatePaiement")
                {
                    listeTutorats = context.Tutorats.Include("Tutore").Include("ListePlageHoraire").Include("StatutPaiement")
                        .Where(r => (r.DatePaiement >= dateDebut && r.DatePaiement <= dateFin) || (dateDebut == null && dateFin == null))
                        .Where(s => s.StatutPaiement.StatutPaiementId == context.StatutPaiements.FirstOrDefault(stat =>
                        stat.Statut == StatutFacture.Valide).StatutPaiementId).ToList();
                }
                else
                {
                    listeTutorats = context.Tutorats.Include("Tutore").Include("ListePlageHoraire").Include("StatutPaiement")
                        .Where(r => r.ListePlageHoraire.Any(p => p.DateHeureDebut >= dateDebut && p.DateHeureFin <= dateFin) || (dateDebut == null && dateFin == null))
                        .Where(s => s.StatutPaiement.StatutPaiementId == context.StatutPaiements.FirstOrDefault(stat =>
                        stat.Statut == StatutFacture.Valide).StatutPaiementId).ToList();
                }

                foreach (Tutorat tutorat in listeTutorats)
                {
                    unTutorat = context.Tutorats.Single(t => t.TutoratId == tutorat.TutoratId);
                    unTuteur = context.Tuteurs.Single(tuteur => tuteur.Id == unTutorat.TuteurId);

                    double heuresTotal = 0;
                    foreach (PlageHoraire plageHoraire in unTutorat.ListePlageHoraire)
                    {
                        TimeSpan difference = plageHoraire.DateHeureFin.Subtract(plageHoraire.DateHeureDebut);
                        heuresTotal += difference.TotalHours;
                        unTutorat.PrixTotal = Math.Round(heuresTotal, 2) * unTuteur.Tarif;
                        unTutorat.NombreHeureTotal = Math.Round(heuresTotal, 2);
                    }
                }
            }
            vm.ListeSeances = listeTutorats;

            return View(vm);
        }

        [Authorize(Roles = RolesNames.TutoreTuteur)]
        public ActionResult ListePaiementsTutores(string Id, string rechercherPar, string rechercher, DateTime? dateDebut, DateTime? dateFin)
        {
            List<Tutorat> listeTutorats = new List<Tutorat>();

            ListePaiementsTutoresVM vm = new ListePaiementsTutoresVM();

            Tutorat unTutorat = new Tutorat();
            Tuteur unTuteur = new Tuteur();

            if (String.IsNullOrEmpty(Id))
            {
                return View("Error");
            }

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                if (rechercherPar == "Tuteur")
                {
                    listeTutorats = context.Tutorats.Include("Tuteur").Include("Tutore").Include("ListePlageHoraire").Include("StatutPaiement")
                        .Where(r => string.Concat(r.Tuteur.Prenom.Trim(), " ", r.Tuteur.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim()))
                        .Where(s => s.StatutPaiement.StatutPaiementId == context.StatutPaiements.FirstOrDefault(stat =>
                        stat.Statut == StatutFacture.AttentePaiement).StatutPaiementId).Where(d => d.TutoreId == Id).ToList();
                }
                else
                {
                    listeTutorats = context.Tutorats.Include("Tutore").Include("Tuteur").Include("ListePlageHoraire").Include("StatutPaiement")
                   .Where(s => s.StatutPaiement.StatutPaiementId == context.StatutPaiements.FirstOrDefault(stat => stat.Statut == StatutFacture.AttentePaiement).StatutPaiementId)
                   .Where(r => r.ListePlageHoraire.Any(p => p.DateHeureDebut >= dateDebut && p.DateHeureFin <= dateFin) || (dateDebut == null && dateFin == null))
                   .Where(d => d.TutoreId == Id).ToList();
                }
                
                

                foreach (Tutorat tutorat in listeTutorats)
                {
                    unTutorat = context.Tutorats.Single(t => t.TutoratId == tutorat.TutoratId);
                    unTuteur = context.Tuteurs.Single(tuteur => tuteur.Id == unTutorat.TuteurId);

                    double heuresTotal = 0;
                    foreach (PlageHoraire plageHoraire in unTutorat.ListePlageHoraire)
                    {
                        TimeSpan difference = plageHoraire.DateHeureFin.Subtract(plageHoraire.DateHeureDebut);
                        heuresTotal += difference.TotalHours;
                        unTutorat.PrixTotal = Math.Round(heuresTotal, 2) * unTuteur.Tarif;
                        unTutorat.NombreHeureTotal = Math.Round(heuresTotal, 2);
                    }
                }
            }
            vm.ListeSeances = listeTutorats;

            return View(vm);
        }
    }
}
