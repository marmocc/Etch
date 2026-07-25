using Etch.UI;
using Etch.Primitives;
using Etch.Deferred;

Deferrer deferrer = new();
Etcher.Create("Etch Demo", 800, 600)
    .Widget(new Widget(deferrer, new(100, 100), new(200, 150), 0, Color.FromRgb(255, 0, 0)))
    .Run();