using Microsoft.AspNet.Identity;
using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.ViewModels.Commentaires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace ProjetIntegrationEquipe2.Controllers
{
    public class CommentairesController : Controller
    {
        // GET: Commentaires

        [Authorize(Roles = RolesNames.Administrateur)] //Affiche tous les commentaires avec la fonction modif ou supprimer POUR LES ADMINS SEULEMENT
        public ActionResult Index(string rechercherPar, string rechercher)
        {
            List<Commentaire> listeCommentaires = new List<Commentaire>();

            CommentairesIndexVM vm = new CommentairesIndexVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                listeCommentaires = dbContext.Commentaires.Include("Tuteur").Include("Tutore").ToList();
                if (rechercherPar == "Tutore")
                { 
                    vm.ListeCommentaires = dbContext.Commentaires.Include("Tuteur").Include("Tutore").Where(r => string.Concat(r.Tutore.Prenom.Trim(), " ", r.Tutore.Nom.Trim()).ToLower().
                    Contains(rechercher.Trim().ToLower())|| string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Tuteur")
                {
                    vm.ListeCommentaires = dbContext.Commentaires.Include("Tuteur").Include("Tutore").Where(r => string.Concat(r.Tuteur.Prenom.Trim(), " ", r.Tuteur.Nom.Trim()).ToLower()
                    .Contains(rechercher.Trim().ToLower())|| string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Titre")
                {
                    vm.ListeCommentaires = dbContext.Commentaires.Include("Tutore").Include("Tuteur")
                        .Where(r => (r.Titre != null && r.Titre.Trim().ToLower().Contains(rechercher.Trim().ToLower())) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else
                {
                    //Verifier si le string peut etre un double
                    bool chiffreBool = double.TryParse(rechercher, NumberStyles.Any, CultureInfo.InvariantCulture, out double rechercherToDouble);
                    //Verifier si le double est plus petit que 10 (voir else if ci-dessous pourquoi)
                    if (!string.IsNullOrEmpty(rechercher)
                        && chiffreBool == true
                        && !double.IsNaN(rechercherToDouble)
                        && !double.IsInfinity(rechercherToDouble)
                        && rechercherToDouble <= 9)
                    {
                        //La recherche se fait sur le chiffre + 1 ex: 4 va rechercher entre 4 et 4.9
                        vm.ListeCommentaires = listeCommentaires.Where(r => r.Cote < (rechercherToDouble + 1.0) && r.Cote >= rechercherToDouble).ToList();
                        return View(vm);
                    }
                    //Si le double est un chiffre a 2 decimales, seulement prendre la premiere
                    //Ex: rechercherToDouble == 22, la recherche se fait sur 2
                    else if (rechercherToDouble > 9)
                    {
                        rechercher = rechercher.Trim()[0] + ",0";
                        double rechercherToDouble2 = double.Parse(rechercher);
                        vm.ListeCommentaires = listeCommentaires.Where(r => r.Cote < (rechercherToDouble2 + 1.0) && r.Cote >= rechercherToDouble2).ToList();
                        return View(vm);
                    }
                    else
                    {
                        vm.ListeCommentaires= listeCommentaires;
                        return View(vm);
                    }

                }
                
            }

        }

        [Authorize(Roles = RolesNames.Tutore)] //Il y aura un bouton sur la pagecommentaires si l'utilisateur est un tutoré ET un Tuteur pour ammener vers cette page de gestion de commentaires
        public ActionResult PageCommentairesTutore(string Id, string rechercherPar, string rechercher)
        {
            List<Commentaire> listeCommentaires = new List<Commentaire>();

            CommentairesTutoreVM vm = new CommentairesTutoreVM();

            if (String.IsNullOrEmpty(Id))
            {
                return View("Error");
            }

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                listeCommentaires = dbContext.Commentaires.Include("Tuteur").Include("Tutore").Where(c => c.TutoreId == Id).ToList();
                if (rechercherPar == "Nom")
                {
                    vm.ListeCommentaires = dbContext.Commentaires.Include("Tuteur").Include("Tutore").Where(r => string.Concat(r.Tuteur.Prenom.Trim(), " ", r.Tuteur.Nom.Trim()).ToLower()
                    .Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).Where(c => c.TutoreId == Id).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Titre")
                {
                    vm.ListeCommentaires = dbContext.Commentaires.Include("Tutore").Include("Tuteur")
                    .Where(r => (r.Titre != null && r.Titre.Trim().ToLower().Contains(rechercher.Trim().ToLower())) || string.IsNullOrEmpty(rechercher.Trim())).Where(c => c.TutoreId == Id).ToList();
                    return View(vm);
                }
                else
                {
                    //Verifier si le string peut etre un double
                    bool chiffreBool = double.TryParse(rechercher, NumberStyles.Any, CultureInfo.InvariantCulture, out double rechercherToDouble);
                    //Verifier si le double est plus petit que 10 (voir else if ci-dessous pourquoi)
                    if (!string.IsNullOrEmpty(rechercher)
                        && chiffreBool == true
                        && !double.IsNaN(rechercherToDouble)
                        && !double.IsInfinity(rechercherToDouble)
                        && rechercherToDouble <= 9)
                    {
                        //La recherche se fait sur le chiffre + 1 ex: 4 va rechercher entre 4 et 4.9
                        vm.ListeCommentaires = listeCommentaires.Where(r => r.Cote < (rechercherToDouble + 1.0) && r.Cote >= rechercherToDouble).Where(c => c.TutoreId == Id).ToList();
                        return View(vm);
                    }
                    //Si le double est un chiffre a 2 decimales, seulement prendre la premiere
                    //Ex: rechercherToDouble == 22, la recherche se fait sur 2
                    else if (rechercherToDouble > 9)
                    {
                        rechercher = rechercher.Trim()[0] + ",0";
                        double rechercherToDouble2 = double.Parse(rechercher);
                        vm.ListeCommentaires = listeCommentaires.Where(r => r.Cote < (rechercherToDouble2 + 1.0) && r.Cote >= rechercherToDouble2).Where(c => c.TutoreId == Id).ToList();
                        return View(vm);
                    }
                    else
                    {
                        vm.ListeCommentaires = listeCommentaires;
                        return View(vm);
                    }
                }
                
            }
        }

        public ActionResult PageCommentaires(string Id, string rechercherPar, string rechercher) //Affiche les commentaires par utilisateur spécifique pour les autres users
        {
            List<Commentaire> listeCommentaires = new List<Commentaire>();

            CommentairesTuteurVM vm = new CommentairesTuteurVM();

            if (String.IsNullOrEmpty(Id))
            {
                return View("Error");
            }

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                listeCommentaires = dbContext.Commentaires.Include("Tuteur").Include("Tutore").Where(c => c.TuteurId == Id).ToList();
                if (rechercherPar == "Nom")
                {
                    vm.ListeCommentaires = dbContext.Commentaires.Include("Tuteur").Include("Tutore").Where(r => string.Concat(r.Tutore.Prenom.Trim(), " ", r.Tutore.Nom.Trim()).ToLower()
                    .Contains(rechercher.Trim().ToLower())|| string.IsNullOrEmpty(rechercher.Trim())).Where(c => c.TuteurId == Id).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Titre")
                {
                    vm.ListeCommentaires = dbContext.Commentaires.Include("Tutore").Include("Tuteur")
                    .Where(r => (r.Titre != null && r.Titre.Trim().ToLower().Contains(rechercher.Trim().ToLower())) || string.IsNullOrEmpty(rechercher.Trim())).Where(c => c.TuteurId == Id).ToList();
                    return View(vm);
                }
                else
                {
                    //Verifier si le string peut etre un double
                    bool chiffreBool = double.TryParse(rechercher, NumberStyles.Any, CultureInfo.InvariantCulture, out double rechercherToDouble);
                    //Verifier si le double est plus petit que 10 (voir else if ci-dessous pourquoi)
                    if (!string.IsNullOrEmpty(rechercher)
                        && chiffreBool == true
                        && !double.IsNaN(rechercherToDouble)
                        && !double.IsInfinity(rechercherToDouble)
                        && rechercherToDouble <= 9)
                    {
                        //La recherche se fait sur le chiffre + 1 ex: 4 va rechercher entre 4 et 4.9
                        vm.ListeCommentaires = listeCommentaires.Where(r => r.Cote < (rechercherToDouble + 1.0) && r.Cote >= rechercherToDouble).Where(c => c.TuteurId == Id).ToList();
                        return View(vm);
                    }
                    //Si le double est un chiffre a 2 decimales, seulement prendre la premiere
                    //Ex: rechercherToDouble == 22, la recherche se fait sur 2
                    else if (rechercherToDouble > 9)
                    {
                        rechercher = rechercher.Trim()[0] + ",0";
                        double rechercherToDouble2 = double.Parse(rechercher);
                        vm.ListeCommentaires = listeCommentaires.Where(r => r.Cote < (rechercherToDouble2 + 1.0) && r.Cote >= rechercherToDouble2).Where(c => c.TuteurId == Id).ToList();
                        return View(vm);
                    }
                    else
                    {
                        vm.ListeCommentaires = listeCommentaires;
                        //répare le bug que j'ait rouvé que si le tuteur n'avait pas de commentaires, son nom n'apparaissait pas dans le titre.
                        Tuteur unTuteur = dbContext.Tuteurs.Single(tuteur => tuteur.Id == Id);
                        vm.Prenom = unTuteur.Prenom;
                        vm.Nom = unTuteur.Nom;
                        
                        return View(vm);
                    }
                }          
            }
        }

        [Authorize(Roles = RolesNames.Tutore)]
        public ActionResult Create()
        {
            List<Tuteur> listeTuteurs = new List<Tuteur>();
            List<Tutorat> listeSeances = new List<Tutorat>();
            List<Tuteur> listeTuteursValide = new List<Tuteur>();
            ApplicationUser user = new ApplicationUser();
            CommentairesCreateVM vm = new CommentairesCreateVM();

            string unId = User.Identity.GetUserId();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                user = dbContext.Users.Single(u => u.Id == unId);
                listeTuteurs = dbContext.Tuteurs.ToList();
                listeSeances = dbContext.Tutorats.ToList();
            }

            foreach (Tuteur tuteur in listeTuteurs)
            {
                foreach (Tutorat seance in listeSeances)
                {
                   
                    if ((seance.TuteurId == tuteur.Id) && (seance.TutoreId == user.Id) && (!listeTuteursValide.Contains(tuteur))) //Ajout de liste.Contains pour empêcher doublons
                    {
                        listeTuteursValide.Add(tuteur);

                    }
                }
            }

            vm.IdTutore = user.Id;
            vm.ListeSeances = listeSeances;
            vm.ListeTuteurs = listeTuteursValide;

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = RolesNames.Tutore)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CommentairesCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                string unId = User.Identity.GetUserId();
                vm.ListeSeances = new List<Tutorat>();
                vm.ListeTuteurs = new List<Tuteur>();
                ApplicationUser user = new ApplicationUser();
                List<Tuteur> listeTuteursValide = new List<Tuteur>();

                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    vm.ListeSeances = dbContext.Tutorats.ToList();
                    vm.ListeTuteurs = dbContext.Tuteurs.ToList();
                    user = dbContext.Users.Single(u => u.Id == unId);
                }

                foreach (Tuteur tuteur in vm.ListeTuteurs) 
                {
                    foreach (Tutorat seance in vm.ListeSeances)
                    {
                        if ((seance.TuteurId == tuteur.Id) && (seance.TutoreId == user.Id) && (!listeTuteursValide.Contains(tuteur)))
                        {
                            listeTuteursValide.Add(tuteur);
                        }
                    }
                }
                vm.ListeTuteurs = listeTuteursValide;
                return View("Create", vm);
            }

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                Commentaire unCommentaire = new Commentaire();
                unCommentaire.TutoreId = User.Identity.GetUserId();
                unCommentaire.TuteurId = vm.IdTuteur;
                unCommentaire.Date = DateTime.Today;
                unCommentaire.Titre = vm.Titre;
                unCommentaire.Cote = vm.Cote;
                unCommentaire.CommentaireTexte = vm.Texte;

                dbContext.Commentaires.Add(unCommentaire);
                dbContext.SaveChanges();
            }

            if (User.IsInRole(RolesNames.Administrateur))
            {
                return PartialView("_PageValide", "Commentaires");
            }
            else
            {
                return RedirectToAction("PageCommentairesTutore", "Commentaires", new { Id = User.Identity.GetUserId() });
            }
        }

        [Authorize(Roles = RolesNames.TutoreAdministrateur)]
        public ActionResult Delete(int id)
        {
            string unId = User.Identity.GetUserId();
            try
            {
                Commentaire unCommentaire = new Commentaire();
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    unCommentaire = context.Commentaires.Single(commentaire => commentaire.CommentaireId == id);

                    if (unCommentaire.TutoreId == User.Identity.GetUserId() || User.IsInRole(RolesNames.Administrateur))
                    {
                        context.Commentaires.Remove(unCommentaire);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                return View("Error");
            }

            if (User.IsInRole(RolesNames.Administrateur))
            {
                return PartialView("_PageValide", "Commentaires");
            }

            return RedirectToAction("PageCommentairesTutore", "Commentaires", new { Id = unId });
        }

        [Authorize(Roles = RolesNames.TutoreAdministrateur)]
        public ActionResult Edit(int Id)
        {
            List<Tuteur> listeTuteurs = new List<Tuteur>();
            List<Tuteur> listeTuteursValide = new List<Tuteur>();
            ApplicationUser user = new ApplicationUser();
            List<Tutorat> listeSeances = new List<Tutorat>();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                Commentaire commentaireModif = dbContext.Commentaires.Single(m => m.CommentaireId == Id);
                string unId = User.Identity.GetUserId();
                user = dbContext.Users.Single(u => u.Id == unId);
                listeTuteurs = dbContext.Tuteurs.ToList();
                listeSeances = dbContext.Tutorats.ToList();

                CommentairesEditVM vm = new CommentairesEditVM
                {
                    Id = commentaireModif.CommentaireId,
                    Titre = commentaireModif.Titre,
                    Texte = commentaireModif.CommentaireTexte,
                    IdTuteur = commentaireModif.TuteurId,
                    Cote = commentaireModif.Cote,
                    IdTutore = commentaireModif.TutoreId,
                    ListeSeances = listeSeances
                };

                foreach (Tuteur tuteur in listeTuteurs)
                {
                    foreach (Tutorat seance in listeSeances)
                    {
                        if ((seance.TuteurId == tuteur.Id) && (seance.TutoreId == user.Id) && (!listeTuteursValide.Contains(tuteur)))
                        {
                            listeTuteursValide.Add(tuteur);
                        }
                    }
                }

                if (User.IsInRole(RolesNames.Administrateur))
                {
                    vm.ListeTuteurs = listeTuteurs; //assumant que l'admin a même le pouvoir absolu sur ca car listeTuteursValide ne fonctionne pas pour l'admin.
                }
                else
                {
                    vm.ListeTuteurs = listeTuteursValide;
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesNames.TutoreAdministrateur)]
        public ActionResult Edit(CommentairesEditVM vm)
        {
            List<Tuteur> listeTuteursValide = new List<Tuteur>();
            List<Tuteur> listeTuteurs = new List<Tuteur>();
            List<Tutorat> listeSeances = new List<Tutorat>();
            ApplicationUser user = new ApplicationUser();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                string unId = User.Identity.GetUserId();
                user = dbContext.Users.Single(u => u.Id == unId);
                listeTuteurs = dbContext.Tuteurs.ToList();
                listeSeances = dbContext.Tutorats.ToList();

                foreach (Tuteur tuteur in listeTuteurs)
                {
                    foreach (Tutorat seance in listeSeances)
                    {
                        if ((seance.TuteurId == tuteur.Id) && (seance.TutoreId == user.Id) && (!listeTuteursValide.Contains(tuteur)))
                        {
                            listeTuteursValide.Add(tuteur);
                        }
                    }
                }

                if (!ModelState.IsValid)
                {
                    CommentairesEditVM vma = new CommentairesEditVM
                    {
                        Cote = vm.Cote
                    };
                    if (User.IsInRole(RolesNames.Administrateur))
                    {
                        vma.ListeTuteurs = listeTuteurs; //la différence entre ici et l'autre en bas
                    }
                    else
                    {
                        vma.ListeTuteurs = listeTuteursValide;
                    }
                    return View(vma);
                }

                Commentaire commentModif = dbContext.Commentaires.Single(c => c.CommentaireId == vm.Id);
                commentModif.CommentaireTexte = vm.Texte;
                commentModif.Titre = vm.Titre;
                commentModif.TuteurId = vm.IdTuteur;
                commentModif.Cote = vm.Cote;
                dbContext.SaveChanges();

                if (User.IsInRole(RolesNames.Administrateur))
                {
                    return PartialView("_PageValide", "Commentaires");
                }

                return RedirectToAction("PageCommentairesTutore", "Commentaires", new { Id = unId });
            }
        }
    }
}
