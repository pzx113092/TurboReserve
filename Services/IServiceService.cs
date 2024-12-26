using System.Collections.Generic;
using System.Threading.Tasks;
using TurboReserve.Models;

namespace TurboReserve.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<Models.Service>> GetAllAsync();
        Task<Models.Service> GetByIdAsync(int id);
        Task AddAsync(Models.Service service);
        Task UpdateAsync(Models.Service service);
        Task DeleteAsync(int id);

        Task<IEnumerable<Models.Service>> GetByServiceProviderIdAsync(int serviceProviderId);
    }
}

