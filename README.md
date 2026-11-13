# Knappen's Yarn Store

A console application for managing the inventory of a small yarn and knitting shop. 

The shop sells four kinds of products: **yarn**, **knitting needles**, **crochet hooks** and **other supplies**. The program keeps track of what is in stock, saves everything to a JSON file, and can present the range to customers.

## Features

- **Browse products** with filtering (category, needs attention), text search and sorting (any column, ascending or descending)
- **Present products** as a customer-friendly list: `Brand - Name - Description - Price`
- **Add, update and delete** products (stock, purchase price, description or other details)
- **Inventory report** with totals, value per category, low stock and out of stock, optionally saved as a text file
- **Automatic saving** to a JSON file after every change, with a backup file
- **Display currency**: prices can be shown in SEK, EUR, NOK, DKK, USD or GBP, using live exchange rates from the Frankfurter API (backup rates if there is no internet)
- **Input validation**: the user mostly chooses from lists, and everything typed is checked

## Prices

Every product has two prices:

- **Purchase price**: what the shop pays (entered by the user, at least 1 SEK)
- **Selling price**: calculated as purchase price x 3, rounded down to a whole number

Example: purchase price 12.50 SEK gives selling price 37 SEK.

All prices are stored in SEK. Other currencies are only used when showing prices.

## How to run

1. Open `IndividualProjectInventoryManagement.slnx` in Visual Studio.
2. Press **F5** (or Ctrl+F5 to run without the debugger).
3. On the very first start the program creates `inventory.json` with 16 sample products. After that, the file is the data.

The data file and saved reports are in the program's output folder, for example `bin/Debug/net10.0/`. The path to the data file is shown in the main menu.

To start over with the sample products, close the program and delete `inventory.json` and `inventory.json.bak`.

## Menu

1. Browse products (filter, search, sort)
2. Present products
3. Show product details
4. Add product
5. Update product (stock, price, description, details)
6. Delete product
7. Inventory report
8. Change display currency
0. Exit

In every list, **0** (or Enter) cancels and goes back.

## Project structure

IndividualProjectInventoryManagement/
  Models/       What a product is
    Product.cs  (abstract), Yarn.cs, Needle.cs, Hook.cs, OtherSupply.cs
    Enums/      Brands, product names, materials, categories, stock status, currencies
  Rules/        What is allowed
    ProductRules.cs   allowed sizes and lengths
    BrandCatalog.cs   which product names belong to which brand
  Services/     What the program does
    Inventory.cs, InventoryReport.cs, ProductFilter.cs, CurrencyConverter.cs
  Data/         How products are stored
    IProductRepository.cs, JsonProductRepository.cs, ProductDto.cs, ProductMapper.cs, SampleData.cs
  UI/           How the user talks to the program
    ConsoleMenu.cs, ConsoleInput.cs, ProductBuilder.cs, TablePrinter.cs, Display.cs
  Program.cs    Starts everything
docs/
  class-diagram.drawio  UML class diagram

Each folder has one job (separation of concerns). The menu only asks `Inventory` to do things and never touches the product list directly. A `Product` knows nothing about the console or the file.

## Class diagram

The diagram source is `docs/class-diagram.drawio`

## Design notes

- **Inheritance and polymorphism**: `Product` is abstract. `Yarn`, `Needle`, `Hook` and `OtherSupply` inherit from it and add what is special about them. `Inventory` holds one `List<Product>` and treats all products alike, for example `GetStockStatus()` automatically uses the right low-stock limit for each kind of product.
- **Brand and name**: each category has its own enums, so a hook cannot be given a yarn brand. Each product name is tagged with its brand (`[Brand(YarnBrand.Drops)] Safran`), and `BrandCatalog` builds the menus from those tags. Changing a brand's range is done in **one place**: the enum file.
- **Sizes and lengths**: only fixed values are allowed. They are kept in one place, `ProductRules.cs`, and shown as pick lists.
- **Yarn** has only a brand and a name.
- **Storage** goes through an interface, `IProductRepository`. `Inventory` does not know the data is stored in JSON. Saving writes a temporary file first and then swaps it in, and the previous version is kept as `inventory.json.bak`.
- **Errors**: a broken rule (duplicate product, removing more stock than exists, invalid value) throws an exception that the menu catches and shows as a red message. An unknown ID is not an error, it just returns `false` or `null`.

## Manual test checklist

Delete `inventory.json` first, so you start with the sample products.

- [ ] First start shows "sample products were loaded"; second start does not
- [ ] Browse: filter by category, toggle "needs attention", search ("jarbo", "bamboo 2.5"), sort and reverse
- [ ] Out of stock rows are red, low stock rows are yellow
- [ ] Present products: one line per product, grouped by category
- [ ] Show details with a valid and an unknown ID
- [ ] Add a yarn: only that brand's names are listed; the selling price is shown after the purchase price
- [ ] Add the same product again: refused with a clear message
- [ ] Invalid input is refused and asked again (letters instead of numbers, price 0, description over 80 characters)
- [ ] Cancel (0 or Enter) at any step adds nothing
- [ ] Update: add stock, remove too much (refused), set exact stock, change purchase price, change description, change other details (ID stays)
- [ ] Delete: answer n keeps the product, y deletes it
- [ ] Close and start again: all changes are still there
- [ ] Report shows totals, value per category, low stock and out of stock; saving creates a text file
- [ ] Change currency: tables, details, present list and report use it
- [ ] Without internet: a warning about backup rates, and the program still works

## Limitations

- The program is single-user and keeps no history of sales or deliveries.
- Exchange rates are fetched once at start-up.
- Needle types and lengths are not checked against each other.
- Yarn has only a brand and a name, colors are not specified at this first version.
