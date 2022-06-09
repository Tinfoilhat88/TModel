using CUE4Parse.FN.Exports.FortniteGame;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Material;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Objects.UObject;
using Serilog;
using System.Collections.Generic;
using TModel.Export.Materials;
using TModel.Modules;
using static CUE4Parse.Utils.StringUtils;

namespace TModel.Export.Exporters
{
    public class CharacterExporter : ExporterBase
    {
        public override SearchTerm SearchTerm => new SearchTerm() { SpecificPaths = new string[] { "FortniteGame/Content/Athena/Items/Cosmetics/Characters/" } };

        public override ExportPreviewInfo GetExportPreviewInfo(IPackage package)
        {
            if (package.Base is UAthenaCharacterItemDefinition Character)
            {
                Character.DeepDeserialize();
                TextureRef previewIcon = null;
                if (Character.HeroDefinition is UFortHeroType HeroDefinition)
                    if (HeroDefinition.LargePreviewImage is FSoftObjectPath IconImagePath)
                        previewIcon = new TextureRef(IconImagePath.Load<UTexture2D>());

                List<ExportPreviewSet>? Styles = null;

                if (Character.ItemVariants is FPackageIndex[] Variants && Variants.Length > 0)
                {
                    Styles = new();
                    foreach (var variant in Variants)
                    {
                        UFortCosmeticVariant CosmeticVariant = variant.Load<UFortCosmeticVariant>();
                        ExportPreviewSet PreviewSet = new ExportPreviewSet();
                        CosmeticVariant.GetPreviewStyle(PreviewSet);
                        Styles.Add(PreviewSet);
                    }
                }

                return new ExportPreviewInfo()
                {
                    Name = Character.DisplayName,
                    Description = Character.Description,
                    PreviewIcon = previewIcon,
                    Package = package,
                    Styles = Styles,
                    FileName = Character.Name
                };
            }
            return null;
        }

        public override ItemTileInfo GetTileInfo(IPackage package)
        {
            if (package.Base is UAthenaCharacterItemDefinition Character)
                return Character.GetPreviewInfo();
            return null;
        }

        public override BlenderExportInfo GetBlenderExportInfo(IPackage package, int[]? styles = null)
        {
            BlenderExportInfo ExportInfo = new BlenderExportInfo() { IsCharacter = true };
            ExportInfo.Name = package.Name;
            if (package.Base is UAthenaCharacterItemDefinition Character)
            {
                Character.DeepDeserialize();
                if (Character.BaseCharacterParts is FSoftObjectPath[] CustomParts)
                {
                    List<UFortCosmeticVariant> CosmeticVariants = new List<UFortCosmeticVariant>();
                    if (Character.ItemVariants is FPackageIndex[] Variants && Variants.Length > 0)
                    {
                        foreach (var variant in Variants)
                        {
                            CosmeticVariants.Add(variant.Load<UFortCosmeticVariant>());
                        }
                    }

                    foreach (var part in CustomParts)
                    {
                        try
                        {
                            UCustomCharacterPart CharacterPart = part.Load<UCustomCharacterPart>();
                            ModelRef PartModelRef = CharacterPart.GetModelRef();
                            if (PartModelRef is not null)
                            {
                                for (int j = 0; j < CosmeticVariants.Count; j++)
                                {
                                    UFortCosmeticVariant variant = CosmeticVariants[j];
                                    StyleOptionBase[] VariantOptions = null;

                                    // Gets part options
                                    if (variant is UFortCosmeticMaterialVariant MaterialVariant)
                                        VariantOptions = MaterialVariant.MaterialOptions;
                                    else if (variant is UFortCosmeticCharacterPartVariant PartVariant)
                                        VariantOptions = PartVariant.PartOptions;



                                    if (VariantOptions != null)
                                    {
                                        try
                                        {
                                            // Should be determined by 'styles'
                                            StyleOptionBase SelectedOption = VariantOptions[styles[j]];

                                            foreach (var variantPart in SelectedOption.VariantParts)
                                            {
                                                UCustomCharacterPart LoadedPart = variantPart.Load<UCustomCharacterPart>();
                                                if (CharacterPart.CharacterPartType == LoadedPart.CharacterPartType)
                                                {
                                                    PartModelRef = LoadedPart.GetModelRef();
                                                }
                                            }


                                            // Applys material overrides to ModelRef
                                            foreach (MaterialVariant overrideMaterial in SelectedOption.VariantMaterials)
                                            {
                                                for (int i = 0; i < PartModelRef.Materials.Count; i++)
                                                {
                                                    if (PartModelRef.Materials[i].Material == overrideMaterial.MaterialToSwap)
                                                    {
                                                        if (overrideMaterial.OverrideMaterial.TryLoad(out UObject LoadedMaterial))
                                                        {
                                                            if (LoadedMaterial is UMaterialInstanceConstant Instance)
                                                            {
                                                                PartModelRef.Materials[i] = CMaterial.CreateReader(Instance);
                                                            }
                                                            else
                                                            {
                                                                Log.Error($"Failed to override material | Can't load material of type: {LoadedMaterial.ExportType}");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (System.Exception e)
                                        {
                                            Log.Error("Failed to apply style: " + e.ToString());
                                        }

                                    }

                                }
                                ExportInfo.Models.Add(PartModelRef);
                            }

                        }
                        catch (System.Exception)
                        {
                            Log.Error($"Failed to export custom part: {part.AssetPathName}");
                            throw;
                        }

                    }
                }
            }

            return ExportInfo;
        }
    }
}
