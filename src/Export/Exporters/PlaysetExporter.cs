using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Component.StaticMesh;
using CUE4Parse.UE4.Assets.Exports.StaticMesh;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.UObject;
using Serilog;
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
    public class PlaysetExporter : ExporterBase
    {
        public override SearchTerm SearchTerm { get; } = new SearchTerm() { SpecificPaths = new string[] 
        {
            "FortniteGame/Content/Playsets/"
        }};

        public override ExportPreviewInfo GetExportPreviewInfo(IPackage package)
        {
            if (package.Base is UFortPlaysetItemDefinition Playset)
            {
                TextureRef previewIcon = null;
                if ((Playset.LargePreviewImage ?? Playset.SmallPreviewImage) is FSoftObjectPath ImagePath)
                {
                    previewIcon = new TextureRef(ImagePath.Load<UTexture2D>());
                }

                return new ExportPreviewInfo()
                {
                    Name = Playset.DisplayName,
                    Package = package,
                    PreviewIcon = previewIcon,
                    FileName = Playset.Name
                };
            }
            return null;
        }

        public override ItemTileInfo GetTileInfo(IPackage package)
        {
            if (package.Base is UFortPlaysetItemDefinition Playset)
                return Playset.GetPreviewInfo();
            return null;
        }

        public override BlenderExportInfo GetBlenderExportInfo(IPackage package, int[]? styles = null)
        {
            BlenderExportInfo ExportInfo = new BlenderExportInfo();

            if (package.Base is UFortPlaysetItemDefinition PlaysetDef)
            {
                PlaysetDef.DeepDeserialize();

                if (package.ExportsLazy[1].Value is ULevelSaveRecordCollection LevelRecord)
                {
                    for (int i = 0; i < LevelRecord.Items.Length; i++)
                    {
                        try
                        {
                            LevelSaveRecordCollectionItem item = LevelRecord.Items[i];
                            FortPreviewActorData ActorData = PlaysetDef.PreviewActorData[i];
                            IPackage PropPackage = App.FileProvider.LoadPackage(Path.ChangeExtension(ActorData.ActorClass.AssetPathName.Text, ".uasset"));
                            foreach (Lazy<UObject> uObject in PropPackage.ExportsLazy)
                            {
                                if (uObject.Value is UStaticMeshComponent Mesh)
                                {
                                    if (Mesh.StaticMesh is ResolvedObject MeshPath)
                                    {
                                        UStaticMesh StaticMesh = MeshPath.Load<UStaticMesh>();
                                        ModelRef modelRef;
                                        if (int.TryParse(item.RecordUniqueName.Text.SubstringAfterLast('_'), out int texIndex))
                                            modelRef = new ModelRef(StaticMesh, texIndex - 1);
                                        else
                                            modelRef = new ModelRef(StaticMesh);
                                        modelRef.Transform = new FTransform(new FQuat(ActorData.RelativeRotation), ActorData.RelativeLocation, item.Transform.Scale3D);
                                        ExportInfo.Models.Add(modelRef);
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error("Failed to read prop:\n" + e.ToString());
                        }

                    }
                }
                else
                {
                    foreach (FortPreviewActorData ActorData in PlaysetDef.PreviewActorData)
                    {
#if true
                        try
                        {
                            IPackage PropPackage = App.FileProvider.LoadPackage(Path.ChangeExtension(ActorData.ActorClass.AssetPathName.Text, ".uasset"));
                            foreach (Lazy<UObject> uObject in PropPackage.ExportsLazy)
                            {
                                if (uObject.Value is UStaticMeshComponent Mesh)
                                {
                                    if (Mesh.StaticMesh is ResolvedObject MeshPath)
                                    {
                                        UStaticMesh StaticMesh = MeshPath.Load<UStaticMesh>();
                                        ModelRef modelRef = new ModelRef(StaticMesh);
                                        modelRef.Transform = new FTransform(new FQuat(ActorData.RelativeRotation), ActorData.RelativeLocation, new FVector(1, 1, 1));
                                        ExportInfo.Models.Add(modelRef);
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error("Failed to read prop:\n" + e.ToString());
                        }
#endif
                    }
                }
            }

            return ExportInfo;
        }
    }
}
#if false
foreach (var playsetProp in PlaysetDef.AssociatedPlaysetProps)
{
    UFortPlaysetPropItemDefinition PropDef = playsetProp.Load<UFortPlaysetPropItemDefinition>();
    foreach (KeyValuePair<string, CUE4Parse.FileProvider.GameFile> file in App.FileProvider.Files)
    {
        if (Path.GetFileName(file.Key).SubstringBeforeLast('.') == PropDef.DisplayName)
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
#endif