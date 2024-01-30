namespace KalumManagement.Helpers
{
    // capa de exception
    public class RabbitConnectionException : Exception
    {
        // constructor de clase base, para personalizar mensaje de error
        public RabbitConnectionException() : base("Error al momento de conectarse a Rabbit")
        {
            
        }
    }
}