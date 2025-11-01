namespace samp_01.Domain.DTO
{
    public class LoginDTO
    {
        public string Role { get; set; } = "user";
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? CompanyName { get; set; }
        public string Password { get; set; } = null!;
    }
}