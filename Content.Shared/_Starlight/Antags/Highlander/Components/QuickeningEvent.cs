using Content.Shared.Highlander.Components;
using Content.Shared.FixedPoint;

namespace Content.Shared.Highlander;

public sealed class Quickening : EntitySystem
{

    // the event is usually defined somewhere in else, often in its own file in Content.Shared
    [ByRefEvent]
    public record struct QuickeningAttemptEvent(bool Cancelled = false);

    [ByRefEvent]
    // here we used the primary constructor feature of C#, which is sharter than, but equivalent to
    public record struct QuickeningEvent
    {
        public EntityUid Quickened;

        // constructor
        public QuickeningEvent(EntityUid quickened)
        {
            Quickened = quickened;
        }

        // here we used the primary constructor feature of C#, which is sharter than, but equivalent to

    }


}


