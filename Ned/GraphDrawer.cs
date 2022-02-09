using Peacock;
using System.Collections.Generic;
using System.Linq;

namespace Ned;

public record StyledState<T>
    : StyledState
{
    public StyledState(T data, Style style)
        : base(style)
        => Data = data;

    public T Data { get; init; }
}

public record GraphComponent : TypedComponent<StyledState<GraphView>>
{
    public GraphComponent(GraphView view, Style style, IReadOnlyList<IComponent>? children, IReadOnlyList<IComponentWrapper>? wrappers)
        : base(view.ToStyledState(style), children, wrappers)
    { }

    public override IComponent With(IState state, IReadOnlyList<IComponent> children, IReadOnlyList<IComponentWrapper> wrappers)
        => new GraphComponent(((StyledState<GraphView>)state).Data, ((StyledState<GraphView>)state).Style, children, wrappers);
}

public class GraphDrawer
{
    public Style Style { get; set; }

    
}

public static class Extension
{
    public static StyledState<T> ToStyledState<T>(this T self, Style style)
        => new StyledState<T>(self, style);
}