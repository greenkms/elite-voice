using System.Collections.Generic;
using System.Threading;

namespace EliteVoice.ConfigReader.Commands
{
    internal class PauseCommand : AbstractCommand
    {
        public override int RunCommand(IDictionary<string, object> parameters)
        {
            if (GetProperties().ContainsKey("value"))
            {
                var val = int.Parse(GetProperties()["value"]);
                Thread.Sleep(val);
            }
            return 0;
        }
    }
}