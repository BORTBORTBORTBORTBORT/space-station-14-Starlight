using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Highlander.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class HighlanderIconComponent : Component
{

    [DataField("HighlanderStatusIcon", customTypeSerializer: typeof(PrototypeIdSerializer<FactionIconPrototype>))]
    public string HighlanderStatusIcon = "HighlanderFaction";
}