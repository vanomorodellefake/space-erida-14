using System.Diagnostics;
using System.Runtime.CompilerServices;
using Content.Shared.Clothing;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Robust.Shared.Timing;
using DependencyAttribute = Robust.Shared.IoC.DependencyAttribute;

namespace Content.Shared._Erida.ShoesBootsSpeed.Components;

public sealed partial class ShoesBootsSpeedSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly ClothingSpeedModifierSystem _clothing = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpeedModifyingShoesComponent, MoveInputEvent>(OnMoveInputEvent);
        SubscribeLocalEvent<ShoesBootsSpeedComponent, GotEquippedEvent>(OnShoeAdded);
        SubscribeLocalEvent<ShoesBootsSpeedComponent, GotUnequippedEvent>(OnShoeRemoved);
    }

    private void OnShoeAdded(Entity<ShoesBootsSpeedComponent> entity, ref GotEquippedEvent args)
    {
        Log.Debug("OnShoeAdded - Start");
        AddComp<SpeedModifyingShoesComponent>(args.Equipee);
        var playerComp = Comp<SpeedModifyingShoesComponent>(args.Equipee);
        playerComp.ShoeEntity = entity.Owner;
    }

    private void OnShoeRemoved(Entity<ShoesBootsSpeedComponent> shoeEntity, ref GotUnequippedEvent args)
    {
        Log.Debug("OnShoeRemoved - Start");
        RemComp<SpeedModifyingShoesComponent>(args.Equipee);
    }

    private void OnMoveInputEvent(Entity<SpeedModifyingShoesComponent> entity, ref MoveInputEvent args)
    {
        if (!(entity.Comp.ShoeEntity != null)
            || !TryComp<ShoesBootsSpeedComponent>(entity.Comp.ShoeEntity.Value, out var shoesComp))
            return;
        Log.Debug("OnMoveInputEvent - Start");
        if (args.HasDirectionalMovement)
        {
            Log.Debug("OnMoveInputEvent - args.HasDirectionalMovement");
            if (!shoesComp.IsActive)
            {
                Log.Debug("OnMoveInputEvent - !entity.Comp.IsActive");
                shoesComp.IsActive = true;
                shoesComp.StartTime = _timing.CurTime;
            }
        }
        else
        {
            Log.Debug("OnMoveInputEvent - else");
            shoesComp.IsActive = false;
        }
    }

    public override void Update(float frameTime)
    {
        var currentTime = _timing.CurTime;

        var query = AllEntityQuery<SpeedModifyingShoesComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.ShoeEntity == null
                || !TryComp<ShoesBootsSpeedComponent>(comp.ShoeEntity.Value, out var shoesComp)
                || !TryComp<ClothingSpeedModifierComponent>(comp.ShoeEntity.Value, out var speedModifierComp))
                continue;

            if (shoesComp.IsActive)
            {
                var elapsed = currentTime - shoesComp.StartTime;
                shoesComp.ElapsedTime = (float)elapsed.TotalSeconds;

                shoesComp.CurrentSpeedModifier = shoesComp.Coeff * shoesComp.ElapsedTime;// (float)Math.Sqrt(shoesComp.Coeff * shoesComp.ElapsedTime);
                Log.Debug($"comp.CurrentSpeedModifier - {shoesComp.CurrentSpeedModifier}");
            }
            else
            {
                shoesComp.CurrentSpeedModifier = shoesComp.StartSpeedModifier;
            }

            _clothing.SetSpeedModifiers(comp.ShoeEntity.Value, shoesComp.CurrentSpeedModifier, shoesComp.CurrentSpeedModifier, speedModifierComp);
        }
    }
}
