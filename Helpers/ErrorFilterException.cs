using KalumManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using RabbitMQ.Client.Exceptions;

namespace KalumManagement.Helpers
{
    //  PARA CONTROLAR ERRORES 
    public class ErrorFilterException : IActionFilter
    {
        // metodos implementados de Interfaz
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Excepcion de SQL
            if (context.Exception is SqlException)
            {
                // Personalizacion de error
                ErrorResponse error = new ErrorResponse()
                {
                    ErrorType = "COM",
                    HttpStatusCode = 503,
                    Message = "Error en el servicio legado de la base de datos"
                };
                // respuesta
                context.Result = new ObjectResult(503)
                {
                    StatusCode = 503,
                    Value = error
                };
            }
            // para capturar error sin un try en el enrollment
            else if (context.Exception is BrokerUnreachableException)
            {
                // Personalizacion de error
                ErrorResponse error = new ErrorResponse()
                {
                    ErrorType = "COM",
                    HttpStatusCode = 503,
                    Message = "Error en el servicio legado al conectarse al Broker Rabbit"
                };
                // respuesta
                context.Result = new ObjectResult(503)
                {
                    StatusCode = 503,
                    Value = error
                };
            }
            else if (context.Exception is RabbitConnectionException)
            {
                ErrorResponse error = new ErrorResponse()
                {
                    ErrorType = "COM",
                    HttpStatusCode = 503,
                    // capturo el error persoanlizado en RabbitConnectionException
                    Message = context.Exception.Message
                };
                // respuesta
                context.Result = new ObjectResult(503)
                {
                    StatusCode = 503,
                    Value = error
                };
            }   
            context.ExceptionHandled = true;
        }
    
        // metodos implementados de Interfaz
        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }
    }
}