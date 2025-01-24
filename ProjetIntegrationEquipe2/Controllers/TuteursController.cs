using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.ViewModels;
using ProjetIntegrationEquipe2.ViewModels.Tuteurs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ProjetIntegrationEquipe2.Controllers
{
    public class TuteursController : Controller
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

        [Authorize]
        public ActionResult Profil(string Id)
        {
            List<Commentaire> listeCommentaires = new List<Commentaire>();

            ProfilTuteurVM vm = new ProfilTuteurVM();
            try
            {
                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    Tuteur modelProfil = dbContext.Tuteurs.Single(u => u.Id == Id);

                    vm.ListeCoursMaitrises = dbContext.CoursMaitrises.Include("Cours").Include("StatutCoursMaitrise").Where(l => l.TuteurId == Id).ToList();
                    //Nous ne voulont pas afficher les cours refuses pour le respect du tuteur
                    vm.ListeCoursMaitrises.RemoveAll(r => r.StatutCoursMaitrise.Statut == StatutCours.Refusé);

                    listeCommentaires = dbContext.Commentaires.Where(l => l.TuteurId == Id).ToList();
                    modelProfil.CoteTotal = listeCommentaires.Sum(x => x.Cote) / listeCommentaires.Count();

                    vm.Id = modelProfil.Id;
                    vm.CheminPhoto = modelProfil.CheminPhoto;
                    vm.Telephone = modelProfil.PhoneNumber;
                    vm.Prenom = modelProfil.Prenom;
                    vm.Nom = modelProfil.Nom;
                    vm.Courriel = modelProfil.Email;
                    vm.Forces = modelProfil.Force;
                    vm.Interets = modelProfil.Interet;
                    vm.Tarif = modelProfil.Tarif;
                    vm.CoteTotal = modelProfil.CoteTotal;

                    return View(vm);
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // GET: Tuteurs
        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult Index(string rechercherPar, string rechercher)
        {
            TuteursIndexVM vm = new TuteursIndexVM();
            List<Tuteur> listeTuteurs = new List<Tuteur>();
            List<Tuteur> listeVraiTuteurs = new List<Tuteur>();
            List<Commentaire> listeCommentaires = new List<Commentaire>();
            bool resultat;
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                vm.listeCoursMaitriseVM = dbContext.CoursMaitrises.Include("Cours").Where(s => s.StatutCoursMaitrise.Statut == StatutCours.Validé).ToList();
                listeTuteurs = dbContext.Tuteurs.Include("ListeCoursMaitrise.StatutCoursMaitrise").ToList();
                foreach (Tuteur tuteur in listeTuteurs)
                {
                    resultat = false;
                    listeCommentaires = dbContext.Commentaires.Where(l => l.TuteurId == tuteur.Id).ToList();
                    tuteur.CoteTotal = listeCommentaires.Sum(x => x.Cote) / listeCommentaires.Count();
                    if (Double.IsNaN(tuteur.CoteTotal))
                    {
                        tuteur.CoteTotal = 0.0;
                    }
                    foreach (CoursMaitrise cours in tuteur.ListeCoursMaitrise)
                    {
                        if (cours.StatutCoursMaitrise.Statut == StatutCours.Validé)
                        {
                            resultat = true;
                        }
                    }
                    if (resultat == true)
                    {
                        listeVraiTuteurs.Add(tuteur);
                    }
                }
                if (rechercherPar == "Nom")
                {
                    vm.listeTuteursVM = dbContext.Tuteurs.Where(r => string.Concat(r.Prenom.Trim(), " ", r.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Cote")
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
                        vm.listeTuteursVM = listeTuteurs.Where(r => r.CoteTotal < (rechercherToDouble + 1.0) && r.CoteTotal >= rechercherToDouble).ToList();
                        return View(vm);
                    }
                    //Si le double est un chiffre a 2 decimales, seulement prendre la premiere
                    //Ex: rechercherToDouble == 22, la recherche se fait sur 2
                    else if(rechercherToDouble > 9)
                    {
                        rechercher = rechercher.Trim()[0] + ",0";
                        double rechercherToDouble2 =  double.Parse(rechercher);
                        vm.listeTuteursVM = listeTuteurs.Where(r => r.CoteTotal < (rechercherToDouble2 + 1.0) && r.CoteTotal >= rechercherToDouble2).ToList();
                        return View(vm);
                    }
                    else
                    {
                        vm.listeTuteursVM = listeTuteurs;
                        return View(vm);
                    }
                }
                else if (rechercherPar == "Courriel")
                {
                    vm.listeTuteursVM = dbContext.Tuteurs.Where(r => r.Email.Trim().ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "CoursMaitrises")
                {
                    vm.listeTuteursVM = dbContext.Tuteurs.Where(r => r.ListeCoursMaitrise.Where(s => s.StatutCoursMaitrise.Statut == StatutCours.Validé).Any(c => string.Concat(c.Cours.Nom.Trim(), " - ", c.Cours.NumeroCours.Trim()).ToLower().Contains(rechercher.Trim().ToLower())) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else
                {
                    vm.listeTuteursVM = listeVraiTuteurs;
                    return View(vm);
               }
            }
        }

        // GET: Tuteurs
        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult Create()
        {
            List<Matiere> listeMatiere = new List<Matiere>();
            List<Cours> listeCours = new List<Cours>();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                listeMatiere = dbContext.Matieres.ToList();
                listeCours = dbContext.Cours.ToList();
            }

            CreateTuteurVM vm = new CreateTuteurVM();
            vm.ListeMatieres = listeMatiere;
            vm.ListeCoursDemandes = listeCours;
            return View(vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(CreateTuteurVM model)
        {
            if (ModelState.IsValid)
            {
                //4194304 est 4mb
                if (model.ImageFile.ContentLength > 4194304)
                {
                    ModelState.AddModelError("ImageSizeError", "L'image est plus grande que 4mb!!");
                    return View("Create", model);
                }

                //Photo !
                //Nom de l'image !
                string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                //Extension (PNG) !
                string extention = Path.GetExtension(model.ImageFile.FileName);
                //Nom de l'image avec l'extension !
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extention;
                //Setter le propriété du VM !
                model.CheminPhoto = "~/upload/Image/" + fileName;
                //Mettre le nom sur le serveur !
                fileName = Path.Combine(Server.MapPath("~/upload/Image/"), fileName);
                //Sauvegarde sur le serveur !
                model.ImageFile.SaveAs(fileName);

                Tuteur tuteur = new Tuteur();
                tuteur.CheminPhoto = model.CheminPhoto;
                tuteur.Prenom = model.Prenom;
                tuteur.Nom = model.Nom;
                tuteur.PhoneNumber = model.PhoneNumber;
                tuteur.Email = model.Email;
                tuteur.Interet = model.Interet;
                tuteur.Force = model.Force;
                tuteur.Tarif = model.Tarif;
                tuteur.UserName = model.Email;

                List<CoursMaitrise> listeCoursMaitrisesDemandes = new List<CoursMaitrise>();

                StatutCoursMaitrise statutCoursMaitrise = new StatutCoursMaitrise();

                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    statutCoursMaitrise = dbContext.StatutCoursMaitrises.Single(s => s.Statut.Equals(StatutCours.Validé));
                }

                //Parcourir les ID de ListeCoursSelectionnesId, qui sont sélectionné par la vue Create (m => m.ListeCoursSelectionnesId) !
                //int cours contient seulement les ID sélectionnés par la vue Create (m => m.ListeCoursSelectionnesId) !
                if (model.ListeCoursSelectionnesId != null)
                {
                    foreach (int cours in model.ListeCoursSelectionnesId)
                    {
                        //Fonctionne avec la création d'un constructeur plein dans le modèle "CoursMaitrise" !
                        CoursMaitrise coursMaitrise = new CoursMaitrise(cours, tuteur.Id, statutCoursMaitrise.StatutCoursMaitriseId);

                        listeCoursMaitrisesDemandes.Add(coursMaitrise);
                    }
                }
                tuteur.ListeCoursMaitrise = listeCoursMaitrisesDemandes;

                var result = UserManager.Create(tuteur, model.Password);

                if (result.Succeeded)
                {
                    UserManager.AddToRole(tuteur.Id, RolesNames.Tuteur);

                    if (User.IsInRole(RolesNames.Administrateur))
                    {
                        //Etant donne que ce tuteur est cree par l'admin il faut ajouter le role tutore aussi
                        UserManager.AddToRoles(tuteur.Id, RolesNames.Tutore);
                        UserManager.AddToRoles(tuteur.Id, RolesNames.Tuteur);
                        return RedirectToAction("Index", "Tuteurs");
                    }
                    else
                    {
                        SignInManager.SignIn(tuteur, isPersistent: false, rememberBrowser: false);

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    //Si le model n'est pas valide reprendre la liste de matières, pour ne pas qu'elle soit null. Empêche que le programme plante si le courriel est pris 2 fois.
                    List<Matiere> listeMatieres = new List<Matiere>();
                    //Si le model n'est pas valide reprendre la liste de cours, pour ne pas qu'elle soit null. Empêche que le programme plante si le courriel est pris 2 fois.
                    List<Cours> listeCours = new List<Cours>();

                    using (ApplicationDbContext dbContext = new ApplicationDbContext())
                    {
                        listeMatieres = dbContext.Matieres.ToList();
                        listeCours = dbContext.Cours.ToList();
                    }

                    CreateTuteurVM vm = new CreateTuteurVM();

                    vm.ListeMatieres = listeMatieres;
                    vm.ListeCoursDemandes = listeCours;

                    ModelState.AddModelError("Email", "Cette adresse courriel est déjà prise");

                    return View(vm);
                }
            }
            else
            {
                //Si le model n'est pas valide reprendre la liste de matières, pour ne pas qu'elle soit null.
                List<Matiere> listeMatieres = new List<Matiere>();
                //Si le model n'est pas valide reprendre la liste de cours, pour ne pas qu'elle soit null.
                List<Cours> listeCours = new List<Cours>();

                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    listeMatieres = dbContext.Matieres.ToList();
                    listeCours = dbContext.Cours.ToList();
                }

                CreateTuteurVM vm = new CreateTuteurVM();

                vm.ListeMatieres = listeMatieres;
                vm.ListeCoursDemandes = listeCours;

                return View(vm);
            }
        }

        // GET: Tuteurs
        [Authorize(Roles = RolesNames.Tutore)]
        public ActionResult CreateTuteurFromTutore()
        {
            List<Matiere> listeMatieres = new List<Matiere>();
            List<Cours> listeCours = new List<Cours>();

            string currentUserID = User.Identity.GetUserId();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                List<CoursMaitrise> listeCoursDejaDemande = dbContext.CoursMaitrises.Where(i => i.TuteurId == currentUserID).ToList();

                listeMatieres = dbContext.Matieres.ToList();
                listeCours = dbContext.Cours.ToList();

                foreach (CoursMaitrise cours in listeCoursDejaDemande)
                {
                    listeCours.Remove(cours.Cours);
                }
            }

            TutoreVersTuteurVM vm = new TutoreVersTuteurVM();

            vm.ListeMatieres = listeMatieres;
            vm.ListeCoursDemandes = listeCours;

            return View(vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateTuteurFromTutore(TutoreVersTuteurVM model)
        {
            if (ModelState.IsValid)
            {
                string unId = User.Identity.GetUserId();

                List<CoursMaitrise> listeCoursMaitrisesDemandes = new List<CoursMaitrise>();

                StatutCoursMaitrise statutCoursMaitriseEnAttente = new StatutCoursMaitrise();

                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    statutCoursMaitriseEnAttente = dbContext.StatutCoursMaitrises.Single(s => s.Statut.Equals(StatutCours.En_attente));
                }

                //Parcourir les ID de ListeCoursSelectionnesId, qui sont sélectionné par la vue Create(m => m.ListeCoursSelectionnesId) !
                //int cours contient seulement les ID sélectionnés par la vue Create(m => m.ListeCoursSelectionnesId) !
                if (model.ListeCoursSelectionnesId != null)
                {
                    foreach (int cours in model.ListeCoursSelectionnesId)
                    {
                        //Fonctionne avec la création d'un constructeur plein dans le modèle "CoursMaitrise" !
                        CoursMaitrise coursMaitrise = new CoursMaitrise(cours, unId, statutCoursMaitriseEnAttente.StatutCoursMaitriseId);

                        listeCoursMaitrisesDemandes.Add(coursMaitrise);
                    }
                }

                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    Tuteur tuteurFind = dbContext.Tuteurs.Where(z => z.Id == unId).FirstOrDefault();
                    //LE TUTEUR EST ECRIT DIRECTEMENT DANS LA BD
                    if (tuteurFind == null)
                    {
                        //Ajoute le ID et le tarif dans la table "Tuteur" !
                        //Il doit y avoir au moins un cours d'approuver avant d'avoir le rôle de tuteur !
                        dbContext.Database.ExecuteSqlCommand("insert into Tuteur(Id, Tarif) values('" + unId + "'," + model.Tarif + ")");
                        tuteurFind = dbContext.Tuteurs.Single(z => z.Id == unId);
                    }

                    //TUTEUR DOIT ETRE AJOUTER AVANT D'AJOUTER LES COURS
                    tuteurFind.ListeCoursMaitrise = listeCoursMaitrisesDemandes;
                    dbContext.SaveChanges();
                }
                return PartialView("_PageValide", "Home");
            }
            else
            {
                List<Cours> listeCours = new List<Cours>();
                List<Matiere> listeMatieres = new List<Matiere>();

                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    listeCours = dbContext.Cours.ToList();
                    listeMatieres = dbContext.Matieres.ToList();
                }

                model.ListeMatieres = listeMatieres;
                model.ListeCoursDemandes = listeCours;

                return View(model);
            }
        }

        [Authorize(Roles = RolesNames.TuteurAdministrateur)]
        public ActionResult Edit(string id)
        {
            //Crée un espace mémoire (new) !
            Tuteur tuteur;

            if (string.IsNullOrEmpty(id))
            {
                return View("Error");
            }

            EditTuteurVM vm = new EditTuteurVM();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                vm.ListeMatieres = dbContext.Matieres.ToList();
                vm.ListeCoursDemandes = dbContext.Cours.ToList();
                vm.ListeCoursMaitrises = dbContext.CoursMaitrises.Include("StatutCoursMaitrise").Where(l => l.TuteurId == id).ToList();
                tuteur = (Tuteur)UserManager.FindById(id);
            }

            if (tuteur == null)
            {
                return View("Error");

            }

            if (tuteur.Id != User.Identity.GetUserId() && (!User.IsInRole(RolesNames.Administrateur)))
            {
                return View("Error");
            }

            foreach (CoursMaitrise coursMaitrise in vm.ListeCoursMaitrises)
            {
                vm.ListeCoursDemandes.Remove(coursMaitrise.Cours);
            }

            vm.Id = tuteur.Id;
            vm.CheminPhoto = tuteur.CheminPhoto;
            vm.Prenom = tuteur.Prenom;
            vm.Nom = tuteur.Nom;
            vm.PhoneNumber = tuteur.PhoneNumber;
            vm.Email = tuteur.Email;
            vm.ConfirmEmail = tuteur.Email;
            vm.Interet = tuteur.Interet;
            vm.Force = tuteur.Force;
            vm.Tarif = tuteur.Tarif;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditTuteurVM model)
        {
            if (model.ImageFile != null)
            {
                //4194304 est 4mb
                if (model.ImageFile.ContentLength > 4194304)
                {
                    ModelState.AddModelError("ImageSizeError", "L'image est plus grande que 4mb!!");
                    return View("Edit", model);
                }
                //Supprimer l'ancienne photo si elle a changé dans le dossier image !
                string cheminComplet = Request.MapPath(model.CheminPhoto);
                if (System.IO.File.Exists(cheminComplet))
                {
                    System.IO.File.Delete(cheminComplet);
                }

                string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                string extention = Path.GetExtension(model.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extention;
                model.CheminPhoto = "~/upload/Image/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/upload/Image/"), fileName);
                model.ImageFile.SaveAs(fileName);
            }

            if (ModelState.IsValid)
            {
                Tuteur tuteurAModifier;

                tuteurAModifier = (Tuteur)UserManager.FindById(model.Id);

                string tuteurId = tuteurAModifier.Id;

                if (User.IsInRole(RolesNames.Tuteur) && string.Equals(User.Identity.GetUserId(), tuteurId)
                    || User.IsInRole(RolesNames.Administrateur))
                {
                    tuteurAModifier.CheminPhoto = model.CheminPhoto;
                    tuteurAModifier.Prenom = model.Prenom;
                    tuteurAModifier.Nom = model.Nom;
                    tuteurAModifier.PhoneNumber = model.PhoneNumber;
                    tuteurAModifier.Email = model.Email;
                    tuteurAModifier.Interet = model.Interet;
                    tuteurAModifier.Force = model.Force;
                    tuteurAModifier.Tarif = model.Tarif;

                    List<CoursMaitrise> listeCoursMaitrisesDemandes = new List<CoursMaitrise>();

                    StatutCoursMaitrise statutCoursMaitrise = new StatutCoursMaitrise();

                    using (ApplicationDbContext dbContext = new ApplicationDbContext())
                    {
                        if (User.IsInRole(RolesNames.Administrateur))
                        {
                            statutCoursMaitrise = dbContext.StatutCoursMaitrises.Single(s => s.Statut.Equals(StatutCours.Validé));
                        }
                        else
                        {
                            statutCoursMaitrise = dbContext.StatutCoursMaitrises.Single(s => s.Statut.Equals(StatutCours.En_attente));
                        }
                    }

                    if (model.ListeCoursSelectionnesId != null)
                    {
                        foreach (int coursId in model.ListeCoursSelectionnesId)
                        {
                            //Fonctionne avec la création d'un constructeur plein dans le modèle "CoursMaitrise" !
                            CoursMaitrise coursMaitrise = new CoursMaitrise(coursId, tuteurAModifier.Id, statutCoursMaitrise.StatutCoursMaitriseId);

                            listeCoursMaitrisesDemandes.Add(coursMaitrise);
                        }
                    }
                    tuteurAModifier.ListeCoursMaitrise = listeCoursMaitrisesDemandes;

                    var result = UserManager.Update(tuteurAModifier);

                    //Si le model n'est pas valdie reprendre la liste de cours, pour ne pas qu'elle soit null. Empêche que le programme plante si le courriel est pris 2 fois.
                    if (!result.Succeeded)
                    {
                        using (ApplicationDbContext dbContext = new ApplicationDbContext())
                        {
                            model.ListeMatieres = dbContext.Matieres.ToList();
                            model.ListeCoursDemandes = dbContext.Cours.ToList();
                            model.ListeCoursMaitrises = dbContext.CoursMaitrises.Where(l => l.TuteurId == model.Id).ToList();

                        }

                        foreach (CoursMaitrise coursMaitrise in model.ListeCoursMaitrises)
                        {
                            model.ListeCoursDemandes.Remove(coursMaitrise.Cours);
                        }

                        ModelState.AddModelError("Email", "Cette adresse courriel est déjà prise");
                        return View(model);
                    }
                    return PartialView("_PageValide", "Home");
                }
            }
            else
            {
                //Code de secours côté serveur, si jamais le côté client ne fonctionne pas comme prévu.
                //Si le model n'est pas valide reprendre la liste de cours, pour ne pas qu'elle soit null.
                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    model.ListeMatieres = dbContext.Matieres.ToList();
                    model.ListeCoursDemandes = dbContext.Cours.ToList();
                    model.ListeCoursMaitrises = dbContext.CoursMaitrises.Where(l => l.TuteurId == model.Id).ToList();
                }

                foreach (CoursMaitrise coursMaitrise in model.ListeCoursMaitrises)
                {
                    model.ListeCoursDemandes.Remove(coursMaitrise.Cours);
                }

                return View(model);
            }

            if (User.IsInRole(RolesNames.Administrateur))
            {
                return PartialView("_PageValide", "Tuteurs");
            }
            else
            {
                return PartialView("_PageValide", "Home");
            }
        }
        [HttpPost]
        [Authorize(Roles = RolesNames.Administrateur)]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            Tutore unTutore;
            List<Tutorat> listeIdTutorat;
            List<DemandeTutorat> listeDemandeTutorats;
            List<OffreTutorat> listeOffresTutorats;
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    //Section tutoré
                    unTutore = context.Tuteurs.Single(tuteur => tuteur.Id == id);

                    listeIdTutorat = context.Tutorats.Where(tutorat => tutorat.TutoreId == id || tutorat.TuteurId == id).ToList();

                    listeDemandeTutorats = context.DemandeTutorats.Where(demande => demande.TutoreId == id).ToList();

                    context.Commentaires.RemoveRange(context.Commentaires.Where(c => c.TutoreId == id || c.TuteurId == id));

                    //Je veux supprimer toutes les plages horaires lié aux tutorats du tutoré à supprimé
                    foreach (Tutorat tutorat1 in listeIdTutorat)
                    {
                        context.PlageHoraires.RemoveRange(context.PlageHoraires.Where(c => c.TutoratId == tutorat1.TutoratId));
                    }

                    context.Tutorats.RemoveRange(context.Tutorats.Where(c => c.TutoreId == id || c.TuteurId == id));

                    //Je veux supprimer toutes les plages horaires lié aux demandes du tutoré à supprimé
                    foreach (DemandeTutorat demandeTutorat in listeDemandeTutorats)
                    {
                        context.PlageHoraires.RemoveRange(context.PlageHoraires.Where(c => c.DemandeTutoratId == demandeTutorat.DemandeTutoratId));
                    }

                    context.DemandeTutorats.RemoveRange(context.DemandeTutorats.Where(c => c.TutoreId == id));

                    //Section tuteur
                    listeOffresTutorats = context.OffreTutorats.Where(offre => offre.TuteurId == id).ToList();

                    foreach (OffreTutorat offreTutorat in listeOffresTutorats)
                    {
                        context.PlageHoraires.RemoveRange(context.PlageHoraires.Where(c => c.OffreId == offreTutorat.OffreId));
                    }

                    context.OffreTutorats.RemoveRange(context.OffreTutorats.Where(c => c.TuteurId == id));
                    context.CoursMaitrises.RemoveRange(context.CoursMaitrises.Where(c => c.TuteurId == id));
                    context.SaveChanges();
                }
                UserManager.Delete(UserManager.FindById(id));
            }
            catch (Exception)
            {
                return View("Error");
            }

            return PartialView("_PageValide", "Tuteurs");
        }

        public ActionResult ListeTuteurs(string rechercherPar, string rechercher)
        {
            List<Tuteur> listeTuteurs = new List<Tuteur>();
            List<Tuteur> listeVraiTuteurs = new List<Tuteur>();
            List<Commentaire> listeCommentaires = new List<Commentaire>();

            TuteursViewOnlyVM vm = new TuteursViewOnlyVM();

            bool resultat;

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                vm.listeCoursMaitriseVM = dbContext.CoursMaitrises.Include("Cours").Where(s => s.StatutCoursMaitrise.Statut == StatutCours.Validé).ToList();
                listeTuteurs = dbContext.Tuteurs.Include("ListeCoursMaitrise.StatutCoursMaitrise").ToList();

                foreach (Tuteur tuteur in listeTuteurs)
                {
                    resultat = false;
                    listeCommentaires = dbContext.Commentaires.Where(l => l.TuteurId == tuteur.Id).ToList();
                    tuteur.CoteTotal = listeCommentaires.Sum(x => x.Cote) / listeCommentaires.Count();
                    if (Double.IsNaN(tuteur.CoteTotal))
                    {
                        tuteur.CoteTotal = 0.0;
                    }
                    foreach (CoursMaitrise cours in tuteur.ListeCoursMaitrise)
                    {
                        if (cours.StatutCoursMaitrise.Statut == StatutCours.Validé)
                        {
                            resultat = true;
                        }
                    }
                    if (resultat == true)
                    {
                        listeVraiTuteurs.Add(tuteur);
                    }
                }

                if (rechercherPar == "Nom")
                {
                    vm.listeTuteursVM = listeVraiTuteurs.Where(r => string.Concat(r.Prenom.Trim(), " ", r.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "CoursMaitrises")
                {
                    vm.listeTuteursVM = listeVraiTuteurs.Where(r => r.ListeCoursMaitrise.Where(s => s.StatutCoursMaitrise.Statut == StatutCours.Validé).Any(c => string.Concat(c.Cours.Nom.Trim(), " - ", c.Cours.NumeroCours.Trim()).ToLower().Contains(rechercher.Trim().ToLower())) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else if (rechercherPar == "Cote")
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
                        vm.listeTuteursVM = listeTuteurs.Where(r => r.CoteTotal < (rechercherToDouble + 1.0) && r.CoteTotal >= rechercherToDouble).ToList();
                        return View(vm);
                    }
                    //Si le double est un chiffre a 2 decimales, seulement prendre la premiere
                    //Ex: rechercherToDouble == 22, la recherche se fait sur 2
                    else if (rechercherToDouble > 9)
                    {
                        rechercher = rechercher.Trim()[0] + ",0";
                        double rechercherToDouble2 = double.Parse(rechercher);
                        vm.listeTuteursVM = listeTuteurs.Where(r => r.CoteTotal < (rechercherToDouble2 + 1.0) && r.CoteTotal >= rechercherToDouble2).ToList();
                        return View(vm);
                    }
                    else
                    {
                        vm.listeTuteursVM = listeTuteurs;
                        return View(vm);
                    }
                }
                else if (rechercherPar == "Courriel")
                {
                    vm.listeTuteursVM = listeVraiTuteurs.Where(r => r.Email.Trim().ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else
                {
                    vm.listeTuteursVM = listeVraiTuteurs;
                    return View(vm);
                }
            }
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

                    listeCoursDejaDemande = dbContext.CoursMaitrises.Where(t => t.TuteurId == tuteurId).ToList();
                }
                catch (Exception)
                {
                }

                foreach (Cours cours in listeCoursSelonMatiereId)
                {
                    listeCours.Add(cours);

                    foreach (CoursMaitrise coursMaitrise in listeCoursDejaDemande)
                    {
                        listeCours.Remove(coursMaitrise.Cours);
                    }
                }
            }
            return Json(listeCours, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetListeCoursEdit(List<int> listeMatieresId, string tuteurId)
        {
            List<Cours> listeCours = new List<Cours>();
            List<Cours> listeCoursSelonMatiereId = new List<Cours>();
            List<CoursMaitrise> listeCoursDejaDemande = new List<CoursMaitrise>();

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                try
                {
                    listeCoursSelonMatiereId = dbContext.Cours.Where(x => listeMatieresId.Any(m => m == x.MatiereId)).ToList();

                    //Je dois aller chercher le tuteurId par l'URL, lorsque je suis connecté en tant qu'administrateur !
                    listeCoursDejaDemande = dbContext.CoursMaitrises.Where(t => t.TuteurId == tuteurId).ToList();
                }
                catch (Exception)
                {
                }

                foreach (Cours cours in listeCoursSelonMatiereId)
                {
                    listeCours.Add(cours);

                    foreach (CoursMaitrise coursMaitrise in listeCoursDejaDemande)
                    {
                        listeCours.Remove(coursMaitrise.Cours);
                    }
                }
            }
            return Json(listeCours, JsonRequestBehavior.AllowGet);
        }
    }
}
