using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.ViewModels.Seances;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetIntegrationEquipe2.Controllers
{
    public class SeancesController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult ListeSeances(string rechercherPar, string rechercher, DateTime? dateDebut, DateTime? dateFin)
        {
            List<Tutorat> listeTutorats = new List<Tutorat>();

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                if (rechercherPar == "Tuteur")
                {
                    listeTutorats = context.Tutorats.Include("Tuteur").Include("Tutore").Include("ListePlageHoraire")
                        .Where(r => string.Concat(r.Tuteur.Prenom.Trim(), " ", r.Tuteur.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                }
                else if (rechercherPar == "Tutore")
                {
                    listeTutorats = context.Tutorats.Include("Tuteur").Include("Tutore").Include("ListePlageHoraire")
                        .Where(r => string.Concat(r.Tutore.Prenom.Trim(), " ", r.Tutore.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                }
                else
                {
                    listeTutorats = context.Tutorats.Include("Tuteur").Include("Tutore").Include("ListePlageHoraire")
                        .Where(r => r.ListePlageHoraire.Any(p => p.DateHeureDebut >= dateDebut && p.DateHeureFin <= dateFin) || (dateDebut == null && dateFin == null)).ToList();
                }
            }
            foreach (Tutorat tutorat in listeTutorats)
            {
                foreach (PlageHoraire plage in tutorat.ListePlageHoraire)
                {
                    tutorat.NombreHeureTotal += Math.Round(plage.DateHeureFin.Subtract(plage.DateHeureDebut).TotalHours, 2);
                }
            }
            ListeSeanceVM vm = new ListeSeanceVM();
            vm.listeTutorats = listeTutorats;
            return View(vm);
        }

        [Authorize(Roles = RolesNames.Tuteur)]
        public ActionResult Create()
        {
            SeanceCreateVM vm = new SeanceCreateVM();

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                vm.ListeTutores = context.Tutores.ToList();

            }
            vm.ListeTutores.Remove(vm.ListeTutores.Single(t => t.Id == User.Identity.GetUserId()));

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SeanceCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    vm.ListeTutores = context.Tutores.ToList();

                }
                vm.ListeTutores.Remove(vm.ListeTutores.Single(t => t.Id == User.Identity.GetUserId()));

                return View("Create", vm);
            }



            StatutPaiement statutPaiement = new StatutPaiement();
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                statutPaiement = context.StatutPaiements.Single(s => s.Statut.Equals(StatutFacture.Attente));
            }

            Tutorat unTutorat = new Tutorat
            {
                ListePlageHoraire = new List<PlageHoraire>(),
                TutoreId = vm.TutoreId,
                TuteurId = User.Identity.GetUserId(),
                NombreRefus = 0,
                StatutPaiementId = statutPaiement.StatutPaiementId
            };

            for (int compteur = 0; compteur < vm.ListeDateTimes.Count(); compteur++)
            {
                DateTime debut = vm.ListeDateTimes[compteur];
                compteur++;
                DateTime fin = vm.ListeDateTimes[compteur];

                PlageHoraire plageHoraire = new PlageHoraire();
                plageHoraire.DateHeureDebut = debut;
                plageHoraire.DateHeureFin = fin;

                unTutorat.ListePlageHoraire.Add(plageHoraire);

            }

            //Je dois mettre une date dans la db si je veux enregistrer, mais la vraie date est celle lorsqu'on finalise le paiement
            unTutorat.DatePaiement = DateTime.Now;
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {

                    context.Tutorats.Add(unTutorat);
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                return View("Error");
            }

            return PartialView("_PageValide", "Seances");
        }

        [Authorize(Roles = RolesNames.Tuteur)]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return View("Error");
            }

            SeanceEditVM vm = new SeanceEditVM();
            Tutorat unTutorat = new Tutorat();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    unTutorat = context.Tutorats.Include("ListePlageHoraire").Single(t => t.TutoratId == id);
                    vm.ListeTutores = context.Tutores.ToList();

                }
            }
            catch (Exception)
            {
                return View("Error");
            }


            vm.ListeTutores.Remove(vm.ListeTutores.Single(t => t.Id == unTutorat.TuteurId));

            //Retourne l'utilisateur à la page d'accueil si la seance ne lui appartient pas
            if (User.IsInRole(RolesNames.Tuteur))
            {
                if (User.Identity.GetUserId() != unTutorat.TuteurId)
                {
                    return View("Error");
                }
            }

            foreach (PlageHoraire horaire in unTutorat.ListePlageHoraire)
            {
                vm.ListeDateTimes.Add(horaire.DateHeureDebut);
                vm.ListeDateTimes.Add(horaire.DateHeureFin);
            }
            vm.TutoratId = (int)id;
            vm.TutoreId = unTutorat.TutoreId;
            return View(vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(SeanceEditVM vm)
        {
            if (!ModelState.IsValid)
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    vm.ListeTutores = context.Tutores.ToList();
                    return View(vm);
                }
            }

            Tutorat unTutorat = new Tutorat();

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                unTutorat = context.Tutorats.Include("ListePlageHoraire").Single(t => t.TutoratId == vm.TutoratId);

                unTutorat.ListePlageHoraire = new List<PlageHoraire>();
                for (int compteur = 0; compteur < vm.ListeDateTimes.Count(); compteur++)
                {
                    DateTime debut = vm.ListeDateTimes[compteur];
                    compteur++;
                    DateTime fin = vm.ListeDateTimes[compteur];

                    PlageHoraire plageHoraire = new PlageHoraire();
                    plageHoraire.DateHeureDebut = debut;
                    plageHoraire.DateHeureFin = fin;

                    unTutorat.ListePlageHoraire.Add(plageHoraire);

                }
                unTutorat.TutoreId = vm.TutoreId;
                context.SaveChanges();
            }

            return PartialView("_PageValide", "Seances");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = RolesNames.TuteurAdministrateur)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                Tutorat unTutorat = new Tutorat();
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    unTutorat = context.Tutorats.Single(t => t.TutoratId == id);
                }

                if (unTutorat.TuteurId == User.Identity.GetUserId() || User.IsInRole(RolesNames.Administrateur))
                {
                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {
                        context.PlageHoraires.RemoveRange(context.PlageHoraires.Where(plageHoraire => plageHoraire.TutoratId == id));
                        context.Tutorats.Remove(context.Tutorats.Single(tutorat => tutorat.TutoratId == id));
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                return View("Error");
            }

            return PartialView("_PageValide", "Seances");
        }

        // GET: Seance
        [Authorize(Roles = RolesNames.Tuteur)]
        public ActionResult Index(string rechercherPar, string rechercher, DateTime? dateDebut, DateTime? dateFin)
        {
            string tuteurId = User.Identity.GetUserId();
            SeancesIndexVM vm = new SeancesIndexVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                vm.ListeSeances = dbContext.Tutorats.Include("Tutore").Include("Tuteur").Include("ListePlageHoraire").Where(d => d.TuteurId == tuteurId).ToList();
                //Afficher seulement les seances qui sont "En attente"
                vm.ListeSeances = vm.ListeSeances.Where(seance => seance.StatutPaiementId == dbContext.StatutPaiements.Single(p => p.Statut == StatutFacture.Attente).StatutPaiementId).ToList();

                if (rechercherPar == "Tutore")
                {
                    vm.ListeSeances = vm.ListeSeances.Where(r => string.Concat(r.Tutore.Prenom.Trim(), " ", r.Tutore.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                }
                else
                {
                    vm.ListeSeances = vm.ListeSeances.Where(r => r.ListePlageHoraire.Any(p => p.DateHeureDebut >= dateDebut && p.DateHeureFin <= dateFin) || (dateDebut == null && dateFin == null)).ToList();
                }
            }

            return View(vm);
        }

        [Authorize(Roles = RolesNames.Tutore)]
        public ActionResult IndexTutore()
        {
            string tutoreId = User.Identity.GetUserId();
            SeancesIndexTutoreVM vm = new SeancesIndexTutoreVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                vm.ListeSeances = dbContext.Tutorats.Include("Tutore").Include("ListePlageHoraire").Include("Tuteur").Where(d => d.TutoreId == tutoreId).ToList();
                vm.ListeSeances = vm.ListeSeances.Where(seance => seance.StatutPaiementId == dbContext.StatutPaiements.Single(p => p.Statut == StatutFacture.Attente).StatutPaiementId).ToList();
            }
            return View(vm);
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesNames.Tuteur)]
        public ActionResult Terminer(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }

            Tutorat unTutorat = new Tutorat();

            StatutPaiement statutPaiement = new StatutPaiement();
            try
            {

                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    unTutorat = context.Tutorats.Include("ListePlageHoraire").Single(t => t.TutoratId == (int)id);

                }

            }
            catch (Exception)
            {
                return View("Error");
            }

            if (unTutorat.TuteurId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Seances");
            }

            foreach (PlageHoraire plageHoraire in unTutorat.ListePlageHoraire)
            {
                if (plageHoraire.DateHeureDebut > DateTime.Now || plageHoraire.DateHeureFin > DateTime.Now)
                {
                    return View("Error");
                }
            }

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                unTutorat = context.Tutorats.Single(t => t.TutoratId == (int)id);
                statutPaiement = context.StatutPaiements.Single(s => s.Statut.Equals(StatutFacture.AttentePaiement));
                unTutorat.StatutPaiementId = statutPaiement.StatutPaiementId;
                context.SaveChanges();
            }
            return PartialView("_PageValide", "Seances");
        }
    }
}
