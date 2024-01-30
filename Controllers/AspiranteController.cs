using AutoMapper;
using KalumManagement.DBContext;
using KalumManagement.DTOs;
using KalumManagement.Entities;
using KalumManagement.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KalumManagement.Controllers
{
    [ApiController]
    [Route("Kalum-management/v1/aspirantes")]

    // para proteger peticiones
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AspiranteController : ControllerBase
    {
        private readonly KalumDBContext KalumDBContext;
        private readonly ILogger Logger;
        private readonly IQueueService QueueService;
        // referencia de AutoMapper
        private readonly IMapper Mapper;
        public AspiranteController(IMapper _Mapper, KalumDBContext _KalumDBContext, ILogger<AspiranteController> _Logger, IQueueService _IQueueService)
        {
            this.KalumDBContext = _KalumDBContext;
            this.Logger = _Logger;
            this.QueueService = _IQueueService;
            this.Mapper = _Mapper;
        }

        //
        /*
        [HttpGet("page/{page}")]
        public async Task<ActionResult<IEnumerable<AspiranteListDTO>>> Get()
        {
            var queryable = this.KalumDBContext.Aspirantes.AsQueryable();
            int registros = await queryable.CountAsync();
            if (registros == 0)
            {
                return NoContent();
            }
            else
            {
                var aspirantes = queryable.OrderBy(a => a.Apellidos);
                
            }

        }*/

        // Listar Aspirantes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AspiranteListDTO>>> Get()
        {
            this.Logger.LogDebug("Iniciando el proceso de consulta de los aspirantes");
            List<Aspirante> aspirantes = await this.KalumDBContext.Aspirantes.Include(a => a.CarreraTecnica).Include(a => a.ExamenAdmision).Include(a => a.Jornada).ToListAsync();
            this.Logger.LogDebug($"Cantidad de registros: {aspirantes.Count}");
            if (aspirantes == null || aspirantes.Count
             == 0)
            {
                this.Logger.LogWarning("No existen datos en la tabla aspirante");
                return NoContent();
            }
            this.Logger.LogInformation("Se ejecuto la consulta exitosamente");
            
            // Conversion de Objetos aspirante a aspiranteListDTO con AutoMapper
            List<AspiranteListDTO> aspirantesDTO = this.Mapper.Map<List<AspiranteListDTO>>(aspirantes);

            return Ok(aspirantesDTO);

            /*
            // para convertir aspirante a aspiranteListDTO sin AutoMapper

            List<AspiranteListDTO> aspirantesDTO = new List<AspiranteListDTO>();

            foreach (Aspirante a in aspirantes)
            {
                AspiranteListDTO nuevo = new AspiranteListDTO();
                nuevo.Apellidos = a.Apellidos;
                nuevo.Nombres = a.Nombres;
                nuevo.Direccion = a.Direccion;
                nuevo.Telefono = a.Telefono;
                nuevo.Email = a.Email;
                nuevo.Estatus = a.Estatus;
                CarreraTecnicaListDTO ctDTO = new CarreraTecnicaListDTO();
                ctDTO.CarreraId = a.CarreraTecnica.CarreraId;
                ctDTO.Nombre = a.CarreraTecnica.Nombre;
                JornadaListDTO jDTO = new JornadaListDTO();
                jDTO.JornadaId = a.Jornada.JornadaId;
                jDTO.Descripcion = a.Jornada.Descripcion;
                jDTO.Prefijo = a.Jornada.Prefijo;
                ExamenAdmisionListDTO exDTO = new ExamenAdmisionListDTO();
                exDTO.ExamenId = a.ExamenAdmision.ExamenId;
                exDTO.FechaExamen = a.ExamenAdmision.FechaExamen;  
                nuevo.CarreraTecnica = ctDTO;
                nuevo.Jornada = jDTO;
                nuevo.ExamenAdmision = exDTO;
                aspiranteDTO.Add(nuevo);
            }
            
            return Ok(aspirantesDTO);
            */
        }

        // Busqueda de Aspirante
        // en el pad se manda el numero de expediente
        [HttpGet("{noExpediente}", Name = "GetAspiranteById")]
        public async Task<ActionResult<AspiranteListDTO>> GetAspiranteById(string noExpediente)
        {
            this.Logger.LogDebug($"Iniciando proceso de busqueda con Numero de expediente: {noExpediente}");
            Aspirante aspirante = await this.KalumDBContext.Aspirantes.Include(a => a.CarreraTecnica).Include(a => a.ExamenAdmision).Include(a => a.Jornada).FirstOrDefaultAsync(a => a.NoExpediente == noExpediente);

            // validar si existe
            if (aspirante == null)
            {
                this.Logger.LogWarning($"No existe registro con numero de expediente: {noExpediente}");
                return NotFound();
            }
            this.Logger.LogInformation($"Se ejecuto la consulta exitosamente");
            // hacer la conversion en el retorno
            return Ok(this.Mapper.Map<AspiranteListDTO>(aspirante));
        }

        
        // Agregar Aspirante 
        [HttpPost]
        // indicar que se mandara informacion en Body [FromBody]
        public async Task<ActionResult<Aspirante>> Post([FromBody] Aspirante value)
        {
            this.Logger.LogDebug("Iniciando proceso de insertar registro en la tabla aspirante");
            CarreraTecnica carrera = await this.KalumDBContext.CarrerasTecnicas.FirstOrDefaultAsync(ct => ct.CarreraId == value.CarreraId);
            // validar que datos que envio existan
            if (carrera == null)
            {
                this.Logger.LogWarning($"No existe registro de carrera tecnica con id: {value.CarreraId}");
                // error del lado del cliente
                return BadRequest();
            }

            Jornada jornada = await this.KalumDBContext.Jornadas.FirstOrDefaultAsync(j => j.JornadaId == value.JornadaId);
            if (jornada == null)
            {
                this.Logger.LogWarning($"No existe la jornada con id: {value.JornadaId}");
                return BadRequest();
            }
            ExamenAdmision examenAdmision = await this.KalumDBContext.ExamenesAdmision.FirstOrDefaultAsync(ea => ea.ExamenId == value.ExamenId);
            if (examenAdmision == null)
            {
                this.Logger.LogWarning($"El examen con id: {value.ExamenId} no existe");
                return BadRequest();
            }
            // si pasa se agrega
            await this.KalumDBContext.Aspirantes.AddAsync(value);
            // se guarda en la DB
            await this.KalumDBContext.SaveChangesAsync();
            this.Logger.LogInformation("Se ejecuto el proceso de insertar registro en la tabla aspirante");
            // para redirigir la ruta a otro endpoint
            return new CreatedAtRouteResult("GetAspiranteById", new { noExpediente = value.NoExpediente }, value);
        }


        // para mandar en cola de Rabbit
        [HttpPost("enrollment")]
        public async Task<ActionResult> PostEnrollment([FromBody] AspiranteCreateDTO value)
        {
            this.Logger.LogDebug("Iniciando proceso de insertar registro en la tabla aspirante");
            CarreraTecnica carrera = await this.KalumDBContext.CarrerasTecnicas.FirstOrDefaultAsync(ct => ct.CarreraId == value.CarreraId);
            // validar que datos que envio existan
            if (carrera == null)
            {
                this.Logger.LogWarning($"No existe registro de carrera tecnica con id: {value.CarreraId}");
                // error del lado del cliente
                return BadRequest();
            }

            Jornada jornada = await this.KalumDBContext.Jornadas.FirstOrDefaultAsync(j => j.JornadaId == value.JornadaId);
            if (jornada == null)
            {
                this.Logger.LogWarning($"No existe la jornada con id: {value.JornadaId}");
                return BadRequest();
            }
            ExamenAdmision examenAdmision = await this.KalumDBContext.ExamenesAdmision.FirstOrDefaultAsync(ea => ea.ExamenId == value.ExamenId);
            if (examenAdmision == null)
            {
                this.Logger.LogWarning($"El examen con id: {value.ExamenId} no existe");
                return BadRequest();
            }
            // inyectar servicio de Cola que construimos 
            bool resultado = await this.QueueService.RequestAspiranteCreateAsync(value);
            if (resultado == true)
            {
                return Ok(new EnrollmentResponseDTO() {HttpStatusCode = "201", Message = "Solicitud creada exitosamente"});
            }
            else
            {
                return Forbid();
            }
        }


        // Se le pone Search para diferenciar la ruta al buscar
        [HttpGet("search")]
        // Para buscar por correo o email
        public async Task<ActionResult<IEnumerable<Aspirante>>> GetAspiranteByType([FromQuery] string value, [FromQuery] string type)
        {
            this.Logger.LogDebug($"Iniciando el proceso de busqueda con tipo {type} y valor {value}");
            List<Aspirante> aspirantes = null;
            // validar el tipo
            if (type.ToLower().Equals("email"))
            {
                Aspirante aspirante = await this.KalumDBContext.Aspirantes.FirstOrDefaultAsync(a => a.Email == value);
                if (aspirante == null)
                {
                    this.Logger
                    .LogWarning($"No se encontraron registros con el email {value}");
                    return NotFound();
                }
                aspirantes = new List<Aspirante>();
                aspirantes.Add(aspirante);
            }
            else if (type.ToLower().Equals("apellidos"))
            {
                aspirantes = await this.KalumDBContext.Aspirantes.Where(a => a.Apellidos.Contains(value)).ToListAsync();
                if (aspirantes == null || aspirantes.Count == 0)
                {
                    this.Logger.LogWarning($"No se econtraron registros con los apellidos {value}");
                    return NotFound();
                }
            }
            this.Logger.LogInformation("Se realizo el proceso de busqueda de forma exitosa");
            return Ok(aspirantes);
        }


        // Para modificar datos de aspirante
        [HttpPut("{noExpediente}")]
        public async Task<ActionResult> Put(string noExpediente, [FromBody] AspiranteCreateDTO value)
        {
            this.Logger.LogDebug($"Iniciando proceso de modificacion de aspirante con numero de expediente: {noExpediente}");
            Aspirante aspirante = await this.KalumDBContext.Aspirantes.FirstOrDefaultAsync(a => a.NoExpediente == noExpediente);
            if (aspirante == null)
            {
                this.Logger.LogWarning($"No se encontraron registros con el numero de expdiente: {noExpediente}");
                return NotFound();
            }
            else
            {
                // modificar informacion del aspirante
                aspirante.Apellidos = value.Apellidos;
                aspirante.Nombres = value.Nombres;
                aspirante.Direccion = value.Direccion;
                aspirante.Telefono = value.Telefono;
                aspirante.Email = value.Email;
                // aspirante.Estatus = value.Estatus;
                aspirante.CarreraId = value.CarreraId;
                aspirante.ExamenId = value.ExamenId;
                aspirante.JornadaId = value.JornadaId;
                // Indicamos que se modifico
                this.KalumDBContext.Entry(aspirante).State = EntityState.Modified;
                await this.KalumDBContext.SaveChangesAsync();
                this.Logger.LogInformation($"Se realizo el proceso de modificacion de forma exitosa");
                return NoContent();
            }
        }

        // Para eliminar un aspirante
        [HttpDelete("{noExpediente}")]
        public async Task<ActionResult<AspiranteListDTO>> Delete(string noExpediente)
        {
            this.Logger.LogDebug($"Iniciando proceso de eliminacion cin el numero de expediente; {noExpediente}");
            Aspirante aspirante = await this.KalumDBContext.Aspirantes.FirstOrDefaultAsync(a => a.NoExpediente == noExpediente);
            if (aspirante == null)
            {
                this.Logger.LogWarning($"No se encontraron registros con el numero de expediente: {noExpediente}");
                return NotFound();
            }
            this.KalumDBContext.Aspirantes.Remove(aspirante);
            await this.KalumDBContext.SaveChangesAsync();
            this.Logger.LogInformation("Se realizo el proceso de eliminación satisfactoriamente");
            // return Ok(this.Mapper.Map<AspiranteListDTO>(aspirante));
            return Ok($"Se elimino aspirante con Numero de Expediente: {aspirante.NoExpediente}");
        }
    }
}