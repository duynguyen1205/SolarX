namespace SolarX.REPOSITORY.Enum;

public enum InventoryTransactionType
{
    ImportFromSupplier = 0,   // Nhập từ NCC
    ExportToBranch = 1,       // Xuất cho chi nhánh (B2B)
    SellToCustomer = 2,       // Bán cho khách (B2C)
    ReturnFromBranch = 3,     // Chi nhánh trả lại hàng
    Adjustment = 4            // Điều chỉnh tồn kho thủ công
}