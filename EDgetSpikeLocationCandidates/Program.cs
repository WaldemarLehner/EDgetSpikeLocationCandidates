using System;
using System.Threading.Tasks;

namespace EDgetSpikeLocationCandidates
{
    public static class Program
    {
        public static async Task<int> Main() => await new CliFx.CliApplicationBuilder().AddCommand(typeof(Commands.DefaultCommand)).Build().RunAsync();
    }
}
