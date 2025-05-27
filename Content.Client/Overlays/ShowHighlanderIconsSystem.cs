using Content.Shared.Overlays;
using Content.Shared.Highlander;
using Content.Shared.Highlander.Components;

using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class ShowHighlanderIconsSystem : EquipmentHudSystem<HighlanderIconComponent>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HighlanderIconComponent, GetStatusIconsEvent>(OnGetStatusIconsEvent);
    }

    private void OnGetStatusIconsEvent(EntityUid uid, HighlanderIconComponent component, ref GetStatusIconsEvent ev)
    {
        if (!IsActive)
            return;

        if (_prototype.TryIndex<FactionIconPrototype>(component.HighlanderStatusIcon, out var iconPrototype))
            ev.StatusIcons.Add(iconPrototype);
    }
}
