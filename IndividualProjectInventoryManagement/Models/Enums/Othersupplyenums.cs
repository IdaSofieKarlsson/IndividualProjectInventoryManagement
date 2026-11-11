using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IndividualProjectInventoryManagement.Models.Enums
{
    // ------------------------------------------------------------------
    // Enums for OTHER SUPPLIES (small things: stitch markers, tape measures ...)
    // ------------------------------------------------------------------
    // Here the "name" is simply the KIND of supply, which means the old SupplyType idea from
    // the plan is not needed any more. Every brand below sells every kind;
    // BrandCatalog does not restrict the combinations for this category.
    // ------------------------------------------------------------------

    public enum OtherSupplyBrand
    {
        Prym,
        Clover,

        [Description("KnitPro")]
        KnitPro
    }

    // The five kinds of supply. EnumExtensions turns "StitchMarkers" into "Stitch Markers".
    public enum OtherSupplyName
    {
        StitchMarkers,
        MeasuringTape,
        NeedleGauge,
        TapestryNeedles,
        RowCounter
    }

    public enum OtherSupplyMaterial
    {
        Plastic,
        Steel,
        Brass
    }
}
