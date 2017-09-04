using System.Collections.Generic;

namespace EliteVoice.ConfigReader.Commands
{
    internal class InitCommand : AbstractCommand
    {
        public override int RunCommand(IDictionary<string, object> parameters)
        {
            RunChilds(parameters);
            return 0;
        }
    }
}