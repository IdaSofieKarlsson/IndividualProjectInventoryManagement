using System.ComponentModel;

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

    // The needle products (all invented), each tagged with its brand.
    // This enum is the one place to change a brand's needle range.
    public enum NeedleName
    {
        [Brand(NeedleBrand.Jarbo)] Classic,
        [Brand(NeedleBrand.Jarbo)] Ripple,
        [Brand(NeedleBrand.Jarbo)] Pocket,
        [Brand(NeedleBrand.Jarbo)] Travel,
        [Brand(NeedleBrand.Jarbo)] Studio,

        [Brand(NeedleBrand.Addi)] SwiftLane,
        [Brand(NeedleBrand.Addi)] FineLace,
        [Brand(NeedleBrand.Addi)] Comfort,
        [Brand(NeedleBrand.Addi)] QuickSwap,
        [Brand(NeedleBrand.Addi)] Everyday,

        [Brand(NeedleBrand.KnitPro)] Smooth,
        [Brand(NeedleBrand.KnitPro)] Ergo,
        [Brand(NeedleBrand.KnitPro)] ColourBurst,
        [Brand(NeedleBrand.KnitPro)] SockMaster,
        [Brand(NeedleBrand.KnitPro)] ChunkyCable
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
    // only needles have, so it lives on the Needle class.
    public enum NeedleType
    {
        Straight,
        Circular,
        DoublePointed
    }
}
