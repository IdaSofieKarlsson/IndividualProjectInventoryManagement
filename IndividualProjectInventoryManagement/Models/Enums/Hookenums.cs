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

    // The hook products (all invented), each tagged with its brand.
    // This enum is the one place to change a brand's hook range.
    public enum HookName
    {
        [Brand(HookBrand.Addi)] SoftGrip,
        [Brand(HookBrand.Addi)] QuickHook,
        [Brand(HookBrand.Addi)] LaceFine,
        [Brand(HookBrand.Addi)] Daily,
        [Brand(HookBrand.Addi)] Tunisian,

        [Brand(HookBrand.Clover)] Classic,
        [Brand(HookBrand.Clover)] Comfort,
        [Brand(HookBrand.Clover)] Petite,
        [Brand(HookBrand.Clover)] Jumbo,
        [Brand(HookBrand.Clover)] Pocket,

        [Brand(HookBrand.KnitPro)] Smooth,
        [Brand(HookBrand.KnitPro)] Ergo,
        [Brand(HookBrand.KnitPro)] Bright,
        [Brand(HookBrand.KnitPro)] Compact,
        [Brand(HookBrand.KnitPro)] Steady
    }

    public enum HookMaterial
    {
        Aluminium,
        Steel,
        Bamboo
    }
}
