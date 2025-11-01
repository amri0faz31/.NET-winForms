using System;

namespace samp_01
{
    // Centralized application configuration helper.
    // Reads connection string from environment variable `SAMP01_CONN` or falls back to a local default.
    public static class AppConfig
    {
        public static string ConnectionString =>
        Environment.GetEnvironmentVariable("SAMP01_CONN")
        ?? "Server=localhost;Database=desk_app;User=root;Password=root4427;";
    }
}
