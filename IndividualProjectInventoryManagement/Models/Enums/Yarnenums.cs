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
        // Enum names cannot practically contain å, ä or ö, so the NAME is
        // plain "Jarbo". The [Description] attribute holds the text that is
        // SHOWN to the user. EnumExtensions.ToDisplayText() reads it.
        [Description("Järbo")]
        Jarbo,

        Drops,

        [Description("Sandnes Garn")]
        Sandnes
    }

    // The yarn products. All names are invented.
    // THIS IS THE ONE PLACE to change a brand's yarn range. Every name is
    // tagged with its brand, and BrandCatalog builds its lists from the tags:
    //   - add a yarn    -> add one line
    //   - rename a yarn -> change the name on its line
    //   - remove a yarn -> delete its line
    // Each name must be unique inside this enum.
    public enum YarnName
    {
        [Brand(YarnBrand.Jarbo)] MorningMist,
        [Brand(YarnBrand.Jarbo)] Dusk,
        [Brand(YarnBrand.Jarbo)] OatField,
        [Brand(YarnBrand.Jarbo)] Lingonberry,
        [Brand(YarnBrand.Jarbo)] BirchBark,

        [Brand(YarnBrand.Drops)] NordicSky,
        [Brand(YarnBrand.Drops)] IceField,
        [Brand(YarnBrand.Drops)] PineForest,
        [Brand(YarnBrand.Drops)] SeaBreeze,
        [Brand(YarnBrand.Drops)] CloudSoft,

        [Brand(YarnBrand.Sandnes)] Fjordline,
        [Brand(YarnBrand.Sandnes)] Coastline,
        [Brand(YarnBrand.Sandnes)] Highland,
        [Brand(YarnBrand.Sandnes)] Lagoon,
        [Brand(YarnBrand.Sandnes)] Granite
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
