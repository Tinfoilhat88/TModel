using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.UObject;
using System;

namespace CUE4Parse.FN.Exports.FortniteGame
{
    public class UFortWeaponRangedItemDefinition : UFortWeaponItemDefinition
    {
        public Lazy<FSoftObjectPath> ProjectileTemplate; // UClass
        public Lazy<FSoftObjectPath> BulletShellFXTemplate; // UClass
        public Lazy<bool> bUseNativeWeaponTrace;
        public Lazy<bool> bTraceThroughPawns;
        public Lazy<bool> bTraceThroughWorld;
        public Lazy<bool> bShouldSpawnBulletShellFX;
        public Lazy<bool> bShouldUsePerfectAimWhenTargetingMinSpread;
        public Lazy<bool> bDoNotAllowDoublePump;
        public Lazy<bool> bUseOnTouch;
        public Lazy<bool> bAllowADSInAir;
        public Lazy<bool> bShowReticleHitNotifyAtImpactLocation;
        public Lazy<bool> bForceProjectileTooltip;
        public Lazy<bool> bSecondaryFireRequiresAmmo;

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
#if false
            ProjectileTemplate =                            new Lazy<FSoftObjectPath>(() => GetOrDefault<FSoftObjectPath>(nameof(ProjectileTemplate)));
            BulletShellFXTemplate =                         new Lazy<FSoftObjectPath>(() => GetOrDefault<FSoftObjectPath>(nameof(BulletShellFXTemplate)));
            bUseNativeWeaponTrace =                         new Lazy<bool>(() => GetOrDefault<bool>(nameof(bUseNativeWeaponTrace)));
            bTraceThroughPawns =                            new Lazy<bool>(() => GetOrDefault<bool>(nameof(bTraceThroughPawns)));
            bTraceThroughWorld =                            new Lazy<bool>(() => GetOrDefault<bool>(nameof(bTraceThroughWorld)));
            bShouldSpawnBulletShellFX =                     new Lazy<bool>(() => GetOrDefault<bool>(nameof(bShouldSpawnBulletShellFX)));
            bShouldUsePerfectAimWhenTargetingMinSpread =    new Lazy<bool>(() => GetOrDefault<bool>(nameof(bShouldUsePerfectAimWhenTargetingMinSpread)));
            bDoNotAllowDoublePump =                         new Lazy<bool>(() => GetOrDefault<bool>(nameof(bDoNotAllowDoublePump)));
            bUseOnTouch =                                   new Lazy<bool>(() => GetOrDefault<bool>(nameof(bUseOnTouch)));
            bAllowADSInAir =                                new Lazy<bool>(() => GetOrDefault<bool>(nameof(bAllowADSInAir)));
            bShowReticleHitNotifyAtImpactLocation =         new Lazy<bool>(() => GetOrDefault<bool>(nameof(bShowReticleHitNotifyAtImpactLocation)));
            bForceProjectileTooltip =                       new Lazy<bool>(() => GetOrDefault<bool>(nameof(bForceProjectileTooltip)));
            bSecondaryFireRequiresAmmo =                    new Lazy<bool>(() => GetOrDefault<bool>(nameof(bSecondaryFireRequiresAmmo)));
#endif
        }
    }
}