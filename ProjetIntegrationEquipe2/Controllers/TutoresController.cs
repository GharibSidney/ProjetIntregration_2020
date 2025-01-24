using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using ProjetIntegrationEquipe2.Models;
using ProjetIntegrationEquipe2.ViewModels;
using ProjetIntegrationEquipe2.ViewModels.Tutores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ProjetIntegrationEquipe2.Controllers
{
    public class TutoresController : Controller
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

        // GET: Tutore
        [Authorize(Roles = RolesNames.Administrateur)]
        public ActionResult Index(string rechercherPar, string rechercher)
        {
            TutoreIndexVM vm = new TutoreIndexVM();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (rechercherPar == "Nom")
                {
                    vm.listeTutore = db.Tutores.Where(r => string.Concat(r.Prenom.Trim(), " ", r.Nom.Trim()).ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
                else
                {
                    vm.listeTutore = db.Tutores.Where(r => r.Email.Trim().ToLower().Contains(rechercher.Trim().ToLower()) || string.IsNullOrEmpty(rechercher.Trim())).ToList();
                    return View(vm);
                }
            }
        }

        [AllowAnonymous]
        public ActionResult Create()
        {
            if (User.IsInRole(RolesNames.Tuteur) || User.IsInRole(RolesNames.Tutore))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TutoreCreateVM vm = new TutoreCreateVM();
                return View("Create", vm);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TutoreCreateVM tutoreCreateVM)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", tutoreCreateVM);
            }
            //4194304 est 4mb
            if (tutoreCreateVM.ImageFile.ContentLength > 4194304)
            {
                ModelState.AddModelError("ImageSizeError", "L'image est plus grande que 4mb!!");
                return View("Create", tutoreCreateVM);
            }
            string fileName = Path.GetFileNameWithoutExtension(tutoreCreateVM.ImageFile.FileName);

            string extention = Path.GetExtension(tutoreCreateVM.ImageFile.FileName);

            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extention;

            tutoreCreateVM.CheminPhoto = "~/upload/Image/" + fileName;

            fileName = Path.Combine(Server.MapPath("~/upload/Image/"), fileName);

            tutoreCreateVM.ImageFile.SaveAs(fileName);

            //Vérifie si le e-mail est déjà pris
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                if (context.Users.Any(u => u.Email == tutoreCreateVM.Email))
                {
                    TutoreCreateVM vm = new TutoreCreateVM();
                    ModelState.AddModelError("Email", "Cette adresse courriel n'est plus disponible");
                    return View(vm);
                }
            }

            Tutore unTutore = new Tutore();

            unTutore.CheminPhoto = tutoreCreateVM.CheminPhoto;
            unTutore.Email = tutoreCreateVM.Email;
            unTutore.Force = tutoreCreateVM.Force;
            unTutore.Interet = tutoreCreateVM.Interet;
            unTutore.Nom = tutoreCreateVM.Nom;
            unTutore.PhoneNumber = tutoreCreateVM.Telephone;
            unTutore.Prenom = tutoreCreateVM.Prenom;
            unTutore.UserName = tutoreCreateVM.Email;
            try
            {
                UserManager.Create(unTutore, tutoreCreateVM.Password);
                UserManager.AddToRole(unTutore.Id, RolesNames.Tutore);
            }
            catch (Exception)
            {
                //Au cas qu'il y ait une erreur majeure
                TutoreCreateVM vm = new TutoreCreateVM();
                ModelState.AddModelError("", "Une erreur c'est produite, veuillez réessayer plus tard.");
                return View(vm);
            }


            if (!User.Identity.IsAuthenticated || UserManager.GetRoles(User.Identity.GetUserId()).Contains(RolesNames.Tutore))
            {
                SignInManager.SignIn(unTutore, isPersistent: false, rememberBrowser: false);
                return PartialView("_PageValide", "Home");
            }
            else
            {
                return PartialView("_PageValide", "Home");
            }
        }

        [Authorize(Roles = RolesNames.TutoreAdministrateur)]
        [Route("tutores/Edit/{id}")]
        public ActionResult Edit(string id)
        {
            Tutore tutoreModif = new Tutore();

            if (String.IsNullOrEmpty(id) && UserManager.GetRoles(User.Identity.GetUserId()).Contains(RolesNames.Tutore))
            {
                id = User.Identity.GetUserId();
            }
            if (String.IsNullOrEmpty(id))
            {
                return View("Error");
            }

            tutoreModif = (Tutore)UserManager.FindById(id);

            if (tutoreModif == null)
            {
                return View("Error");
            }

            if (UserManager.GetRoles(User.Identity.GetUserId()).Contains(RolesNames.Administrateur) || User.Identity.GetUserId() == id)
            {
                TutoreEditVM vm = new TutoreEditVM();

                vm.CheminPhoto = tutoreModif.CheminPhoto;
                vm.Email = tutoreModif.Email;
                vm.ConfirmEmail = tutoreModif.Email;
                vm.Force = tutoreModif.Force;
                vm.Interet = tutoreModif.Interet;
                vm.Nom = tutoreModif.Nom;
                vm.Telephone = tutoreModif.PhoneNumber;
                vm.Prenom = tutoreModif.Prenom;

                return View("Edit", vm);
            }
            else
            {
                return View("Error");
            }
        }

        [Authorize]
        public ActionResult Profil(string Id)
        {

            if (User.Identity.GetUserId() != Id && (!User.IsInRole(RolesNames.Administrateur)))
            {
                return View("Error");
            }

            ProfilTutoreVM vm = new ProfilTutoreVM();
            try
            {
                using (ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    Tutore modelProfil = dbContext.Tutores.Single(u => u.Id == Id);

                    vm.ListeCoursMaitrises = dbContext.CoursMaitrises.Include("Cours").Include("StatutCoursMaitrise")
                            .Where(w => w.TuteurId == Id).ToList();
                    vm.Id = modelProfil.Id;
                    vm.CheminPhoto = modelProfil.CheminPhoto;
                    vm.Prenom = modelProfil.Prenom;
                    vm.Telephone = modelProfil.PhoneNumber;
                    vm.Nom = modelProfil.Nom;
                    vm.Courriel = modelProfil.Email;
                    vm.Forces = modelProfil.Force;
                    vm.Interets = modelProfil.Interet;
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TutoreEditVM vm)
        {
            if (vm.ImageFile != null)
            {
                //4194304 est 4mb
                if (vm.ImageFile.ContentLength > 4194304)
                {
                    ModelState.AddModelError("ImageSizeError", "L'image est plus grande que 4mb!!");
                    return View("Edit", vm);
                }
                //Supprimer l'ancienne photo si elle a changé dans le dossier image
                string cheminComplet = Request.MapPath(vm.CheminPhoto);
                if (System.IO.File.Exists(cheminComplet))
                {
                    System.IO.File.Delete(cheminComplet);
                }

                string fileName = Path.GetFileNameWithoutExtension(vm.ImageFile.FileName);

                string extention = Path.GetExtension(vm.ImageFile.FileName);

                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extention;

                vm.CheminPhoto = "~/upload/Image/" + fileName;

                fileName = Path.Combine(Server.MapPath("~/upload/Image/"), fileName);

                vm.ImageFile.SaveAs(fileName);
            }


            if (!ModelState.IsValid)
            {
                return View("Edit", vm);
            }

            Tutore unTutore = new Tutore();
            unTutore = (Tutore)UserManager.FindById(vm.Id);

            unTutore.CheminPhoto = vm.CheminPhoto;
            unTutore.Email = vm.Email;
            unTutore.UserName = vm.Email;
            unTutore.Force = vm.Force;
            unTutore.Interet = vm.Interet;
            unTutore.Nom = vm.Nom;
            unTutore.PhoneNumber = vm.Telephone;
            unTutore.Prenom = vm.Prenom;

            var result = UserManager.Update(unTutore);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("Email", "Cette adresse courriel est déjà prise");
                return View(vm);
            }

            return PartialView("_PageValide", "Home");

        }

        [Authorize(Roles = RolesNames.Administrateur)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("tutores/Delete/{id}")]
        public ActionResult Delete(string id)
        {
            Tutore unTutore;
            List<Tutorat> listeIdTutorat;
            List<DemandeTutorat> listeDemandeTutorats;
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    unTutore = context.Tutores.Single(tutore => tutore.Id == id);

                    listeIdTutorat = context.Tutorats.Where(tutorat => tutorat.TutoreId == id).ToList();

                    listeDemandeTutorats = context.DemandeTutorats.Where(demande => demande.TutoreId == id).ToList();

                    context.Commentaires.RemoveRange(context.Commentaires.Where(c => c.TutoreId == id));


                    //Je veux supprimer toutes les plages horaires lié aux tutorats du tutoré à supprimé
                    foreach (Tutorat tutorat1 in listeIdTutorat)
                    {
                        context.PlageHoraires.RemoveRange(context.PlageHoraires.Where(c => c.TutoratId == tutorat1.TutoratId));
                    }

                    context.Tutorats.RemoveRange(context.Tutorats.Where(c => c.TutoreId == id));

                    //Je veux supprimer toutes les plages horaires lié aux demandes du tutoré à supprimé
                    foreach (DemandeTutorat demandeTutorat in listeDemandeTutorats)
                    {
                        context.PlageHoraires.RemoveRange(context.PlageHoraires.Where(c => c.DemandeTutoratId == demandeTutorat.DemandeTutoratId));
                    }

                    context.DemandeTutorats.RemoveRange(context.DemandeTutorats.Where(c => c.TutoreId == id));

                    context.SaveChanges();
                }

                UserManager.Delete(UserManager.FindById(id));
            }
            catch (Exception)
            {
                return View("Error");
            }
            return PartialView("_PageValide", "Home");
        }

        [Authorize(Roles = RolesNames.TutoreTuteur)]
        public ActionResult Password()
        {
            TutorePasswordVM vm = new TutorePasswordVM();
            //Prend le id de l'utilisateur connecté
            vm.Id = User.Identity.GetUserId();

            return View("Password", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> PasswordAsync(TutorePasswordVM vm)
        {

            if (!ModelState.IsValid)
            {
                return View("Password", vm);
            }
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);

                String hashedNewPassword = UserManager.PasswordHasher.HashPassword(vm.Password);

                ApplicationUser cUser = await store.FindByIdAsync(vm.Id);
                await store.SetPasswordHashAsync(cUser, hashedNewPassword);
                await store.UpdateAsync(cUser);
            }
            catch (Exception)
            {
                return View("Error");
            }

            return PartialView("_PageValide", "Home");

        }
    }
}
