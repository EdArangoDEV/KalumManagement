using System.Text;
using System.Text.Json;
using KalumManagement.DTOs;
using KalumManagement.Helpers;
using RabbitMQ.Client;

namespace KalumManagement.Services
{
    // metodo para conectarse a la cola de Rabbit e inyectar la solicitu
    public class QueueEnrollmentService : IQueueService
    {
        private readonly ILogger<QueueEnrollmentService> Logger;
        // constructor
        public QueueEnrollmentService(ILogger<QueueEnrollmentService> _Logger)
        {
            this.Logger = _Logger;
        }


        public async Task<bool> RequestAspiranteCreateAsync(AspiranteCreateDTO aspirante)
        {
            bool response = false;
            ConnectionFactory connectionFactory = new ConnectionFactory();
            IConnection connection = null;
            IModel channel = null;
            connectionFactory.HostName = "localhost";
            connectionFactory.VirtualHost = "/";
            connectionFactory.Port = 5672;
            connectionFactory.UserName = "guest";
            connectionFactory.Password = "guest";

            // controlar conexion a rabbit
            try
            {
                connection = connectionFactory.CreateConnection();
                Logger.LogDebug("Conexion a rabbit de forma exitosa");
                channel = connection.CreateModel();
                // publicar mensaje a la cola
                channel.BasicPublish("net.dev23.enrollments", "", null, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(aspirante)));
                Logger.LogDebug("Mensaje publicado en la cola");
                response = true;
            }
            catch(Exception ex)
            {
                this.Logger.LogError($"Hubo un error al momento de conectarse a la cola, error {ex.Message}");
                // throw es lanzar, para que lanze esa capa de exception
                connection.Close();
                throw new RabbitConnectionException(); 
            }
            // para que acepte el proceso por ser async y responder sin problema
            await Task.Delay(100);
            return response;
        }
    }
}