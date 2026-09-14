using Etch.Common;

namespace Etch.Terminal;

public static class ConsoleExtensions
{
    extension(System.Console)
    {
        public static IContext GetContext()
        {
            return new Context();
        }
    }
}
