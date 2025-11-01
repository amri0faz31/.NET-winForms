using System;
using samp_01.Data.Repositories;
using samp_01.Domain.Entities;

namespace samp_01.Services.Orders
{
    public class OrderService
    {
        private readonly IOrderRepository _orders;
        private readonly IMessageRepository _messages;
        private readonly IPaymentRepository _payments;

        public OrderService() : this(
            new AdoOrderRepository(AppConfig.ConnectionString),
            new AdoMessageRepository(AppConfig.ConnectionString),
            new AdoPaymentRepository(AppConfig.ConnectionString)) { }

        public OrderService(IOrderRepository orders, IMessageRepository messages, IPaymentRepository payments)
        { _orders = orders; _messages = messages; _payments = payments; }

        public int CreateRequest(int userId, int sellerId, int serviceId, string title, string? requirements)
        {
            var order = new Order
            {
                UserId = userId,
                SellerId = sellerId,
                ServiceId = serviceId,
                Title = title,
                Requirements = requirements,
                TotalPrice = 0m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var id = _orders.Create(order);

            if (id > 0 && !string.IsNullOrWhiteSpace(requirements))
            {
                _messages.Add(new samp_01.Domain.Entities.Message
                {
                    OrderId = id,
                    SenderId = userId,
                    SenderType = "User",
                    Body = requirements,
                    SentAt = DateTime.UtcNow
                });
                _orders.UpdateStatus(id, "In_Conversation");
            }
            return id;
        }

        public bool ApprovePrice(int orderId, decimal price)
        {
            return _orders.UpdateStatus(orderId, "In_Progress", price);
        }

        public void MarkDelivered(int orderId, int sellerId, string? filePath, string? note)
        {
            _messages.Add(new samp_01.Domain.Entities.Message
            {
                OrderId = orderId,
                SenderId = sellerId,
                SenderType = "Seller",
                FilePath = filePath,
                Body = note,
                SentAt = DateTime.UtcNow
            });
            _orders.UpdateStatus(orderId, "Delivered");
        }

        // Upsert payment: insert if missing, else update to Paid; then complete order.
        public bool CompleteAndPay(int orderId, decimal amount)
        {
            var existing = _payments.GetByOrder(orderId);
            bool paid;
            if (existing == null)
            {
                var id = _payments.Add(new Payment
                {
                    OrderId = orderId,
                    Amount = amount,
                    Status = "Paid",
                    PaidAt = DateTime.UtcNow
                });
                paid = id > 0;
            }
            else
            {
                paid = _payments.UpdateStatus(orderId, "Paid", amount);
            }

            if (paid)
            {
                _orders.UpdateStatus(orderId, "Completed");
                return true;
            }
            return false;
        }
    }
}
