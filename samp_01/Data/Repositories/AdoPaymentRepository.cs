using System;
using MySql.Data.MySqlClient;
using samp_01.Domain.Entities;

namespace samp_01.Data.Repositories
{
    /// <summary>
    /// ADO.NET implementation of <see cref="IPaymentRepository"/> for MySQL.
    /// </summary>
    public class AdoPaymentRepository : IPaymentRepository
    {
        private readonly string _cs;
        public AdoPaymentRepository(string cs) { _cs = cs; }

        /// <summary>
        /// Inserts a payment row; returns generated id.
        /// </summary>
        public int Add(Payment p)
        {
            using var db = new AdoDbContext(_cs);
            var sql = @"INSERT INTO payments (orderid, amount, status, paidat)
                        VALUES (@oid,@amt,@status,@paid)";
            var rows = db.ExecuteNonQuery(sql,
                new MySqlParameter("@oid", p.OrderId),
                new MySqlParameter("@amt", p.Amount),
                new MySqlParameter("@status", p.Status),
                new MySqlParameter("@paid", (object?)p.PaidAt ?? DBNull.Value));
            if (rows > 0)
            {
                using var db2 = new AdoDbContext(_cs);
                return Convert.ToInt32(db2.ExecuteScalar("SELECT LAST_INSERT_ID();"));
            }
            return 0;
        }

        /// <summary>
        /// Gets a payment row for an order (if any).
        /// </summary>
        public Payment? GetByOrder(int orderId)
        {
            using var db = new AdoDbContext(_cs);
            var sql = "SELECT id, orderid, amount, status, paidat FROM payments WHERE orderid=@oid LIMIT 1";
            var list = db.Query(sql, r => new Payment
            {
                Id = r.GetInt32(0),
                OrderId = r.GetInt32(1),
                Amount = r.GetDecimal(2),
                Status = r.GetString(3),
                PaidAt = r.IsDBNull(4) ? (DateTime?)null : r.GetDateTime(4)
            }, new MySqlParameter("@oid", orderId));
            return list.Count > 0 ? list[0] : null;
        }

        /// <summary>
        /// Updates payment status and optionally sets amount and paid timestamp.
        /// </summary>
        public bool UpdateStatus(int orderId, string status, decimal? amount = null)
        {
            using var db = new AdoDbContext(_cs);
            var sql = amount.HasValue
                ? "UPDATE payments SET status=@st, amount=@amt, paidat=@paid WHERE orderid=@oid"
                : "UPDATE payments SET status=@st WHERE orderid=@oid";
            var rows = db.ExecuteNonQuery(sql,
                new MySqlParameter("@st", status),
                new MySqlParameter("@amt", (object?)amount ?? DBNull.Value),
                new MySqlParameter("@paid", status == "Paid" ? DateTime.UtcNow : (object?)DBNull.Value),
                new MySqlParameter("@oid", orderId));
            return rows > 0;
        }
    }
}
