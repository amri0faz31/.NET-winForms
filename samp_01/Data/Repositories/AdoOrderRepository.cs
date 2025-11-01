using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using samp_01.Domain.Entities;

namespace samp_01.Data.Repositories
{
    /// <summary>
    /// ADO.NET implementation of <see cref="IOrderRepository"/> for MySQL.
    /// </summary>
    /// <remarks>
    /// Used by:
    /// - Services/Orders/OrderService (CreateRequest, ApprovePrice, MarkDelivered, CompleteAndPay)
    /// - Forms/User/UserOrdersForm (GetByUser for buyer list)
    /// - Forms/ServiceProvider/ProviderOrdersForm (GetBySeller for seller list)
    /// </remarks>
    public class AdoOrderRepository : IOrderRepository
    {
        private readonly string _cs;
        public AdoOrderRepository(string cs) { _cs = cs; }

        /// <summary>
        /// Inserts a new order.
        /// </summary>
        /// <param name="o">Order entity to create.</param>
        /// <returns>Newly created order id or0 on failure.</returns>
        public int Create(Order o)
        {
            using var db = new AdoDbContext(_cs);
            var sql = @"INSERT INTO orders (userid, sellerid, serviceid, title, requirements, totalprice, status, createdat, updatedat)
                        VALUES (@uid,@sid,@svc,@title,@req,@price,@status,@now,@now)";
            var rows = db.ExecuteNonQuery(sql,
                new MySqlParameter("@uid", o.UserId),
                new MySqlParameter("@sid", o.SellerId),
                new MySqlParameter("@svc", o.ServiceId),
                new MySqlParameter("@title", o.Title),
                new MySqlParameter("@req", (object?)o.Requirements ?? DBNull.Value),
                new MySqlParameter("@price", o.TotalPrice),
                new MySqlParameter("@status", o.Status),
                new MySqlParameter("@now", DateTime.UtcNow));
            if (rows > 0)
            {
                using var db2 = new AdoDbContext(_cs);
                var id = Convert.ToInt32(db2.ExecuteScalar("SELECT LAST_INSERT_ID();"));
                return id;
            }
            return 0;
        }

        /// <summary>
        /// Gets a single order by id.
        /// </summary>
        /// <param name="id">Order id.</param>
        /// <returns>Order or null.</returns>
        public Order? GetById(int id)
        {
            using var db = new AdoDbContext(_cs);
            var sql = "SELECT id, userid, sellerid, serviceid, title, requirements, totalprice, status, createdat, updatedat FROM orders WHERE id=@id LIMIT 1";
            var list = db.Query(sql, r => new Order
            {
                Id = r.GetInt32(0),
                UserId = r.GetInt32(1),
                SellerId = r.GetInt32(2),
                ServiceId = r.GetInt32(3),
                Title = r.GetString(4),
                Requirements = r.IsDBNull(5) ? null : r.GetString(5),
                TotalPrice = r.GetDecimal(6),
                Status = r.GetString(7),
                CreatedAt = r.GetDateTime(8),
                UpdatedAt = r.GetDateTime(9)
            }, new MySqlParameter("@id", id));
            return list.Count > 0 ? list[0] : null;
        }

        /// <summary>
        /// Lists orders for a given user (buyer) ordered by last update desc.
        /// </summary>
        public List<Order> GetByUser(int userId)
        {
            using var db = new AdoDbContext(_cs);
            var sql = "SELECT id, userid, sellerid, serviceid, title, requirements, totalprice, status, createdat, updatedat FROM orders WHERE userid=@uid ORDER BY updatedat DESC";
            return db.Query(sql, r => new Order
            {
                Id = r.GetInt32(0),
                UserId = r.GetInt32(1),
                SellerId = r.GetInt32(2),
                ServiceId = r.GetInt32(3),
                Title = r.GetString(4),
                Requirements = r.IsDBNull(5) ? null : r.GetString(5),
                TotalPrice = r.GetDecimal(6),
                Status = r.GetString(7),
                CreatedAt = r.GetDateTime(8),
                UpdatedAt = r.GetDateTime(9)
            }, new MySqlParameter("@uid", userId));
        }      

        /// <summary>
        /// Lists orders for a given seller (provider) ordered by last update desc.
        /// </summary>                             
        public List<Order> GetBySeller(int sellerId)
        {
            using var db = new AdoDbContext(_cs);
            var sql = "SELECT id, userid, sellerid, serviceid, title, requirements, totalprice, status, createdat, updatedat FROM orders WHERE sellerid=@sid ORDER BY updatedat DESC";
            return db.Query(sql, r => new Order
            {
                Id = r.GetInt32(0),
                UserId = r.GetInt32(1),
                SellerId = r.GetInt32(2),
                ServiceId = r.GetInt32(3),
                Title = r.GetString(4),
                Requirements = r.IsDBNull(5) ? null : r.GetString(5),
                TotalPrice = r.GetDecimal(6),
                Status = r.GetString(7),
                CreatedAt = r.GetDateTime(8),
                UpdatedAt = r.GetDateTime(9)
            }, new MySqlParameter("@sid", sellerId));
        }

        /// <summary>
        /// Updates order status and optionally total price. Also updates the timestamp.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="status">New status.</param>
        /// <param name="totalPrice">Optional price.</param>
        /// <returns>True if one or more rows affected.</returns>
        public bool UpdateStatus(int orderId, string status, decimal? totalPrice = null)
        {
            using var db = new AdoDbContext(_cs);
            var sql = totalPrice.HasValue
                ? "UPDATE orders SET status=@st, totalprice=@pr, updatedat=@now WHERE id=@id"
                : "UPDATE orders SET status=@st, updatedat=@now WHERE id=@id";
            var rows = db.ExecuteNonQuery(sql,
                new MySqlParameter("@st", status),
                new MySqlParameter("@pr", (object?)totalPrice ?? DBNull.Value),
                new MySqlParameter("@now", DateTime.UtcNow),
                new MySqlParameter("@id", orderId));
            return rows > 0;
        }
    }
}
