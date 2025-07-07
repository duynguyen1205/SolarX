namespace SolarX.REPOSITORY.Enum;

public enum InventoryTransactionType
{
    ImportForInventory = 0,
    ImportFromSupplier = 1,   // Nhập từ NCC
    ExportToBranch = 2,       // Xuất cho chi nhánh (B2B)
    SellToCustomer = 3,       // Bán cho khách (B2C)
    ReturnFromBranch = 4,     // Chi nhánh trả lại hàng
    Adjustment = 5            // Điều chỉnh tồn kho thủ công
}