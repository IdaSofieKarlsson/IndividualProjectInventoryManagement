using IndividualProjectInventoryManagement.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndividualProjectInventoryManagement.Services
{
    // ------------------------------------------------------------------
    // ProductFilter - what the user wants to SEE
    // ------------------------------------------------------------------
    // A small record that carries the choices from the menu to Inventory:
    //     new ProductFilter(Category: ProductCategory.Yarn)
    //     new ProductFilter(NeedsAttentionOnly: true)
    //     new ProductFilter(SearchText: "merino")
    //
    // Every parameter has a DEFAULT value, so you only write the ones you
    // need, and "new ProductFilter()" means "no filter, show everything".
    // The "?" after a type (ProductCategory?, string?) means "may be null",
    // and null here means "do not filter on this".
    // ------------------------------------------------------------------
    public record ProductFilter(
        ProductCategory? Category = null,     // only this category
        bool NeedsAttentionOnly = false,      // only low stock and out of stock
        string? SearchText = null);           // only products matching this text
}
