using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.ViewModels.AjouterCours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProjetIntegrationEquipe2.Controllers
{
    public class AjouterCoursController : Controller
    {

        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult Index(string rechercherPar, string rechercher)
        {
            AjouterCoursIndexVM vm = new AjouterCoursIndexVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                if (rechercherPar == "Nom")
                {
                    vm.CoursList = dbContext.Cours.Include("Matiere").Where(r => r.Nom.Trim().ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Matiere")
                {
                    vm.CoursList = dbContext.Cours.Include("Matiere").Where(r => r.Matiere.Nom.Trim().ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else
                {
                    vm.CoursList = dbContext.Cours.Include("Matiere").Where(r => r.NumeroCours.Trim().ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
            }
        }

        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult Create()
        {
            List<Matiere> listeMatieres = new List<Matiere>();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                listeMatieres = dbContext.Matieres.ToList();
            }

            AjouterCoursCreateVM vm = new AjouterCoursCreateVM();
            vm.MatiereList = listeMatieres;
            return View(vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(AjouterCoursCreateVM model)
        {
            if (ModelState.IsValid)
            {
                Cours nouveauCours = new Cours();
                try
                {
                    nouveauCours.Nom = model.Nom;
                    nouveauCours.NumeroCours = model.NumeroCours;
                    using (ApplicationDbContext dbContext = new ApplicationDbContext())
                    {
                        nouveauCours.Matiere = dbContext.Matieres.Single(s => s.MatiereId == model.MatiereID);
                        dbContext.Cours.Add(nouveauCours);
                        dbContext.SaveChanges();
                    }
                    return PartialView("_PageValide", "AjouterCours");
                }
                catch (Exception)
                {
                    return View("Error");
                }

            }
            else
            {
                //Si le model n'est pas valide
                AjouterCoursIndexVM vm = new AjouterCoursIndexVM();
                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    vm.CoursList = dbContext.Cours.Include("Matiere").ToList();
                }
                ModelState.AddModelError("", "Une erreur c'est produite, veuillez réessayer");
                return View(vm);
            }
        }

        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult Edit(int? id)
        {
            Cours cours;

            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            AjouterCoursEditVM vm = new AjouterCoursEditVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                vm.MatiereList = dbContext.Matieres.ToList();
                cours = dbContext.Cours.Find(id);
            }

            if (cours == null)
            {
                return RedirectToAction("Index", "Home");

            }
            vm.MatiereID = cours.MatiereId;
            vm.Nom = cours.Nom;
            vm.NumeroCours = cours.NumeroCours;
            vm.CoursId = cours.CoursId;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AjouterCoursEditVM model)
        {
            if (ModelState.IsValid)
            {
                Cours CoursAModifier;
                if (model == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    CoursAModifier = dbContext.Cours.Find(model.CoursId);
                    if (CoursAModifier == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        CoursAModifier.MatiereId = model.MatiereID;
                        CoursAModifier.Nom = model.Nom;
                        CoursAModifier.NumeroCours = model.NumeroCours;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception)
                        {
                            return View("Error");
                        }
                    }
                }
                return PartialView("_PageValide", "AjouterCours");
            }
            else
            {
                //Model invalid
                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    model.MatiereList = dbContext.Matieres.ToList();
                }

                return View(model);
            }
        }

        [HttpPost]
        [Authorize(Roles = RolesNames.Administrateur)]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Cours coursToDelete = new Cours();
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                //Faire les cascades avant de supprimer le cours
                //Si l'offre a plus d'un cours en offre, retirer le cours seulement. Sinon retirer l'offre au complet
                try
                {
                    foreach (OffreTutorat offre in context.OffreTutorats.Include("ListeDeCours").ToList())
                    {
                        if (offre.ListeDeCours.Count() > 1)
                        {

                            foreach (Cours cours in offre.ListeDeCours.ToList())
                            {
                                if (cours.CoursId == (int)id)
                                {
                                    offre.ListeDeCours.Remove(cours);
                                }
                            }
                        }
                        else if (offre.ListeDeCours.Count() == 1)
                        {
                            if (offre.ListeDeCours.First().CoursId == (int)id)
                            {
                                context.PlageHoraires.RemoveRange(context.PlageHoraires.Where(w => w.OffreId == offre.OffreId));
                                context.OffreTutorats.Remove(offre);
                            }
                        }

                    }
                    //Si la demande a plus d'un cours en demande, retirer le cours seulement. Sinon retirer la demande au complet
                    foreach (DemandeTutorat demande in context.DemandeTutorats.Include("ListeDeCours").ToList())
                    {
                        if (demande.ListeDeCours.Count() > 1)
                        {
                            foreach (Cours cours in demande.ListeDeCours.ToList())
                            {
                                if (cours.CoursId == (int)id)
                                {
                                    demande.ListeDeCours.Remove(cours);
                                }
                            }
                        }
                        else if (demande.ListeDeCours.Count() == 1)
                        {
                            if (demande.ListeDeCours.First().CoursId == (int)id)
                            {
                                context.PlageHoraires.RemoveRange(context.PlageHoraires.Where(w => w.DemandeTutoratId == demande.DemandeTutoratId));
                                context.DemandeTutorats.Remove(demande);
                            }
                        }

                    }
                    //Cours maitrises des tuteurs peuvent contenir le cours
                    foreach (Tuteur tuteur in context.Tuteurs)
                    {
                        if (tuteur.ListeCoursMaitrise != null)
                        {
                            foreach (CoursMaitrise coursMaitrise in tuteur.ListeCoursMaitrise.ToList())
                            {
                                if (coursMaitrise.CoursId == (int)id)
                                {
                                    tuteur.ListeCoursMaitrise.Remove(coursMaitrise);
                                }
                            }
                        }
                    }
                    coursToDelete = context.Cours.Find(id);
                    context.Cours.Remove(coursToDelete);

                    context.SaveChanges();
                }
                catch (Exception)
                {
                    return View("Error");
                }
            }
            return PartialView("_PageValide", "AjouterCours");
        }
    }
}