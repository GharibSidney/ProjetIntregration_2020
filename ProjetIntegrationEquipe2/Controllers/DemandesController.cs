using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.ViewModels.Demandes;
using ProjetIntegrationEquipe2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace ProjetIntegrationEquipe2.Controllers
{
    public class DemandesController : Controller
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

        // GET: Demandes
        [Authorize(Roles = RolesNames.TutoreTuteurAdministrateur)]
        public ActionResult Index(string rechercherPar, string rechercher, DateTime? dateDebut, DateTime? dateFin)
        {
            string tutoreId = User.Identity.GetUserId();
            DemandesInfoVM vm = new DemandesInfoVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                if (User.IsInRole(RolesNames.Tutore) || User.IsInRole(RolesNames.Tuteur))
                {
                    vm.ListeDemandes = dbContext.DemandeTutorats.Include("Tutore").Include("ListePlageHoraire").Include("ListeDeCours").Where(d => d.TutoreId == tutoreId).ToList();
                }
                else
                {
                    vm.ListeDemandes = dbContext.DemandeTutorats.Include("Tutore").Include("ListePlageHoraire").Include("ListeDeCours").ToList();
                }

                if (rechercherPar == "Nom")
                {
                    vm.ListeDemandes = vm.ListeDemandes.Where(r => string.Concat(r.Tutore.Prenom.Trim(), " ", r.Tutore.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Courriel")
                {
                    vm.ListeDemandes = vm.ListeDemandes.Where(r => r.Tutore.Email.Trim().ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Titre")
                {
                    vm.ListeDemandes = vm.ListeDemandes.Where(r => (r.Titre != null && r.Titre.Trim().ToLower().Contains(rechercher.Trim().ToLower())) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Cours")
                {
                    vm.ListeDemandes = vm.ListeDemandes.Where(r => r.ListeDeCours.Any(c => string.Concat(c.Nom.Trim(), " - ", c.NumeroCours.Trim()).ToLower().Contains(rechercher.Trim().ToLower())) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else
                {
                    vm.ListeDemandes = vm.ListeDemandes.Where(r => r.ListePlageHoraire.Any(p => p.DateHeureDebut >= dateDebut && p.DateHeureFin <= dateFin) || (dateDebut == null && dateFin == null)).ToList();
                    return View(vm);
                }
            }
        }

        [Authorize(Roles = RolesNames.Tuteur)]
        public ActionResult ListeDemandes(string rechercherPar, string rechercher, DateTime? dateDebut, DateTime? dateFin)
        {
            DemandeViewOnlyVM vm = new DemandeViewOnlyVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                if (rechercherPar == "Nom")
                {
                    vm.ListeDemandes = dbContext.DemandeTutorats.Include("Tutore").Include("ListePlageHoraire").Include("ListeDeCours")
                        .Where(r => string.Concat(r.Tutore.Prenom.Trim(), " ", r.Tutore.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Titre")
                {
                    vm.ListeDemandes = dbContext.DemandeTutorats.Include("Tutore").Include("ListePlageHoraire").Include("ListeDeCours")
                        .Where(r => (r.Titre != null && r.Titre.Trim().ToLower().Contains(rechercher.Trim().ToLower())) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Courriel")
                {
                    vm.ListeDemandes = dbContext.DemandeTutorats.Include("Tutore").Include("ListePlageHoraire").Include("ListeDeCours")
                        .Where(r => r.Tutore.Email.Trim().ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Cours")
                {
                    vm.ListeDemandes = dbContext.DemandeTutorats.Include("Tutore").Include("ListePlageHoraire").Include("ListeDeCours")
                        .Where(r => r.ListeDeCours.Any(c => string.Concat(c.Nom.Trim(), " - ", c.NumeroCours.Trim()).ToLower().Contains(rechercher.Trim().ToLower())) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else
                {
                    vm.ListeDemandes = dbContext.DemandeTutorats.Include("Tutore").Include("ListePlageHoraire").Include("ListeDeCours")
                        .Where(r => r.ListePlageHoraire.Any(p => p.DateHeureDebut >= dateDebut && p.DateHeureFin <= dateFin) || (dateDebut == null && dateFin == null)).ToList();
                    return View(vm);
                }
            }
        }

        [Authorize(Roles = RolesNames.TutoreTuteur)]
        public ActionResult Create()
        {
            List<Cours> listeCours = new List<Cours>();
            List<Matiere> listeMatiere = new List<Matiere>();
            List<CoursMaitrise> listeCoursDejaDemande = new List<CoursMaitrise>();

            ApplicationUser user = new ApplicationUser();
            string unId = User.Identity.GetUserId();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                listeCours = dbContext.Cours.ToList();
                listeMatiere = dbContext.Matieres.ToList();
                listeCoursDejaDemande = dbContext.CoursMaitrises.Where(l => l.TuteurId == unId).Where(s => s.StatutCoursMaitrise.Statut == StatutCours.Validé).ToList();
                user = dbContext.Users.Single(u => u.Id == unId);
            }

            DemandeCreateVM vm = new DemandeCreateVM();

            vm.ListeDeTousLesCours = listeCours;
            vm.ListeDeToutesLesMatieres = listeMatiere;
            vm.ListeDateTimes = new List<DateTime>();

            //Enlever les cours maîtrisés validés !
            foreach (CoursMaitrise coursMaitrise in listeCoursDejaDemande)
            {
                listeCours.Remove(coursMaitrise.Cours);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DemandeCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.ListeDeTousLesCours = new List<Cours>();
                vm.ListeDeToutesLesMatieres = new List<Matiere>();
                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    vm.ListeDeTousLesCours = dbContext.Cours.ToList();
                    vm.ListeDeToutesLesMatieres = dbContext.Matieres.ToList();
                }

                return View("Create", vm);
            }

            DemandeTutorat uneDemande = new DemandeTutorat();

            uneDemande.TutoreId = User.Identity.GetUserId();
            if (!String.IsNullOrEmpty(vm.Titre))
            {
                uneDemande.Titre = vm.Titre;
            }
            uneDemande.ListePlageHoraire = new List<PlageHoraire>();

            for (int compteur = 0; compteur < vm.ListeDateTimes.Count(); compteur++)
            {
                DateTime debut = vm.ListeDateTimes[compteur];
                compteur++;
                DateTime fin = vm.ListeDateTimes[compteur];

                PlageHoraire plageHoraire = new PlageHoraire();
                plageHoraire.DateHeureDebut = debut;
                plageHoraire.DateHeureFin = fin;

                uneDemande.ListePlageHoraire.Add(plageHoraire);

            }

            if (vm.ListeCoursSelectionnesId != null)
            {
                List<Cours> laListeDeCours = new List<Cours>();

                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    uneDemande.ListeDeCours = context.Cours.Where(c => vm.ListeCoursSelectionnesId.Any(cs => cs == c.CoursId)).ToList();
                    context.DemandeTutorats.Add(uneDemande);
                    context.SaveChanges();
                }
            }
            return PartialView("_PageValide", "Demandes");
        }

        [HttpPost]
        public JsonResult GetListeCours(List<int> listeMatieresId)
        {
            List<Cours> listeCours = new List<Cours>();
            List<Cours> listeCoursSelonMatiereId = new List<Cours>();
            List<CoursMaitrise> listeCoursDejaDemande = new List<CoursMaitrise>();

            string tuteurId = User.Identity.GetUserId();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                try
                {
                    listeCoursSelonMatiereId = dbContext.Cours.Where(x => listeMatieresId.Any(m => m == x.MatiereId)).ToList();

                    listeCoursDejaDemande = dbContext.CoursMaitrises.Where(l => l.TuteurId == tuteurId).Where(s => s.StatutCoursMaitrise.Statut == StatutCours.Validé).ToList();
                }
                catch (Exception)
                {
                }

                foreach (var cours in listeCoursSelonMatiereId)
                {
                    listeCours.Add(cours);

                    //Enlever les cours maîtrisés validés !
                    foreach (CoursMaitrise coursMaitrise in listeCoursDejaDemande)
                    {
                        listeCours.Remove(coursMaitrise.Cours);
                    }
                }
            }
            return Json(listeCours, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = RolesNames.TutoreTuteurAdministrateur)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                DemandeTutorat uneDemande = new DemandeTutorat();
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    uneDemande = context.DemandeTutorats.Single(demande => demande.DemandeTutoratId == id);
                }

                if (uneDemande.TutoreId == User.Identity.GetUserId() || User.IsInRole(RolesNames.Administrateur))
                {
                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {
                        context.PlageHoraires.RemoveRange(context.PlageHoraires.Where(plageHoraire => plageHoraire.DemandeTutoratId == (int)id));
                        context.DemandeTutorats.Remove(context.DemandeTutorats.Single(demande => demande.DemandeTutoratId == (int)id));
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
            return PartialView("_PageValide", "Demandes");
        }

        [Authorize(Roles = RolesNames.TutoreTuteur)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }

            List<CoursMaitrise> listeCoursDejaDemande = new List<CoursMaitrise>();

            string tutoreId = User.Identity.GetUserId();
            DemandeTutorat demande = new DemandeTutorat();

            DemandeEditVM vm = new DemandeEditVM();


            try
            {
                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    vm.ListeDeToutesLesMatieres = dbContext.Matieres.ToList();
                    vm.ListeDeTousLesCours = dbContext.Cours.ToList();
                    listeCoursDejaDemande = dbContext.CoursMaitrises.Where(l => l.TuteurId == tutoreId).Where(s => s.StatutCoursMaitrise.Statut == StatutCours.Validé).ToList();

                    demande = dbContext.DemandeTutorats.Include("ListePlageHoraire").Include("ListeDeCours").Single(l => l.DemandeTutoratId == id);
                }
            }
            catch (Exception)
            {
                return View("Error");
            }

            if (demande.TutoreId != User.Identity.GetUserId())
            {
                return View("Error");
            }

            vm.ListeCoursDejaChoisis = new List<Cours>();
            vm.ListeCoursDejaChoisis = demande.ListeDeCours;
            List<Cours> listeCours = new List<Cours>();

            //Retire les cours déjà choisi
            foreach (Cours unCours in vm.ListeDeTousLesCours)
            {
                foreach (Cours unCoursChoisi in vm.ListeCoursDejaChoisis)
                {
                    if (unCours.CoursId == unCoursChoisi.CoursId)
                    {
                        listeCours.Add(unCours);
                    }
                }
            }

            //Enlever les cours maîtrisés validés !
            foreach (CoursMaitrise coursMaitrise in listeCoursDejaDemande)
            {
                vm.ListeDeTousLesCours.Remove(coursMaitrise.Cours);
            }

            foreach (Cours cours in listeCours)
            {
                vm.ListeDeTousLesCours.RemoveAll(c => c.CoursId == cours.CoursId);
            }


            //Prend les id des cours non choisi (ceux qui reste)
            vm.ListeCoursDejaChoisisId = new List<int>();

            foreach (Cours cours in vm.ListeCoursDejaChoisis)
            {
                vm.ListeCoursDejaChoisisId.Add(cours.CoursId);
            }

            if (User.Identity.GetUserId() != demande.TutoreId)
            {
                return View("Error");
            }

            if (String.IsNullOrEmpty(demande.Titre))
            {
                vm.Titre = "";
            }
            else
            {
                vm.Titre = demande.Titre;
            }

            vm.DemandeId = (int)id;
            vm.ListeDateTimes = new List<DateTime>();
            vm.ListeCoursSelectionnesId = new List<int>();

            foreach (PlageHoraire horaire in demande.ListePlageHoraire)
            {
                vm.ListeDateTimes.Add(horaire.DateHeureDebut); //S'assurer que lorsque l'enregistrement on supprimer et on refait la collection au complet 
                //ou bien qu'on vérifie seulement ce qui a changé pour éviter de créer des doublons
                vm.ListeDateTimes.Add(horaire.DateHeureFin);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DemandeEditVM vm)
        {
            if (vm.ListeCoursSelectionnesId == null && vm.ListeCoursDejaChoisisId == null)
            {
                ModelState.AddModelError("ListeCoursDejaChoisisId", "Il doit y avoir au moins un cours pour la demande");
            }

            if (!ModelState.IsValid)
            {
                string tutoreId = User.Identity.GetUserId();

                //Ajouter les cours a affiché s'ils sont déjà dans choisi dans l'offre dans une autre liste
                vm.ListeDeTousLesCours = new List<Cours>();
                vm.ListeDeToutesLesMatieres = new List<Matiere>();
                vm.ListeCoursDejaChoisis = new List<Cours>();
                DemandeTutorat demande = new DemandeTutorat();

                List<CoursMaitrise> listeCoursDejaDemande = new List<CoursMaitrise>();

                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    vm.ListeDeTousLesCours = dbContext.Cours.ToList();
                    vm.ListeDeToutesLesMatieres = dbContext.Matieres.ToList();
                    demande = dbContext.DemandeTutorats.Include("ListePlageHoraire").Include("ListeDeCours").Single(l => l.DemandeTutoratId == vm.DemandeId);

                    listeCoursDejaDemande = dbContext.CoursMaitrises.Where(l => l.TuteurId == tutoreId).Where(s => s.StatutCoursMaitrise.Statut == StatutCours.Validé).ToList();
                }

                vm.ListeCoursDejaChoisis = demande.ListeDeCours;
                List<Cours> listeCours = new List<Cours>();
                //Retire les cours déjà choisi
                foreach (Cours unCours in vm.ListeDeTousLesCours)
                {
                    foreach (Cours unCoursChoisi in vm.ListeCoursDejaChoisis)
                    {
                        if (unCours.CoursId == unCoursChoisi.CoursId)
                        {
                            listeCours.Add(unCours);
                        }
                    }
                }

                //Enlever les cours maîtrisés validés !
                foreach (CoursMaitrise coursMaitrise in listeCoursDejaDemande)
                {
                    vm.ListeDeTousLesCours.Remove(coursMaitrise.Cours);
                }

                foreach (Cours cours in listeCours)
                {
                    vm.ListeDeTousLesCours.RemoveAll(c => c.CoursId == cours.CoursId);
                }


                //Prend les id des cours non choisi (ceux qui reste)
                vm.ListeCoursDejaChoisisId = new List<int>();

                foreach (Cours cours in vm.ListeCoursDejaChoisis)
                {
                    vm.ListeCoursDejaChoisisId.Add(cours.CoursId);
                }

                return View("Edit", vm);
            }

            DemandeTutorat uneDemande = new DemandeTutorat();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                uneDemande = dbContext.DemandeTutorats.Include("ListeDeCours").Include("ListePlageHoraire").Single(d => d.DemandeTutoratId == vm.DemandeId);
                if (!String.IsNullOrEmpty(vm.Titre))
                {
                    uneDemande.Titre = vm.Titre;
                }
                uneDemande.ListePlageHoraire = new List<PlageHoraire>();
                for (int compteur = 0; compteur < vm.ListeDateTimes.Count(); compteur++)
                {
                    DateTime debut = vm.ListeDateTimes[compteur];
                    compteur++;
                    DateTime fin = vm.ListeDateTimes[compteur];

                    PlageHoraire plageHoraire = new PlageHoraire();
                    plageHoraire.DateHeureDebut = debut;
                    plageHoraire.DateHeureFin = fin;

                    uneDemande.ListePlageHoraire.Add(plageHoraire);

                }

                if (vm.ListeCoursSelectionnesId == null)
                {
                    vm.ListeCoursSelectionnesId = new List<int>();
                }

                if (vm.ListeCoursDejaChoisisId == null)
                {
                    vm.ListeCoursDejaChoisisId = new List<int>();
                }

                uneDemande.ListeDeCours = dbContext.Cours.Where(c => vm.ListeCoursSelectionnesId.Any(cs => cs == c.CoursId)).Concat(dbContext.Cours.Where(c => vm.ListeCoursDejaChoisisId.Any(cs => cs == c.CoursId))).ToList();

                dbContext.SaveChanges();
            }
            return PartialView("_PageValide", "Demandes");
        }
    }
}
