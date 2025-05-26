using Content.Server.Antag;
using Content.Server.Atmos.Components;
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
using Content.Shared.Store;
using Content.Shared.Store.Components;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Nutrition.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using System.Text;

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

    public readonly SoundSpecifier BriefingSound = new SoundPathSpecifier("/Audio/Ambience/Antag/Highlander_start.ogg");

    public readonly ProtoId<AntagPrototype> HighlanderPrototypeId = "Highlander";

    public readonly ProtoId<NpcFactionPrototype> ChangelingFactionId = "Highlander";

    public readonly ProtoId<NpcFactionPrototype> NanotrasenFactionId = "NanoTrasen";



    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HighlanderRuleComponent, AfterAntagEntitySelectedEvent>(OnSelectAntag);
        SubscribeLocalEvent<HighlanderRuleComponent, ObjectivesTextPrependEvent>(OnTextPrepend);
    }

    private void OnSelectAntag(EntityUid mindId, HighlanderRuleComponent comp, ref AfterAntagEntitySelectedEvent args)
    {
        MakeHighlander(args.EntityUid, comp);
    }
    
    public bool MakeHighlander(EntityUid target, HighlanderRuleComponent rule)
    {
        if (!_mind.TryGetMind(target, out var mindId, out var mind))
            return false;

        // briefing
        if (TryComp<MetaDataComponent>(target, out var metaData))
        {
            var briefing = Loc.GetString("Highlander-role-greeting", ("name", metaData?.EntityName ?? "Unknown"));

            _antag.SendBriefing(target, MakeBriefing(target), Color.Yellow, BriefingSound);
            _role.MindHasRole<HighlanderRoleComponent>(mindId, out var HighlanderRole);
            _role.MindHasRole<RoleBriefingComponent>(mindId, out var briefingComp);
            if (HighlanderRole is not null && briefingComp is null)
            {
                AddComp<RoleBriefingComponent>(HighlanderRole.Value.Owner);
                Comp<RoleBriefingComponent>(HighlanderRole.Value.Owner).Briefing = briefing;
            }
        }
        // Highlander stuff
        _npcFaction.RemoveFaction(target, NanotrasenFactionId, false);
        _npcFaction.AddFaction(target, ChangelingFactionId);

        // make sure it's initial chems are set to max
        var HighlanderComponent = EnsureComp<HighlanderComponent>(target);
        EnsureComp<HighlanderIconComponent>(target);
        EnsureComp<HighlanderSpaceDamageComponent>(target);
        var HighlanderAlertComponent = EnsureComp<HighlanderAlertComponent>(target);
        var interfaceComponent = EnsureComp<UserInterfaceComponent>(target);
        
        if (HasComp<UserInterfaceComponent>(target))
            _uiSystem.SetUiState(target, HighlanderMutationUiKey.Key, new HighlanderMutationBoundUserInterfaceState(HighlanderComponent.HighlanderMutations, HighlanderComponent.CurrentMutation));
        
        var Highlander = new Entity<HighlanderComponent>(target, HighlanderComponent);
        
        RemComp<PerishableComponent>(Highlander);
        RemComp<BarotraumaComponent>(Highlander);
        RemComp<ThirstComponent>(Highlander);

        HighlanderComponent.Balance = new() { { HighlanderComponent.CurrencyProto, 0 } };

        rule.HighlanderMinds.Add(mindId);
        
        _Highlander.AddStartingAbilities(Highlander);
        _Highlander.MakeVulnerableToHoly(Highlander);
        _alerts.ShowAlert(Highlander, HighlanderAlertComponent.BloodAlert);
        _alerts.ShowAlert(Highlander, HighlanderAlertComponent.StellarWeaknessAlert);
        
        Random random = new Random();

        foreach (var objective in rule.BaseObjectives)
            _mind.TryAddObjective(mindId, mind, objective);
            
        if (rule.EscapeObjectives.Count > 0)
        {
            var randomEscapeObjective = rule.EscapeObjectives[random.Next(rule.EscapeObjectives.Count)];
            _mind.TryAddObjective(mindId, mind, randomEscapeObjective);
        }
        
        if (rule.StealObjectives.Count > 0)
        {
            var randomEscapeObjective = rule.StealObjectives[random.Next(rule.StealObjectives.Count)];
            _mind.TryAddObjective(mindId, mind, randomEscapeObjective);
        }

        return true;
    }
    
    private string MakeBriefing(EntityUid ent)
    {
        if (TryComp<MetaDataComponent>(ent, out var metaData))
        {
            var briefing = Loc.GetString("Highlander-role-greeting", ("name", metaData?.EntityName ?? "Unknown"));
            
            return briefing;
        }
        
        return "";
    }

    private void OnTextPrepend(EntityUid uid, HighlanderRuleComponent comp, ref ObjectivesTextPrependEvent args)
    {
        var mostDrainedName = string.Empty;
        var mostDrained = 0f;

        foreach (var vamp in EntityQuery<HighlanderComponent>())
        {
            if (!_mind.TryGetMind(vamp.Owner, out var mindId, out var mind))
                continue;

            if (!TryComp<MetaDataComponent>(vamp.Owner, out var metaData))
                continue;

            if (vamp.TotalBloodDrank > mostDrained)
            {
                mostDrained = vamp.TotalBloodDrank;
                mostDrainedName = _objective.GetTitle((mindId, mind), metaData.EntityName);
            }
        }

        var sb = new StringBuilder();
        sb.AppendLine(Loc.GetString($"roundend-prepend-Highlander-drained{(!string.IsNullOrWhiteSpace(mostDrainedName) ? "-named" : "")}", ("name", mostDrainedName), ("number", mostDrained)));

        args.Text = sb.ToString();
    }
}
