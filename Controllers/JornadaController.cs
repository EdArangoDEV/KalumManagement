using KalumManagement.DBContext;
using Microsoft.AspNetCore.Mvc;
using KalumManagement.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using KalumManagement.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace KalumManagement.Controllers
{
    [ApiController]
    [Route("Kalum-management/v1/jornadas")]
    public class JornadaController : ControllerBase
    {
        private readonly KalumDBContext KalumDBContext;
        private readonly ILogger Logger;
        private readonly IMapper Mapper;

        public JornadaController(IMapper _Mapper,KalumDBContext _KalumDBContext, ILogger<JornadaController> _Logger)
        {
            this.KalumDBContext = _KalumDBContext;
            this.Logger = _Logger;
            this.Mapper = _Mapper;
        }

        // Listar Jornadas
        [HttpGet]
        // Para que no necesite autorizacion para visualizar
        [AllowAnonymous]
        // para menejo de cache
        [ResponseCache(Duration = 100)]
        public async Task<ActionResult<IEnumerable<JornadaListDTO>>> Get()
        {
            this.Logger.LogDebug($"iniciando el proceso de consulta de las jornadas");
            List<Jornada> jornadas = await this.KalumDBContext.Jornadas.ToListAsync();
            this.Logger.LogDebug($"Cantidad de registros: {jornadas.Count}");
            if (jornadas == null || jornadas.Count == 0)
            {
                this.Logger.LogWarning("No existen datos en la tabla Jornadas");
                return NotFound();
            }
            this.Logger.LogInformation("Se ejecuto la consulta exitosamente");
            return Ok(this.Mapper.Map<List<JornadaListDTO>>(jornadas));

            // return await this.KalumDBContext.Jornadas.ToListAsync();
        }

        // Busqueda de Jornadas en el pad se manda Id
        [HttpGet("{jornadaId}", Name = "GetJornadaById")]
        public async Task<ActionResult<JornadaListDTO>> GetJornadaById(string jornadaId)
        {
            this.Logger.LogDebug($"iniciando el proceso de consulta de la jornada con id: {jornadaId}");

            Jornada jornada = await this.KalumDBContext.Jornadas.FirstOrDefaultAsync(j => j.JornadaId == jornadaId);

            if (jornada == null)
            {
                this.Logger.LogWarning($"No existe registro de Jornada con Id: {jornadaId}");
                return NotFound();
            }
            this.Logger.LogInformation($"Se ejecuto la consulta exitosamente");
            return Ok(this.Mapper.Map<JornadaListDTO>(jornada));
        }

        // Agregar Jornada 
        [HttpPost]
        // indicar que se mandara informacion en Body [FromBody]
        public async Task<ActionResult> Post([FromBody] Jornada value)
        {
            this.Logger.LogDebug("Iniciando proceso de insertar registro en la tabla Jornanda");

            Jornada valueJd = this.Mapper.Map<Jornada>(value);
            valueJd.JornadaId = Guid.NewGuid().ToString();

            // si pasa se agrega
            await this.KalumDBContext.Jornadas.AddAsync(valueJd);
            // se guarda en la DB
            await this.KalumDBContext.SaveChangesAsync();
            this.Logger.LogInformation("Se ejecuto el proceso de insertar registro en la tabla jornada");
            // para redirigir la ruta a otro endpoint
            return new CreatedAtRouteResult("GetJornadaById", new { jornadaId = value.JornadaId }, value);
        }

        // Se le pone Search para diferenciar la ruta al buscar
        // para buscar por descripcion
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Jornada>>> GetAspiranteByType([FromQuery] string value, [FromQuery] string type)
        {
            this.Logger.LogDebug($"Iniciando el proceso de busqueda con tipo {type} y valor {value}");
            List<Jornada> jornadas = null;
            if (type.ToLower().Equals("descripcion"))
            {
                jornadas = await this.KalumDBContext.Jornadas.Where(j => j.Descripcion.Contains(value)).ToListAsync();
                if (jornadas == null || jornadas.Count == 0)
                {
                    this.Logger.LogWarning($"No se econtraron registros con la descripcion {value}");
                    return NotFound();
                }
            }
            this.Logger.LogInformation("Se realizo el proceso de busqueda de forma exitosa");
            return Ok(jornadas);
        }

        // Para modificar datos de aspirante
        [HttpPut("{jornadaId}")]
        public async Task<ActionResult> Put(string jornadaId, [FromBody] JornadaCreateDTO value)
        {
            this.Logger.LogDebug($"Iniciando proceso de modificacion de jornada con numero de Id: {jornadaId}");
            Jornada jornada = await this.KalumDBContext.Jornadas.FirstOrDefaultAsync(j => j.JornadaId == jornadaId);
            if (jornada == null)
            {
                this.Logger.LogWarning($"No se encontraron registros con el Id: {jornadaId}");
                return NotFound();
            }
            else
            {
                // modificar informacion del aspirante
                jornada.Prefijo = value.Prefijo;
                jornada.Descripcion = value.Descripcion;
                
                // Indicamos que se modifico
                this.KalumDBContext.Entry(jornada).State = EntityState.Modified;
                await this.KalumDBContext.SaveChangesAsync();
                this.Logger.LogInformation($"Se realizo el proceso de modificacion de forma exitosa");
                return NoContent();
            }
        }

        // Para eliminar una jornada
        [HttpDelete("{jornadaId}")]
        public async Task<ActionResult<JornadaListDTO>> Delete(string jornadaId)
        {
            this.Logger.LogDebug($"Iniciando proceso de eliminacion con el Id; {jornadaId}");
            Jornada jornada = await this.KalumDBContext.Jornadas.FirstOrDefaultAsync(j => j.JornadaId == jornadaId);
            if (jornada == null)
            {
                this.Logger.LogWarning($"No se encontraron registros con el numero de expediente: {jornadaId}");
                return NotFound();
            }
            this.KalumDBContext.Jornadas.Remove(jornada);
            await this.KalumDBContext.SaveChangesAsync();
            this.Logger.LogInformation("Se realizo el proceso de eliminación satisfactoriamente");
            return Ok(this.Mapper.Map<List<JornadaListDTO>>(jornada));
        }
    }
}