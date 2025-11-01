using System.Diagnostics.CodeAnalysis;
using samp_01.Data.Repositories;
using samp_01.Domain.DTO;
using samp_01.Helpers;

namespace samp_01.Services.Admin
{
    public class AdminLoginService
    {
        private readonly IAdminRepository _repo;
        public AdminLoginService() : this(new AdoAdminRepository(AppConfig.ConnectionString)) { }
        public AdminLoginService(IAdminRepository repo) { _repo = repo; }

        public bool Authenticate(string name, string password, [NotNullWhen(true)] out AdminProfileDTO? profile)
        {
            profile = null;
            var cred = _repo.GetCredentialsByName(name);
            if (cred == null || cred.Value.Hash == null || cred.Value.Salt == null) return false;
            if (!PasswordHelper.VerifyPassword(password, cred.Value.Salt, cred.Value.Hash)) return false;
            profile = _repo.GetProfileById(cred.Value.Id);
            return profile != null;
        }
    }
}
