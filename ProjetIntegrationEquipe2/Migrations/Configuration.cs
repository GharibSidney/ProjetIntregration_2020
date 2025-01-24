namespace ProjetIntegrationEquipe2.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using ProjetIntegrationEquipe2.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ProjetIntegrationEquipe2.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ProjetIntegrationEquipe2.Models.ApplicationDbContext context)
        {
            //Delete all data from the DB
            context.Commentaires.RemoveRange(context.Commentaires);
            context.Cours.RemoveRange(context.Cours);
            context.CoursMaitrises.RemoveRange(context.CoursMaitrises);
            context.DemandeTutorats.RemoveRange(context.DemandeTutorats);
            context.Matieres.RemoveRange(context.Matieres);
            context.OffreTutorats.RemoveRange(context.OffreTutorats);
            context.PlageHoraires.RemoveRange(context.PlageHoraires);
            context.StatutCoursMaitrises.RemoveRange(context.StatutCoursMaitrises);
            context.StatutPaiements.RemoveRange(context.StatutPaiements);
            context.Tuteurs.RemoveRange(context.Tuteurs);
            context.Tutorats.RemoveRange(context.Tutorats);
            context.Tutores.RemoveRange(context.Tutores);
            context.Users.Remove(context.Users.Single(a => a.UserName == "carrefour@cegepgranby.qc.ca"));
            context.Promotions.RemoveRange(context.Promotions);
            context.SaveChanges();

            //Create new data
            List<StatutCoursMaitrise> listeStatutCoursMaitrises = new List<StatutCoursMaitrise>()
            {
                new StatutCoursMaitrise(){Statut="Validé"},
                new StatutCoursMaitrise(){Statut="En attente"},
                new StatutCoursMaitrise(){Statut="Refusé"}
            };
            context.StatutCoursMaitrises.AddOrUpdate(a => a.StatutCoursMaitriseId, listeStatutCoursMaitrises.ToArray());
            context.SaveChanges();
            // Nouvelle section, mis à jour des matières/programme
            List<Matiere> listeMatieres = new List<Matiere>()
            {
                new Matiere(){Nom="Littérature"},
                new Matiere(){Nom="Philosophie"},
                new Matiere(){Nom="Mathématiques"},
                new Matiere(){Nom="Informatique"},
                new Matiere(){Nom="Complémentaire"}
            };
            context.Matieres.AddOrUpdate(a => a.MatiereId, listeMatieres.ToArray());
            context.SaveChanges();
            //Nouvelle section, mis à jour des cours
            List<Cours> listeCours = new List<Cours>()
            {
                new Cours(){Nom="Littérature et Français",NumeroCours="501-104-GR", Matiere=listeMatieres.ElementAt(0)},
                new Cours(){Nom="Littérature et société",NumeroCours="501-204-GR", Matiere=listeMatieres.ElementAt(0)},
                new Cours(){Nom="Français écrit", NumeroCours="501-304-GR",  Matiere=listeMatieres.ElementAt(0)},
                new Cours(){Nom="Français appliqué au programme",NumeroCours="501-404-GR", Matiere=listeMatieres.ElementAt(0)},
                new Cours(){Nom="Éthique et comportement",NumeroCours="305-104-GR", Matiere=listeMatieres.ElementAt(1)},
                new Cours(){Nom="Philosophie",NumeroCours="305-204-GR", Matiere=listeMatieres.ElementAt(1)},
                new Cours(){Nom="Société et monde",NumeroCours="305-304-GR", Matiere=listeMatieres.ElementAt(1)},
                new Cours(){Nom="L’Humain",NumeroCours="305-404-GR", Matiere=listeMatieres.ElementAt(1)},
                new Cours(){Nom="Statistiques des absurdes",NumeroCours="201-105-GR", Matiere=listeMatieres.ElementAt(2)},
                new Cours(){Nom="Mathématiques appliqués à l’informatique",NumeroCours="201-365-GR", Matiere=listeMatieres.ElementAt(2)},
                new Cours(){Nom="Algèbre",NumeroCours="201-205-GR", Matiere=listeMatieres.ElementAt(2)},
                new Cours(){Nom="Analyse et solutions",NumeroCours="201-305-GR", Matiere=listeMatieres.ElementAt(2)},
                new Cours(){Nom="Programmation objet 1",NumeroCours="420-126-GR", Matiere=listeMatieres.ElementAt(3)},
                new Cours(){Nom="Programmation objet 2",NumeroCours="420-226-GR", Matiere=listeMatieres.ElementAt(3)},
                new Cours(){Nom="SGBD",NumeroCours="420-246-GR", Matiere=listeMatieres.ElementAt(3)},
                new Cours(){Nom="Développement Web 1",NumeroCours="420-316-GR", Matiere=listeMatieres.ElementAt(3)},
                new Cours(){Nom="Développement Web 2",NumeroCours="420-416-GR", Matiere=listeMatieres.ElementAt(3)},
                new Cours(){Nom="Projet d’intégration",NumeroCours="420-52P", Matiere=listeMatieres.ElementAt(3)},
                new Cours(){Nom="Astronomie",NumeroCours="350-103-GR", Matiere=listeMatieres.ElementAt(4)},
                new Cours(){Nom="Chimie des vins",NumeroCours="350-365-GR", Matiere=listeMatieres.ElementAt(4)},
                new Cours(){Nom="Mandarin 1",NumeroCours="350-123-GR", Matiere=listeMatieres.ElementAt(4)},
                new Cours(){Nom="Mandarin 2",NumeroCours="350-223-GR", Matiere=listeMatieres.ElementAt(4)},

            };
            context.Cours.AddOrUpdate(a => a.CoursId, listeCours.ToArray());
            context.SaveChanges();

            List<StatutPaiement> listeStatutPaiement = new List<StatutPaiement>()
            {
                new StatutPaiement(){Statut="Validé"},
                new StatutPaiement(){Statut="En attente"},
                new StatutPaiement(){Statut="En attente de paiement"},
                new StatutPaiement(){Statut="Refusé"}
            };
            context.StatutPaiements.AddOrUpdate(a => a.StatutPaiementId, listeStatutPaiement.ToArray());
            context.SaveChanges();

            RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(context);
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(roleStore);

            IdentityRole roleTutore = new IdentityRole();
            roleTutore.Name = "Tutore";
            roleManager.Create(roleTutore);

            IdentityRole roleTuteur = new IdentityRole();
            roleTuteur.Name = "Tuteur";
            roleManager.Create(roleTuteur);

            IdentityRole roleAdministrateur = new IdentityRole();
            roleAdministrateur.Name = "Administrateur";
            roleManager.Create(roleAdministrateur);

            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);

            ApplicationUser administrateur = new ApplicationUser();
            administrateur.Email = "carrefour@cegepgranby.qc.ca";
            administrateur.UserName = "carrefour@cegepgranby.qc.ca";
            administrateur.Prenom = "Administrateur";
            administrateur.Nom = "Administrateur";
            administrateur.PhoneNumber = "(450)-783-5541";

            IdentityResult identityResult = userManager.Create(administrateur, "Carrefour1!");
            userManager.AddToRole(administrateur.Id, RolesNames.Administrateur);

            context.SaveChanges();

            //Nouvelle section création du tuteur

            int idValide = context.StatutCoursMaitrises.Single(s => s.Statut.Equals(StatutCours.Validé)).StatutCoursMaitriseId;

            Tuteur victor = new Tuteur
            {
                Email = "1919101@cegepgranby.qc.ca",
                UserName = "1919101@cegepgranby.qc.ca",
                Prenom = "Victor",
                Nom = "Lapierre",
                PhoneNumber = "(450)-682-5541",
                Tarif = 15,
                Interet = "J'aime les oiseaux",
                Force = "Je suis fort en natation",
                CheminPhoto = "~/upload/Images/Avatar/4043232-avatar-batman-comics-hero_113278.png"
            };
            userManager.Create(victor, "Aa1919101!");
            userManager.AddToRole(victor.Id, RolesNames.Tutore);
            userManager.AddToRole(victor.Id, RolesNames.Tuteur);

            CoursMaitrise absurdes = new CoursMaitrise(context.Cours.Single(c => c.Nom == "Statistiques des absurdes").CoursId, context.Users.Single(u => u.UserName == "1919101@cegepgranby.qc.ca").Id
                , idValide);
            victor.ListeCoursMaitrise = new List<CoursMaitrise>();
            victor.ListeCoursMaitrise.Add(absurdes);

            OffreTutorat offreVicStats = new OffreTutorat();
            offreVicStats.TuteurId = victor.Id;
            PlageHoraire plageHoraire = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 11, 30, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 4, 11, 0, 0)
            };
            PlageHoraire plageHoraire2 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 7, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 11, 11, 0, 0)
            };
            PlageHoraire plageHoraire3 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 14, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 14, 11, 0, 0)
            };
            offreVicStats.ListePlageHoraire.Add(plageHoraire);
            offreVicStats.ListePlageHoraire.Add(plageHoraire2);
            offreVicStats.ListePlageHoraire.Add(plageHoraire3);
            offreVicStats.ListeDeCours = new List<Cours>();
            offreVicStats.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "Statistiques des absurdes"));
            context.OffreTutorats.Add(offreVicStats);

            Tuteur beatrice = new Tuteur
            {
                Email = "1919102@cegepgranby.qc.ca",
                UserName = "1919102@cegepgranby.qc.ca",
                Prenom = "Béatrice",
                Nom = "Bégin",
                PhoneNumber = "(450)-241-5541",
                Tarif = 16,
                Interet = "J'aime les crèpes",
                Force = "Je suis forte en math",
                CheminPhoto = "~/upload/Images/Avatar/4043250-avatar-child-girl-kid_113270.png"
            };
            userManager.Create(beatrice, "Aa1919102!");
            userManager.AddToRole(beatrice.Id, RolesNames.Tutore);
            userManager.AddToRole(beatrice.Id, RolesNames.Tuteur);

            CoursMaitrise absurdes2 = new CoursMaitrise(context.Cours.Single(c => c.Nom == "Statistiques des absurdes").CoursId, context.Users.Single(u => u.UserName == "1919102@cegepgranby.qc.ca").Id
                , idValide);
            beatrice.ListeCoursMaitrise = new List<CoursMaitrise>();
            beatrice.ListeCoursMaitrise.Add(absurdes2);

            Tuteur stephane = new Tuteur
            {
                Email = "1919103@cegepgranby.qc.ca",
                UserName = "1919103@cegepgranby.qc.ca",
                Prenom = "Stéphane",
                Nom = "BriseBois",
                PhoneNumber = "(450)-141-5345",
                Tarif = 17,
                Interet = "J'aime le spaghetti !",
                Force = "Je suis bon au soccer",
                CheminPhoto = "~/upload/Images/Avatar/4043241-builder-helmet-worker_113249.png"
            };

            userManager.Create(stephane, "Aa1919103!");
            userManager.AddToRole(stephane.Id, RolesNames.Tutore);
            userManager.AddToRole(stephane.Id, RolesNames.Tuteur);

            CoursMaitrise pro2 = new CoursMaitrise(context.Cours.Single(c => c.Nom == "Programmation objet 2").CoursId, context.Users.Single(u => u.UserName == "1919102@cegepgranby.qc.ca").Id
                , idValide);
            stephane.ListeCoursMaitrise = new List<CoursMaitrise>();
            stephane.ListeCoursMaitrise.Add(pro2);

            //Offre de stéphane
            OffreTutorat offreSteProg = new OffreTutorat();
            offreSteProg.TuteurId = stephane.Id;
            PlageHoraire plageHoraireS1 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 2, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 9, 16, 0, 0)
            };
            offreSteProg.ListePlageHoraire.Add(plageHoraireS1);
            offreSteProg.ListeDeCours = new List<Cours>();
            offreSteProg.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "Programmation objet 2"));
            context.OffreTutorats.Add(offreSteProg);

            Tuteur syril = new Tuteur
            {
                Email = "1919104@cegepgranby.qc.ca",
                UserName = "1919104@cegepgranby.qc.ca",
                Prenom = "Syril",
                Nom = "Bienvenue",
                PhoneNumber = "(450)-401-5345",
                Tarif = 17,
                Interet = "J'aime World of Warcraft!",
                Force = "J'aime la politique",
                CheminPhoto = "~/upload/Images/Avatar/4043255-beard-hipster-male-man_113243.png"
            };
            userManager.Create(syril, "Aa1919104!");
            userManager.AddToRole(syril.Id, RolesNames.Tutore);
            userManager.AddToRole(syril.Id, RolesNames.Tuteur);

            CoursMaitrise mathInfo = new CoursMaitrise(context.Cours.Single(c => c.Nom == "Mathématiques appliqués à l’informatique").CoursId, context.Users.Single(u => u.UserName == "1919104@cegepgranby.qc.ca").Id
                , idValide);

            CoursMaitrise SGBD = new CoursMaitrise(context.Cours.Single(c => c.Nom == "SGBD").CoursId, context.Users.Single(u => u.UserName == "1919104@cegepgranby.qc.ca").Id
                , idValide);

            CoursMaitrise ethiqueComportement = new CoursMaitrise(context.Cours.Single(c => c.Nom == "Éthique et comportement").CoursId, context.Users.Single(u => u.UserName == "1919104@cegepgranby.qc.ca").Id
                , idValide);

            CoursMaitrise absurdes3 = new CoursMaitrise(context.Cours.Single(c => c.Nom == "Statistiques des absurdes").CoursId, context.Users.Single(u => u.UserName == "1919104@cegepgranby.qc.ca").Id
                , idValide);

            syril.ListeCoursMaitrise = new List<CoursMaitrise>();
            syril.ListeCoursMaitrise.Add(mathInfo);
            syril.ListeCoursMaitrise.Add(SGBD);
            syril.ListeCoursMaitrise.Add(ethiqueComportement);
            syril.ListeCoursMaitrise.Add(absurdes3);


            //Section offre de Syril
            OffreTutorat offreSyrilSGBD = new OffreTutorat();
            offreSyrilSGBD.TuteurId = syril.Id;

            PlageHoraire plageHoraireSy1 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 11, 30, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 4, 11, 0, 0)
            };
            PlageHoraire plageHoraireSy2 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 7, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 11, 11, 0, 0)
            }; PlageHoraire plageHoraireSy3 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 14, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 14, 11, 0, 0)
            };

            offreSyrilSGBD.ListePlageHoraire.Add(plageHoraireSy1);
            offreSyrilSGBD.ListePlageHoraire.Add(plageHoraireSy2);
            offreSyrilSGBD.ListePlageHoraire.Add(plageHoraireSy3);
            offreSyrilSGBD.ListeDeCours = new List<Cours>();
            offreSyrilSGBD.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "SGBD"));
            context.OffreTutorats.Add(offreSyrilSGBD);

            //Offre de math à l'informatique de syril
            OffreTutorat offreSyrilMathInfo = new OffreTutorat();
            offreSyrilMathInfo.TuteurId = syril.Id;
            offreSyrilMathInfo.ListeDeCours = new List<Cours>();
            offreSyrilMathInfo.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "Mathématiques appliqués à l’informatique"));

            PlageHoraire plageHoraireSy4 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 11, 30, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 11, 30, 16, 0, 0)
            };
            offreSyrilMathInfo.ListePlageHoraire.Add(plageHoraireSy4);

            context.OffreTutorats.Add(offreSyrilMathInfo);

            //Offre d'éthique et de comportement
            OffreTutorat offreSyrilEthique = new OffreTutorat();
            offreSyrilEthique.TuteurId = syril.Id;
            offreSyrilEthique.ListeDeCours = new List<Cours>();
            offreSyrilEthique.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "Éthique et comportement"));
            PlageHoraire plageHoraireSy5 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 11, 30, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 5, 11, 0, 0)
            };

            PlageHoraire plageHoraireSy6 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 16, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 16, 12, 0, 0)
            };

            offreSyrilEthique.ListePlageHoraire.Add(plageHoraireSy5);
            offreSyrilEthique.ListePlageHoraire.Add(plageHoraireSy6);
            context.OffreTutorats.Add(offreSyrilEthique);

            // Offre Statistique des absurdes de Syril
            OffreTutorat offreSyrilStat = new OffreTutorat();
            offreSyrilStat.TuteurId = syril.Id;
            offreSyrilStat.ListeDeCours = new List<Cours>();
            offreSyrilStat.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "Statistiques des absurdes"));

            PlageHoraire plageHoraireSy7 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 9, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 9, 12, 0, 0)
            };
            offreSyrilStat.ListePlageHoraire.Add(plageHoraireSy7);
            context.OffreTutorats.Add(offreSyrilStat);

            Tuteur vincent = new Tuteur
            {
                Email = "1919105@cegepgranby.qc.ca",
                UserName = "1919105@cegepgranby.qc.ca",
                Prenom = "Vincent",
                Nom = "Lacroix",
                PhoneNumber = "(450)-982-5345",
                Tarif = 8,
                Force = "Je suis un bon cuisinier",
                Interet = "J'aime le poulet !",
                CheminPhoto = "~/upload/Images/Avatar/4043260-avatar-male-man-portrait_113269.png"
            };
            userManager.Create(vincent, "Aa1919105!");
            userManager.AddToRole(vincent.Id, RolesNames.Tutore);
            userManager.AddToRole(vincent.Id, RolesNames.Tuteur);

            CoursMaitrise absurdes4 = new CoursMaitrise(context.Cours.Single(c => c.Nom == "Statistiques des absurdes").CoursId, context.Users.Single(u => u.UserName == "1919105@cegepgranby.qc.ca").Id
                , idValide);
            vincent.ListeCoursMaitrise = new List<CoursMaitrise>();
            vincent.ListeCoursMaitrise.Add(absurdes4);

            Tuteur denis = new Tuteur
            {
                Email = "1919106@cegepgranby.qc.ca",
                UserName = "1919106@cegepgranby.qc.ca",
                Prenom = "Denis",
                Nom = "Proulx",
                PhoneNumber = "(450)-266-5345",
                Tarif = 18,
                Force = "Je suis bon en ski alpin",
                Interet = "Je suis bon en ski alpin",
                CheminPhoto = "~/upload/Images/Avatar/4043260-avatar-male-man-portrait_113269.png"
            };
            userManager.Create(denis, "Aa1919106!");
            userManager.AddToRole(denis.Id, RolesNames.Tutore);
            userManager.AddToRole(denis.Id, RolesNames.Tuteur);
            CoursMaitrise astronomie = new CoursMaitrise(context.Cours.Single(c => c.Nom == "Astronomie").CoursId, context.Users.Single(u => u.UserName == "1919106@cegepgranby.qc.ca").Id
                , idValide);


            denis.ListeCoursMaitrise = new List<CoursMaitrise>();
            denis.ListeCoursMaitrise.Add(astronomie);

            //Offre d'astronomie de Denis
            OffreTutorat offreDenisAstro = new OffreTutorat();
            offreDenisAstro.TuteurId = denis.Id;
            offreDenisAstro.ListeDeCours = new List<Cours>();
            offreDenisAstro.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "Astronomie"));

            PlageHoraire plageHoraireD = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 4, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 4, 12, 0, 0)
            };
            offreDenisAstro.ListePlageHoraire.Add(plageHoraireD);
            context.OffreTutorats.Add(offreDenisAstro);


            context.SaveChanges();

            //Section création de tutorés

            Tutore claire = new Tutore
            {
                Email = "1919107@cegepgranby.qc.ca",
                UserName = "1919107@cegepgranby.qc.ca",
                Prenom = "Claire",
                Nom = "Courtemanche",
                PhoneNumber = "(450)-277-5345",
                CheminPhoto = "~/upload/Images/Avatar/4043247-1-avatar-female-portrait-woman_113261.png"
            };
            userManager.Create(claire, "Aa1919107!");
            userManager.AddToRole(claire.Id, RolesNames.Tutore);

            Tutore maurice = new Tutore
            {
                Email = "1919108@cegepgranby.qc.ca",
                UserName = "1919108@cegepgranby.qc.ca",
                Prenom = "Maurice",
                Nom = "Étienne",
                PhoneNumber = "(450)-316-5775",
                CheminPhoto = "~/upload/Images/Avatar/4043255-beard-hipster-male-man_113243.png"
            };
            userManager.Create(maurice, "Aa1919108!");
            userManager.AddToRole(maurice.Id, RolesNames.Tutore);

            //Demande de stats absurde de Maurice
            DemandeTutorat demandeMauriceStats = new DemandeTutorat();
            demandeMauriceStats.TutoreId = maurice.Id;
            demandeMauriceStats.ListeDeCours = new List<Cours>();
            demandeMauriceStats.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "Statistiques des absurdes"));


            PlageHoraire plageHoraireMau1 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 11, 30, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 4, 11, 0, 0)
            };
            PlageHoraire plageHoraireMau2 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 9, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 9, 12, 0, 0)
            };
            demandeMauriceStats.ListePlageHoraire.Add(plageHoraireMau1);
            demandeMauriceStats.ListePlageHoraire.Add(plageHoraireMau2);
            context.DemandeTutorats.Add(demandeMauriceStats);

            Tutore yvan = new Tutore
            {
                Email = "1919109@cegepgranby.qc.ca",
                UserName = "1919109@cegepgranby.qc.ca",
                Prenom = "Yvan",
                Nom = "Demers",
                PhoneNumber = "(450)-716-5001",
                CheminPhoto = "~/upload/Images/Avatar/4043241-builder-helmet-worker_113249.png"
            };
            userManager.Create(yvan, "Aa1919109!");
            userManager.AddToRole(yvan.Id, RolesNames.Tutore);


            //Demande math info d'Yvan
            DemandeTutorat demandeYvanMathInfo = new DemandeTutorat();
            demandeYvanMathInfo.TutoreId = yvan.Id;
            demandeYvanMathInfo.ListeDeCours = new List<Cours>();
            demandeYvanMathInfo.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "Mathématiques appliqués à l’informatique"));
            PlageHoraire plageHoraireYvanMathinfo1 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 11, 30, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 11, 30, 16, 0, 0)
            };
            demandeYvanMathInfo.ListePlageHoraire.Add(plageHoraireYvanMathinfo1);
            context.DemandeTutorats.Add(demandeYvanMathInfo);


            //Demande Ethique d'Yvan
            DemandeTutorat demandeYvanEthique = new DemandeTutorat();
            demandeYvanEthique.TutoreId = yvan.Id;
            demandeYvanEthique.ListeDeCours = new List<Cours>();
            demandeYvanEthique.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "Éthique et comportement"));
            PlageHoraire plageHoraireYvanEthique1 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 4, 9, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 10, 13, 0, 0)
            };
            demandeYvanEthique.ListePlageHoraire.Add(plageHoraireYvanEthique1);
            context.DemandeTutorats.Add(demandeYvanEthique);


            //Demande Astronomie d'Yvan
            DemandeTutorat demandeYvanAstronomie = new DemandeTutorat();
            demandeYvanAstronomie.TutoreId = yvan.Id;
            demandeYvanAstronomie.ListeDeCours = new List<Cours>();
            demandeYvanAstronomie.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "Astronomie"));
            PlageHoraire plageHoraireYvanAstronomie1 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 4, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 4, 12, 0, 0)
            };
            demandeYvanAstronomie.ListePlageHoraire.Add(plageHoraireYvanAstronomie1);
            context.DemandeTutorats.Add(demandeYvanEthique);

            Tutore hector = new Tutore
            {
                Email = "1919110@cegepgranby.qc.ca",
                UserName = "1919110@cegepgranby.qc.ca",
                Prenom = "Hector",
                Nom = "Degarde",
                PhoneNumber = "(450)-897-5781",
                CheminPhoto = "~/upload/Images/Avatar/4043255-beard-hipster-male-man_113243.png"
            };
            userManager.Create(hector, "Aa1919110!");
            userManager.AddToRole(hector.Id, RolesNames.Tutore);

            Tutore etienne = new Tutore
            {
                Email = "1919111@cegepgranby.qc.ca",
                UserName = "1919111@cegepgranby.qc.ca",
                Prenom = "Étienne",
                Nom = "Givré",
                PhoneNumber = "(450)-007-5234",
                CheminPhoto = "~/upload/Images/Avatar/4043255-beard-hipster-male-man_113243.png"
            };
            userManager.Create(etienne, "Aa1919111!");
            userManager.AddToRole(etienne.Id, RolesNames.Tutore);


            DemandeTutorat demandeEtienneMathInfo = new DemandeTutorat();
            demandeEtienneMathInfo.TutoreId = etienne.Id;
            demandeEtienneMathInfo.ListeDeCours = new List<Cours>();
            demandeEtienneMathInfo.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "Mathématiques appliqués à l’informatique"));
            PlageHoraire plageHoraireEtienneMathInfo1 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 11, 30, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 11, 30, 16, 0, 0)
            };

            Tutore olivier = new Tutore
            {
                Email = "6148287@cegepgranby.qc.ca",
                UserName = "6148287@cegepgranby.qc.ca",
                Prenom = "Olivier",
                Nom = "Jacques",
                PhoneNumber = "(450)-555-5555",
                Interet = "J'aime les Miatas",
                Force = "Je suis un bon gars",
                CheminPhoto = "~/upload/Images/Avatar/4043232-avatar-batman-comics-hero_113278.png"
            };
            userManager.Create(olivier, "NineOli9!");
            userManager.AddToRole(olivier.Id, RolesNames.Tutore);

            DemandeTutorat demandeTutoratOliLite = new DemandeTutorat();
            demandeTutoratOliLite.TutoreId = olivier.Id;
            demandeTutoratOliLite.ListeDeCours = new List<Cours>();
            demandeTutoratOliLite.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "Littérature et Français"));

            PlageHoraire plageHoraireOlilite1 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 4, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 4, 12, 0, 0)
            };
            PlageHoraire plageHoraireOlilite2 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 11, 30, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 4, 16, 0, 0)
            };
            demandeTutoratOliLite.ListePlageHoraire = new List<PlageHoraire>();
            demandeTutoratOliLite.ListePlageHoraire.Add(plageHoraireOlilite1);
            demandeTutoratOliLite.ListePlageHoraire.Add(plageHoraireOlilite2);
            context.DemandeTutorats.Add(demandeTutoratOliLite);


            DemandeTutorat demandeTutoratOliLite2 = new DemandeTutorat();
            demandeTutoratOliLite2.TutoreId = olivier.Id;
            demandeTutoratOliLite2.ListeDeCours = new List<Cours>();
            demandeTutoratOliLite2.ListeDeCours.Add(context.Cours.Single(c => c.Nom == "Français écrit"));

            PlageHoraire plageHoraireOlilite3 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 4, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 4, 12, 0, 0)
            };
            PlageHoraire plageHoraireOlilite4 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 11, 30, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 4, 16, 0, 0)
            };
            demandeTutoratOliLite2.ListePlageHoraire = new List<PlageHoraire>();
            demandeTutoratOliLite2.ListePlageHoraire.Add(plageHoraireOlilite3);
            demandeTutoratOliLite2.ListePlageHoraire.Add(plageHoraireOlilite4);
            context.DemandeTutorats.Add(demandeTutoratOliLite2);

            context.SaveChanges();

            //Section tutorat (jumelage)

            Tutorat yvanSyril1 = new Tutorat();
            yvanSyril1.TuteurId = syril.Id;
            yvanSyril1.TutoreId = yvan.Id;
            PlageHoraire plageHoraireYvanSyril1 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 11, 30, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 11, 30, 16, 0, 0)
            };
            yvanSyril1.ListePlageHoraire = new List<PlageHoraire>();
            yvanSyril1.ListePlageHoraire.Add(plageHoraireYvanSyril1);
            yvanSyril1.StatutPaiementId = context.StatutPaiements.Single(statut => statut.Statut == StatutFacture.AttentePaiement).StatutPaiementId;
            yvanSyril1.NombreRefus = 0;
            yvanSyril1.DatePaiement = DateTime.Now;
            context.Tutorats.Add(yvanSyril1);

            Tutorat yvanSyril2 = new Tutorat();
            yvanSyril2.TuteurId = syril.Id;
            yvanSyril2.TutoreId = yvan.Id;
            PlageHoraire plageHoraireYvanSyril2 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 5, 9, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 5, 11, 0, 0)
            };
            yvanSyril2.ListePlageHoraire = new List<PlageHoraire>();
            yvanSyril2.ListePlageHoraire.Add(plageHoraireYvanSyril2);
            yvanSyril2.StatutPaiementId = context.StatutPaiements.Single(statut => statut.Statut == StatutFacture.AttentePaiement).StatutPaiementId;
            yvanSyril2.NombreRefus = 0;
            yvanSyril2.DatePaiement = DateTime.Now;
            context.Tutorats.Add(yvanSyril2);


            Tutorat yvanDenis = new Tutorat();
            yvanDenis.TuteurId = denis.Id;
            yvanDenis.TutoreId = yvan.Id;
            PlageHoraire plageHoraireYvanDenis = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 4, 8, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 4, 12, 0, 0)
            };
            yvanDenis.ListePlageHoraire = new List<PlageHoraire>();
            yvanDenis.ListePlageHoraire.Add(plageHoraireYvanDenis);
            yvanDenis.StatutPaiementId = context.StatutPaiements.Single(statut => statut.Statut == StatutFacture.AttentePaiement).StatutPaiementId;
            yvanDenis.NombreRefus = 0;
            yvanDenis.DatePaiement = DateTime.Now;
            context.Tutorats.Add(yvanDenis);

            context.SaveChanges();
            //Section tutorat qui sont payés et qui ont un commentaire et une cote

            Tutorat etienneSyril = new Tutorat();
            etienneSyril.TuteurId = syril.Id;
            etienneSyril.TutoreId = etienne.Id;
            PlageHoraire plageHoraireEtienneSyril = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 11, 30, 13, 0, 0),
                DateHeureFin = new DateTime(2020, 11, 30, 16, 0, 0)
            };
            etienneSyril.ListePlageHoraire = new List<PlageHoraire>();
            etienneSyril.ListePlageHoraire.Add(plageHoraireEtienneSyril);
            etienneSyril.StatutPaiementId = context.StatutPaiements.Single(statut => statut.Statut == StatutFacture.Valide).StatutPaiementId;
            etienneSyril.NombreRefus = 0;
            etienneSyril.DatePaiement = new DateTime(2020, 12, 2);
            context.Tutorats.Add(etienneSyril);

            Commentaire commentaireEtienneSyril = new Commentaire();
            commentaireEtienneSyril.TuteurId = syril.Id;
            commentaireEtienneSyril.TutoreId = etienne.Id;
            commentaireEtienneSyril.Titre = "Commentaire sur Syril";
            commentaireEtienneSyril.CommentaireTexte = "exigence trop grande, je capote! Au secours ";
            commentaireEtienneSyril.Cote = 2.0;
            commentaireEtienneSyril.Date = new DateTime(2020, 12, 2);
            context.Commentaires.Add(commentaireEtienneSyril);

            context.SaveChanges();
            //Double jumelage et un seul commentaire

            Tutorat vincentHector = new Tutorat();
            vincentHector.TuteurId = vincent.Id;
            vincentHector.TutoreId = hector.Id;
            PlageHoraire plageHoraireVincentHector1 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 11, 30, 15, 0, 0),
                DateHeureFin = new DateTime(2020, 11, 30, 16, 0, 0)
            };
            vincentHector.ListePlageHoraire = new List<PlageHoraire>();
            vincentHector.ListePlageHoraire.Add(plageHoraireVincentHector1);
            vincentHector.StatutPaiementId = context.StatutPaiements.Single(statut => statut.Statut == StatutFacture.Valide).StatutPaiementId;
            vincentHector.NombreRefus = 0;
            vincentHector.DatePaiement = new DateTime(2020, 12, 1);
            context.Tutorats.Add(vincentHector);

            Tutorat vincentHector2 = new Tutorat();
            vincentHector2.TuteurId = vincent.Id;
            vincentHector2.TutoreId = hector.Id;
            PlageHoraire plageHoraireVincentHector2 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 11, 20, 15, 0, 0),
                DateHeureFin = new DateTime(2020, 11, 20, 17, 0, 0)
            };
            vincentHector2.ListePlageHoraire = new List<PlageHoraire>();
            vincentHector2.ListePlageHoraire.Add(plageHoraireVincentHector2);
            vincentHector2.StatutPaiementId = context.StatutPaiements.Single(statut => statut.Statut == StatutFacture.Valide).StatutPaiementId;
            vincentHector2.NombreRefus = 0;
            vincentHector2.DatePaiement = new DateTime(2020, 11, 22);
            context.Tutorats.Add(vincentHector2);

            Commentaire commentaireVincentHector = new Commentaire();
            commentaireVincentHector.TuteurId = vincent.Id;
            commentaireVincentHector.TutoreId = hector.Id;
            commentaireVincentHector.Titre = "Commentaire sur Vincent";
            commentaireVincentHector.CommentaireTexte = "J'ai compris plusieurs nouvelles notions !";
            commentaireVincentHector.Cote = 4.0;
            commentaireVincentHector.Date = new DateTime(2020, 11, 26);
            context.Commentaires.Add(commentaireVincentHector);

            Tutorat vincentclaire = new Tutorat();
            vincentclaire.TuteurId = vincent.Id;
            vincentclaire.TutoreId = claire.Id;
            PlageHoraire plageHoraireVincentClaire1 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 5, 12, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 5, 14, 0, 0)
            };
            vincentclaire.ListePlageHoraire = new List<PlageHoraire>();
            vincentclaire.ListePlageHoraire.Add(plageHoraireVincentClaire1);
            vincentclaire.StatutPaiementId = context.StatutPaiements.Single(statut => statut.Statut == StatutFacture.Valide).StatutPaiementId;
            vincentclaire.NombreRefus = 0;
            vincentclaire.DatePaiement = new DateTime(2020, 12, 9);
            context.Tutorats.Add(vincentclaire);

            Tutorat vincentclaire2 = new Tutorat();
            vincentclaire2.TuteurId = vincent.Id;
            vincentclaire2.TutoreId = claire.Id;
            PlageHoraire plageHoraireVincentClaire2 = new PlageHoraire
            {
                DateHeureDebut = new DateTime(2020, 12, 6, 12, 0, 0),
                DateHeureFin = new DateTime(2020, 12, 6, 17, 0, 0)
            };
            vincentclaire2.ListePlageHoraire = new List<PlageHoraire>();
            vincentclaire2.ListePlageHoraire.Add(plageHoraireVincentClaire2);
            vincentclaire2.StatutPaiementId = context.StatutPaiements.Single(statut => statut.Statut == StatutFacture.Valide).StatutPaiementId;
            vincentclaire2.NombreRefus = 0;
            vincentclaire2.DatePaiement = new DateTime(2020, 12, 9);
            context.Tutorats.Add(vincentclaire2);

            Commentaire commentaireVincentclaire = new Commentaire();
            commentaireVincentclaire.TuteurId = vincent.Id;
            commentaireVincentclaire.TutoreId = claire.Id;
            commentaireVincentclaire.Titre = "Commentaire sur Vincent 2";
            commentaireVincentclaire.CommentaireTexte = "C'est un tuteur très patient !";
            commentaireVincentclaire.Cote = 5.0;
            commentaireVincentclaire.Date = new DateTime(2020, 12, 9);
            context.Commentaires.Add(commentaireVincentclaire);

            context.SaveChanges();
            //promotions

            Promotion article = new Promotion
            {
                Titre = "Article",
                Description = "Sprint de fin de session. On lâche pas!"
            };
            context.Promotions.Add(article);

            Promotion nouvelle = new Promotion
            {
                Titre = "Nouvelle",
                Description = "Recherche de tuteurs pour l’hiver 2021, inscrivez-vous. C’est stimulant et payant!"
            };
            context.Promotions.Add(nouvelle);
            context.SaveChanges();
        }
    }
}
