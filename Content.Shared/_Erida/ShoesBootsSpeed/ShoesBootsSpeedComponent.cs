using Robust.Shared.GameStates;

namespace Content.Shared._Erida.ShoesBootsSpeed.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(ShoesBootsSpeedSystem))]
public sealed partial class ShoesBootsSpeedComponent : Component
{
    [DataField]
    public int Coeff { get; set; } = 1;

    [DataField]
    public float StartSpeedModifier { get; set; } = 0.85f;

    public float CurrentSpeedModifier;
    public TimeSpan StartTime;
    public bool IsActive = false;
    public float ElapsedTime = 0f;
}

[RegisterComponent]
public sealed partial class SpeedModifyingShoesComponent : Component
{
    public EntityUid? ShoeEntity;
}
