using Microsoft.EntityFrameworkCore;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.REPOSITORY.Enum;
using SolarX.SERVICE.Abstractions.IOrderServices;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.OrderServices;

public class OrderServices : IOrderServices
{
    private readonly IBaseRepository<Order, Guid> _orderRepository;
    private readonly IBaseRepository<AgencyWallet, Guid> _agencyWalletRepository;
    private readonly IBaseRepository<OrderItem, Guid> _orderItemRepository;
    private readonly IBaseRepository<Inventory, Guid> _inventoryRepository;
    private readonly IBaseRepository<InventoryTransaction, Guid> _inventoryTransactionRepository;
    private readonly IBaseRepository<Payment, Guid> _paymentRepository;
    private readonly IBaseRepository<WalletTransaction, Guid> _walletTransactionRepository;
    private readonly IBaseRepository<Product, Guid> _productRepository;
    private readonly IBaseRepository<Agency, Guid> _agencyRepository;
    private readonly IBaseRepository<Customer, Guid> _customerRepository;

    public OrderServices(IBaseRepository<Order, Guid> orderRepository,
        IBaseRepository<Inventory, Guid> inventoryRepository,
        IBaseRepository<Payment, Guid> paymentRepository, IBaseRepository<Product, Guid> productRepository,
        IBaseRepository<OrderItem, Guid> orderItemRepository, IBaseRepository<Agency, Guid> agencyRepository,
        IBaseRepository<AgencyWallet, Guid> agencyWalletRepository,
        IBaseRepository<InventoryTransaction, Guid> inventoryTransactionRepository, IBaseRepository<Customer, Guid> customerRepository,
        IBaseRepository<WalletTransaction, Guid> walletTransactionRepository)
    {
        _orderRepository = orderRepository;
        _inventoryRepository = inventoryRepository;
        _paymentRepository = paymentRepository;
        _productRepository = productRepository;
        _orderItemRepository = orderItemRepository;
        _agencyRepository = agencyRepository;
        _agencyWalletRepository = agencyWalletRepository;
        _inventoryTransactionRepository = inventoryTransactionRepository;
        _customerRepository = customerRepository;
        _walletTransactionRepository = walletTransactionRepository;
    }

    public async Task<Result> CreateB2BOrder(Guid sellerAgencyId, Guid buyerAgencyId, RequestModel.CreateOrderReq request)
    {
        var orderId = Guid.NewGuid();
        var orderCode = $"B2B-{DateTime.UtcNow:yyyyMMdd}-{orderId.ToString()[..8].ToUpper()}";

        var productIds = request.OrderItems.Select(i => i.ProductId).Distinct().ToList();
        var products = await _productRepository.GetAllWithQuery(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);
        if (products.Count != request.OrderItems.Count)
            return Result.CreateResult("Some products not found", 400);

        var sellerAgency = await _agencyRepository.GetAllWithQuery(a => a.Id == sellerAgencyId && !a.IsDeleted).FirstOrDefaultAsync();
        if (sellerAgency == null)
            return Result.CreateResult("Seller agency not found", 400);

        var buyerAgency = await _agencyRepository.GetAllWithQuery(a => a.Id == buyerAgencyId && !a.IsDeleted)
            .Include(x => x.DefaultWallet)
            .FirstOrDefaultAsync();
        if (buyerAgency == null)
            return Result.CreateResult("Buyer agency not found", 400);

        var inventoryCheck = await ValidateInventoryAsync(sellerAgencyId, request.OrderItems);
        if (!inventoryCheck.Data)
            return Result.CreateResult(inventoryCheck.Message, 400);

        var (orderItems, totalAmount) = GenerateOrderItems(orderId, request.OrderItems, products);

        if (request.PaymentMethod == PaymentMethod.Credit)
        {
            var agencyWallet = buyerAgency.DefaultWallet!;
            if (agencyWallet.CreditLimit < totalAmount)
                return Result.CreateResult("Credit Limit", 400);

            agencyWallet.CurrentDebt += totalAmount;
            var newTransaction = new WalletTransaction
            {
                Id = Guid.NewGuid(),
                AgencyWalletId = agencyWallet.Id,
                Amount = totalAmount,
                Reason = $"Charge by credit for order with order code: {orderCode}"
            };
            _agencyWalletRepository.UpdateEntity(agencyWallet);
            _walletTransactionRepository.AddEntity(newTransaction);
        }

        var order = new Order
        {
            Id = orderId,
            SellerAgencyId = sellerAgencyId,
            BuyerAgencyId = buyerAgencyId,
            IsB2C = false,
            OrderCode = orderCode,
            TotalAmount = totalAmount,
            Status = PaymentStatus.Paid,
            DeliveryStatus = DeliveryStatus.Pending
        };

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Amount = totalAmount,
            PaidAt = DateTimeOffset.UtcNow,
            Method = request.PaymentMethod
        };

