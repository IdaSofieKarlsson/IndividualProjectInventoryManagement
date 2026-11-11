using IndividualProjectInventoryManagement.Models;
using IndividualProjectInventoryManagement.Models.Enums;
using IndividualProjectInventoryManagement.Rules;
using IndividualProjectInventoryManagement.Services;
using IndividualProjectInventoryManagement.Data;
using System.Text;


Console.WriteLine("Inventory Management - Knappen's Yarn Store");

// TEMPORARY test program: runs every Inventory method and prints the result,
// so we can see that the in-memory part works before building the menu.
// It will be replaced by the real start-up code later.

// Makes the console show å, ä and ö correctly
Console.OutputEncoding = Encoding.UTF8;

Inventory inventory = new Inventory();

// ---------------------------------------------------------------
// 1. Fill the inventory. Each product receives an ID here.
// ---------------------------------------------------------------
foreach (Product product in SampleData.Create())
{
    inventory.AddProduct(product);
}

Console.WriteLine($"{inventory.Count} products added.");
Console.WriteLine();

// ---------------------------------------------------------------
// 2. Duplicates: the same article cannot be added twice
// ---------------------------------------------------------------
Console.WriteLine("DUPLICATES");

try
{
    // Exactly the same as product #1
    inventory.AddProduct(new Yarn(YarnBrand.Jarbo, YarnName.MorningMist, YarnMaterial.Merino, 50m, 200m, 5, 39m));
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  Rejected: {ex.Message}");
}

// The same yarn in a 100 g ball is a DIFFERENT product, so this is fine
Product added = inventory.AddProduct(new Yarn(YarnBrand.Jarbo, YarnName.MorningMist, YarnMaterial.Merino, 100m, 200m, 5, 69m));
Console.WriteLine($"  Added: {added}");
Console.WriteLine();

// ---------------------------------------------------------------
// 3. Change stock and price (all changes go through Inventory)
// ---------------------------------------------------------------
Console.WriteLine("STOCK AND PRICE");
Console.WriteLine($"  Delivery of 20 to #4: {inventory.AdjustStock(4, 20)}");
Console.WriteLine($"  Sold all of #16:      {inventory.AdjustStock(16, -9)}");
Console.WriteLine($"  New price for #1:     {inventory.SetPrice(1, 42m)}");
Console.WriteLine($"  Stock of #99:         {inventory.AdjustStock(99, 1)}   (no such product)");

try
{
    inventory.AdjustStock(1, -1000);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  Rejected: {ex.Message}");
}

Console.WriteLine();

// ---------------------------------------------------------------
// 4. View: filter, search and sort
// ---------------------------------------------------------------
PrintList("NEEDS ATTENTION (low stock or out of stock)",
    inventory.ViewProducts(new ProductFilter(NeedsAttentionOnly: true)));

PrintList("SEARCH \"jarbo\" (the products show Järbo)",
    inventory.ViewProducts(new ProductFilter(SearchText: "jarbo")));

PrintList("SEARCH \"bamboo 2.5\" (both words must match)",
    inventory.ViewProducts(new ProductFilter(SearchText: "bamboo 2.5")));

PrintList("YARN, MOST EXPENSIVE FIRST",
    inventory.ViewProducts(new ProductFilter(Category: ProductCategory.Yarn), SortField.Price, descending: true));

// ---------------------------------------------------------------
// 5. Update (replace) and delete
// ---------------------------------------------------------------
Console.WriteLine("UPDATE AND DELETE");

// Product #12 becomes a 2.5 mm hook. The size cannot be edited on the
// existing object, so we hand in a new one. It keeps the ID 12.
bool updated = inventory.UpdateProduct(12, new Hook(HookBrand.Clover, HookName.Petite, HookMaterial.Steel, 2.5m, 12m, 7, 49m));
Console.WriteLine($"  Updated #12: {updated}, size is now {inventory.FindById(12)?.SizeText}");

Console.WriteLine($"  Deleted #5:  {inventory.DeleteProduct(5)}");
Console.WriteLine($"  Deleted #99: {inventory.DeleteProduct(99)}   (no such product)");
Console.WriteLine($"  Products left: {inventory.Count}");
Console.WriteLine();

// ---------------------------------------------------------------
// 6. Report
// ---------------------------------------------------------------
Console.WriteLine(inventory.GenerateReport().ToText());

// Prints a heading and one line per product
static void PrintList(string title, List<Product> products)
{
    Console.WriteLine($"{title}: {products.Count} found");

    foreach (Product product in products)
    {
        Console.WriteLine($"  {product,-44} {product.Quantity,3} pcs  {product.Price,7:F2} {Product.BaseCurrency}");
    }

    Console.WriteLine();
}