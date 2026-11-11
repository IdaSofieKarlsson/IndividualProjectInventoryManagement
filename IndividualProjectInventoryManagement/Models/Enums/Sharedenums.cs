using System;
using System.Collections.Generic;
using System.Text;

namespace IndividualProjectInventoryManagement.Models.Enums
{
    // ------------------------------------------------------------------
    // Enums used by ALL categories
    // ------------------------------------------------------------------

    // The four kinds of product in the shop. The abstract Product class will
    // have an abstract Category property (like AssetType in Asset Tracking),
    // and menus and filters use this enum to talk about "what kind of product".
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
}
