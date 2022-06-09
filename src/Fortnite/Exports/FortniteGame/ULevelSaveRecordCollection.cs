using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Assets.Utils;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.UObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TModel.Fortnite.Exports.FortniteGame
{
    public class ULevelSaveRecordCollection : UObject
    {
        public LevelSaveRecordCollectionItem[] Items;

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);

            Items = GetOrDefault<LevelSaveRecordCollectionItem[]>(nameof(Items));
        }
    }

    [StructFallback]
    public class LevelSaveRecordCollectionItem
    {
        public FSoftObjectPath LevelSaveRecord;
        public FName RecordUniqueName;
        public FTransform Transform;

        public LevelSaveRecordCollectionItem(FStructFallback fallback)
        {
            LevelSaveRecord = fallback.GetOrDefault<FSoftObjectPath>(nameof(LevelSaveRecord));
            RecordUniqueName = fallback.GetOrDefault<FName>(nameof(RecordUniqueName));
            Transform = fallback.GetOrDefault<FTransform>(nameof(Transform));
        }
    }
}
