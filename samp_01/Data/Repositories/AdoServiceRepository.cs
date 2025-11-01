using System.Collections.Generic;
using MySql.Data.MySqlClient;
using samp_01.Domain.Entities;

namespace samp_01.Data.Repositories
{
    public class AdoServiceRepository : IServiceRepository
    {
        private readonly string _connectionString;
        public AdoServiceRepository(string connectionString) => _connectionString = connectionString;

        public List<Service> GetAll()
        {
            using var db = new AdoDbContext(_connectionString);
            var sql = "SELECT id, name FROM services ORDER BY name";
            return db.Query(sql, r => new Service { Id = r.GetInt32(0), Name = r.GetString(1) });
        }
    }
}
