using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using IndividualProjectInventoryManagement.Models;
using IndividualProjectInventoryManagement.Models.Enums;

namespace IndividualProjectInventoryManagement.Services
{
    // ------------------------------------------------------------------
    // CurrencyConverter - shows SEK prices in other currencies
    // ------------------------------------------------------------------
    // Same idea as in the Asset Tracking project, with two changes:
    //   - the base currency is SEK (all prices are stored in SEK)
    //   - it is a normal class, not a static one. Program.cs creates ONE
    //     object and hands it to the menu, so the chosen display currency
    //     is simply part of that object (the property "Selected").
    //
    // The stored data never changes. Only what is SHOWN is converted.
    // Rates come from the Frankfurter API (European Central Bank). If the
    // API cannot be reached, approximate backup rates are used and the
    // program still works.
    // ------------------------------------------------------------------
    public class CurrencyConverter
    {
        private const string ApiBaseUrl = "https://api.frankfurter.app/latest";

        // ONE HttpClient for the whole program, with a 10 second timeout
        private static readonly HttpClient Client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        // How much 1 SEK is worth in each currency.
        // These are BACKUP values (approximate), replaced by live rates when
        // the API call succeeds.
        private Dictionary<Currency, decimal> ratesFromSek = new Dictionary<Currency, decimal>
        {
            { Currency.SEK, 1.00m },
            { Currency.EUR, 0.09m },
            { Currency.NOK, 1.06m },
            { Currency.DKK, 0.68m },
            { Currency.USD, 0.10m },
            { Currency.GBP, 0.08m }
        };

        // The currency the user has chosen to SEE prices in
        public Currency Selected { get; set; } = Currency.SEK;

        public bool IsUsingLiveRates { get; private set; }

        // The day the ECB published the rates (null = no live rates)
        public DateTime? RatesDate { get; private set; }

        // If the API call failed: why
        public string? ErrorMessage { get; private set; }

        // A short text for the screen
        public string RateInfo => IsUsingLiveRates
            ? $"ECB rates from {RatesDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}"
            : "approximate backup rates (no live rates)";

        // ==============================================================
        // LoadRatesAsync - fetch live rates (async/await, as before)
        // ==============================================================
        public async Task LoadRatesAsync()
        {
            // Every currency except SEK: "EUR,NOK,DKK,USD,GBP"
            List<Currency> targets = Enum.GetValues<Currency>()
                .Where(currency => currency != Currency.SEK)
                .ToList();

            string url = $"{ApiBaseUrl}?from={Product.BaseCurrency}&to={string.Join(",", targets)}";

            try
            {
                FrankfurterResponse? response = await Client.GetFromJsonAsync<FrankfurterResponse>(url);

                if (response is null)
                {
                    ErrorMessage = "The API returned no data.";
                    return;
                }

                // Build the new rates separately, and only swap them in if
                // ALL of them exist. Never half live and half backup.
                Dictionary<Currency, decimal> newRates = new Dictionary<Currency, decimal>
                {
                    { Currency.SEK, 1.00m }
                };

                foreach (Currency currency in targets)
                {
                    if (!response.Rates.TryGetValue(currency.ToString(), out decimal rate))
                    {
                        ErrorMessage = $"The API did not return a rate for {currency}.";
                        return;
                    }

                    newRates[currency] = rate;
                }

                ratesFromSek = newRates;
                RatesDate = response.Date;
                IsUsingLiveRates = true;
                ErrorMessage = null;
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Could not reach the currency API ({ex.Message}).";
            }
            catch (TaskCanceledException)
            {
                ErrorMessage = "The currency API did not answer in time.";
            }
            catch (JsonException)
            {
                ErrorMessage = "The currency API sent data in an unexpected format.";
            }
        }

        // ==============================================================
        // Conversion
        // ==============================================================
        public decimal GetRate(Currency currency) => ratesFromSek[currency];

        // An amount in SEK -> the same amount in the selected currency
        public decimal Convert(decimal amountSek) => Convert(amountSek, Selected);

        public decimal Convert(decimal amountSek, Currency currency)
            => Math.Round(amountSek * ratesFromSek[currency], 2);

        // Just the number: "3.58", or "39" for a whole number of SEK
        public string FormatNumber(decimal amountSek)
        {
            decimal value = Convert(amountSek);
            bool isWhole = value == Math.Truncate(value);

            // Whole kronor are shown without decimals. Other currencies always
            // get two decimals.
            string format = Selected == Currency.SEK && isWhole ? "F0" : "F2";
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        // "3.58 EUR" or "39 SEK"
        public string Format(decimal amountSek) => $"{FormatNumber(amountSek)} {Selected}";
    }
}