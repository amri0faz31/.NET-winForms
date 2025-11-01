using System.Collections.Generic;
using samp_01.Domain.DTO;

namespace samp_01.Data.Repositories
{
    public interface IAdminRepository
    {
        (int Id, string? Hash, string? Salt)? GetCredentialsByName(string name);
        AdminProfileDTO? GetProfileById(int id);
    }
}
