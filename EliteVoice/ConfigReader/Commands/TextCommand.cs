using System.Collections.Generic;

namespace EliteVoice.ConfigReader.Commands
{
    internal class TextCommand : AbstractCommand
    {
        public override int RunCommand(IDictionary<string, object> parameters)
        {
            var sp = Speech.Instance;
            string text = null;
            if (GetProperties().ContainsKey("select"))
            {
                var parameter = GetProperties()["select"];
                if (parameters.ContainsKey(parameter))
                {
                    //text = string.Format("{0:F0}",parameters[parameter]);
                    text = $"{parameters[parameter]:#.##;#.##;0}";
                    Logger.Log("Text Select = " + text);
                }
            }
            else if (GetProperties().ContainsKey("@text"))
            {
                text = GetProperties()["@text"];
                //logger.log("Replacers count " + EventContext.instance.replacers.Count);
                foreach (var rp in EventContext.Instance.Replacers)
                    text = rp.Replace(text);
            }

            if (!string.IsNullOrEmpty(text))
                sp.Speak(text);
            return 0;
        }
    }
}