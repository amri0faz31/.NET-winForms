# Forms data-flow and repository mapping

This document explains how each UI in `Forms/*` fetches and updates data, tracing to the services and repositories that hit the database. It also highlights important tables (from migrations) touched in each flow.

Conventions in codebase
- Database access (MySQL) is abstracted by repositories using `AdoDbContext`.
- Services encapsulate business operations that compose repositories.
- Forms embed other forms as child views (TopLevel=false, Dock=Fill) inside dashboard containers.
- File attachments are saved under `data/` in the application base directory.

Key repositories and services
- Orders
 - `IOrderRepository` -> `AdoOrderRepository`: CRUD for `orders` table.
 - `OrderService`: wraps orders/messages/payments for higher-level flows.
- Messages
 - `IMessageRepository` -> `AdoMessageRepository`: CRUD for `messages`, including order chat and direct chat (orderless).
- Services (service catalog)
 - `IServiceRepository` -> `AdoServiceRepository`: reads `services` table.
- Sellers (service providers)
 - `ISellerRepository` -> `AdoSellerRepository`: reads/writes `sellers`; discovery helpers.
- Portfolios
 - `PortfolioService` -> `AdoPortfolioRepository`: `seller_portfolios` and `seller_projects`.
- Users
 - `IUserRepository` -> `AdoUserRepository`: reads/writes `users`.

Migrations relevant tables
- Users: `users` (20251025_InitialCreate_AddUsersTable)
- Services & Sellers: `services`, `sellers` (AddServicesAndSellers)
- Orders & Messaging & Payments: `orders`, `messages`, `payments` (AddOrdersMessagingPayments)
- Direct chat extensions: allow `messages.orderid` NULL, `messages.receiverid`/`receivertype`, indexes (AlterMessages_ForDirectChat)

-------------------------

User dashboard and navigation
- File: `Forms/User/DashboardForm.cs`
- Purpose: Shell container with sidebar; hosts embedded child forms in `_pnlMain`.
- Data usage: Only counts
 - Repos: `AdoOrderRepository` (`GetByUser`) and `AdoMessageRepository` (`GetByOrder`) to derive counts.
- Child forms launched:
 - `UserServicesForm` (services & sellers)
 - `UserOrdersForm` (orders, conversations)
 - `DirectMessagesForm` (direct chat)
 - `UserProfileForm` (if present)

User services browser
- File: `Forms/User/UserServicesForm.cs`
- Displays:
 - Services grid (cards) -> Providers under a service -> Provider portfolio (description, skills, price range, past projects)
 - New: Message button to jump into direct chat with provider
- Fetch:
 - Services: `AdoServiceRepository.GetAll()` -> `services`
 - Providers by service: `AdoSellerRepository.GetSellersByService(serviceId)` -> `sellers` + `seller_portfolios` (LEFT JOIN for summary)
 - Portfolio details: `PortfolioService.Get(sellerId)` -> `AdoPortfolioRepository.GetPortfolio` -> `seller_portfolios`
 - Past projects: `PortfolioService.ListProjects(sellerId)` -> `AdoPortfolioRepository.GetProjects` -> `seller_projects`
- Update:
 - Request order: `OrderService.CreateRequest(user.Id, sellerId, serviceId, title, requirements)`
 - Inserts into `orders` (`AdoOrderRepository.Create`)
 - May write system conversation entries later via `IMessageRepository` (service layer uses it for delivery/requirements flows)
- UX notes:
 - Lists use fill-docked `FlowLayoutPanel` with `AutoScroll=true` to fit inside dashboard main area.
 - Portfolio description uses a top-down stack; description above past projects, with Show more/less.

User orders
- File: `Forms/User/UserOrdersForm.cs`
- Displays: User’s orders list and per-order conversation (order chat)
- Fetch:
 - Orders list: `AdoOrderRepository.GetByUser(user.Id)` -> `orders`
 - Seller names for cards and bubbles: `AdoSellerRepository.GetSellerEntityById(order.SellerId)` -> `sellers`
 - Conversation (order chat): `IMessageRepository.GetByOrder(order.Id)` -> `messages` (orderid = specific id)
