using PTD.Processing;
using PTD.Core.Config;

namespace PTD.Emit
{
    public class PipeLine
    {
        public static void StartProcessing()
        {
            Decompiler decompiler = new Decompiler();
            decompiler.DecompileCS(GlobalConfig.Files.InputFileLocation, GlobalConfig.Files.OutputFile);
        }
    }
}
