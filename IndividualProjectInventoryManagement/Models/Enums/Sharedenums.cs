using System;
using System.Collections.Generic;
using System.Text;

namespace IndividualProjectInventoryManagement.Models.Enums
{
    // ------------------------------------------------------------------
    // Enums used by ALL categories
    // ------------------------------------------------------------------

    // The four kinds of product in the shop. Menus and filters use this
    // enum to talk about "what kind of product".
    public enum ProductCategory
    {
        Yarn,
        Needle,
        Hook,
        OtherSupply
    }

    // The replacement for LifecycleStatus from Asset Tracking.
    // Red row    = OutOfStock
    // Yellow row = LowStock (quantity at or below the low-stock level)
    // Normal row = InStock
    public enum StockStatus
    {
        InStock,
        LowStock,
        OutOfStock
    }

    // What a list of products can be sorted by.
    // Status sorts in the order InStock, LowStock, OutOfStock, so sorting
    // DESCENDING by status puts the products that need attention first.
    public enum SortField
    {
        Id,
        Category,
        Brand,
        Name,
        Material,
        Size,
        Length,
        Quantity,
        Price,
        TotalValue,
        Status
    }
}
