using Content.Shared.Damage;
using Content.Shared.Interaction.Events;
using Robust.Shared.GameStates;

namespace Content.Shared.Highlander.Components;


[RegisterComponent, NetworkedComponent]
public sealed partial class HighlanderComponent : Component
{

    /// <summary>
    /// Healing each second
    /// </summary>
    [DataField("passiveHealing")]
    public DamageSpecifier PassiveHealing = new()
    {
        DamageDict = new()
        {
            { "Blunt", -0.4 },
            { "Slash", -0.4 },
            { "Piercing", -0.4 },
            { "Heat", -0.4 },
            { "Shock", -0.4 }
        }
    };
}