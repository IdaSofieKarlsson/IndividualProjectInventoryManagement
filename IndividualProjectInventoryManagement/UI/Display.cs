using System;
using System.Collections.Generic;
using System.Text;

namespace IndividualProjectInventoryManagement.UI
{
    // ------------------------------------------------------------------
    // Display - coloured messages in one place
    // ------------------------------------------------------------------
    // Every message in the program is printed through these methods, so the
    // colours are decided here and nowhere else. To change "errors are red"
    // to another colour, there is exactly one line to edit.
    // ------------------------------------------------------------------
    public static class Display
    {
        public static void Header(string title)
        {
            Console.WriteLine();
            Coloured(title, ConsoleColor.Cyan);
            Coloured(new string('=', title.Length), ConsoleColor.Cyan);
        }

        public static void Success(string message) => Coloured(message, ConsoleColor.Green);

        public static void Warning(string message) => Coloured(message, ConsoleColor.Yellow);

        public static void Error(string message) => Coloured(message, ConsoleColor.Red);

        public static void Coloured(string message, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(message);

            // Always go back to the normal colour, or everything after this
            // line would stay coloured too
            Console.ResetColor();
        }
    }
}
