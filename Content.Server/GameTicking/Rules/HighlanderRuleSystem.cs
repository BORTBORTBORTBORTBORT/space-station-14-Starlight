using Content.Server.Antag;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Mind;
using Content.Server.Objectives;
using Content.Server.Roles;
using Content.Server.Highlander;
using Content.Shared.Alert;
using Content.Shared.Highlander.Components;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Content.Shared.Roles;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Content.Server.Administration.Commands;
using Content.Server.RoundEnd;
using Content.Shared.Mind;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Timing;


namespace Content.Server.GameTicking.Rules;

public sealed partial class HighlanderRuleSystem : GameRuleSystem<HighlanderRuleComponent>
{
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly SharedRoleSystem _role = default!;
    [Dependency] private readonly NpcFactionSystem _npcFaction = default!;
    [Dependency] private readonly ObjectivesSystem _objective = default!;
    [Dependency] private readonly HighlanderSystem _Highlander = default!;
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SharedMindSystem _mindSystem = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;


    [Dependency] private readonly RoundEndSystem _roundEnd = default!;

    [Dependency] private readonly IGameTiming _timing = default!;



    public readonly SoundSpecifier BriefingSound = new SoundPathSpecifier("/Audio/Ambience/Antag/vampire_start.ogg");

    public readonly ProtoId<NpcFactionPrototype> HighlanderPrototypeId = "Highlander";


    public readonly ProtoId<NpcFactionPrototype> HighlanderFactionId = "Highlander";

    public readonly ProtoId<NpcFactionPrototype> NanotrasenFactionId = "NanoTrasen";

    [ValidatePrototypeId<EntityPrototype>]
    private const string HighlanderGearId = "HighlanderGear";


    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HighlanderComponent, GetBriefingEvent>(OnGetBriefing);

        // SubscribeLocalEvent<HighlanderRuleComponent, AfterAntagEntitySelectedEvent>(OnSelectAntag);
    }

    private void OnGetBriefing(Entity<HighlanderComponent> role, ref GetBriefingEvent args)
    {
        if (!_role.MindHasRole<HighlanderRoleComponent>(args.Mind.Owner))
            args.Append(Loc.GetString("Highlander-role-greeting"));
    }

    public bool MakeHighlander(EntityUid target, HighlanderRuleComponent rule)
    {
        if (!_mind.TryGetMind(target, out var mindId, out var mind))
            return false;

        EnsureComp<HighlanderIconComponent>(target);
        EnsureComp<HighlanderComponent>(target);

        SetOutfitCommand.SetOutfit(target, HighlanderGearId, EntityManager);

        foreach (var objective in rule.BaseObjectives)
            _mind.TryAddObjective(mindId, mind, objective);

        return true;
    }
    
}
