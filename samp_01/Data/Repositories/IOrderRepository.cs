using System.Collections.Generic;
using samp_01.Domain.Entities;

namespace samp_01.Data.Repositories
{
 
 // Repository abstraction for CRUD operations on <see cref="Order"/> entities.
 
 
 // Implemented by <c>AdoOrderRepository</c>. Used by <c>OrderService</c>, user and provider order UIs.
     public interface IOrderRepository
     {

        // Creates a new order and returns its identifier.
        int Create(Order order);



     // Gets an order by id.
 
        Order? GetById(int id);
     // Lists orders created by the given user (buyer).
        List<Order> GetByUser(int userId);
     // Lists orders assigned to the given seller (provider).
         List<Order> GetBySeller(int sellerId);
     // Updates status and optionally total price.
        bool UpdateStatus(int orderId, string status, decimal? totalPrice = null);
     }
}
