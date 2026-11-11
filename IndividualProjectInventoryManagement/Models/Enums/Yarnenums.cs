using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IndividualProjectInventoryManagement.Models.Enums
{
    // ------------------------------------------------------------------
    // Enums for YARN
    // ------------------------------------------------------------------
    // Yarn has its OWN brand enum, its own name enum and its own material
    // enum. Why separate enums per category? Because the brands differ:
    // Järbo sells yarn AND needles, but no hooks. If all brands lived in
    // one shared enum, nothing would stop a hook from getting the brand
    // Drops (which only sells yarn). With one enum per category the
    // compiler itself refuses it: a Yarn can only ever have a YarnBrand.
    //
    // Several small enums share one file per category. C# allows that, and
    // for tiny enums it is easier to read than 14 separate files.
    // ------------------------------------------------------------------

    // The three yarn brands in the shop
    public enum YarnBrand
    {
        // Enum names cannot contain å, ä or ö in a practical way (they would
        // be awkward to type and to save in JSON), so the NAME is plain
        // "Jarbo". The [Description] attribute holds the text that is SHOWN
        // to the user. EnumExtensions.ToDisplayText() reads it.
        [Description("Järbo")]
        Jarbo,

        Drops,

        [Description("Sandnes Garn")]
        Sandnes
    }

    // The yarn products. Five per brand, 15 in total.
    // All names are invented. They are grouped by brand with comments, and
    // BrandCatalog (in the Rules folder) records which names belong to which brand.
    // Each name must be unique inside this enum.
    public enum YarnName
    {
        // Järbo
        MorningMist,
        Dusk,
        OatField,
        Lingonberry,
        BirchBark,

        // Drops
        NordicSky,
        IceField,
        PineForest,
        SeaBreeze,
        CloudSoft,

        // Sandnes Garn
        Fjordline,
        Coastline,
        Highland,
        Lagoon,
        Granite
    }

    // What the yarn is made of
    public enum YarnMaterial
    {
        Cotton,
        Wool,
        Merino,
        Alpaca,
        Acrylic
    }
}
