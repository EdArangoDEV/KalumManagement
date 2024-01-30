using AutoMapper;
using KalumManagement.Entities;

namespace KalumManagement.DTOs
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Saber de donde a donde quiero convertir 
            // ConstructUsing para unir propiedades
            CreateMap<Aspirante, AspiranteListDTO>().ConstructUsing(e => new AspiranteListDTO {NombresCompleto = $"{e.Apellidos} {e.Nombres}"});
            // ConstructUsing para unir propiedades
            CreateMap<Alumno, AlumnoListDTO>().ConstructUsing(al => new AlumnoListDTO {NombresCompleto = $"{al.Apellidos} {al.Nombres}"});


            CreateMap<CarreraTecnicaCreateDTO, CarreraTecnica>();
            CreateMap<CarreraTecnica, CarreraTecnicaListDTO>();
            CreateMap<JornadaCreateDTO, Jornada>();
            CreateMap<Jornada, JornadaListDTO>();
            CreateMap<ExamenAdmision, ExamenAdmisionListDTO>();
        }
    }
}