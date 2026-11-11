using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IndividualProjectInventoryManagement.Models.Enums
{
    // ------------------------------------------------------------------
    // Enums for CROCHET HOOKS
    // ------------------------------------------------------------------
    // Järbo is missing here on purpose: Järbo has yarn and needles, but no hooks.
    // ------------------------------------------------------------------

    public enum HookBrand
    {
        Addi,
        Clover,

        [Description("KnitPro")]
        KnitPro
    }

    // Five hook products per brand, 15 in total (all invented)
    public enum HookName
    {
        // Addi
        SoftGrip,
        QuickHook,
        LaceFine,
        Daily,
        Tunisian,

        // Clover
        Classic,
        Comfort,
        Petite,
        Jumbo,
        Pocket,

        // KnitPro
        Smooth,
        Ergo,
        Bright,
        Compact,
        Steady
    }

    public enum HookMaterial
    {
        Aluminium,
        Steel,
        Bamboo
    }
}
