using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IndividualProjectInventoryManagement.UI
{
    // ------------------------------------------------------------------
    // ConsoleInput - safe reading from the keyboard
    // ------------------------------------------------------------------
    // The user can type anything: letters instead of numbers, nothing at all,
    // a price of -5. Every method here keeps asking until the answer is
    // valid, so the rest of the program never has to deal with bad input.
    // This class (like Display) is the ONLY place that calls Console.ReadLine.
    //
    // Methods that can be cancelled return a nullable value ("int?"):
    // null means "the user pressed Enter without typing anything".
    // ------------------------------------------------------------------
    public static class ConsoleInput
    {
        public static string ReadText(string prompt)
        {
            Console.Write($"{prompt}: ");

            // ReadLine returns null if the input stream ends; treat that as ""
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        // A whole number between min and max. Enter = cancel (null).
        public static int? ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                string text = ReadText($"{prompt} [{FormatLimit(min)} to {FormatLimit(max)}, Enter = cancel]");

                if (text.Length == 0)
                {
                    return null;
                }

                // TryParse returns false instead of crashing on "abc".
                // InvariantCulture = the same rules on every computer.
                if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                    && value >= min && value <= max)
                {
                    return value;
                }

                Display.Error($"Please enter a whole number between {FormatLimit(min)} and {FormatLimit(max)}.");
            }
        }

        // A decimal number between min and max. Both 12.5 and 12,5 are accepted.
        public static decimal? ReadDecimal(string prompt, decimal min, decimal max)
        {
            while (true)
            {
                string text = ReadText($"{prompt} [{min} to {max}, Enter = cancel]");

                if (text.Length == 0)
                {
                    return null;
                }

                // Swedish keyboards type a comma. Turn it into a dot first.
                text = text.Replace(',', '.');

                if (decimal.TryParse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                        CultureInfo.InvariantCulture, out decimal value)
                    && value >= min && value <= max)
                {
                    return value;
                }

                Display.Error($"Please enter a number between {min} and {max}.");
            }
        }

        // A menu choice from 0 to max. Keeps asking until it gets one.
        public static int ReadChoice(int max)
        {
            while (true)
            {
                string text = ReadText($"Choice [0-{max}]");

                if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                    && value >= 0 && value <= max)
                {
                    return value;
                }

                Display.Error($"Please choose a number from 0 to {max}.");
            }
        }

        public static bool Confirm(string question)
        {
            while (true)
            {
                string answer = ReadText($"{question} (y/n)").ToLowerInvariant();

                // "is" with "or" = matches any of these values
                if (answer is "y" or "yes")
                {
                    return true;
                }

                if (answer is "n" or "no")
                {
                    return false;
                }

                Display.Error("Please answer y or n.");
            }
        }

        // Shows a numbered list and lets the user pick one item.
        // Returns false if the user cancels (0).
        //
        // GENERICS: <T> means "any type". The same method picks a brand
        // (an enum), a size (a decimal) or a sort field. "label" is a
        // function that turns one item into the text to show for it.
        // "out" is a second way of returning a value: the chosen item.
        public static bool TryPick<T>(string title, IReadOnlyList<T> options, Func<T, string> label, out T choice)
        {
            Console.WriteLine();
            Console.WriteLine($"{title}:");

            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"  {i + 1,2}. {label(options[i])}");
            }

            Console.WriteLine("   0. Cancel");

            int number = ReadChoice(options.Count);

            if (number == 0)
            {
                choice = default!;
                return false;
            }

            choice = options[number - 1];
            return true;
        }

        public static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press Enter to continue...");
            Console.ReadLine();
        }

        // Shows "no limit" instead of 2147483647
        private static string FormatLimit(int limit)
            => Math.Abs(limit) >= 100000 ? "any" : limit.ToString(CultureInfo.InvariantCulture);
    }
}
