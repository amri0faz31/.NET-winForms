using System;
using System.Linq;
using samp_01.Data.Repositories;
using samp_01.Domain.DTO;
using samp_01.Domain.Entities;
using samp_01.Helpers;

namespace samp_01.Services.User
{
    using DomainUser = samp_01.Domain.Entities.User;

    public class RegistrationService
    {
        private readonly IUserRepository _repo;

        public RegistrationService() : this(new AdoUserRepository(AppConfig.ConnectionString)) { }

        public RegistrationService(IUserRepository repo)
        {
            _repo = repo;
        }
        //used to register a new user using RegisterDTO
        public bool Register(RegisterDTO dto, out string? error)
        {
            error = null;
            if (dto == null)
            {
                error = "Invalid data.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Password))
            {
                error = "Name and password required.";
                return false;
            }

            if (_repo.ExistsByName(dto.Name))
            {
                error = "User already exists.";
                return false;
            }

            var (hash, salt) = PasswordHelper.HashPassword(dto.Password);

            var user = new DomainUser
            {
                Name = dto.Name,
                PasswordHash = hash,
                Salt = salt,
                Email = dto.Email,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                CardHolderName = dto.CardHolderName,
                CardMasked = MaskCard(dto.CardNumber),
                CardLast4 = MaskLast4(dto.CardNumber),
                CardExpiry = dto.CardExpiry,
                CreatedAt = DateTime.UtcNow
            };

            var id = _repo.AddUser(user);
            return id > 0;
        }

        private static string? MaskCard(string? card)
        {
            if (string.IsNullOrWhiteSpace(card)) return null;
            var digits = new string(card.Where(char.IsDigit).ToArray());
            if (digits.Length <= 4) return digits;
            return new string('*', digits.Length - 4) + digits.Substring(digits.Length - 4);
        }

        private static string? MaskLast4(string? card)
        {
            if (string.IsNullOrWhiteSpace(card)) return null;
            var digits = new string(card.Where(char.IsDigit).ToArray());
            if (digits.Length <= 4) return digits;
            return digits.Substring(digits.Length - 4);
        }
    }
}
