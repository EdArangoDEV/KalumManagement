using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KalumManagement.Utilities;

namespace KalumManagement.DTOs
{
    public class AspiranteCreateDTO //: IValidatableObject
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        // Etqueta personalizada de PrimeraLetraMayusculaAttribute
        [PrimeraLetraMayuscula (ErrorMessage = "La perimera letra de cada Apellido debe iniciar con mayuscula")]
        public string Apellidos { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [PrimeraLetraMayuscula (ErrorMessage = "La perimera letra de cada Nombre debe iniciar con mayuscula")]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Direccion { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Telefono { get; set; }
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electronico valido")]
        public string Email { get; set; }
        // no es necesario enviarlo
        // public string Estatus { get; set; }

        // Relaciones con otras clases
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string ExamenId { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string CarreraId { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string JornadaId { get; set; }


        // definicion de DTOs para las relaciones, no se necesitan las referencias para crear objeto aspirante
        // public virtual CarreraTecnicaCreateDTO CarreraTecnica { get; set; }
        // public virtual ExamenAdmisionCreateDTO ExamenAdmision { get; set; }
        // public virtual JornadaCreateDTO Jornada { get; set; }

        
        // METODO heredado de IValidatableObject
        // Validar que apellidos y Nombres empieze con Mayuscula en etiquetas [Required(ErrorMessage = "El campo {0} es requerido")]
        /*
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var primeraLetra = Apellidos[0].ToString();
            if (Apellidos[0].ToString() != primeraLetra.ToUpper())
            {
                // indicamos que no esta ne mayuscula
                yield return new ValidationResult("La primera Letra del apellido debe ser mayuscula", new string[] {nameof(Apellidos)});
            }

            primeraLetra = Nombres[0].ToString();
            if (Nombres[0].ToString() != primeraLetra.ToUpper())
            {
                yield return new ValidationResult("La primera letra del nombre debe ser mayuscula", new string[] {nameof(Nombres)});
            }
        }
        */
        
    }
}