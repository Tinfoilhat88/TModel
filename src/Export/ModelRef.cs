using CUE4Parse.FN.Structs.FortniteGame;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Material;
using CUE4Parse.UE4.Assets.Exports.SkeletalMesh;
using CUE4Parse.UE4.Assets.Exports.StaticMesh;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse_Conversion.Meshes;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using TModel.Export.Materials;
using static CUE4Parse.Utils.StringUtils;

namespace TModel.Export
{
    public class ModelRef
    {
        USkeletalMesh? SkeletalMesh;

        UStaticMesh? StaticMesh;

        public FTransform Transform = new FTransform(new FQuat(0, 0, 0, 0), new FVector(0, 0,0), new FVector(1,1,1));

        public List<CMaterial> Materials { get; } = new List<CMaterial>();

        public ModelRef(USkeletalMesh skeletalMesh)
        {
            SkeletalMesh = skeletalMesh;
            if (SkeletalMesh.Materials is not null)
            {
                foreach (var item in SkeletalMesh.Materials)
                {
                    if (item.Material != null)
                    {
                        UMaterialInterface Material = item.Material.Load<UMaterialInterface>();
                        if (Material is UMaterialInstanceConstant InstanceConstant)
                        {
                            Materials.Add(CMaterial.CreateReader(InstanceConstant));
                        }
                    }
                    else
                    {
                        Log.Warning($"Material is null | Slot Name: {item.MaterialSlotName.PlainText ?? "NULL"}");
                    }
                }
            }
        }

        // texIndex: The texture index for prop materials
        public ModelRef(UStaticMesh staticMesh, int texIndex = 0)
        {
            StaticMesh = staticMesh;
            if (staticMesh is not null)
            {
                foreach (FStaticMaterial material in staticMesh.StaticMaterials)
                {
                    UMaterialInterface LoadedMaterial = material.MaterialInterface.Load<UMaterialInterface>();
                    if (LoadedMaterial is UMaterialInstanceConstant InstanceConstant)
                    {
                        Materials.Add(CMaterial.CreateReader(InstanceConstant, texIndex));
                    }
                }
            }
        }

        public void ApplyMaterialOverrides(FCustomPartMaterialOverrideData[] MaterialOverrides)
        {
            foreach (FCustomPartMaterialOverrideData overrideMaterial in MaterialOverrides)
            {
                UObject LoadedOverriedMaterial;
                try
                {
                    LoadedOverriedMaterial = overrideMaterial.OverrideMaterial.Load();
                    if (LoadedOverriedMaterial is UMaterialInstanceConstant InstanceMat)
                    {
                        Materials[overrideMaterial.MaterialOverrideIndex] = CMaterial.CreateReader(InstanceMat);
                    }
                }
                catch
                {
                    Log.Error("Failed to load override material");
                };
            }
        }

        // This saves the model as a .psk(x) to the ExportsPath defined in Preferences
        public void SaveMesh(CBinaryWriter writer)
        {


            MeshExporter meshExporter = null;
            if (SkeletalMesh is USkeletalMesh skeletalMesh)
            {
                for (int i = 0; i < Materials.Count; i++)
                {
                    skeletalMesh.Materials[i] = Materials[i].GetSkeletalMaterial();
                }
                meshExporter = new MeshExporter(skeletalMesh);
            }
            else if (StaticMesh is UStaticMesh staticMesh)
            {
                meshExporter = new MeshExporter(staticMesh);
                for (int i = 0; i < Materials.Count; i++)
                {
                    staticMesh.StaticMaterials[i] = Materials[i].GetStaticMaterial();
                    staticMesh.Materials[i] = new ResolvedLoadedObject(Materials[i].Material);
                }
            }
            else
            {
#if DEBUG
                throw new System.Exception("Failed to save mesh");
#endif
            }

            if (meshExporter != null)
            {
                Directory.CreateDirectory(Preferences.ExportsPath);
                Mesh FirstLod = meshExporter.MeshLods[0];
                string GameMeshFullPath = FirstLod.FileName;
                string MeshExtentionWithPeriod = Path.GetExtension(GameMeshFullPath);
                string MeshNameWithOutPathAndExtension = Path.GetFileName(GameMeshFullPath).SubstringBeforeLast('.');
                string ParentFolderName = FirstLod.FileName.SubstringBeforeLast('/').SubstringAfterLast('/');
                string ParentFolderFirstCharacterInName = ParentFolderName.Substring(0,1);
                int NumOfSlashesInGameMeshPath = GameMeshFullPath.NumOccurrences('/');
                string NewMeshNameWithParentNameFirstChar = $"{MeshNameWithOutPathAndExtension}_{ParentFolderFirstCharacterInName}{NumOfSlashesInGameMeshPath}{MeshExtentionWithPeriod}";
                string NewFullSavePath = Path.Combine(Preferences.ExportsPath, NewMeshNameWithParentNameFirstChar);;
                File.WriteAllBytes(NewFullSavePath, FirstLod.FileData);

                writer.Write(NewFullSavePath); // Mesh name

                // == Transform data for mesh (only needed if is static mesh) == 

                // Location
                writer.Write((Single)Transform.Translation.X);
                writer.Write((Single)Transform.Translation.Y);
                writer.Write((Single)Transform.Translation.Z);

                // Rotation
                FRotator MeshRotation = Transform.Rotation.Rotator();

                writer.Write(MeshRotation.Roll);
                writer.Write(MeshRotation.Pitch);
                writer.Write(MeshRotation.Yaw);

                // Scale
                writer.Write((Single)Transform.Scale3D.X);
                writer.Write((Single)Transform.Scale3D.Y);
                writer.Write((Single)Transform.Scale3D.Z);

                if (SkeletalMesh == null)
                    Log.Information("Saved StaticMesh: " + StaticMesh.Name);
                else
                    Log.Information("Saved SkeletalMesh: " + SkeletalMesh.Name);
            }
            else
            {
                Log.Error("MeshExporter is null for: " + StaticMesh?.Name ?? StaticMesh?.Name ?? "NULL");
            }
        }

        public override string ToString() => SkeletalMesh?.Name ?? StaticMesh.Name ?? "None";
    }
}
