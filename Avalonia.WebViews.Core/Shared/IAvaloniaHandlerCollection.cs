using System.Collections;

namespace Avalonia.WebViews.Core.Shared;

public interface IAvaloniaHandlerCollection
    : IList<Type>,
        ICollection<Type>,
        IEnumerable<Type>,
        IEnumerable
{ }
