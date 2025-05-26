using Content.Shared.NPC.Prototypes;
using Content.Shared.Roles;
using Content.Server.Highlander.Components;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Rules.Components;

[RegisterComponent, Access(typeof(HighlanderRuleSystem))]
public sealed partial class HighlanderRuleComponent : Component
{
    public readonly List<EntityUid> HighlanderMinds = new();


    public readonly List<ProtoId<EntityPrototype>> BaseObjectives = new()
    {
        "HighlanderObjective",

    };

    public readonly List<ProtoId<EntityPrototype>> EscapeObjectives = new()
    {
        "HighlanderSurviveObjective",
    };

}
