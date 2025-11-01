using samp_01.Data.Repositories;
using samp_01.Domain.DTO;

namespace samp_01.Services.ServiceProvider
{
    // Service for retrieving service provider profile information 
    // this method is used by the Service Provider Dashboard
    public class ServProProfileService
    {
        private readonly ISellerRepository _repo;
        public ServProProfileService() : this(new AdoSellerRepository(AppConfig.ConnectionString)) { }
        public ServProProfileService(ISellerRepository repo) 
        { 
            _repo = repo; 
        }

        public SellerProfileDTO? GetProfile(int id) => _repo.GetSellerProfileById(id);
    }
}
