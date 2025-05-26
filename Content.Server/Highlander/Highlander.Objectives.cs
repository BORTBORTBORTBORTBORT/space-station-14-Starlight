using Content.Server.Objectives.Components;
using Content.Server.Objectives.Systems;
using Content.Shared.Objectives.Components;
using Content.Shared.Vampire.Components;
using Content.Shared.Vampire;

namespace Content.Server.Vampire;

public sealed partial class HighlanderSystem
{
    [Dependency] private readonly NumberObjectiveSystem _number = default!;

}