using System;
using samp_01.Data.Repositories;
using samp_01.Domain.DTO;
using samp_01.Domain.Entities;
using samp_01.Helpers;

namespace samp_01.Services.ServiceProvider
{
    // Service for retrieving service provider profiles
    // called by DashboardForm and other forms
    public class ServProRegistrationService
    {
        private readonly ISellerRepository _repo;
        public ServProRegistrationService() : this(new AdoSellerRepository(AppConfig.ConnectionString)) { }
        public ServProRegistrationService(ISellerRepository repo) { _repo = repo; }

        public bool Register(SellerRegisterDTO dto, out string? error)
        {
            error = null;
            if (dto == null) { error = "Invalid data"; return false; }
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password)) { error = "Email and password required"; return false; }
            if (_repo.ExistsByEmail(dto.Email)) { error = "Email already registered"; return false; }
            var (hash, salt) = PasswordHelper.HashPassword(dto.Password);
            var seller = new Seller
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = hash,
                Salt = salt,
                Phone = dto.Phone,
                ServiceId = dto.ServiceId,
                CompanyName = dto.CompanyName,
                CompanyAddress = dto.CompanyAddress,
                LogoPath = dto.LogoPath,
                CreatedAt = DateTime.UtcNow
            };
            var id = _repo.AddSeller(seller);
            return id > 0;
        }
    }
}
