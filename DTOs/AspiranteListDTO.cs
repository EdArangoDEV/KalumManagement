namespace KalumManagement.DTOs
{
    public class AspiranteListDTO
    {
        // Para delimitar infotmacion al consultar

            public string NoExpediente { get; set; }
            public string Apellidos { get; set; }
            // propiedad para concatenar elementos con AutoMapper
            public string NombresCompleto { get; set; }
            public string Nombres { get; set; }
            public string Direccion { get; set; }
            public string Telefono { get; set; }
            public string Email { get; set; }
            public string Estatus { get; set; }

            // Hacemos referencia a los ListDTO de cada entidad
            public virtual CarreraTecnicaListDTO CarreraTecnica { get; set; }
            public virtual ExamenAdmisionListDTO ExamenAdmision { get; set; }
            public virtual JornadaListDTO Jornada { get; set; }
    }
}    