using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ProjetIntegrationEquipe2.ViewModels.Tutores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjetIntegrationEquipe2.ViewModels.ApprouverCours;

namespace ProjetIntegrationEquipe2.Controllers
{
    public class ApprouverCoursController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

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
        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult Index(string rechercherPar, string rechercher)
        {
            List<Tuteur> listeTuteurs = new List<Tuteur>();
            List<CoursMaitrise> listeCoursMaitrises = new List<CoursMaitrise>();
            Tuteur tuteur = new Tuteur();
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                listeCoursMaitrises = dbContext.CoursMaitrises.Include("Cours").Include("Tuteur").Include("StatutCoursMaitrise").Where(z => z.StatutCoursMaitrise.Statut == StatutCours.En_attente).ToList();
                List<string> listeIDTuteurs = listeCoursMaitrises.Select(s => s.TuteurId).Distinct().ToList();
                foreach(string s in listeIDTuteurs)
                {
                    Tuteur tuteurTrouve = dbContext.Tuteurs.Single(x => x.Id == s);
                    listeTuteurs.Add(tuteurTrouve);
                }
            }

            ApprouverCoursIndexVM vm = new ApprouverCoursIndexVM();
            vm.CoursMaitriseList = listeCoursMaitrises;
            vm.TuteurList = listeTuteurs;
            vm.ListeStatusCoursMaitrise = new List<string>();
            if (rechercherPar == "Nom")
            {
                vm.TuteurList = listeTuteurs.Where(r => string.Concat(r.Prenom.Trim(), " ", r.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                return View(vm);
            }
            else if(rechercherPar == "Courriel")
            {
                vm.TuteurList = listeTuteurs.Where(r => r.Email.Trim().ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                return View(vm);
            }
            else
            {
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult Enregistrer(ApprouverCoursIndexVM vm)
        {
            foreach (var modelValue in ModelState.Values)
            {
                modelValue.Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                //TRI LES COURS PAR LE BON ORDRE DE TUTEURLIST
                //Les cours ne sont pas en ordre mais la liste de tuteur l'est donc, les cours sont mis en ordre avec l'ordre des tuteurs
                List<CoursMaitrise> CoursMaitriseEnOrdre = vm.CoursMaitriseList.OrderBy(o => vm.TuteurList.FindIndex(a => a.Id == o.TuteurId)).ToList();
                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    try
                    {
                        for (int i = 0; i < CoursMaitriseEnOrdre.Count(); i++)
                        {
                            //Il faut mettre le bon id de statutCoursMaitrise sur le cours maitrise
                            //Nous avons une liste de string avec ex: validé, refusé etc
                            //Donc, il faut trouver validé est == quel id et le mettre sur le coursMaitrisé pour son statutCoursMaitriseId
                            string statut = vm.ListeStatusCoursMaitrise[i];
                            CoursMaitriseEnOrdre[i].StatutCoursMaitriseId = dbContext.StatutCoursMaitrises.Single
                                (c => c.Statut == statut).StatutCoursMaitriseId;
                        }
                        //J'ai maintenant une liste de coursMaitrise avec le bon statutCoursMaitriseId, il manque de les associés au bon tuteurs
                        for (int iu = 0; iu < vm.TuteurList.Count(); iu++)
                        {
                            //Tuteur list contient seulement les Id des tuteurs
                            string tuteurID = vm.TuteurList[iu].Id;
                            //Je crée une liste d'objets tuteurs avec les Ids que j'ai
                            Tuteur tuteurToUpdate = dbContext.Tuteurs.Include("ListeCoursMaitrise").Single(s => s.Id == tuteurID);
                            for (int i = 0; i < tuteurToUpdate.ListeCoursMaitrise.Count(); i++)
                            {
                                //Je cherche voir si le cours que je viens de donner un nouveau statut est présent dans la liste de cours maitrise du tuteur
                                CoursMaitrise resultat = CoursMaitriseEnOrdre.Find(s => s.CoursMaitriseId == tuteurToUpdate.ListeCoursMaitrise[i].CoursMaitriseId);
                                //Si le coursMaitrise est présent, j'update son statutCoursMaitriseId ce qui change son statut
                                if (resultat != null)
                                {
                                    tuteurToUpdate.ListeCoursMaitrise[i].StatutCoursMaitriseId = resultat.StatutCoursMaitriseId;
                                }
                            }
                        }
                        dbContext.SaveChanges();
                    }
                    catch (Exception)
                    {
                        return View("Error");
                    }    
                }
                //AJOUTER ROLE TUTEUR SI NOUVEAU TUTEUR
                //Parcourir la liste de tuteur et sa liste de coursMaitrise pour trouver un cours avec statut accepté
                foreach (Tuteur tuteurRoleAdd in vm.TuteurList)
                {
                    try
                    {
                        bool resultatCoursAccepte = false;
                        List<CoursMaitrise> listeCoursMaitriseParTuteur = new List<CoursMaitrise>();
                        //Vérifier si il y a au moins un cours qui à été accepté pour chaque tuteur
                        //Si le tuteur ne se fait pas accepté un cours il ne recoit pas le role tuteur
                        using (ApplicationDbContext dbContext = new ApplicationDbContext())
                        {
                            listeCoursMaitriseParTuteur = dbContext.CoursMaitrises.Include("StatutCoursMaitrise").Where(i => i.TuteurId == tuteurRoleAdd.Id).ToList();
                        }
                        foreach (CoursMaitrise cours in listeCoursMaitriseParTuteur)
                        {
                            if (cours.StatutCoursMaitrise.Statut == StatutCours.Validé)
                            {
                                resultatCoursAccepte = true;
                            }
                        }
                        if (resultatCoursAccepte == true)
                        {
                            //Vérifier si le tuteur n'avait pas déja le role pour une raison et lui donner
                            if (!UserManager.IsInRole(tuteurRoleAdd.Id, RolesNames.Tuteur))
                            {
                                UserManager.AddToRole(tuteurRoleAdd.Id, RolesNames.Tuteur);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return View("Error");
                    }         
                }
                //RETOUR NORMAL
                return PartialView("_PageValide", "ApprouverCours");
            }
            //SI MODELSTATE EST INVALID
            return RedirectToAction("Index", "ApprouverCours");
        }
    }        
}