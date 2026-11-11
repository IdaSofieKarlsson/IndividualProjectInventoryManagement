using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace IndividualProjectInventoryManagement.Models.Enums
{
    // ------------------------------------------------------------------
    // EnumExtensions - turns an enum value into text that looks good on screen
    // ------------------------------------------------------------------
    // The problem: enum NAMES must follow C# rules (no spaces, no å/ä/ö in
    // practice), but the menu should say "Järbo", "Sandnes Garn" and
    // "Stitch Markers" - not "Jarbo", "Sandnes" and "StitchMarkers".
    //
    // The solution has two steps:
    //   1. If the enum value has a [Description("...")] attribute, show that text.
    //   2. Otherwise, put a space before every capital letter in the name:
    //        "MorningMist"   -> "Morning Mist"
    //        "DoublePointed" -> "Double Pointed"
    //
    // WHAT IS AN EXTENSION METHOD?
    // A static method whose first parameter starts with "this". It lets you
    // call the method as if it belonged to the type itself:
    //     brand.ToDisplayText()
    // instead of
    //     EnumExtensions.ToDisplayText(brand)
    // It works for ANY enum, so this one method covers all the enums in the project.
    // ------------------------------------------------------------------
    public static class EnumExtensions
    {
        public static string ToDisplayText(this Enum value)
        {
            // The enum value as text, e.g. "Jarbo"
            string name = value.ToString();

            // REFLECTION: the program inspects its own types while running.
            // GetType() gives the enum type (e.g. YarnBrand), and GetField(name)
            // finds the member called "Jarbo" inside it.
            FieldInfo? field = value.GetType().GetField(name);

            // Does that member have a [Description] attribute?
            // The "?." means: if field is null, do nothing and return null.
            DescriptionAttribute? description = field?.GetCustomAttribute<DescriptionAttribute>();

            if (description != null)
            {
                return description.Description;
            }

            // No attribute: insert a space wherever a lower-case letter is
            // followed by a capital letter. (?<=[a-z]) looks BEHIND, (?=[A-Z])
            // looks AHEAD. Both are zero-width, so the match is the gap
            // between two letters, and Replace puts a space in that gap.
            return Regex.Replace(name, "(?<=[a-z])(?=[A-Z])", " ");
        }
    }
}
