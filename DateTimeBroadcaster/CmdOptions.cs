using CommandLine;

namespace DateTimeBroadcaster
{
    public class CmdOptions
    {
        [Option("port", Default = 80, MetaValue = "INT", HelpText = "port of the service listens on")]
        public int Port { get; set; }

        [Option("path", Default = "/", HelpText = "path of the service")]
        public string Path { get; set; }
    }
}
