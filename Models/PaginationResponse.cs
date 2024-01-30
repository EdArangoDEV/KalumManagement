using KalumManagement.DTOs;

namespace KalumManagement.Models
{
    public class PaginationResponse<T> : PaginationDTO<T>
    {
        public PaginationResponse(List<T> _source, int _number, int _registers)
        {
            this.Number = _number;
            this.TotalPages = (int)Math.Ceiling((double)_registers/2);
            this.Content = _source;
            if (this.Number == 0)
            {
                this.Firts = true;
            }
            else if ((this.Number + 1) == this.TotalPages)
            {
                this.Last = true;
            }
            else
            {
                this.Firts = false;
                this.Last = false;
            }
        }
    }
}