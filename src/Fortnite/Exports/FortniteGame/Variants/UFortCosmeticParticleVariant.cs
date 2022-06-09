using CUE4Parse.UE4.Assets.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TModel.Modules;

namespace CUE4Parse.FN.Exports.FortniteGame
{
    class UFortCosmeticParticleVariant : UFortCosmeticVariant
    {
        public StyleOptionBase[]? ParticleOptions;

        public override void GetPreviewStyle(ExportPreviewSet style)
        {
            base.GetPreviewStyle(style);

            SetOptions(style, ParticleOptions);
        }

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);

            ParticleOptions = GetOrDefault<StyleOptionBase[]>(nameof(ParticleOptions));
        }
    }
}
