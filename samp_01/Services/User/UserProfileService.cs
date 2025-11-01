using System;
using samp_01.Data.Repositories;
using samp_01.Domain.DTO;

namespace samp_01.Services.User
{
    public class UserProfileService
    {
        private readonly IUserRepository _repo;

        public UserProfileService() : this(new AdoUserRepository(AppConfig.ConnectionString)) { }
        public UserProfileService(IUserRepository repo)
        {
            _repo = repo;
        }

        public UserProfileDTO? GetByName(string name) => _repo.GetProfileByName(name);
        public UserProfileDTO? GetById(int id) => _repo.GetProfileById(id);
    }
}
