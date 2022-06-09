using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Assets.Utils;
using CUE4Parse.UE4.Objects.UObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TModel.Modules;

namespace CUE4Parse.FN.Exports.FortniteGame
{
    class UFortCosmeticMaterialVariant : UFortCosmeticVariant
    {
        public StyleOptionBase[] MaterialOptions;

        public override void GetPreviewStyle(ExportPreviewSet style)
        {
            base.GetPreviewStyle(style);

            SetOptions(style, MaterialOptions);
        }

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);

            MaterialOptions = GetOrDefault<StyleOptionBase[]>(nameof(MaterialOptions), Array.Empty<StyleOptionBase>());
        }
    }

    [StructFallback]
    public class MaterialVariant
    {
        public FSoftObjectPath MaterialToSwap;
        public int MaterialOverrideIndex;
        public FSoftObjectPath OverrideMaterial;

        public MaterialVariant(FStructFallback fallback)
        {
            MaterialToSwap = fallback.GetOrDefault<FSoftObjectPath>(nameof(MaterialToSwap));
            MaterialOverrideIndex = fallback.GetOrDefault<int>(nameof(MaterialOverrideIndex));
            OverrideMaterial = fallback.GetOrDefault<FSoftObjectPath>(nameof(OverrideMaterial));
        }
    }

}
