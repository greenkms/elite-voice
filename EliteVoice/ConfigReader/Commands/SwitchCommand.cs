using System.Collections.Generic;

namespace EliteVoice.ConfigReader.Commands
{
    internal class SwitchCommand : AbstractCommand
    {
        private string _selectParam;

        public string Select { get; private set; }

        public override void AddProperty(string key, string value)
        {
            base.AddProperty(key, value);
            switch (key)
            {
                case "select":
                    _selectParam = value;
                    break;
            }
        }

        public override int RunCommand(IDictionary<string, object> parameters)
        {
            Select = null;
            if (!string.IsNullOrEmpty(_selectParam))
            {
                Select = "";
                if (parameters.ContainsKey(_selectParam))
                    Select = parameters[_selectParam] != null ? parameters[_selectParam].ToString() : "";
                RunChilds(parameters);
            }
            else
            {
                Logger.Log("Check \"select\" parameter at Switch command!");
            }
            return 0;
        }
    }
}