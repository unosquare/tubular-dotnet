using System;
using System.IO;
using EmbedIO;
using EmbedIO.WebApi;
using Swan;

namespace Unosquare.Tubular.EmbedioSample
{
    internal class Program
    {
        /// <summary>
        /// Gets the HTML root path.
        /// </summary>
        /// <value>
        /// The HTML root path.
        /// </value>
        public static string HtmlRootPath => Path.Combine(SwanRuntime.EntryAssemblyDirectory, "html");

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            var url = args.Length > 0 ? args[0] : "http://localhost:9696/";

            // Our web server is disposable. Note that if you don't want to use logging,
            // there are alternate constructors that allow you to skip specifying an ILog object.
            using (var server = new WebServer(url))
            {
                // First, we will configure our web server by adding Modules.
                server
                    .WithWebApi("/api", m => m
                        .WithController<PeopleController>())
                    .WithStaticFolder("/", HtmlRootPath, true);

                // Once we've registered our modules and configured them, we call the RunAsync() method.
                // This is a non-blocking method (it return immediately) so in this case we avoid
                // disposing of the object until a key is pressed.
                server.RunAsync();

                // Fire up the browser to show the content if we are debugging!
                var browser = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true }
                };
                browser.Start();

                // Wait for any key to be pressed before disposing of our web server.
                // In a service we'd manage the lifecycle of of our web server using
                // something like a BackgroundWorker or a ManualResetEvent.
                Console.ReadKey(true);
            }
        }
    }
}