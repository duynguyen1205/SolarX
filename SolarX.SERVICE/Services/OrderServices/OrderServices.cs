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
    private readonly IBaseRepository<OrderItem, Guid> _orderItemRepository;
    private readonly IBaseRepository<Inventory, Guid> _inventoryRepository;
    private readonly IBaseRepository<Payment, Guid> _paymentRepository;
    private readonly IBaseRepository<Product, Guid> _productRepository;
    private readonly IBaseRepository<Agency, Guid> _agencyRepository;

    public OrderServices(IBaseRepository<Order, Guid> orderRepository,
        IBaseRepository<Inventory, Guid> inventoryRepository,
        IBaseRepository<Payment, Guid> paymentRepository, IBaseRepository<Product, Guid> productRepository,
        IBaseRepository<OrderItem, Guid> orderItemRepository, IBaseRepository<Agency, Guid> agencyRepository)
    {
        _orderRepository = orderRepository;
        _inventoryRepository = inventoryRepository;
        _paymentRepository = paymentRepository;
        _productRepository = productRepository;
        _orderItemRepository = orderItemRepository;
        _agencyRepository = agencyRepository;
    }

    public Task<Result> CreateOrder(RequestModel.CreateOrderReq request)
    {
        if (request.IsB2C)
        {
            //check 
        }

        throw new NotImplementedException();
    }

    public async Task<Result> CreateB2COrder(Guid agencyId, RequestModel.PublicOrderRequest request)
    {
        // 1. Tạo orderId mới
        var orderId = Guid.NewGuid();
        var orderCode = $"B2C-{DateTime.UtcNow:yyyyMMdd}-{orderId.ToString().Substring(0, 8).ToUpper()}";

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
            return Result.CreateResult("Agency not found", 400);
        }

        float percent = 0;

        if (agencyExisting.DisplayWithMarkup)
        {
            percent = agencyExisting.MarkupPercent;
        }

        // 3. Kiểm tra số lượng tồn kho theo kho của agency
        var inventoryValidation = await ValidateInventoryAsync(agencyId, request.Items);
        if (!inventoryValidation.Data)
        {
            return Result.CreateResult(inventoryValidation.Message, 400);
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
            IsB2C = true,
            OrderCode = orderCode,
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
        var inventoryUpdateResult = await UpdateInventoryAsync(agencyId, request.Items);
        if (!inventoryUpdateResult.Data)
        {
            return Result.CreateResult(inventoryUpdateResult.Message, 400);
        }


        _orderRepository.AddEntity(order);
        _orderItemRepository.AddBulkAsync(orderItems);
        _paymentRepository.AddEntity(payment);

        return Result.CreateResult($"Order created successfully with ID: {orderCode}", 200);

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

    private async Task<Result<bool>> UpdateInventoryAsync(Guid agencyId, List<RequestModel.CreateOrderItemDto> items)
    {
        var productIds = items.Select(i => i.ProductId).ToList();
        var inventories = await _inventoryRepository
            .GetAllWithQuery(i => i.AgencyId == agencyId && productIds.Contains(i.ProductId))
            .ToListAsync();

        var inventoryDict = inventories.ToDictionary(i => i.ProductId);

        foreach (var item in items)
        {
            if (!inventoryDict.TryGetValue(item.ProductId, out var inventory))
                continue;

            if (inventory.Quantity < item.Quantity)
            {
                return Result<bool>.CreateResult(
                    $"Insufficient inventory for product {item.ProductId}. Available: {inventory.Quantity}, Required: {item.Quantity}",
                    400, false);
            }

            inventory.Quantity -= item.Quantity;
            _inventoryRepository.UpdateEntity(inventory);
        }

        return Result<bool>.CreateResult("Inventory updated successfully", 200, true);
    }

}