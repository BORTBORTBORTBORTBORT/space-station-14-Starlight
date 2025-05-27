using Content.Shared.Highlander.Components;
using Content.Shared.Highlander;
using Content.Shared.FixedPoint;
using static Content.Shared.Highlander.Quickening;

namespace Content.Shared.Highlander;

public sealed class SharedHighlanderSystem : EntitySystem
{

    public bool QuickeningLightning(EntityUid player, EntityUid soap)
    {
        // create an event instance
        var attemptEv = new QuickeningEvent();
        // we have to ask other systems if this mob can currently be slipped
        RaiseLocalEvent(player, ref attemptEv);

        Quicken(player); // whatever code that actually does the stun, sound effect, animation etc

    // raise another event, this time on the soap entity to inform it that someone slipped on it
        var ev = new SlipEvent(player);
        RaiseLocalEvent(soap, ref ev);

        return true; // success!
    }




    public sealed partial class Quicken;


}