- Update:
 - Send chat message (order chat): `IMessageRepository.Add(new Message { OrderId = order.Id, SenderId = user.Id, SenderType = "User", ... })`
 - Inserts message row (order-scoped)
 - Pay & Complete: `OrderService.CompleteAndPay(order.Id, amount)`
 - Updates `payments` via `AdoPaymentRepository.UpdateStatus`
 - Marks order `Completed` via `IOrderRepository.UpdateStatus`
- Files:
 - User attaches files -> stored under `data/orders/{orderId}/attachments/`
- UX:
 - Chat bubbles show names (seller company or user name) rather than raw IDs.

User direct messages
- File: `Forms/User/DirectMessagesForm.cs`
- Displays: Split view with conversations + seller discovery + direct chat thread
- Fetch:
 - Conversations (distinct peers with last preview): `IMessageRepository.GetDirectConversations(meId, "User")` -> `messages` (orderid IS NULL)
 - Thread: `IMessageRepository.GetDirect(meId, "User", sellerId, "Seller")` -> `messages` (orderid IS NULL)
 - Services for search filter: `AdoServiceRepository.GetAll()` -> `services`
 - Seller discovery by service: `AdoSellerRepository.GetSellersByService(serviceId)` -> `sellers`
 - Seller discovery by name: `AdoSellerRepository.SearchByCompanyName(nameLike)` -> `sellers`
 - Name resolution (labels):
 - Users: `AdoUserRepository.GetProfileById(id)` -> `users`
 - Sellers: `AdoSellerRepository.GetSellerProfileById(id)` -> `sellers`
- Update:
 - Send direct message: `IMessageRepository.Add(new Message { OrderId = null, SenderId = me.Id, SenderType = "User", ReceiverId = sellerId, ReceiverType = "Seller", ... })`
 - Delete chat (user-only action): `IMessageRepository.DeleteDirectThread(userId, sellerId)` (deletes messages where orderid IS NULL between that pair)
- UX:
 - Conversation list shows peer company/name and last message preview.
 - Thread shows names instead of IDs.
 - Search pane: filter by service and/or seller name; double-click to open chat.

Service provider dashboard and navigation
- File: `Forms/ServiceProvider/ServiceProviderDashboardForm.cs`
- Hosts:
 - `ServiceProviderPortfolioForm`
 - `ProviderOrdersForm`
 - `ProviderDirectMessagesForm`
 - `ServiceProviderProfileForm`

Provider portfolio management
- File: `Forms/ServiceProvider/ServiceProviderPortfolioForm.cs`
- Displays/Updates:
 - Portfolio: description, skills, price range, profile picture
 - Fetch: `PortfolioService.Get(seller.Id)` -> `AdoPortfolioRepository.GetPortfolio` -> `seller_portfolios`
 - Save: `PortfolioService.Save(...)` -> `AdoPortfolioRepository.UpsertPortfolio`
 - Projects list:
 - Fetch: `PortfolioService.ListProjects(seller.Id)` -> `AdoPortfolioRepository.GetProjects` -> `seller_projects`
 - Add project: `PortfolioService.AddProject(seller.Id, title, desc, imagePath)` -> `AdoPortfolioRepository.AddProject`
 - Delete project: `PortfolioService.DeleteProject(projectId, seller.Id)` -> `AdoPortfolioRepository.DeleteProject`

Provider orders
- File: `Forms/ServiceProvider/ProviderOrdersForm.cs`
- Displays: Provider’s incoming/active/completed orders and per-order conversation
- Fetch:
 - Orders list: `AdoOrderRepository.GetBySeller(seller.Id)` -> `orders`
 - Buyer name for cards/bubbles: `AdoUserRepository.GetProfileById(order.UserId)` -> `users`
 - Conversation (order chat): `IMessageRepository.GetByOrder(order.Id)` -> `messages`
