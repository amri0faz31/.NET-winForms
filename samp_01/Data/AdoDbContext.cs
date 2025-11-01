using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace samp_01.Data
{
    public class AdoDbContext : IDisposable
    {
        private readonly string _connectionString;
        public AdoDbContext(string connectionString) => _connectionString = connectionString;

        public MySqlConnection CreateConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        public int ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            using var conn = CreateConnection();
            using var cmd = new MySqlCommand(sql, conn);
            if (parameters != null && parameters.Length > 0) cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery();
        }

        public object? ExecuteScalar(string sql, params MySqlParameter[] parameters)
        {
            using var conn = CreateConnection();
            using var cmd = new MySqlCommand(sql, conn);
            if (parameters != null && parameters.Length > 0) cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteScalar();
        }

        public List<T> Query<T>(string sql, Func<IDataRecord, T> map, params MySqlParameter[] parameters)
        {
            var results = new List<T>();
            using var conn = CreateConnection();
            using var cmd = new MySqlCommand(sql, conn);
            if (parameters != null && parameters.Length > 0) cmd.Parameters.AddRange(parameters);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read()) results.Add(map(rdr));
            return results;
        }

        public void Dispose() { }
    }
}
