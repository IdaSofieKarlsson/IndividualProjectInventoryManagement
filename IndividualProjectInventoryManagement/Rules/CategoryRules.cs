using System;
using System.Collections.Generic;
using System.Text;

namespace IndividualProjectInventoryManagement.Rules
{
    // ------------------------------------------------------------------
    // CategoryRules - the allowed sizes and lengths for ONE category
    // ------------------------------------------------------------------
    // A "record" is a short way to write a class that only holds data.
    // This one line gives you a constructor, five read-only properties and
    // sensible Equals/ToString for free:
    //     new CategoryRules(sizes, "mm", 1, lengths, "cm")
    //
    // An EMPTY array means "not applicable": other supplies, for example,
    // have no length, so their Lengths array is empty.
    // ------------------------------------------------------------------
    public record CategoryRules(
        decimal[] Sizes,       // the allowed sizes, e.g. 2.0, 2.5, 3.0 ...
        string SizeUnit,       // what a size is measured in, e.g. "mm"
        int SizeDecimals,      // decimals to show: 1 gives "4.0 mm", 0 gives "50 g"
        decimal[] Lengths,     // the allowed lengths (empty = not applicable)
        string LengthUnit);    // e.g. "cm" or "m"
}
