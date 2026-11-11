namespace IndividualProjectInventoryManagement.Models.Enums
{
    // The currencies the shop can show prices in.
    // All prices are STORED in SEK (Product.BaseCurrency). Other currencies
    // are only for display, calculated from the current exchange rate.
    // To add a currency, add it here. The menu and the API request follow
    // automatically.
    public enum Currency
    {
        SEK,
        EUR,
        NOK,
        DKK,
        USD,
        GBP
    }
}