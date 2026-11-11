using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IndividualProjectInventoryManagement.Models.Enums
{
    // ------------------------------------------------------------------
    // Enums for KNITTING NEEDLES
    // ------------------------------------------------------------------
    // Note that the brand list is NOT the same as for yarn:
    //   Järbo   -> yarn and needles        (no hooks)
    //   Drops   -> yarn only
    //   Addi    -> needles and hooks       (no yarn)
    //   KnitPro -> needles and hooks       (no yarn)
    // That is exactly why each category needs its own brand enum.
    // ------------------------------------------------------------------

    public enum NeedleBrand
    {
        [Description("Järbo")]
        Jarbo,

        Addi,

        // Without [Description], "KnitPro" would be shown as "Knit Pro",
        // because ToDisplayText() puts a space before every capital letter.
        [Description("KnitPro")]
        KnitPro
    }

    // Five needle products per brand, 15 in total (all invented)
    public enum NeedleName
    {
        // Järbo
        Classic,
        Ripple,
        Pocket,
        Travel,
        Studio,

        // Addi
        SwiftLane,
        FineLace,
        Comfort,
        QuickSwap,
        Everyday,

        // KnitPro
        Smooth,
        Ergo,
        ColourBurst,
        SockMaster,
        ChunkyCable
    }

    public enum NeedleMaterial
    {
        Beech,
        Birch,
        Bamboo,
        Steel,
        Aluminium
    }

    // Needles come in different shapes. This is an extra field that
    // only needles have, so it lives on the Needle class later.
    public enum NeedleType
    {
        Straight,
        Circular,
        DoublePointed
    }
}
