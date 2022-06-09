using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Component.StaticMesh;
using CUE4Parse.UE4.Assets.Exports.StaticMesh;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Objects.UObject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TModel.Fortnite.Exports.FortniteGame;
using TModel.Modules;
using static CUE4Parse.Utils.StringUtils;

namespace TModel.Export.Exporters
{
    public class PropExporter : ExporterBase
    {
        public override SearchTerm SearchTerm => new SearchTerm() { SpecificPaths = new string[] 
        {
            "FortniteGame/Content/Playsets/PlaysetProps/"
        }};

        public override ExportPreviewInfo GetExportPreviewInfo(IPackage package)
        {
            if (package.Base is UFortPlaysetPropItemDefinition Prop)
            {
                TextureRef previewIcon = null;
                if (Prop.SmallPreviewImage is FSoftObjectPath ImagePath)
                {
                    previewIcon = new TextureRef(ImagePath.Load<UTexture2D>());
                }

                return new ExportPreviewInfo()
                {
                    Name = Prop.DisplayName,
                    Description = Prop.Description,
                    Package = package,
                    PreviewIcon = previewIcon,
                    FileName = Prop.Name
                };
            }
            return null;
        }

        public override ItemTileInfo GetTileInfo(IPackage package)
        {
            if (package.Base is UFortPlaysetPropItemDefinition Prop)
                return Prop.GetPreviewInfo();
            return null;
        }

        public override BlenderExportInfo GetBlenderExportInfo(IPackage package, int[]? styles = null)
        {
            BlenderExportInfo ExportInfo = new BlenderExportInfo();
            ExportInfo.Name = package.Name;
            if (package.Base is UFortPlaysetPropItemDefinition Prop)
            {
                foreach (KeyValuePair<string, CUE4Parse.FileProvider.GameFile> file in App.FileProvider.Files)
                {
                    if (Path.GetFileName(file.Key).SubstringBeforeLast('.') == Prop.DisplayName)
                    {
                        IPackage PropPackage = App.FileProvider.LoadPackage(file.Value);
                        foreach (Lazy<CUE4Parse.UE4.Assets.Exports.UObject> uObject in PropPackage.ExportsLazy)
                        {
                            if (uObject.Value is UStaticMeshComponent Mesh)
                            {
                                if (Mesh.StaticMesh is ResolvedObject MeshPath)
                                {
                                    UStaticMesh StaticMesh = MeshPath.Load<UStaticMesh>();
                                    ExportInfo.Models.Add(new ModelRef(StaticMesh));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return ExportInfo;
        }
    }
}
