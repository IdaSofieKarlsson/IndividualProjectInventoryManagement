using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IndividualProjectInventoryManagement.Models.Enums
{
    // ------------------------------------------------------------------
    // Enums for OTHER SUPPLIES (small things: stitch markers, tape measures ...)
    // ------------------------------------------------------------------
    // Here the "name" is simply the KIND of supply, and every brand sells
    // every kind, so the names carry no [Brand] tags.
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
