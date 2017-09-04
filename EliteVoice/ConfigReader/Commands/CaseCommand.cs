using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EliteVoice.ConfigReader.Commands
{
    internal class CaseCommand : AbstractCommand
    {
        private string _eq;
        private string _ieq;
        private Regex _reg;

        public override void AddProperty(string key, string value)
        {
            base.AddProperty(key, value);
            switch (key)
            {
                case "equal":
                case "equals":
                    _eq = value;
                    break;
                case "iequal":
                case "iequals":
                    _ieq = value.ToLower();
                    break;
                case "imatch":
                case "match":
                    try
                    {
                        var options = key.Equals("imatch")
                            ? RegexOptions.IgnoreCase | RegexOptions.Compiled
                            : RegexOptions.Compiled;
                        _reg = new Regex(value, options);
                    }
                    catch (ArgumentException e1)
                    {
                        _reg = null;
                        Logger.Log("Error parsing regexp \"" + value + "\"");
                    }
                    break;
            }
        }

        public override int RunCommand(IDictionary<string, object> parameters)
        {
            var result = 0;

            if (Parent.GetType() == typeof(SwitchCommand))
            {
                var success = false;
                var select = ((SwitchCommand) Parent).Select;
                Logger.Log("Select has value \"" + select + "\"");
                if (_reg != null)
                    success = _reg.IsMatch(select);
                else if (_eq != null)
                    success = _eq.Equals(select);
                else if (_ieq != null)
                    success = _ieq.Equals(select.ToLower());

                if (success)
                {
                    result = -1;
                    RunChilds(parameters);
                }
            }
            else
            {
                Logger.Log("Error! Case command no has parent Switch command!");
            }

            return result;
        }
    }
}