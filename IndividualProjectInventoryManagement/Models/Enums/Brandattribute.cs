using System;
using System.Collections.Generic;
using System.Text;

namespace IndividualProjectInventoryManagement.Models.Enums
{
    // ------------------------------------------------------------------
    // BrandAttribute - "this product name belongs to that brand"
    // ------------------------------------------------------------------
    // An ATTRIBUTE is a label you stick on a piece of code, written in
    // square brackets. You have already seen [Description("Järbo")].
    // This one is our own:
    //
    //     [Brand(YarnBrand.Jarbo)] MorningMist,
    //
    // reads as "the yarn MorningMist belongs to the brand Jarbo".
    //
    // WHY? So that everything about a product name lives in ONE line of
    // ONE file. BrandCatalog reads these labels and builds its lists
    // automatically. To change Järbo's range you edit the enum and nothing else:
    // not the catalog, not the menu, not the Product classes.
    //
    // [AttributeUsage] says where the label may be used. Enum members are
    // technically "fields", so AttributeTargets.Field.
    // ------------------------------------------------------------------
    [AttributeUsage(AttributeTargets.Field)]
    public class BrandAttribute : Attribute
    {
        // The brand, stored as a general Enum so ONE attribute works for
        // YarnBrand, NeedleBrand and HookBrand.
        public Enum Brand { get; }

        // Attribute parameters may only be simple constant values. An enum
        // value is allowed, but must be received as "object".
        public BrandAttribute(object brand)
        {
            if (brand is not Enum enumValue)
            {
                throw new ArgumentException("The brand must be an enum value.", nameof(brand));
            }

            Brand = enumValue;
        }
    }
}
