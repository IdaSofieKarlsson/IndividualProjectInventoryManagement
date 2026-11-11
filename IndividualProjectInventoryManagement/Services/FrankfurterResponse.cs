namespace IndividualProjectInventoryManagement.Services
{
    // ------------------------------------------------------------------
    // WHAT IS THIS CLASS?
    // ------------------------------------------------------------------
    // This class describes the SHAPE of the JSON that the Frankfurter API
    // sends back. It is a so-called DTO ("Data Transfer Object"): a class
    // that only holds data and has no logic.
    //
    // The API returns JSON like this:
    //
    //   {
    //     "amount": 1.0,
    //     "base":   "SEK",
    //     "date":   "2026-11-05",
    //     "rates":  { "EUR": 0.091, "NOK": 1.06 }
    //   }
    //
    // C# then copies each JSON value into the property with the same name.
    // This is called DESERIALISATION (JSON text -> C# object).
    //
    // Compared to JavaScript: in JS, res.json() gives you an object with
    // whatever is in the JSON. C# is strictly typed, so we must describe
    // the shape in advance - very similar to a TypeScript interface.
    //
    // Name matching: the JSON uses "rates" (lower case) and C# uses
    // "Rates" (PascalCase). GetFromJsonAsync ignores upper/lower case,
    // so they still match automatically.
    // ------------------------------------------------------------------
    public class FrankfurterResponse
    {
        // "amount": how many units of the base currency the rates are for (always 1 here)
        public decimal Amount { get; set; }

        // "base": the currency we convert FROM (SEK)
        // "= string.Empty" gives a default value so it is never null
        public string Base { get; set; } = string.Empty;

        // "date": which day the ECB published these rates.
        // The JSON text "2026-11-05" is converted to a DateTime automatically.
        public DateTime Date { get; set; }

        // "rates": a JSON object whose keys are currency codes.
        // A JSON object with unknown keys maps nicely to a Dictionary:
        //   key   = currency code as text, e.g. "EUR"
        //   value = how much 1 SEK is worth in that currency, e.g. 0.091
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    }
}