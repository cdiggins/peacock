using System;
using System.Collections.Generic;
using System.Linq;

namespace Peacock;

public class CommonComponents
{

    public record StackState(StyledState State)
        : StyledState(State);

    public record StackComponent(StackState State, IEnumerable<IComponent>? Components)
        : Component<StackComponent, StackState>(State, State.Dimensions.Actual.ComputeStackLayout(Components ?? Array.Empty<IComponent>()))
    {
        public override StackComponent With(StackState state, IEnumerable<IComponent>? children)
            => new(state, children);
    }

    public record ButtonState(StyledState State, bool Down = false)
        : StyledState(State);

    public record ButtonComponent(ButtonState State, IComponent Child)
        : Component<ButtonComponent, ButtonState>(State, new[] { Child })
    {
        public override ButtonComponent With(ButtonState state, IEnumerable<IComponent>? children)
            => new(state, children?.FirstOrDefault() ?? throw new ArgumentException("Button must have exactly one child"));
    }

    public record LabelState(StyledState State, string Text)
        : StyledState(State);

    public record LabelComponent(LabelState State)
        : Component<LabelComponent, LabelState>(State)
    {
        public override LabelComponent With(LabelState state, IEnumerable<IComponent>? children)
            => new(state);
    }
}