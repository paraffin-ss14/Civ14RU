using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Trigger.Components;

/// <summary>
/// Component used for tracking active timers triggers.
/// Used internally for performance reasons.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ActiveTimerTriggerComponent : Component
{
    [DataField] public float TimeRemaining;

    [DataField] public EntityUid? User;

    [DataField] public float BeepInterval;

    [DataField] public float TimeUntilBeep;

    [DataField] public SoundSpecifier? BeepSound;
}
