using KalumManagement.DTOs;

namespace KalumManagement.Services
{
    public interface IQueueService
    {
        public Task<bool> RequestAspiranteCreateAsync(AspiranteCreateDTO aspirante);
    }
}