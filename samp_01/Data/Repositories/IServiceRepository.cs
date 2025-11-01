using System.Collections.Generic;
using samp_01.Domain.Entities;

namespace samp_01.Data.Repositories
{
    public interface IServiceRepository
    {
        List<Service> GetAll();
    }
}
