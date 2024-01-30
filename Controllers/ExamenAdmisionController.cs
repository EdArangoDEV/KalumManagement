using AutoMapper;
using KalumManagement.DBContext;
using KalumManagement.DTOs;
using KalumManagement.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KalumManagement.Controllers
{
    // NOTA: corregir el endPoint de examenes
    [ApiController]
    [Route("Kalum-management/v1/examenes-admision")]
    public class ExamenAdmisionController : ControllerBase
    {
        private readonly KalumDBContext KalumDBContext;

        private readonly ILogger Logger;
        // referencia de AutoMapper
        private readonly IMapper Mapper;

        public ExamenAdmisionController(IMapper _Mapper ,KalumDBContext _KalumDBContext, ILogger<ExamenAdmisionController> _Logger)
        {
            this.KalumDBContext = _KalumDBContext;
            this.Logger = _Logger;
            this.Mapper = _Mapper;
        }

        // Listar Examenes Admision
        [HttpGet]
        // Para que no necesite autorizacion para visualizar
        [AllowAnonymous]
        // para menejo de cache
        [ResponseCache(Duration = 100)]
        public async Task<ActionResult<IEnumerable<ExamenAdmisionListDTO>>> Get()
        {
            this.Logger.LogDebug("Iniciando el proceso de consulta de los Examenes de Admision");
            List<ExamenAdmision> examenesAdmision = await this.KalumDBContext.ExamenesAdmision.ToListAsync();
            this.Logger.LogDebug($"Cantidad de registros: {examenesAdmision.Count}");
            if (examenesAdmision == null || examenesAdmision.Count == 0)
            {
                this.Logger.LogWarning($"No existen datos en tabla Examenes Admision");
                return NotFound();
            }
            this.Logger.LogInformation("Se ejecuto la consulta exitosamente");
            return Ok(this.Mapper.Map<List<ExamenAdmisionListDTO>>(examenesAdmision));

            // return await this.KalumDBContext.ExamenesAdmision.ToListAsync();
        }

        // Busqueda de Examen de Admision por ID
        [HttpGet("{examenId}", Name = "GetExamenAdmisionById")]
        public async Task<ActionResult<ExamenAdmisionListDTO>> GetExamenAdmisionById(string examenId)
        {

            this.Logger.LogDebug($"Iniciando el proceso de consulta de Examen de Admision por ID: {examenId}");

            ExamenAdmision examenAdmision = await this.KalumDBContext.ExamenesAdmision.FirstOrDefaultAsync(ea => ea.ExamenId == examenId);

            if (examenAdmision == null)
            {
                this.Logger.LogWarning($"No existe registro de Examen de Admision con ID: {examenId}");
                return NotFound();
            }
            this.Logger.LogInformation("Se ejecuto la consulta exitosamente");
            return Ok(this.Mapper.Map<ExamenAdmisionListDTO>(examenAdmision));
            // return Ok(this.Mapper.Map<ExamenAdmisionListDTO>(examenAdmision));
        }

        // Agregar Examen Admision
        [HttpPost]
        public async Task<ActionResult<ExamenAdmision>> Post([FromBody] ExamenAdmision value)
        {
            this.Logger.LogDebug("Iniciando el proceso de insertar Examen de Admision");

            // Agregar en todos los EndPoints
            // para mandar Id de examen Admision
            ExamenAdmision valueEA = this.Mapper.Map<ExamenAdmision>(value);
            valueEA.ExamenId = Guid.NewGuid().ToString();
            valueEA.FechaExamen = DateTime.Parse(value.FechaExamen.ToString());
            
            await this.KalumDBContext.ExamenesAdmision.AddAsync(value);
            await this.KalumDBContext.SaveChangesAsync();
            this.Logger.LogInformation("Se ejecuto la operacion exitosamente");
            return CreatedAtRoute("GetExamenAdmisionById", new { examenId = value.ExamenId }, value);
        }


        // para buscar examen por tipo
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ExamenAdmision>>> GetExamenAdmisionByType([FromQuery] string value, [FromQuery] string type)
        {
            this.Logger.LogDebug($"Iniciando el proceso de busqueda de Examen de Admision con tipo {type} y valor {value}");
            List<ExamenAdmision> examenesAdmision = null;
            if (type.ToLower().Equals("fecha"))
            {
                examenesAdmision = await this.KalumDBContext.ExamenesAdmision.Where(ex => ex.FechaExamen.ToString().Contains(value)).ToListAsync();
                if (examenesAdmision == null || examenesAdmision.Count == 0)
                {
                    this.Logger.LogWarning($"No existe registro de Examen de Admision con fecha {value}");
                    return NotFound();
                }
            }
            this.Logger.LogInformation("Se realizo el proceso de busqueda de forma exitosa");
            return Ok(examenesAdmision);
        }

        // para modificar datos de examen admision
        [HttpPut("{examenId}")]
        public async Task<ActionResult<ExamenAdmision>> Put(string examenId, [FromBody] ExamenAdmisionCreateDTO value)
        {
            this.Logger.LogDebug($"Iniciando el proceso de modificar Examen de Admision con ID: {examenId}");
            ExamenAdmision examenAdmision = await this.KalumDBContext.ExamenesAdmision.FirstOrDefaultAsync(ea => ea.ExamenId == examenId);
            if (examenAdmision == null)
            {
                this.Logger.LogWarning($"No se encontraron registros con el Id: {examenId}");
                return NotFound();
            }
            else
            {
                // modificar informacion del aspirante
                examenAdmision.FechaExamen = value.FechaExamen;
                // Indicamos que se modifico
                this.KalumDBContext.Entry(examenAdmision).State = EntityState.Modified;
                await this.KalumDBContext.SaveChangesAsync();
                this.Logger.LogInformation($"Se realizo el proceso de modificacion de forma exitosa");
                return NoContent();
            }
        }

        // para eliminar un examen de admision
        [HttpDelete("{examenId}")]
        public async Task<ActionResult<ExamenAdmisionListDTO>> Delete(string examenId)
        {
            this.Logger.LogDebug($"Iniciando el proceso de eliminacion de Examen de Admision con ID: {examenId}");
            ExamenAdmision examenAdmision = await this.KalumDBContext.ExamenesAdmision.FirstOrDefaultAsync(ea => ea.ExamenId == examenId);
            if (examenAdmision == null)
            {
                this.Logger.LogWarning($"No se encontraron registros con el Id: {examenId}");
                return NotFound();
            }
            this.KalumDBContext.ExamenesAdmision.Remove(examenAdmision);
            await this.KalumDBContext.SaveChangesAsync();
            this.Logger.LogInformation("Se realizo el proceso de eliminación satisfactoriamente");
            return Ok(this.Mapper.Map<ExamenAdmisionListDTO>(examenAdmision));
        }    
    }
}   