- Update:
 - Set Price / Start: `OrderService.ApprovePrice(order.Id, price)` -> updates `orders` (status & total price)
 - Deliver work: `OrderService.MarkDelivered(order.Id, seller.Id, filePath, note)`
 - Saves file under `data/orders/{orderId}/deliveries/`
 - Posts system/delivery entry via `IMessageRepository.Add`
 - Send chat message (order chat): `IMessageRepository.Add(new Message { OrderId = order.Id, SenderId = seller.Id, SenderType = "Seller", ... })`

Provider direct messages
- File: `Forms/ServiceProvider/ProviderDirectMessagesForm.cs`
- Displays: Split view with conversation list and thread
- Fetch:
 - Conversations: `IMessageRepository.GetDirectConversations(meId, "Seller")`
 - Thread: `IMessageRepository.GetDirect(meId, "Seller", userId, "User")`
 - Name resolution: `AdoUserRepository.GetProfileById(id)` to show user names in list and bubbles
- Update:
 - Send direct message: `IMessageRepository.Add(new Message { OrderId = null, SenderId = seller.Id, SenderType = "Seller", ReceiverId = userId, ReceiverType = "User", ... })`

Admin (if applicable)
- File: `Forms/Admin/AdminDashboardForm.cs`
- Typical usage: manage users/sellers/services (not detailed here). Admin auth seeded by `AddAdmins_SeedRoot` migration; repository `AdoAdminRepository` and helper `PasswordHelper` are used by services.

-------------------------

End-to-end flows

1) User requests service from a provider
- UI: `UserServicesForm` -> Request button opens `RequestOrderDialog` -> `OrderService.CreateRequest`
- Writes: `orders` (new row)
- Later, order routing occurs in provider orders UI.

2) Order chat (user/provider)
- UI: `UserOrdersForm` or `ProviderOrdersForm` -> open conversation
- Fetch: `IMessageRepository.GetByOrder(order.Id)`
- Send: `IMessageRepository.Add({ OrderId = order.Id, SenderType = 'User'/'Seller' })`
- Files: stored in `data/orders/{orderId}/attachments|deliveries`

3) Direct chat (no order)
- UI: `DirectMessagesForm` (user) or `ProviderDirectMessagesForm` (seller)
- Fetch dialogs: `GetDirectConversations(meId, meType)`
- Fetch thread: `GetDirect(meId, meType, peerId, peerType)`
- Send: `Add({ OrderId = null, ... })`
- Delete (user): `DeleteDirectThread(userId, sellerId)`

4) Provider portfolio maintenance
- UI: `ServiceProviderPortfolioForm`
- Fetch: `PortfolioService.Get` + `ListProjects`
- Save: `PortfolioService.Save`; Add/Delete project: `AddProject`/`DeleteProject`

Notes on schema and idempotency
- Migrations use `CREATE TABLE IF NOT EXISTS` and conditional `ALTER`/index operations to be idempotent on MySQL.
- Direct messages rely on `messages` rows with `orderid IS NULL` and composite index `(sendertype, senderid, receivertype, receiverid)`.

-------------------------

References
- Forms
 - `Forms/User/DashboardForm.cs`
 - `Forms/User/UserServicesForm.cs`
 - `Forms/User/UserOrdersForm.cs`
 - `Forms/User/DirectMessagesForm.cs`
 - `Forms/ServiceProvider/ServiceProviderDashboardForm.cs`
 - `Forms/ServiceProvider/ServiceProviderPortfolioForm.cs`
 - `Forms/ServiceProvider/ProviderOrdersForm.cs`
 - `Forms/ServiceProvider/ProviderDirectMessagesForm.cs`
- Repositories / Services
 - `Data/Repositories/*.cs`
 - `Services/Orders/OrderService.cs`
 - `Services/ServiceProvider/PortfolioService.cs`
- Migrations
 - `Migrations/20251025_InitialCreate_AddUsersTable.cs`
 - `Migrations/20251025_AddServicesAndSellers.cs`
 - `Migrations/20251029_AddOrdersMessagingPayments.cs`
 - `Migrations/20251030_AlterMessages_ForDirectChat.cs`
