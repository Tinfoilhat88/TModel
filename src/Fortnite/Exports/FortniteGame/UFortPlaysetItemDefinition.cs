using CUE4Parse.FN.Exports.FortniteGame;
using CUE4Parse.FN.Structs.FortniteGame;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Utils;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.UObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TModel.Fortnite.Exports.FortniteGame
{
    public class UFortPlaysetItemDefinition : UFortItemDefinition
    {
        public int SizeX = 10;
        public int SizeY = 6;
        public int SizeZ = 1;
        public FFortCreativeTagsHelper CreativeTagsHelper;
        public FRotator? DefaultRotation;
        public bool bUsePlaysetProps = false;
        public FSoftObjectPath? PlaysetPropLevelSaveRecordCollection;
        public UScriptMap ActorClassCount;
        public FSoftObjectPath[] AssociatedPlaysetProps; // All uniquely referenced props in this playset
        public FortPreviewActorData[] PreviewActorData; // Path to actor with transform data

        public override void DeepDeserialize()
        {
            base.DeepDeserialize();

            var Propseties = this.Properties;
            // ActorClassCount
            SizeX = GetOrDefault<int>(nameof(SizeX));
            SizeY = GetOrDefault<int>(nameof(SizeY));
            SizeZ = GetOrDefault<int>(nameof(SizeZ));
            DefaultRotation = GetOrDefault<FRotator>(nameof(DefaultRotation));
            bUsePlaysetProps = GetOrDefault<bool>(nameof(bUsePlaysetProps));
            CreativeTagsHelper = GetOrDefault<FFortCreativeTagsHelper>(nameof(CreativeTagsHelper));
            ActorClassCount = GetOrDefault<UScriptMap>(nameof(ActorClassCount));
            AssociatedPlaysetProps = GetOrDefault<FSoftObjectPath[]>(nameof(AssociatedPlaysetProps), Array.Empty<FSoftObjectPath>());
            PreviewActorData = GetOrDefault<FortPreviewActorData[]>(nameof(PreviewActorData), Array.Empty<FortPreviewActorData>());
            PlaysetPropLevelSaveRecordCollection = GetOrDefault<FSoftObjectPath>(nameof(PlaysetPropLevelSaveRecordCollection));
            
        }
    }

    [StructFallback]
    public class FortPreviewActorData
    {
        public FSoftObjectPath ActorClass;
        public FVector RelativeLocation;
        public FRotator RelativeRotation;

        public FortPreviewActorData(FStructFallback fallback)
        {
            ActorClass = fallback.GetOrDefault<FSoftObjectPath>(nameof(ActorClass));
            RelativeLocation = fallback.GetOrDefault<FVector>(nameof(RelativeLocation));
            RelativeRotation = fallback.GetOrDefault<FRotator>(nameof(RelativeRotation));
        }
    }
}
