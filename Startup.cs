using System.Text;
using KalumManagement.DBContext;
using KalumManagement.Helpers;
using KalumManagement.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KalumManagement
{

    // para puente de configuracion
    public class Startup
    {
        private readonly string OriginKalum = "OriginKalum";

        public IConfiguration Configuration { get; }
        // hacer referencia a cadena de conexion para inyectar
        public Startup(IConfiguration _Configuration)
        {
            this.Configuration = _Configuration;
        }


        // metodo para enlacar DBContext
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => {
                options.AddPolicy(name: OriginKalum, buider =>
                {
                    buider.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200");
                    buider.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://app-kalum:4200");
                });
            });
            // configuracion para validacion de Token con JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Configuration:JWT:Key"])),
                ClockSkew = TimeSpan.Zero
            });
            // para agregar servicio de cache
            services.AddResponseCaching();
            // para inyectar servicio a controladores, cuando haga referencia a la Interface
            services.AddTransient<IQueueService, QueueEnrollmentService>();
            services.AddDbContext<KalumDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("kalumConnectionStrings")));

            // Add services to the container.
            services.AddControllers();
            // configuracion para ignorar enciclado en objetos con sus relaciones
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            // configuracion para ignorar valores nulos
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore);
            // Agregar clase de Filtros de excepciones
            services.AddControllers(options => options.Filters.Add(typeof(ErrorFilterException)));
            // configuracion para el AutoMapper de conversion de Objetos
            services.AddAutoMapper(typeof(Startup));
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors(OriginKalum);
            // para usar cache
            app.UseResponseCaching();
            // -- Para que las peticiones pasen por Https
            // app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endPoints =>
            {
                endPoints.MapControllers();
            });
        }
    }
}