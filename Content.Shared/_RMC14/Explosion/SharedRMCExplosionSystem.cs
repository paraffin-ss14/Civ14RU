using Content.Shared.Throwing;
using Robust.Shared.Random;

namespace Content.Shared._RMC14.Explosion;

public sealed class SharedRMCExplosionSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<CMExplosionEffectComponent, CMExplosiveTriggeredEvent>
        ((owner, comp, _) => DoEffect((owner, comp)));
    }

    private void DoEffect(Entity<CMExplosionEffectComponent> ent)
    {
        if (ent.Comp.ShockWave is { } shockwave)
            SpawnNextToOrDrop(shockwave, ent);

        if (ent.Comp.Explosion is { } explosion)
            SpawnNextToOrDrop(explosion, ent);

        if (ent.Comp.MaxShrapnel <= 0)
            return;

        foreach (var effect in ent.Comp.ShrapnelEffects)
        {
            var shrapnelCount = _random.Next(ent.Comp.MinShrapnel, ent.Comp.MaxShrapnel);
            for (var i = 0; i < shrapnelCount; i++)
            {
                var angle = _random.NextAngle();
                var direction = angle.ToVec().Normalized() * 10;
                var shrapnel = SpawnNextToOrDrop(effect, ent);
                _throwing.TryThrow(shrapnel, direction, ent.Comp.ShrapnelSpeed / 10);
            }
        }
    }
}
