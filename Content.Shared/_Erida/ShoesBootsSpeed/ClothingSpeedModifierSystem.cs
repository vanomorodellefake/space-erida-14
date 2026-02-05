namespace Content.Shared.Clothing;

public sealed partial class ClothingSpeedModifierSystem : EntitySystem
{
    public void SetSpeedModifiers(EntityUid uid, float walkModifier, float sprintModifier, ClothingSpeedModifierComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        component.WalkModifier = walkModifier;
        component.SprintModifier = sprintModifier;
        Dirty(uid, component);
        _movementSpeed.RefreshMovementSpeedModifiers(uid);
        if (_container.TryGetContainingContainer((uid, null, null), out var container))
        {
            // inventory system will automatically hook into the event raised by this and update accordingly
            _movementSpeed.RefreshMovementSpeedModifiers(container.Owner);
        }
    }
}
