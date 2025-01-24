using ProjetIntegrationEquipe2.ViewModels.Offres;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetIntegrationEquipe2.Validations
{
    public class DateTimePickerValidations : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (HttpContext.Current.Request.Url.AbsolutePath.Contains("Create") || HttpContext.Current.Request.Url.AbsolutePath.Contains("create"))
            {
                CreateOffreVM vm = (CreateOffreVM)validationContext.ObjectInstance;
                try
                {
                    foreach (DateTime dateTime in vm.ListeDateTimes)
                    {
                        if (dateTime < DateTime.Now)
                        {
                            return new ValidationResult("Vous devez choisir une date dans le futur.");
                        }
                        if (dateTime > (DateTime.Now.AddYears(1)))
                        {
                            return new ValidationResult("Vous devez choisir une date qui est au maximum 1 an d'aujourd'hui.");
                        }
                    }
                }
                catch (Exception)
                {
                    return new ValidationResult("Aucun champ de la plage horaire doit être vide.");
                }

                for (int compteur = 0; compteur < vm.ListeDateTimes.Count(); compteur++)
                {
                    DateTime debut = vm.ListeDateTimes[compteur];
                    compteur++;
                    DateTime fin = vm.ListeDateTimes[compteur];

                    if (fin <= debut)
                    {
                        return new ValidationResult("La fin de la plage horaire doit se terminer après le début.");
                    }
                }

                return ValidationResult.Success;

            }
            else
            {
                EditOffreVM vm = (EditOffreVM)validationContext.ObjectInstance;
                try
                {
                    foreach (DateTime dateTime in vm.ListeDateTimes)
                    {
                        if (dateTime < DateTime.Now)
                        {
                            return new ValidationResult("Vous devez choisir une date dans le futur.");
                        }
                        if (dateTime > (DateTime.Now.AddYears(1)))
                        {
                            return new ValidationResult("Vous devez choisir une date qui est au maximum 1 an d'aujourd'hui.");
                        }
                    }
                }
                catch (Exception)
                {
                    return new ValidationResult("Aucun champ de la plage horaire doit être vide.");
                }

                for (int compteur = 0; compteur < vm.ListeDateTimes.Count(); compteur++)
                {
                    DateTime debut = vm.ListeDateTimes[compteur];
                    compteur++;
                    DateTime fin = vm.ListeDateTimes[compteur];

                    if (fin <= debut)
                    {
                        return new ValidationResult("La fin de la plage horaire doit se terminer après le début.");
                    }
                }

                return ValidationResult.Success;
            }
        }
    }
}
