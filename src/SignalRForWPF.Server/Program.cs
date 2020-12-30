// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SignalRForWPF.Server
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The main program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().RunAsync();
        }

        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>A new <see cref="IHostBuilder"/>.</returns>
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(
                webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}