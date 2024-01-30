using System.ComponentModel.DataAnnotations;

namespace KalumManagement.Utilities
{
    public class PrimeraLetraMayusculaAttribute : ValidationAttribute
    {
        // Metodo sobrecargado de ValidationAttribute
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            foreach (string valor in value.ToString().Split(" "))
            {
                string primeraLetra = valor[0].ToString();
                if (primeraLetra != primeraLetra.ToUpper())
                {
                    return new ValidationResult("La primera letra debe ser mayuscula");
                }
            }

            // string primeraLetra = value.ToString()[0].ToString();
            // if (primeraLetra != primeraLetra.ToUpper())
            // {
            //     return new ValidationResult("Las primera letra debe ser mayuscula");
            // }
            return ValidationResult.Success;
        }
    }
}