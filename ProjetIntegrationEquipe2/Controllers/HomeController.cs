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

namespace ProjetIntegrationEquipe2.Controllers
{
    public class HomeController : Controller
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

        public ActionResult Index()
        {
            List<Tuteur> listeTuteurs = new List<Tuteur>();
            List<Tuteur> listeTuteursAvecCoteTotal = new List<Tuteur>();
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                listeTuteurs = dbContext.Tuteurs.ToList();
                
                foreach (Tuteur tuteur in listeTuteurs)
                {
                    List<Commentaire> listeCommentaires = dbContext.Commentaires.Where(c => c.TuteurId == tuteur.Id).ToList();
                    tuteur.CoteTotal = Math.Round(listeCommentaires.Sum(x => x.Cote) / listeCommentaires.Count(), 1);
                    if (Double.IsNaN(tuteur.CoteTotal))
                    {
                        tuteur.CoteTotal = 0.0;
                    }
                    listeTuteursAvecCoteTotal.Add(tuteur);

                }
            }
            listeTuteurs = listeTuteursAvecCoteTotal.OrderByDescending(o => o.CoteTotal).Take(3).ToList();
            HomeIndexVM vm = new HomeIndexVM();
            vm.listeTuteursVM = listeTuteurs;
            vm.listeIntrosTuteur = new List<string>() { "Le plaisir d'apprendre", "Passe par la volonté", "Des meilleurs Tuteurs!" };
            return View(vm);
        }

        [HttpGet]
        public ActionResult Promotion()
        {
            List<Promotion> promotions = new List<Promotion>();
            List<PromotionInfoVM> vm = new List<PromotionInfoVM>();
           
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                promotions = dbContext.Promotions.ToList();

                foreach (Promotion c in promotions)
                {
                    PromotionInfoVM promoVM = new PromotionInfoVM();
                    promoVM.PromotionId = c.PromotionId;
                    promoVM.CheminImage = c.CheminImage;
                    promoVM.Titre = c.Titre;
                    promoVM.Description = c.Description;
                    vm.Add(promoVM);
                }

            }
            return PartialView("_NePasManquer",vm);
        }

        public ActionResult About()
        {
            return View();
        }

       
        [HttpGet]
        [Authorize(Roles =RolesNames.Administrateur)]
        public ActionResult CreerPromotion()
        {
            return View("CreerPromotion");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles =RolesNames.Administrateur)]
        public ActionResult CreerPromotion(PromotionCreateVM vm)
        {
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                if (!ModelState.IsValid)
                {
                    PromotionCreateVM vme = new PromotionCreateVM
                    {
                        Titre = vm.Titre,
                        Description = vm.Description,
                        CheminImage = vm.CheminImage
                    };
                    return View("CreerPromotion", "Home", vme);
                }
                if (vm.ImageFile!=null)
                {
                    //4194304 est 4mb
                    if (vm.ImageFile.ContentLength > 4194304)
                    {
                        ModelState.AddModelError("ImageSizeError", "L'image est plus grande que 4mb!!");
                        return View("CreerPromotion", vm);
                    }
                    string fileName = Path.GetFileNameWithoutExtension(vm.ImageFile.FileName);

                    string extention = Path.GetExtension(vm.ImageFile.FileName);

                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extention;

                    vm.CheminImage = "~/upload/Image/" + fileName;

                    fileName = Path.Combine(Server.MapPath("~/upload/Image/"), fileName);

                    vm.ImageFile.SaveAs(fileName);
                }

                Promotion unePromotion = new Promotion();
                unePromotion.CheminImage = vm.CheminImage;
                unePromotion.Titre = vm.Titre;
                unePromotion.Description = vm.Description;
                dbContext.Promotions.Add(unePromotion);
                dbContext.SaveChanges();
            }
            return PartialView("_PageValide", "Home");

        }

        [Authorize(Roles=RolesNames.Administrateur)]
        public ActionResult EditPromotion(int? Id)
        {
            if (Id == null)
            {
                return View("Error");
            }
            using (ApplicationDbContext dbContext=new ApplicationDbContext())
            {
                Promotion promoModif = dbContext.Promotions.Single(m => m.PromotionId == Id);

                PromotionEditVM vm = new PromotionEditVM
                {
                    PromotionId = promoModif.PromotionId,
                    CheminImage = promoModif.CheminImage,
                    Titre = promoModif.Titre,
                    Description = promoModif.Description
                };
                return View(vm);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles =RolesNames.Administrateur)]
        public ActionResult EditPromotion(PromotionEditVM vm)
        {
            using(ApplicationDbContext dbContext=new ApplicationDbContext())
            {
                if (vm.ImageFile != null)
                {
                    //4194304 est 4mb
                    if (vm.ImageFile.ContentLength > 4194304)
                    {
                        ModelState.AddModelError("ImageSizeError", "L'image est plus grande que 4mb!!");
                        return View("EditPromotion", vm);
                    }
                    string fileName = Path.GetFileNameWithoutExtension(vm.ImageFile.FileName);

                    string extention = Path.GetExtension(vm.ImageFile.FileName);

                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extention;

                    vm.CheminImage = "~/upload/Image/" + fileName;

                    fileName = Path.Combine(Server.MapPath("~/upload/Image/"), fileName);

                    vm.ImageFile.SaveAs(fileName);
                }
                if (!ModelState.IsValid)
                {
                    PromotionEditVM vme = new PromotionEditVM
                    {
                        CheminImage=vm.CheminImage,
                        Titre=vm.Titre,
                        Description=vm.Description

                    };
                    return View(vme);
                }

                Promotion promoUpdate = dbContext.Promotions.Single(m => m.PromotionId == vm.PromotionId);
                promoUpdate.CheminImage = vm.CheminImage;
                promoUpdate.Titre = vm.Titre;
                promoUpdate.Description = vm.Description;
                dbContext.SaveChanges();
                return PartialView("_PageValide", "Home");
            }
        }

        [HttpPost]
        [Authorize(Roles =RolesNames.Administrateur)]
        public ActionResult DeletePromotion(int Id)
        {
            using(ApplicationDbContext dbContext=new ApplicationDbContext())
            {
                Promotion promoToDelete = dbContext.Promotions.Single(m => m.PromotionId == Id);
                dbContext.Promotions.Remove(promoToDelete);
                dbContext.SaveChanges();
            }
            return new EmptyResult();
            
        }
        [ChildActionOnly]
        public PartialViewResult RenderLoginPartial()
        {
            var userCurrentID = HttpContext.User.Identity.GetUserId();
            Tutore CurrentUserForPartial = new Tutore();
            if(userCurrentID == null || (User.IsInRole(RolesNames.Administrateur)))
            {
                return PartialView("_LoginPartial");
            }
            
            else
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    CurrentUserForPartial = context.Tutores.Find(userCurrentID);
                }

                LoginPartialViewModel PartialIndexVue = new LoginPartialViewModel();
                PartialIndexVue.CheminPhoto = CurrentUserForPartial.CheminPhoto;
                PartialIndexVue.Email = CurrentUserForPartial.Email;
                PartialIndexVue.Force = CurrentUserForPartial.Force;
                PartialIndexVue.Interet = CurrentUserForPartial.Interet;
                PartialIndexVue.Nom = CurrentUserForPartial.Nom;
                PartialIndexVue.Prenom = CurrentUserForPartial.Prenom;
                PartialIndexVue.Telephone = CurrentUserForPartial.PhoneNumber;


                return PartialView("_LoginPartial", PartialIndexVue);
            }
            
        }

        public ActionResult PageAide()
        {
            return View();
        }
    }
}