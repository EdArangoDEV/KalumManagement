namespace KalumManagement.Entities
{
    public class Jornada
    {
        public string JornadaId { get; set; }
        public string Prefijo { get; set; }
        public string Descripcion { get; set; }
        
        public virtual List<Aspirante> Aspirantes { get; set; }
    }
}