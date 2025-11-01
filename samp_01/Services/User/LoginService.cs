using System;
using System.Diagnostics.CodeAnalysis;
using samp_01.Data.Repositories;
using samp_01.Domain.DTO;
using samp_01.Helpers;

namespace samp_01.Services.User
{
    // Service for user, seller, and admin authentication
    public class LoginService
    {
        private readonly IUserRepository _userRepo;
        private readonly ISellerRepository _sellerRepo;
        private readonly IAdminRepository _adminRepo;

        // default constructor
        public LoginService()
         : this(new AdoUserRepository(AppConfig.ConnectionString),
               new AdoSellerRepository(AppConfig.ConnectionString),
               new AdoAdminRepository(AppConfig.ConnectionString))
        { }

        // legacy2-arg constructor (kept for compatibility)
        public LoginService(IUserRepository userRepo, ISellerRepository sellerRepo)
         : this(userRepo, sellerRepo, new AdoAdminRepository(AppConfig.ConnectionString))
        { }

        // full constructor
        public LoginService(IUserRepository userRepo, ISellerRepository sellerRepo, IAdminRepository adminRepo)
        {
            _userRepo = userRepo;
            _sellerRepo = sellerRepo;
            _adminRepo = adminRepo;
        }

        // Authenticate and retrieve a typed profile DTO (UserProfileDTO, SellerProfileDTO, or AdminProfileDTO)
        public bool Authenticate(LoginDTO dto, [NotNullWhen(true)] out object? profile)
        {
            profile = null;
            if (dto == null) return false;

            // Admin
            if (string.Equals(dto.Role, "admin", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(dto.Name)) return false;
                var cred = _adminRepo.GetCredentialsByName(dto.Name);
                if (cred == null || cred.Value.Hash == null || cred.Value.Salt == null) return false;
                if (!PasswordHelper.VerifyPassword(dto.Password, cred.Value.Salt, cred.Value.Hash)) return false;
                profile = _adminRepo.GetProfileById(cred.Value.Id);
                return profile != null;
            }

            // Seller
            if (string.Equals(dto.Role, "seller", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(dto.Email)) return false;
                var cred = _sellerRepo.GetCredentialsByEmail(dto.Email);
                if (cred == null || cred.Value.Hash == null || cred.Value.Salt == null) return false;
                if (!PasswordHelper.VerifyPassword(dto.Password, cred.Value.Salt, cred.Value.Hash)) return false;
                profile = _sellerRepo.GetSellerProfileById(cred.Value.Id);
                return profile != null;
            }

            // User (default)
            if (string.IsNullOrWhiteSpace(dto.Name)) return false;
            var ucred = _userRepo.GetCredentialsByName(dto.Name);
            if (ucred == null || ucred.Value.Hash == null || ucred.Value.Salt == null) return false;
            if (!PasswordHelper.VerifyPassword(dto.Password, ucred.Value.Salt, ucred.Value.Hash)) return false;
            profile = _userRepo.GetProfileById(ucred.Value.Id);
            return profile != null;
        }
    }
}
