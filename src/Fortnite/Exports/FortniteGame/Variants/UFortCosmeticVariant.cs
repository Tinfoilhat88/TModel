using CUE4Parse.FN.Structs.GT;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.UObject;
using Serilog;
using TModel.Export;
using TModel.Modules;

namespace CUE4Parse.FN.Exports.FortniteGame
{
    public class UFortCosmeticVariant : UObject
    {
        public FSoftObjectPath CustomPreviewTileMaterial; // UMaterialInterface
        public FGameplayTag? VariantChannelTag;
        public FText? VariantChannelName;
        public FGameplayTag? ActiveVariantTag;

        public virtual void GetPreviewStyle(ExportPreviewSet style)
        {
            style.Name = VariantChannelName;
        }

        public virtual UCustomCharacterPart ApplyVariant(UCustomCharacterPart part, int index)
        {
            return null;
        }

        protected void SetOptions(ExportPreviewSet style, StyleOptionBase[] options)
        {
            foreach (StyleOptionBase option in options)
            {
                TextureRef? PreviewIcon = null;
                try
                {
                    // Softobject path could be currupt
                    PreviewIcon = new TextureRef(option.PreviewImage.Load<UTexture2D>());
                } catch { }

                style.Options.Add(new ExportPreviewOption()
                {
                    Name = option.VariantName,
                    Icon = PreviewIcon
                });
            }
        }

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);

            CustomPreviewTileMaterial = GetOrDefault<FSoftObjectPath>(nameof(CustomPreviewTileMaterial));
            VariantChannelTag = GetOrDefault<FGameplayTag>(nameof(VariantChannelTag));
            VariantChannelName = GetOrDefault<FText>(nameof(VariantChannelName));
            ActiveVariantTag = GetOrDefault<FGameplayTag>(nameof(ActiveVariantTag));
        }
    }
}