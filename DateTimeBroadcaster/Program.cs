using System;
using CommandLine;
using WebSocketSharp.Server;

namespace DateTimeBroadcaster
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CmdOptions>(args)
                .WithParsed(opts =>
                {
                    WebSocketServer server = new WebSocketServer(opts.Port, false);
                    server.AddWebSocketService<DateTimeService>(opts.Path);
                    server.Start();
                    Console.ReadKey(true);
                    server.Stop();
                })
                .WithNotParsed(opts =>
                {
                    Environment.Exit(-2);
                });
        }
    }
}
