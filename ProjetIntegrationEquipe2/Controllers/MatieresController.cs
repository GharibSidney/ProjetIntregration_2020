using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.ViewModels.Matieres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProjetIntegrationEquipe2.Controllers
{
    public class MatieresController : Controller
    {

        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult Index(string rechercherPar, string rechercher)
        {
            MatieresIndexVM vm = new MatieresIndexVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                vm.MatieresList = dbContext.Matieres.Where(r => r.Nom.Trim().ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                return View(vm);
            }
        }

        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult Create()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(MatieresCreateVM model)
        {
            if (ModelState.IsValid)
            {
                Matiere matiere = new Matiere();

                matiere.Nom = model.Nom;
                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    dbContext.Matieres.Add(matiere);
                    dbContext.SaveChanges();
                }
                return PartialView("_PageValide", "Matieres");
            }
            else
            {
                ModelState.AddModelError("", "Une erreur c'est produite, veuillez réessayer");
                return View();
            }
        }

        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult Edit(int? id)
        {
            Matiere matiere;

            if (id == null)
            {
                return View("Error");
            }

            MatieresEditVM vm = new MatieresEditVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                matiere = dbContext.Matieres.Find((int)id);
            }

            if (matiere == null)
            {
                return View("Error");

            }
            vm.MatiereId = matiere.MatiereId;
            vm.Nom = matiere.Nom;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MatieresEditVM model)
        {
            if (ModelState.IsValid)
            {
                Matiere matiereAModifier;
                if (model == null)
                {
                    return View("Error");
                }

                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    matiereAModifier = dbContext.Matieres.Find(model.MatiereId);
                    if (matiereAModifier == null)
                    {
                        return View("Error");
                    }
                    else
                    {
                        try
                        {
                            matiereAModifier.Nom = model.Nom;
                            dbContext.SaveChanges();
                        }
                        catch (Exception)
                        {
                            return View("Error");
                        }
                    }
                }
                return PartialView("_PageValide", "Matieres");
            }
            else
            {
                //Model invalid
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
                return View("Error");
            }
            Matiere matiereToDelete;
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    //La cascade se fait automatiquement, cela comprent coursMaitrises et cours. Il faut simplement
                    // supprimer les offres/demandes sans cours
                    matiereToDelete = context.Matieres.Find((int)id);
                    context.Matieres.Remove(matiereToDelete);
                    context.SaveChanges();

                    //Verifier si il y a des demandes / offres vides et les supprimer apres avoir supprimer les matieres et cours
                    foreach (OffreTutorat offre in context.OffreTutorats.Include("ListeDeCours").ToList())
                    {
                        if (offre.ListeDeCours.Count() == 0 || offre.ListeDeCours == null)
                        {
                            context.PlageHoraires.RemoveRange(context.PlageHoraires.Where(w => w.OffreId == offre.OffreId));
                            context.OffreTutorats.Remove(offre);
                        }
                    }
                    foreach (DemandeTutorat demande in context.DemandeTutorats.Include("ListeDeCours").ToList())
                    {
                        if (demande.ListeDeCours.Count() == 0 || demande.ListeDeCours == null)
                        {
                            context.PlageHoraires.RemoveRange(context.PlageHoraires.Where(w => w.DemandeTutoratId == demande.DemandeTutoratId));
                            context.DemandeTutorats.Remove(demande);
                        }
                    }
                    context.SaveChanges();
                }
                catch (Exception)
                {
                    return View("Error");
                }
            }
            return PartialView("_PageValide", "Matieres");
        }
    }
}