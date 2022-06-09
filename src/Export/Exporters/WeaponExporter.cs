using CUE4Parse.FN.Exports.FortniteGame;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.SkeletalMesh;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Objects.UObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TModel.Modules;

namespace TModel.Export.Exporters
{
    public class WeaponExporter : ExporterBase
    {
        public override SearchTerm SearchTerm => new SearchTerm() 
        {
            FileNameStart = new string[] { "WID" },
            FileFalseNameStart = new string[] { "WID_Harvest" }
        };

        public override ExportPreviewInfo GetExportPreviewInfo(IPackage package)
        {
            if (package.Base is UFortWeaponItemDefinition Weapon)
            {
                TextureRef SmallImageRef = null;
                if (Weapon.LargePreviewImage is FSoftObjectPath ImagePath)
                    SmallImageRef = new TextureRef(ImagePath.Load<UTexture2D>());
                return new ExportPreviewInfo()
                {
                    Name = Weapon.DisplayName,
                    Package = package,
                    PreviewIcon = SmallImageRef,
                    FileName = Weapon.Name
                };
            }
            return null;
        }

        public override ItemTileInfo GetTileInfo(IPackage package)
        {
            if (package.Base is UFortWeaponRangedItemDefinition RangedWeapon)
                return RangedWeapon.GetPreviewInfo();
            if (package.Base is UFortWeaponModItemDefinition ModItem)
                return ModItem.GetPreviewInfo();
            if (package.Base is UFortAmmoItemDefinition AmmoItem)
                return AmmoItem.GetPreviewInfo();
            return null;
        }

        public override BlenderExportInfo GetBlenderExportInfo(IPackage package, int[]? styles = null)
        {
            BlenderExportInfo ExportInfo = new BlenderExportInfo();
            ExportInfo.Name = package.Name;
            if (package.Base is UFortWeaponItemDefinition WeaponDef)
            {
                WeaponDef.DeepDeserialize();
                USkeletalMesh WeaponMesh = null;
                try
                {
                    WeaponMesh = WeaponDef.WeaponMeshOverride.Load<USkeletalMesh>();
                }
                catch
                {
                    try
                    {
                        WeaponMesh = WeaponDef.PickupSkeletalMesh.Load<USkeletalMesh>();
                    }
                    catch (Exception e)
                    {
                       // pp.LogMessage("Can't Export Weapon"
#if DEBUG              // 
                       //    + "\n\n" + e.ToString());
#else                  // 
                       //    );
#endif                 // 
                    }

                }
                finally
                {
                    ExportInfo.Models.Add(new ModelRef(WeaponMesh));
                }
            }

            return ExportInfo;
        }
    }
}
