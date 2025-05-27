using Content.Shared.Radio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Implants.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class QuickeningRattleComponent : Component
{
    // The radio channel the message will be sent to
    [DataField]
    public ProtoId<RadioChannelPrototype> RadioChannel = "Quickening";


    // The message that the implant will send when dead
    [DataField]
    public LocId DeathMessage = "Quickeningrattle-implant-dead-message";
}
