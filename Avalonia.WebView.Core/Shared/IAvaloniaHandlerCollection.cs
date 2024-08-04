using System.Collections;

namespace Avalonia.WebView.Core.Shared;

public interface IAvaloniaHandlerCollection
    : IList<Type>,
        ICollection<Type>,
        IEnumerable<Type>,
        IEnumerable { }
