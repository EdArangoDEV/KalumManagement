namespace KalumManagement.DTOs
{
    // la clase tendra un tipo de dato generico, para poder usarlo en otras clases
    public class PaginationDTO<T>
    {
        public int Number { get; set; }
        public int TotalPages { get; set; }
        public bool Firts { get; set; }
        public bool Last { get; set; }
        // T significa que son datos genericos
        public List<T> Content { get; set; }
    }
}