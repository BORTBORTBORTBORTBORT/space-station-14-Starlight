using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Highlander.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class HighlanderIconComponent : Component
{
    [DataField("HighlanderStatusIcon")]
    public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = "HighlanderFaction";
}