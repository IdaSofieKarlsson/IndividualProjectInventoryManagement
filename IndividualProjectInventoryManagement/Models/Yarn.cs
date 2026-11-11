using IndividualProjectInventoryManagement.Models.Enums;
using IndividualProjectInventoryManagement.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndividualProjectInventoryManagement.Models
{
    // A Yarn IS a Product (inheritance), so it gets all the Product code.
    // Size = ball weight in grams, Length = metres of yarn per ball.
    public class Yarn : Product
    {
        // The TYPED values: a Yarn can only have a YarnBrand, a YarnName
        // and a YarnMaterial. The compiler enforces it.
        public YarnBrand Brand { get; }
        public YarnName Name { get; }
        public YarnMaterial Material { get; }

        // "override" fills in the abstract members from Product. Here the
        // typed enum values are turned into display text ("Jarbo" -> "Järbo").
        public override string BrandText => Brand.ToDisplayText();
        public override string NameText => Name.ToDisplayText();
        public override string MaterialText => Material.ToDisplayText();

        // Yarn sells fast, so it warns earlier than the default of 5
        public override int LowStockThreshold => 10;

        public Yarn(
            YarnBrand brand,
            YarnName name,
            YarnMaterial material,
            decimal size,
            decimal length,
            int quantity,
            decimal price)
            // ": base(...)" runs the Product constructor first. It checks
            // size, length, quantity and price.
            : base(ProductCategory.Yarn, size, length, quantity, price)
        {
            // Then the check only a Yarn can do: does this brand sell this yarn?
            if (!BrandCatalog.IsValidCombination(brand, name))
            {
                throw new ArgumentException($"{brand.ToDisplayText()} does not sell a yarn called {name.ToDisplayText()}.");
            }

            Brand = brand;
            Name = name;
            Material = material;
        }
    }
}
