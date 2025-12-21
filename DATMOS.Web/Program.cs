using DATMOS.Web;
using Microsoft.Extensions.Hosting;

namespace DATMOS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create and run the host on port 5243 (as configured in launchSettings.json)
            var host = WebEntryPoint.CreateHostBuilder(args, 5243);
            host.Run();
        }
    }
}
