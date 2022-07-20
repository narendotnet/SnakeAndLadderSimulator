using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;

namespace SnakeAndLadder
{
    public class Program
    {
        static void Main(string[] args)
        {
            string[] strings = { "Volvo", "BMW", "Ford", "Mazda" };
            string quote = ", ";
            var res = ToCommaSeparatedString(strings,quote);
            var services = new ServiceCollection();
            //services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IGame, Game>();
            services.AddSingleton<GameApp>();
            var serviceProvider = services.BuildServiceProvider();
            var userAppService = serviceProvider.GetService<GameApp>();
            userAppService.StartSimulation();
        }

        public static string ToCommaSeparatedString(string[] items, string quote) //corrected the method name
        {
            //refactored/simplified the code using LINQ
            string result = items.Aggregate((x, y) => x + quote + y);
            return result;
        }
    }
}
