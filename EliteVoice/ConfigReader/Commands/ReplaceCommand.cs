using System.Collections.Generic;

namespace EliteVoice.ConfigReader.Commands
{
    internal class ReplaceCommand : AbstractCommand
    {
        private string _ignorecase;

        private string _match;

        private string _replace;
        private string _source;

        private string _target;

        public override void AddProperty(string key, string value)
        {
            base.AddProperty(key, value);
            switch (key)
            {
                case "source":
                    _source = value;
                    break;
                case "match":
                    _match = value;
                    break;
                case "ignorecase":
                    _ignorecase = value;
                    break;
                case "replace":
                    _replace = value;
                    break;
                case "target":
                    _target = value;
                    break;
            }
        }

        public override int RunCommand(IDictionary<string, object> parameters)
        {
            var rp = new Replacer(_match, _replace, _source, _target, _ignorecase);
            if (rp.IsValid)
                EventContext.Instance.Replacers.Add(rp);
            return 0;
        }
    }
}