        var sellerResult = await UpdateInventoryAsync(sellerAgencyId, request.OrderItems, orderId, orderCode);
        if (!sellerResult.Data)
            return Result.CreateResult(sellerResult.Message, 400);

        _orderRepository.AddEntity(order);
        _orderItemRepository.AddBulkAsync(orderItems);
        _paymentRepository.AddEntity(payment);

        return Result.CreateResult($"Order created successfully with ID: {orderCode}", 200);
    }

    private static (List<OrderItem> items, decimal totalAmount) GenerateOrderItems(Guid orderId,
        List<RequestModel.CreateOrderItemDto> inputItems, Dictionary<Guid, Product> productDict)
    {
        decimal total = 0;
        var result = inputItems.Select(i =>
        {
            var price = productDict[i.ProductId].BasePrice;
            total += price * i.Quantity;
            return new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = price
            };
        }).ToList();

        return (result, total);
    }


    public async Task<Result> CreateB2COrder(Guid agencyId, RequestModel.PublicOrderRequest request)
    {
        // 1. Tạo orderId mới
        var orderId = Guid.NewGuid();
        var orderCode = $"B2C-{DateTime.UtcNow:yyyyMMdd}-{orderId.ToString()[..8].ToUpper()}";

        // 2. Validate products exist
        var products = await _productRepository.GetAllWithQuery(p => request.Items.Select(i => i.ProductId).Contains(p.Id))
            .ToListAsync();
        var productDict = products.ToDictionary(p => p.Id);

        if (productDict.Count != request.Items.Count)
        {
            return Result.CreateResult("Some products not found", 400);
        }

        var agencyExisting = await _agencyRepository.GetAllWithQuery(a => a.Id == agencyId && !a.IsDeleted).FirstOrDefaultAsync();
        if (agencyExisting == null)
        {
            return Result.CreateResult("Agency not found ", 400);
        }

        float percent = 0;

        if (agencyExisting.DisplayWithMarkup)
        {
            percent = agencyExisting.MarkupPercent;
        }

        // 3. Kiểm tra số  tồn kho theo kho của agency
        var inventoryValidation = await ValidateInventoryAsync(agencyId, request.Items);
        if (!inventoryValidation.Data)
        {
            return Result.CreateResult(inventoryValidation.Message, 400);
        }

        var customer = await _customerRepository.GetAllWithQuery(x => x.Email == request.Email).FirstOrDefaultAsync();
        if (customer != null)
        {
            if (customer.IsDeleted)
            {
                customer.IsDeleted = false;
                _customerRepository.UpdateEntity(customer);
            }
        }
        else
        {
            customer = new Customer
            {
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                IsDeleted = false
            };
            _customerRepository.AddEntity(customer);
        }

        // 4. Tính toán tổng tiền
        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in request.Items)
        {
            var product = productDict[item.ProductId];
            var itemTotal = product.BasePrice * item.Quantity;
            totalAmount += itemTotal;

            orderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.BasePrice * (decimal)(1 + percent / 100)
            });
        }


        // 5. Tạo order với thanh toán mặc định là tiền mặt
        var order = new Order
        {
            Id = orderId,
            SellerAgencyId = agencyId,
            CustomerId = customer.Id,
            IsB2C = true,
            OrderCode = orderCode,
            OrderStatus = OrderStatus.Pending,
            DeliveryStatus = DeliveryStatus.Pending,
            TotalAmount = totalAmount
        };

        // 6. Tạo payment record
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Amount = totalAmount,
            PaidAt = DateTimeOffset.UtcNow,
            Method = PaymentMethod.Cash
        };

        // 7. Trừ tồn kho
        var inventoryUpdateResult =
            await UpdateInventoryAsync(agencyId, request.Items, order.Id, orderCode, isSeller: true, isB2C: true);
        if (!inventoryUpdateResult.Data)
        {
            return Result.CreateResult(inventoryUpdateResult.Message, 400);
        }

        _orderRepository.AddEntity(order);
        _orderItemRepository.AddBulkAsync(orderItems);
        _paymentRepository.AddEntity(payment);

        return Result.CreateResult($"Order created successfully with ID: {orderCode}", 200);

    }

    public async Task<Result<PagedResult<ResponseModel.OrderResponseModel>>> GetAllOrder(Guid agencyId, string? searchTerm,
        DateTimeOffset? dateOrder, OrderStatus? status, DeliveryStatus? deliveryStatus, bool seller,
        int pageIndex, int pageSize)
    {

        var query = seller
            ? _orderRepository.GetAllWithQuery(x => x.SellerAgencyId == agencyId)
            : _orderRepository.GetAllWithQuery(x => x.BuyerAgencyId == agencyId && !x.IsB2C);

        query = query.Include(x => x.Items);
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(x => x.OrderCode.Contains(searchTerm));
        }

        if (dateOrder != null)
        {
            query = query.Where(x => x.CreatedAt.Date == dateOrder.Value.Date);
        }

        if (status != null)
        {
            query = query.Where(x => x.OrderStatus == status);
        }

        if (deliveryStatus != null)
        {
            query = query.Where(x => x.DeliveryStatus == deliveryStatus);
        }

        var resultList = await PagedResult<Order>.CreateAsync(query, pageIndex, pageSize);
        var result = resultList.Items.Select(x => new ResponseModel.OrderResponseModel(
            x.Id,
            x.OrderCode,
            x.OrderStatus.ToString(),
            x.DeliveryStatus.ToString(),
            x.CreatedAt,
            x.TotalAmount
        )).ToList();

        var response =
            new PagedResult<ResponseModel.OrderResponseModel>(result, resultList.PageIndex, resultList.PageSize, resultList.TotalCount);
        return Result<PagedResult<ResponseModel.OrderResponseModel>>.CreateResult("Get All Order", 200, response);
    }

    public async Task<Result<List<ResponseModel.OrderItemResponseModel?>>> GetOrderDetail(Guid orderId)
    {
        var orderExisting = await _orderRepository.GetAllWithQuery(x => x.Id == orderId && !x.IsDeleted)
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync();

        if (orderExisting == null)
        {
            return Result<List<ResponseModel.OrderItemResponseModel?>>.CreateResult("Bad request", 400, null!);
        }

        var order = orderExisting.Items.Select(x => new ResponseModel.OrderItemResponseModel(
            x.ProductId,
            x.Product.Name,
            x.UnitPrice,
            x.Quantity
        )).ToList();

        return Result<List<ResponseModel.OrderItemResponseModel?>>.CreateResult("Get order detail", 200, order!);
    }

    public async Task<Result> UpdateOrderStatus(Guid orderId, RequestModel.UpdateOrderStatusReq request)
    {
        var order = await _orderRepository.GetAllWithQuery(x => x.Id == orderId && !x.IsDeleted)
            .Include(x => x.Items)
            .Include(x => x.Payment)
            .Include(x => x.BuyerAgency)
            .ThenInclude(x => x.DefaultWallet)
            .FirstOrDefaultAsync();

        if (order == null)
            return Result.CreateResult("Bad request", 400);

        var items = order.Items.Select(i => new RequestModel.CreateOrderItemDto(
            i.ProductId,
            i.Quantity
        )).ToList();

        // Huỷ đơn → hoàn kho
        if (request.DeliveryStatus == DeliveryStatus.Cancel || request.Status == OrderStatus.Canceled)
        {
            var agencyId = order.SellerAgencyId;

            var restoreResult = await UpdateInventoryAsync(agencyId, items, order.Id, order.OrderCode, isSeller: false, order.IsB2C);
            if (!restoreResult.Data)
                return Result.CreateResult(restoreResult.Message, 400);

            if (order is { IsB2C: false, Payment.Method: PaymentMethod.Credit })
            {
                var wallet = order.BuyerAgency.DefaultWallet;
                if (wallet != null)
                {
                    wallet.CurrentDebt = Math.Max(0, wallet.CurrentDebt - order.TotalAmount);
                    var newTransaction = new WalletTransaction
                    {
                        Id = Guid.NewGuid(),
                        AgencyWalletId = wallet.Id,
                        Amount = order.TotalAmount,
                        Reason = "Cancel order"
                    };
                    _agencyWalletRepository.UpdateEntity(wallet);
                    _walletTransactionRepository.AddEntity(newTransaction);
                }
            }

            order.OrderStatus = OrderStatus.Canceled;
            order.DeliveryStatus = DeliveryStatus.Cancel;
            _orderRepository.UpdateEntity(order);
            return Result.CreateResult("Update order status", 200);
        }

        if (!order.IsB2C && request.DeliveryStatus == DeliveryStatus.Delivered)
        {
            var buyerAgencyId = order.BuyerAgencyId!.Value;

            var receiveResult =
                await UpdateInventoryAsync(buyerAgencyId, items, order.Id, order.OrderCode, isSeller: false, isB2C: false);
            if (!receiveResult.Data)
                return Result.CreateResult(receiveResult.Message, 400);
            order.OrderStatus = OrderStatus.Completed;
        }

        if (request.Status != null)
            order.OrderStatus = request.Status.Value;

        if (request.DeliveryStatus != null)
            order.DeliveryStatus = request.DeliveryStatus.Value;

        _orderRepository.UpdateEntity(order);

        return Result.CreateResult("Update order status", 200);
    }


    private async Task<Result<bool>> ValidateInventoryAsync(Guid agencyId, List<RequestModel.CreateOrderItemDto> items)
    {
        var productIds = items.Select(i => i.ProductId).ToList();
        var inventories = await _inventoryRepository
            .GetAllWithQuery(i => i.AgencyId == agencyId && productIds.Contains(i.ProductId))
            .ToListAsync();

        var inventoryDict = inventories.ToDictionary(i => i.ProductId);

        foreach (var item in items)
        {
            if (!inventoryDict.TryGetValue(item.ProductId, out var inventory))
            {
                return Result<bool>.CreateResult($"Product {item.ProductId} not available in agency inventory", 400, false);
            }

            if (item.Quantity <= 0)
            {
                return Result<bool>.CreateResult($"Invalid quantity for product {item.ProductId}. Quantity must be greater than 0", 400,
                    false);
            }

            if (inventory.Quantity < 0)
            {
                return Result<bool>.CreateResult(
                    $"Invalid inventory quantity for product {item.ProductId}. Current inventory is negative", 400, false);
            }

            if (inventory.Quantity < item.Quantity)
            {
                return Result<bool>.CreateResult(
                    $"Insufficient inventory for product {item.ProductId}. Available: {inventory.Quantity}, Required: {item.Quantity}",
                    400, false);
            }
        }

        return Result<bool>.CreateResult("Inventory validation passed", 200, true);
    }

    private async Task<Result<bool>> UpdateInventoryAsync(Guid agencyId, List<RequestModel.CreateOrderItemDto> items, Guid orderId,
        string orderCode, bool isSeller = true, bool isB2C = true)
    {
        var productIds = items.Select(i => i.ProductId).Distinct().ToList();
        var inventories = await _inventoryRepository
            .GetAllWithQuery(i => i.AgencyId == agencyId && productIds.Contains(i.ProductId))
            .ToDictionaryAsync(i => i.ProductId);

        var transactions = new List<InventoryTransaction>();

        foreach (var item in items)
        {
            var exists = inventories.TryGetValue(item.ProductId, out var inventory);

            if (isSeller)
            {
                if (!exists || inventory!.Quantity < item.Quantity)
                    return Result<bool>.CreateResult($"Insufficient inventory for product {item.ProductId}", 400, false);

                inventory.Quantity -= item.Quantity;
                _inventoryRepository.UpdateEntity(inventory);
            }
            else
            {
                if (exists)
                {
                    inventory!.Quantity += item.Quantity;
                    _inventoryRepository.UpdateEntity(inventory);
                }
                else
                {
                    inventory = new Inventory
                    {
                        Id = Guid.NewGuid(),
                        AgencyId = agencyId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };
                    _inventoryRepository.AddEntity(inventory);
                }
            }

            var transactionType = isSeller
                ? isB2C ? InventoryTransactionType.SellToCustomer : InventoryTransactionType.ExportToBranch
                : InventoryTransactionType.ReturnFromBranch;

            var note = isSeller
                ? isB2C
                    ? $"B2C Order - Sold {item.Quantity} units"
                    : $"B2B Order - Exported {item.Quantity} units to branch"
                : $"Order {orderCode} - Restocked {item.Quantity} units due to cancel";

            transactions.Add(new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                AgencyId = agencyId,
                ProductId = item.ProductId,
                InventoryId = inventory.Id,
                OrderId = orderId,
                Type = transactionType,
                QuantityChanged = isSeller ? -item.Quantity : item.Quantity,
                Note = note
            });
        }

        if (transactions.Count > 0)
            _inventoryTransactionRepository.AddBulkAsync(transactions);

        return Result<bool>.CreateResult("Inventory updated successfully", 200, true);
    }

}