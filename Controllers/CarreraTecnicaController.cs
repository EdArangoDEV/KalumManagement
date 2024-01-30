using AutoMapper;
using KalumManagement.DBContext;
using KalumManagement.DTOs;
using KalumManagement.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KalumManagement.Controllers
{
    [ApiController]
    [Route("Kalum-management/v1/carreras-tecnicas")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CarreraTecnicaController : ControllerBase
    {
        private readonly KalumDBContext KalumDBContext;
        private readonly ILogger Logger;
        private readonly IMapper Mapper;
        public CarreraTecnicaController(IMapper _Mapper ,KalumDBContext _KalumDBContext, ILogger<CarreraTecnicaController> _Logger)
        {
            this.KalumDBContext = _KalumDBContext;
            this.Logger = _Logger;
            this.Mapper = _Mapper;
        }
        
        // Listar carreras tecnicas
        [HttpGet]
        // Para que no necesite autorizacion para visualizar
        [AllowAnonymous]
        // para menejo de cache
        [ResponseCache(Duration = 100)]
        public async Task<ActionResult<IEnumerable<CarreraTecnicaListDTO>>> Get()
        {
            this.Logger.LogDebug("Iniciando el proceso de consulta de las Carreras Tecnicas");
            List<CarreraTecnica> carrerasTecnicas = await this.KalumDBContext.CarrerasTecnicas.ToListAsync();
            this.Logger.LogDebug($"Cantidad de registros: {carrerasTecnicas.Count}");
            if (carrerasTecnicas == null || carrerasTecnicas.Count == 0)
            {
                this.Logger.LogWarning("No existen datos en la tabla Carrera Tecnica");
                return NoContent();
            }
            // return await this.KalumDBContext.CarrerasTecnicas.ToListAsync();
            this.Logger.LogInformation("Se ejecuto la consulta exitosamente");
            return Ok(this.Mapper.Map<List<CarreraTecnicaListDTO>>(carrerasTecnicas));
        }

        // http://localhost:5002/Kalum-management/v1/carreras-tecnicas para enviar peticio en Postman

        // Busqueda de Carrera tecnica
        // en el pad se manda el Id de carrera Tecnica
        [HttpGet("{carreraId}", Name = "GetCarreraTecnicaById")]
        public async Task<ActionResult<CarreraTecnicaListDTO>> GetCarreraTecnicaById(string carreraId)
        {
            this.Logger.LogDebug($"Iniciando el proceso de busqueda de la Carrera Técnica  con ID: {carreraId}");

            CarreraTecnica carreraTecnica = await this.KalumDBContext.CarrerasTecnicas.FirstOrDefaultAsync(ct => ct.CarreraId == carreraId);

            if (carreraTecnica == null)
            {
                this.Logger.LogWarning($"No existe registro con ID: {carreraId}");
                return NotFound();
            }
            this.Logger.LogInformation("Se ejecuto la consulta exitosamente");
            return Ok(this.Mapper.Map<CarreraTecnicaListDTO>(carreraTecnica));
        }

        // Agregar carrera tecnica
        [HttpPost]
        public async Task<ActionResult<CarreraTecnica>> Post([FromBody] CarreraTecnica value)
        {
            this.Logger.LogDebug("Iniciando el proceso de agregar Carrera Técnica");

            CarreraTecnica valueCT = this.Mapper.Map<CarreraTecnica>(value);
            valueCT.CarreraId = Guid.NewGuid().ToString();
            // valueCT.GetCarreraTecnicaById = Guid.NewId().ToString().ToUpper();
            await this.KalumDBContext.CarrerasTecnicas.AddAsync(valueCT);
            // se guarda en la DB
            await this.KalumDBContext.SaveChangesAsync();
            this.Logger.LogInformation("Se ejecuto el proceso de insertar registro en la tabla carrera Tecnica");
            // para redirigir la ruta a otro endpoint
            return new CreatedAtRouteResult("GetCarreraTecnicaById", new { carreraId = value.CarreraId }, value);
        }

        // Buscar carrera Tecnica por Nombre
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CarreraTecnica>>> GetCarreraTecnicaByType([FromQuery] string value, [FromQuery] string type)
        {
            this.Logger.LogDebug($"Iniciando el proceso de busqueda de carrera Tecnica con tipo {type} y valor {value}");
            List<CarreraTecnica> carreraTecnica = null;
            if (type.ToLower().Equals("nombre"))
            {
                carreraTecnica = await this.KalumDBContext.CarrerasTecnicas.Where(ct => ct.Nombre.Contains(value)).ToListAsync();
                if (carreraTecnica == null || carreraTecnica.Count == 0)
                {
                    this.Logger.LogWarning($"No se econtraron registros con el nombre: {value}");
                    return NotFound();
                }
            }
            this.Logger.LogInformation("Se realizo el proceso de busqueda de forma exitosa");
            return Ok(carreraTecnica);
        }

        // Para modificar datos de carreratecnica
        [HttpPut("{carreraId}")]
        public async Task<ActionResult> Put(string carreraId, [FromBody] CarreraTecnicaCreateDTO value)
        {
            this.Logger.LogDebug($"Iniciando proceso de modificacion de carrera Tecnica con Id: {carreraId}");
            CarreraTecnica carreraTecnica = await this.KalumDBContext.CarrerasTecnicas.FirstOrDefaultAsync(ct => ct.CarreraId == carreraId);
            if (carreraId == null)
            {
                this.Logger.LogWarning($"No se encontraron registros con el Id: {carreraId}");
                return NotFound();
            }
            else
            {
                // modificar informacion de la carrera tecnica
                carreraTecnica.Nombre = value.Nombre;
                
                // Indicamos que se modifico
                this.KalumDBContext.Entry(carreraTecnica).State = EntityState.Modified;
                await this.KalumDBContext.SaveChangesAsync();
                this.Logger.LogInformation($"Se realizo el proceso de modificacion de forma exitosa");
                return NoContent();
            }
        }


        // Para eliminar una carrera Tecnica
        [HttpDelete("{CarreraId}")]
        public async Task<ActionResult<CarreraTecnicaListDTO>> Delete(string carreraId)
        {
            this.Logger.LogDebug($"Iniciando proceso de eliminacion con el Id: {carreraId}");
            CarreraTecnica carreraTecnica = await this.KalumDBContext.CarrerasTecnicas.FirstOrDefaultAsync(a => a.CarreraId == carreraId);
            if (carreraTecnica == null)
            {
                this.Logger.LogWarning($"No se encontraron registros con el Id: {carreraId}");
                return NotFound();
            }
            this.KalumDBContext.CarrerasTecnicas.Remove(carreraTecnica);
            await this.KalumDBContext.SaveChangesAsync();
            this.Logger.LogInformation("Se realizo el proceso de eliminación satisfactoriamente");
            return Ok(this.Mapper.Map<List<CarreraTecnicaListDTO>>(carreraTecnica));
        }
    }
}


