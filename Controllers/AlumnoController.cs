using KalumManagement.DBContext;
using KalumManagement.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KalumManagement.Controllers
{

    [ApiController]
    [Route("Kalum-management/v1/alumnos")]
        
    public class AlumnoController : ControllerBase
    {
        private readonly KalumDBContext kalumDBContext;
        private readonly ILogger Logger;
        
        public AlumnoController(KalumDBContext _KalumDBContext, ILogger<AlumnoController> _Logger)
        {
            this.kalumDBContext = _KalumDBContext;
            this.Logger = _Logger;
        }
        
        // listar alumnos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alumno>>> Get()
        {
            this.Logger.LogDebug("Iniciando proceso de consulta de los alumnos");

            List<Alumno> alumnos = await this.kalumDBContext.Alumnos.ToListAsync();
            this.Logger.LogDebug($"Cantidad de registros: {alumnos.Count}");
            if (alumnos == null || alumnos
            .Count == 0)
            {
                this.Logger.LogDebug("No existen datos en la tabla de alumnos");
                return NoContent();
            }
            this.Logger.LogInformation("Se ejecuto la consulta exitosamente");
            return Ok(alumnos);
        }

        // busqueda de alumno con numero de expediente 
        [HttpGet("{carne}", Name = "GetAlumnoById")]
        public async Task<ActionResult<Alumno>> GetAlumnoById(string carne)
        {
            this.Logger.LogDebug($"Iniciando busqueda de alumno con numero de expediente: {carne}");
            Alumno alumno = await this.kalumDBContext.Alumnos.FirstOrDefaultAsync(al => al.Carne == carne);

            if (alumno == null)
            {
                this.Logger.LogWarning($"No existe registro con numero de expediente: {carne}");
                return NotFound();

            }
            this.Logger.LogInformation("Se ejecuto la consulta exitosamente");
            return Ok(alumno);
        }

// PENDIENTE DE PROGRAMAR
/*
        // Agregar Alumno 
        [HttpPost]
        // indicar que se mandara informacion en Body [FromBody]
        public async Task<ActionResult<Alumno>> Post([FromBody] Alumno value)
        {
            this.Logger.LogDebug("Iniciando proceso de insertar registro en la tabla alumno");
            CarreraTecnica carrera = await this.kalumDBContext.CarrerasTecnicas.FirstOrDefaultAsync(ct => ct.CarreraId == value.);
            // validar que datos que envio existan
            if (carrera == null)
            {
                this.Logger.LogWarning($"No existe registro de carrera tecnica con id: {value.CarreraId}");
                // error del lado del cliente
                return BadRequest();
            }

            Jornada jornada = await this.kalumDBContext.Jornadas.FirstOrDefaultAsync(j => j.JornadaId == value.JornadaId);
            if (jornada == null)
            {
                this.Logger.LogWarning($"No existe la jornada con id: {value.JornadaId}");
                return BadRequest();
            }
            // si pasa se agrega
            await this.kalumDBContext.Alumnos.AddAsync(value);
            // se guarda en la DB
            await this.kalumDBContext.SaveChangesAsync();
            this.Logger.LogInformation("Se ejecuto el proceso de insertar registro en la tabla alumno");
            // para redirigir la ruta a otro endpoint
            return new CreatedAtRouteResult("GetAlumnoById", new { carne = value.carne }, value);
        }

        // Se le pone Search para diferenciar la ruta al buscar
        [HttpGet("search")]
        // Para buscar por correo o email
        public async Task<ActionResult<IEnumerable<Alumno>>> GetAlumnoByType([FromQuery] string value, [FromQuery] string type)
        {
            this.Logger.LogDebug($"Iniciando el proceso de busqueda con tipo {type} y valor {value}");
            List<Aspirante> aspirantes = null;
            // validar el tipo
            if (type.ToLower().Equals("email"))
            {
                Alumno alumno = await this.kalumDBContext.Alumnos.FirstOrDefaultAsync(al => al.Email == value);
                if (alumno == null)
                {
                    this.Logger
                    .LogWarning($"No se encontraron registros con el email {value}");
                    return NotFound();
                }
                alumnos = new List<Alumno>();
                alumnos.Add(alumno);
            }
            else if (type.ToLower().Equals("apellidos"))
            {
                alumnos = await this.kalumDBContext.Alumnos.Where(al => al.Apellidos.Contains(value)).ToListAsync();
                if (alumnos == null || alumnos.Count == 0)
                {
                    this.Logger.LogWarning($"No se econtraron registros con los apellidos {value}");
                    return NotFound();
                }
            }
            this.Logger.LogInformation("Se realizo el proceso de busqueda de forma exitosa");
            return Ok(alumnos);
        }

        // Para modificar datos de alumno
        [HttpPut("{carne}")]
        public async Task<ActionResult> Put(string carne, [FromBody] Alumno value)
        {
            this.Logger.LogDebug($"Iniciando proceso de modificacion de alumno con numero de expediente: {carne}");
            Alumno alumno = await this.kalumDBContext.Alumnos.FirstOrDefaultAsync(al => al.carne == carne);
            if (alumno == null)
            {
                this.Logger.LogWarning($"No se encontraron registros con el numero de expdiente: {carne}");
                return NotFound();
            }
            else
            {
                // modificar informacion del aspirante
                alumno.Apellidos = value.Apellidos;
                alumno.Nombres = value.Nombres;
                alumno.Direccion = value.Direccion;
                alumno.Telefono = value.Telefono;
                alumno.Email = value.Email;
                // Indicamos que se modifico
                this.kalumDBContext.Entry(alumno).State = EntityState.Modified;
                await this.kalumDBContext.SaveChangesAsync();
                this.Logger.LogInformation($"Se realizo el proceso de modificacion de forma exitosa");
                return NoContent();
            }
        }
*/      

        // Para eliminar un alumno
        [HttpDelete("{carne}")]
        public async Task<ActionResult<Alumno>> Delete(string carne)
        {
            this.Logger.LogDebug($"Iniciando proceso de eliminacion cin el numero de expediente; {carne}");
            Alumno alumno = await this.kalumDBContext.Alumnos.FirstOrDefaultAsync(al => al.Carne == carne);
            if (alumno == null)
            {
                this.Logger.LogWarning($"No se encontraron registros con el numero de expediente: {carne}");
                return NotFound();
            }
            this.kalumDBContext.Alumnos.Remove(alumno);
            await this.kalumDBContext.SaveChangesAsync();
            this.Logger.LogInformation("Se realizo el proceso de eliminación satisfactoriamente");
            return Ok(alumno);
        }
    }
}