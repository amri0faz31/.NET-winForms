using System.Collections.Generic;
using samp_01.Domain.Entities;

namespace samp_01.Data.Repositories
{
    public interface IPaymentRepository
    {
        int Add(Payment p);
        Payment? GetByOrder(int orderId);
        bool UpdateStatus(int orderId, string status, decimal? amount = null);
    }
}
