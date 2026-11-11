using IndividualProjectInventoryManagement.Data;
using IndividualProjectInventoryManagement.Models;
using IndividualProjectInventoryManagement.Models.Enums;
using IndividualProjectInventoryManagement.Rules;
using IndividualProjectInventoryManagement.Services;
using IndividualProjectInventoryManagement.UI;
using System.Text;


Console.WriteLine("Inventory Management - Knappen's Yarn Store");

// Makes the console show and accept å, ä and ö
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

Inventory inventory = new Inventory();

// For now the shop starts with the sample data every time.
// When JSON storage is added, this is replaced by "load the saved file".
foreach (Product product in SampleData.Create())
{
    inventory.AddProduct(product);
}

new ConsoleMenu(inventory).Run();