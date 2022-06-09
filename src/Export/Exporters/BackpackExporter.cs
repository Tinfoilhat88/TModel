using CUE4Parse.FN.Exports.FortniteGame;
using CUE4Parse.FN.Exports.FortniteGame.NoProperties;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Material;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Objects.UObject;
using Serilog;
using System.Collections.Generic;
using TModel.Export;
using TModel.Export.Exporters;
using TModel.Export.Materials;
using TModel.Modules;

namespace TModel.Exporters
{
    public class BackpackExporter : ExporterBase
    {
        public override SearchTerm SearchTerm => new SearchTerm() { SpecificPaths = new string[] { "FortniteGame/Content/Athena/Items/Cosmetics/Backpacks/" } };

        public override ExportPreviewInfo GetExportPreviewInfo(IPackage package)
        {
            if (package.Base is UAthenaBackpackItemDefinition Backpack)
            {
                Backpack.DeepDeserialize();
                TextureRef previewIcon = null;
                if (Backpack.LargePreviewImage is FSoftObjectPath ImagePath)
                    previewIcon = new TextureRef(ImagePath.Load<UTexture2D>());


                List<ExportPreviewSet>? Styles = null;

                if (Backpack.ItemVariants is FPackageIndex[] Variants && Variants.Length > 0)
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
                    Name = Backpack.DisplayName,
                    Description = Backpack.Description,
                    Package = package,
                    PreviewIcon = previewIcon,
                    Styles = Styles,
                    FileName = Backpack.Name
                };
            }
            return null;
        }

        public override ItemTileInfo GetTileInfo(IPackage package)
        {
            if (package.Base is UAthenaBackpackItemDefinition Backpack)
                return Backpack.GetPreviewInfo();
            return null;
        }

        public override BlenderExportInfo GetBlenderExportInfo(IPackage package, int[]? styles = null)
        {
            BlenderExportInfo ExportInfo = new BlenderExportInfo();
            ExportInfo.Name = package.Name;
            if (package.Base is UAthenaBackpackItemDefinition Backpack)
            {
                Backpack.DeepDeserialize();
                if (Backpack.CharacterParts is UCustomCharacterPart[] CustomParts)
                {
                    List<UFortCosmeticVariant> CosmeticVariants = new List<UFortCosmeticVariant>();

                    if (Backpack.ItemVariants is FPackageIndex[] Variants && Variants.Length > 0)
                    {
                        foreach (var variant in Variants)
                        {
                            CosmeticVariants.Add(variant.Load<UFortCosmeticVariant>());
                        }
                    }

                    foreach (var CharacterPart in CustomParts)
                    {
                        ModelRef PartModelRef = CharacterPart.GetModelRef();
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
                                                    Log.Error($"Failed to override material | Can't read material of type: {LoadedMaterial.ExportType}");
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        ExportInfo.Models.Add(PartModelRef);
                    }
                }


            }

            return ExportInfo;
        }
    }
}
