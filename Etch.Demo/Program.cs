using Etch.Graphics;
using Etch.Graphics.Terminal;

var surface = new Surface();
var context = surface.Context;

context.Clear(Color.Black);

for (int x = 0; x < 20; x++)
{
    context.Plot(new Vector2<int>(x, 0), Color.Red);
    context.Plot(new Vector2<int>(x, 10), Color.Red);
}

surface.Flush();