using System.Text;
using IndividualProjectInventoryManagement.Data;
using IndividualProjectInventoryManagement.Services;
using IndividualProjectInventoryManagement.UI;

// Makes the console show and accept å, ä and ö
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// Set to true when there is something the user should read before the menu
// clears the screen
bool needsPause = false;

// ---------------------------------------------------------------
// 1. Exchange rates (internet). Failing is fine: backup rates are used.
// ---------------------------------------------------------------
CurrencyConverter currencyConverter = new CurrencyConverter();

Console.WriteLine("Fetching exchange rates...");
await currencyConverter.LoadRatesAsync();

if (!currencyConverter.IsUsingLiveRates)
{
    Display.Warning($"Using approximate backup rates. {currencyConverter.ErrorMessage}");
    needsPause = true;
}

// ---------------------------------------------------------------
// 2. The inventory and its JSON file
// ---------------------------------------------------------------
// The data file lives next to the program (bin/Debug/net8.0 in Visual Studio)
string dataFile = Path.Combine(AppContext.BaseDirectory, "inventory.json");

// Inventory gets its storage from here. It does not know it is a JSON file.
Inventory inventory = new Inventory(new JsonProductRepository(dataFile));

try
{
    LoadOutcome outcome = inventory.Load();

    foreach (string warning in outcome.Warnings)
    {
        Display.Warning(warning);
        needsPause = true;
    }

    // No file yet = the very first start: begin with the sample shop
    if (!outcome.FileFound)
    {
        inventory.AddProducts(SampleData.Create());
        Display.Success("First start: the sample products were loaded and saved.");
        needsPause = true;
    }
}
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
{
    Display.Error($"Could not read or write the data file {dataFile}: {ex.Message}");
    return;
}

if (needsPause)
{
    ConsoleInput.Pause();
}

new ConsoleMenu(inventory, currencyConverter).Run();