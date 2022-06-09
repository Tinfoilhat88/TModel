using CUE4Parse.FN.Exports.FortniteGame;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports.SkeletalMesh;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Objects.UObject;
using System.Collections.Generic;
using TModel.Modules;

namespace TModel.Export.Exporters
{
    public class GliderExporter : ExporterBase
    {
        public override SearchTerm SearchTerm => new SearchTerm() { SpecificPaths = new string[] { "FortniteGame/Content/Athena/Items/Cosmetics/Gliders/" } };

        public override ExportPreviewInfo GetExportPreviewInfo(IPackage package)
        {
            if (package.Base is UAthenaGliderItemDefinition Glider)
            {
                Glider.DeepDeserialize();
                TextureRef SmallImageRef = null;
                if (Glider.LargePreviewImage is FSoftObjectPath ImagePath)
                    SmallImageRef = new TextureRef(ImagePath.Load<UTexture2D>());

                return new ExportPreviewInfo()
                {
                    Name = Glider.DisplayName,
                    Description = Glider.Description,
                    Package = package,
                    PreviewIcon = SmallImageRef,
                    FileName = Glider.Name
                };
            }
            return null;
        }

        public override ItemTileInfo GetTileInfo(IPackage package)
        {
            if (package.Base is UAthenaGliderItemDefinition Glider)
                return Glider.GetPreviewInfo();
            return null;
        }

        public override BlenderExportInfo GetBlenderExportInfo(IPackage package, int[]? styles = null)
        {
            BlenderExportInfo ExportInfo = new BlenderExportInfo();
            ExportInfo.Name = package.Name;
            if (package.Base is UAthenaGliderItemDefinition Glider)
            {
                Glider.DeepDeserialize();
                USkeletalMesh GliderMesh = Glider.SkeletalMesh.Load<USkeletalMesh>();
                ModelRef model = new ModelRef(GliderMesh);
                model.ApplyMaterialOverrides(Glider.MaterialOverrides);
                ExportInfo.Models.Add(model);
            }

            return ExportInfo;
        }
    }
}
