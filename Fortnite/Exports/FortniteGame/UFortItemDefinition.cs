using CUE4Parse.FN.Enums.FortniteGame;
using CUE4Parse.FN.Structs.Engine;
using CUE4Parse.FN.Structs.GA;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.SkeletalMesh;
using CUE4Parse.UE4.Assets.Exports.StaticMesh;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.GameplayTags;
using CUE4Parse.UE4.Objects.UObject;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TModel;
using TModel.Export;
using TModel.Modules;

namespace CUE4Parse.FN.Exports.FortniteGame
{
    public class UFortItemDefinition : UObject
    {
        public EFortRarity Rarity;
        public EFortItemType ItemType;
        public EFortItemType PrimaryAssetIdItemTypeOverride;
        public EFortInventoryFilter FilterOverride;
        public EFortItemTier Tier;
        public EFortItemTier MaxTier;
        public EFortTemplateAccess Access;
        public bool bIsAccountItem;
        public bool bNeverPersisted;
        public bool bAllowMultipleStacks;
        public bool bAutoBalanceStacks;
        public bool bForceAutoPickup;
        public bool bInventorySizeLimited;
        public FText? ItemTypeNameOverride;
        public FText? DisplayName;
        public FText? ShortDescription;
        public FText? Description;
        public FText? DisplayNamePrefix;
        public FText? SearchTags;
        public FName GiftBoxGroupName;
        public FGameplayTagContainer GameplayTags;
        public FGameplayTagContainer AutomationTags;
        public FGameplayTagContainer SecondaryCategoryOverrideTags;
        public FGameplayTagContainer TertiaryCategoryOverrideTags;
        public FScalableFloat? MaxStackSize;
        public FScalableFloat? PurchaseItemLimit;
        public float FrontendPreviewScale;
        public UClass? TooltipClass;
        public UFortTooltipDisplayStatsList? StatList;
        public FCurveTableRowHandle? RatingLookup;
        public FSoftObjectPath? WidePreviewImage;
        public FSoftObjectPath? SmallPreviewImage;
        public FSoftObjectPath? LargePreviewImage;
        public FSoftObjectPath DisplayAssetPath;
        public FDataTableRowHandle? PopupDetailsTag;
        public UFortItemSeriesDefinition? Series;
        public FVector FrontendPreviewPivotOffset;
        public FRotator FrontendPreviewInitialRotation;
        public UStaticMesh? FrontendPreviewMeshOverride;
        public USkeletalMesh? FrontendPreviewSkeletalMeshOverride;

        public virtual ItemTileInfo? GetPreviewInfo()
        {

            return new ItemTileInfo() { PreviewIcon = new Lazy<TextureRef>(() => 
            {
                TextureRef SmallImage = null;
                if (SmallPreviewImage != null)
                    if (!SmallPreviewImage?.AssetPathName.IsNone ?? false)
                        SmallImage = new TextureRef(SmallPreviewImage?.Load<UTexture2D>());
                return SmallImage;
            }), DisplayName = DisplayName, FileName = Name };
        }

        public virtual void DeepDeserialize()
        {
            ShortDescription = GetOrDefault<FText>(nameof(ShortDescription));
            Description = GetOrDefault<FText>(nameof(Description));
#if false
            ItemType = GetOrDefault<EFortItemType>(nameof(ItemType));
            PrimaryAssetIdItemTypeOverride = GetOrDefault<EFortItemType>(nameof(PrimaryAssetIdItemTypeOverride));
            FilterOverride = GetOrDefault<EFortInventoryFilter>(nameof(FilterOverride));
            Tier = GetOrDefault<EFortItemTier>(nameof(Tier));
            MaxTier = GetOrDefault<EFortItemTier>(nameof(MaxTier));
            Access = GetOrDefault<EFortTemplateAccess>(nameof(Access));
            bIsAccountItem = GetOrDefault<bool>(nameof(bIsAccountItem));
            bNeverPersisted = GetOrDefault<bool>(nameof(bNeverPersisted));
            bAllowMultipleStacks = GetOrDefault<bool>(nameof(bAllowMultipleStacks));
            bAutoBalanceStacks = GetOrDefault<bool>(nameof(bAutoBalanceStacks));
            bForceAutoPickup = GetOrDefault<bool>(nameof(bForceAutoPickup));
            bInventorySizeLimited = GetOrDefault<bool>(nameof(bInventorySizeLimited));
            ItemTypeNameOverride = GetOrDefault<FText>(nameof(ItemTypeNameOverride));
            DisplayNamePrefix = GetOrDefault<FText>(nameof(DisplayNamePrefix));
            SearchTags = GetOrDefault<FText>(nameof(SearchTags));
            GiftBoxGroupName = GetOrDefault<FName>(nameof(GiftBoxGroupName));
            GameplayTags = GetOrDefault<FGameplayTagContainer>(nameof(GameplayTags));
            AutomationTags = GetOrDefault<FGameplayTagContainer>(nameof(AutomationTags));
            SecondaryCategoryOverrideTags = GetOrDefault<FGameplayTagContainer>(nameof(SecondaryCategoryOverrideTags));
            TertiaryCategoryOverrideTags = GetOrDefault<FGameplayTagContainer>(nameof(TertiaryCategoryOverrideTags));
            MaxStackSize = GetOrDefault<FScalableFloat>(nameof(MaxStackSize));
            PurchaseItemLimit = GetOrDefault<FScalableFloat>(nameof(PurchaseItemLimit));
            FrontendPreviewScale = GetOrDefault<float>(nameof(FrontendPreviewScale));
            TooltipClass = GetOrDefault<UClass>(nameof(TooltipClass));
            // StatList = GetOrDefault<UFortTooltipDisplayStatsList>(nameof(StatList)); // Doesnt load - dont know why
            RatingLookup = GetOrDefault<FCurveTableRowHandle>(nameof(RatingLookup));
            WidePreviewImage = GetOrDefault<FSoftObjectPath>(nameof(WidePreviewImage));
            DisplayAssetPath = GetOrDefault<FSoftObjectPath>(nameof(DisplayAssetPath));
            PopupDetailsTag = GetOrDefault<FDataTableRowHandle>(nameof(PopupDetailsTag));
            Series = GetOrDefault<UFortItemSeriesDefinition>(nameof(Series));
            FrontendPreviewPivotOffset = GetOrDefault<FVector>(nameof(FrontendPreviewPivotOffset));
            FrontendPreviewInitialRotation = GetOrDefault<FRotator>(nameof(FrontendPreviewInitialRotation));
            FrontendPreviewMeshOverride = GetOrDefault<UStaticMesh>(nameof(FrontendPreviewMeshOverride));
            FrontendPreviewSkeletalMeshOverride = GetOrDefault<USkeletalMesh>(nameof(FrontendPreviewSkeletalMeshOverride));
#endif
        }

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);

            SmallPreviewImage = GetOrDefault<FSoftObjectPath>(nameof(SmallPreviewImage));
            LargePreviewImage = GetOrDefault<FSoftObjectPath>(nameof(LargePreviewImage));
            DisplayName = GetOrDefault<FText>(nameof(DisplayName));
            Rarity = GetOrDefault(nameof(Rarity), EFortRarity.Epic);
        }
    }
}