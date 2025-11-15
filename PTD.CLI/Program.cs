using PTD.Core.Config;
using PTD.Emit;

namespace PTD.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GlobalConfig.LoadConfig();

            PipeLine.StartProcessing();
        }
    }
}
