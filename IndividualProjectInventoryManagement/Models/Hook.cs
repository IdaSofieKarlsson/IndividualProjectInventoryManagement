using IndividualProjectInventoryManagement.Models.Enums;
using IndividualProjectInventoryManagement.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndividualProjectInventoryManagement.Models
{
    // A Hook IS a Product (inheritance).
    // Size = diameter in mm, Length = length in cm.
    public class Hook : Product
    {
        public HookBrand Brand { get; }
        public HookName Name { get; }
        public HookMaterial Material { get; }

        public override string BrandText => Brand.ToDisplayText();
        public override string NameText => Name.ToDisplayText();
        public override string MaterialText => Material.ToDisplayText();

        // Hooks are bought less often than yarn
        public override int LowStockThreshold => 3;

        public Hook(
            HookBrand brand,
            HookName name,
            HookMaterial material,
            decimal size,
            decimal length,
            int quantity,
            decimal price)
            : base(ProductCategory.Hook, size, length, quantity, price)
        {
            if (!BrandCatalog.IsValidCombination(brand, name))
            {
                throw new ArgumentException($"{brand.ToDisplayText()} does not sell a hook called {name.ToDisplayText()}.");
            }

            Brand = brand;
            Name = name;
            Material = material;
        }
    }
}
