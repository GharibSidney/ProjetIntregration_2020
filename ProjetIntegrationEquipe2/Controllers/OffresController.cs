using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.ViewModels.Offres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetIntegrationEquipe2.Controllers
{
    public class OffresController : Controller
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

        // GET: Offres
        [Authorize(Roles = RolesNames.TuteurAdministrateur)]
        public ActionResult Index(string rechercherPar, string rechercher, DateTime? dateDebut, DateTime? dateFin)
        {
            string id = User.Identity.GetUserId();
            OffresInfoVM vm = new OffresInfoVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                if (User.IsInRole(RolesNames.Tuteur))
                {
                    vm.ListeOffres = dbContext.OffreTutorats.Include("Tuteur").Include("ListePlageHoraire").Include("ListeDeCours").Where(d => d.TuteurId == id).ToList();
                }
                else
                {
                    vm.ListeOffres = dbContext.OffreTutorats.Include("Tuteur").Include("ListePlageHoraire").Include("ListeDeCours").ToList();
                }

                if (rechercherPar == "Nom")
                {
                    vm.ListeOffres = vm.ListeOffres.Where(r => string.Concat(r.Tuteur.Prenom.Trim(), " ", r.Tuteur.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Titre")
                {
                    vm.ListeOffres = vm.ListeOffres.Where(r => (r.Titre != null && r.Titre.Trim().ToLower().Contains(rechercher.Trim().ToLower())) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Courriel")
                {
                    vm.ListeOffres = vm.ListeOffres.Where(r => r.Tuteur.Email.Trim().ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Cours")
                {
                    vm.ListeOffres = vm.ListeOffres.Where(r => r.ListeDeCours.Any(c => string.Concat(c.Nom.Trim(), " - ", c.NumeroCours.Trim()).ToLower().Contains(rechercher.Trim().ToLower())) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else
                {
                    vm.ListeOffres = vm.ListeOffres.Where(r => r.ListePlageHoraire.Any(p => p.DateHeureDebut >= dateDebut && p.DateHeureFin <= dateFin) || (dateDebut == null && dateFin == null)).ToList();
                    return View(vm);
                }
            }
        }
        [Authorize(Roles = RolesNames.TutoreTuteur)]
        public ActionResult ListeOffres(string rechercherPar, string rechercher, DateTime? dateDebut, DateTime? dateFin)
        {
            OffresViewOnlyVM vm = new OffresViewOnlyVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                if (rechercherPar == "Nom")
                {
                    vm.ListeOffres = dbContext.OffreTutorats.Include("Tuteur").Include("ListePlageHoraire").Include("ListeDeCours")
                        .Where(r => string.Concat(r.Tuteur.Prenom.Trim(), " ", r.Tuteur.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Titre")
                {
                    vm.ListeOffres = dbContext.OffreTutorats.Include("Tuteur").Include("ListePlageHoraire").Include("ListeDeCours")
                        .Where(r => (r.Titre != null && r.Titre.Trim().ToLower().Contains(rechercher.Trim().ToLower())) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Courriel")
                {
                    vm.ListeOffres = dbContext.OffreTutorats.Include("Tuteur").Include("ListePlageHoraire").Include("ListeDeCours")
                        .Where(r => r.Tuteur.Email.Trim().ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Cours")
                {
                    vm.ListeOffres = dbContext.OffreTutorats.Include("Tuteur").Include("ListePlageHoraire").Include("ListeDeCours")
                        .Where(r => r.ListeDeCours.Any(c => string.Concat(c.Nom.Trim(), " - ", c.NumeroCours.Trim()).ToLower().Contains(rechercher.Trim().ToLower())) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else
                {
                    vm.ListeOffres = dbContext.OffreTutorats.Include("Tuteur").Include("ListePlageHoraire").Include("ListeDeCours")
                        .Where(r => r.ListePlageHoraire.Any(p => p.DateHeureDebut >= dateDebut && p.DateHeureFin <= dateFin) || (dateDebut == null && dateFin == null)).ToList();
                    return View(vm);
                }
            }
        }

        // GET: Offres
        [Authorize(Roles = RolesNames.Tuteur)]
        public ActionResult Create()
        {
            string tuteurId = User.Identity.GetUserId();

            CreateOffreVM vm = new CreateOffreVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                vm.ListeCoursMaitrises = dbContext.CoursMaitrises.Include("Cours").Where(l => l.TuteurId == tuteurId)
                    .Where(s => s.StatutCoursMaitrise.Statut == StatutCours.Validé).ToList();
            }

            //Ne pas oublier !
            vm.ListeDateTimes = new List<DateTime>();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateOffreVM model)
        {
            if (!ModelState.IsValid)
            {
                string tuteurId = User.Identity.GetUserId();

                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    //On doit resetter la propriété "ListeCoursMaitrises", pour qu'il réaffiche les cours maîtrisés lors d'une erreur !
                    model.ListeCoursMaitrises = dbContext.CoursMaitrises.Include("Cours").Where(l => l.TuteurId == tuteurId).Where(s => s.StatutCoursMaitrise.Statut == StatutCours.Validé).ToList();
                }

                return View("Create", model);
            }

            OffreTutorat offre = new OffreTutorat();

            offre.TuteurId = User.Identity.GetUserId();

            if (!String.IsNullOrEmpty(model.Titre))
            {
                offre.Titre = model.Titre;
            }

            offre.ListePlageHoraire = new List<PlageHoraire>();

            for (int compteur = 0; compteur < model.ListeDateTimes.Count(); compteur++)
            {
                DateTime debut = model.ListeDateTimes[compteur];
                compteur++;
                DateTime fin = model.ListeDateTimes[compteur];

                PlageHoraire plageHoraire = new PlageHoraire();
                plageHoraire.DateHeureDebut = debut;
                plageHoraire.DateHeureFin = fin;

                offre.ListePlageHoraire.Add(plageHoraire);
            }

            if (model.ListeCoursSelectionnesId != null)
            {
                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    offre.ListeDeCours = dbContext.Cours.Where(c => model.ListeCoursSelectionnesId.Any(cs => cs == c.CoursId)).ToList();
                    dbContext.OffreTutorats.Add(offre);
                    dbContext.SaveChanges();
                }
            }
            return PartialView("_PageValide", "Offres");
        }

        [Authorize(Roles = RolesNames.Tuteur)]
        public ActionResult Edit(int? id)
        {
            //Crée un espace mémoire (new) !
            OffreTutorat offre;

            string tuteurId = User.Identity.GetUserId();

            //Mettre le int id à nullable "?", car je ne veux pas que la page tombe en erreur si il n'y a pas de id à l'offre !
            if (id == null)
            {
                return View("Error");
            }

            EditOffreVM vm = new EditOffreVM();
            try
            {
                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    vm.ListeCoursMaitrises = dbContext.CoursMaitrises.Include("Cours").Where(l => l.TuteurId == tuteurId)
                        .Where(s => s.StatutCoursMaitrise.Statut == StatutCours.Validé).ToList();

                    //Include prend le nom de l'objet et non le nom de la classe !
                    //Constructeur dans le modèle "OffreTutorat", afin d'instancier une liste et ne pas à avoir à l'instancier à chaque fois !
                    offre = dbContext.OffreTutorats.Include("ListeDeCours").Include("ListePlageHoraire").Single(l => l.OffreId == id);
                }
            }
            catch (Exception)
            {
                return View("Error");
            }

            vm.Titre = offre.Titre;
            //Ne pas oublier d'inclure la propriété "ListeDeCours" du modèle "OffreTutorat", car sinon rien ne s'affichera !
            vm.ListeCoursOfferts = offre.ListeDeCours;

            List<Cours> listeCours = new List<Cours>();
            foreach (CoursMaitrise unCours in vm.ListeCoursMaitrises)
            {
                foreach (Cours unCoursChoisi in vm.ListeCoursOfferts)
                {
                    if (unCours.Cours.CoursId == unCoursChoisi.CoursId)
                    {
                        listeCours.Add(unCours.Cours);
                    }
                }
            }
            //Retire les cours maîtrisés déjà sélectionnés !
            foreach (Cours cours in listeCours)
            {
                vm.ListeCoursMaitrises.RemoveAll(c => c.Cours.CoursId == cours.CoursId);
            }

            //Ajoute les plages horaires !
            vm.ListeDateTimes = new List<DateTime>();
            foreach (PlageHoraire horaire in offre.ListePlageHoraire)
            {
                vm.ListeDateTimes.Add(horaire.DateHeureDebut);
                vm.ListeDateTimes.Add(horaire.DateHeureFin);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditOffreVM model)
        {
            if (model.ListeCoursMaitrisesId == null && model.ListeCoursSelectionnesId == null)
            {
                ModelState.AddModelError("ListeCoursMaitrisesId", "Il doit y avoir au moins un cours pour cette offre !");
            }

            string tuteurId = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                OffreTutorat offreAModifier;

                if (User.IsInRole(RolesNames.Tuteur) && string.Equals(User.Identity.GetUserId(), tuteurId))
                {
                    using (ApplicationDbContext dbContext = new ApplicationDbContext())
                    {
                        offreAModifier = dbContext.OffreTutorats.Include("ListeDeCours").Include("ListePlageHoraire").Single(l => l.OffreId == model.Id);
                        offreAModifier.Titre = model.Titre;

                        offreAModifier.ListePlageHoraire = new List<PlageHoraire>();
                        for (int compteur = 0; compteur < model.ListeDateTimes.Count(); compteur++)
                        {
                            DateTime debut = model.ListeDateTimes[compteur];
                            compteur++;
                            DateTime fin = model.ListeDateTimes[compteur];
                            PlageHoraire plageHoraire = new PlageHoraire();
                            plageHoraire.DateHeureDebut = debut;
                            plageHoraire.DateHeureFin = fin;
                            offreAModifier.ListePlageHoraire.Add(plageHoraire);
                        }

                        //Je dois vérifier que ma liste (DropDownList) n'est pas vide avant de faire le concat, sinon cela ne fonctionne pas !
                        if (model.ListeCoursSelectionnesId == null)
                        {
                            model.ListeCoursSelectionnesId = new List<int>();
                        }
                        //Je dois vérifier que ma liste (CheckBox) n'est pas vide avant de faire le concat, sinon cela ne fonctionne pas !
                        if (model.ListeCoursMaitrisesId == null)
                        {
                            model.ListeCoursMaitrisesId = new List<int>();
                        }
                        //Prend la liste des cours sélectionnées du (DropDownList) et fusionne (Concat) avec la liste de cours offerts (CheckBox) !
                        //Ne pas oublier le Include("ListeDeCours") dans la variable "offreAModifier" !
                        offreAModifier.ListeDeCours = dbContext.Cours.Where(c => model.ListeCoursSelectionnesId.Any(cs => cs == c.CoursId)).Concat(dbContext.Cours.Where(c => model.ListeCoursMaitrisesId.Any(cs => cs == c.CoursId))).ToList();

                        dbContext.SaveChanges();
                    }
                }
                return PartialView("_PageValide", "Offres");
            }

            OffreTutorat offre = new OffreTutorat();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                //On doit resetter la propriété "ListeCoursMaitrises", pour qu'il réaffiche les cours maîtrisés lors d'une erreur !
                model.ListeCoursMaitrises = dbContext.CoursMaitrises.Include("Cours").Where(l => l.TuteurId == tuteurId).Where(s => s.StatutCoursMaitrise.Statut == StatutCours.Validé).ToList();

                offre = dbContext.OffreTutorats.Include("ListePlageHoraire").Include("ListeDeCours").Single(l => l.OffreId == model.Id);

                model.ListeCoursOfferts = offre.ListeDeCours;

                //Retirer les cours déjà sélectionnés lors d'une erreur !
                List<Cours> listeCours = new List<Cours>();
                foreach (CoursMaitrise unCours in model.ListeCoursMaitrises)
                {
                    foreach (Cours unCoursChoisi in model.ListeCoursOfferts)
                    {
                        if (unCours.Cours.CoursId == unCoursChoisi.CoursId)
                        {
                            listeCours.Add(unCours.Cours);
                        }
                    }
                }
                //Retire les cours maîtrisés déjà sélectionnés !
                foreach (Cours cours in listeCours)
                {
                    model.ListeCoursMaitrises.RemoveAll(c => c.Cours.CoursId == cours.CoursId);
                }
            }

            return View("Edit", model);
        }

        [Authorize(Roles = RolesNames.TuteurAdministrateur)]
        public ActionResult Delete(int id)
        {
            try
            {
                OffreTutorat offre = new OffreTutorat();

                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    offre = dbContext.OffreTutorats.Single(o => o.OffreId == id);
                }

                if (offre.TuteurId == User.Identity.GetUserId() || User.IsInRole(RolesNames.Administrateur))
                {
                    using (ApplicationDbContext dbContext = new ApplicationDbContext())
                    {
                        dbContext.PlageHoraires.RemoveRange(dbContext.PlageHoraires.Where(ph => ph.OffreId == id));
                        dbContext.OffreTutorats.Remove(dbContext.OffreTutorats.Single(o => o.OffreId == id));
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
            return PartialView("_PageValide", "Offres");
        }
    }
}
