using CUE4Parse.UE4.Assets.Exports.Material;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.UObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TModel.Export.Materials
{
    // M_FN_Weapon_MASTER
    public class CMR_Default : CMaterial
    {
        public CMR_Default(UMaterialInstanceConstant material) : base(material)
        {

        }

        protected override void ReadParameters()
        {
            if (!TrySetTexture($"Diffuse_Texture_{TextureIndex}", ref Diffuse))
            {
                TrySetTexture("Diffuse", ref Diffuse);
            }
            TrySetTexture("SpecularMasks", ref SpecularMasks);
            TrySetTexture("Normals", ref Normals);
            TrySetTexture("Emissive", ref Emissive);
            TrySetTexture("M", ref Misc);

            TrySetVector("Skin Boost Color And Exponent", ref SkinBoostColor);
        }
    }
}
