using System;
using System.Collections.Generic;
using System.Xml;
using EliteVoice.ConfigReader.Commands;

namespace EliteVoice.ConfigReader
{
    internal class ConfigReader
    {
        private readonly string _config;

        private readonly TextLogger _logger = TextLogger.Instance;
        private readonly IDictionary<string, Type> _registration = new Dictionary<string, Type>();

        public ConfigReader(string config)
        {
            _registration.Add("TextToSpeech", Type.GetType("EliteVoice.ConfigReader.Commands.TextToSpeechCommand"));
            _registration.Add("Text", Type.GetType("EliteVoice.ConfigReader.Commands.TextCommand"));
            _registration.Add("Pause", Type.GetType("EliteVoice.ConfigReader.Commands.PauseCommand"));
            //registration.Add("Play", Type.GetType("EliteVoice.ConfigReader.Commands.PlaySoundCommand"));
            _registration.Add("Play", Type.GetType("EliteVoice.ConfigReader.Commands.PlayCommand"));
            _registration.Add("Stop", Type.GetType("EliteVoice.ConfigReader.Commands.StopCommand"));
            _registration.Add("Randomize", Type.GetType("EliteVoice.ConfigReader.Commands.RandomizeCommand"));
            _registration.Add("Block", Type.GetType("EliteVoice.ConfigReader.Commands.BlockCommand"));

            _registration.Add("Switch", Type.GetType("EliteVoice.ConfigReader.Commands.SwitchCommand"));
            _registration.Add("Case", Type.GetType("EliteVoice.ConfigReader.Commands.CaseCommand"));
            _registration.Add("Default", Type.GetType("EliteVoice.ConfigReader.Commands.BlockCommand"));

            _registration.Add("Replace", Type.GetType("EliteVoice.ConfigReader.Commands.ReplaceCommand"));

            _config = config;
        }

        public IDictionary<string, EventsCommand> Events { get; } = new Dictionary<string, EventsCommand>();
        public InitCommand Init { get; } = new InitCommand();

        public ICommand GetEvent(string eventName)
        {
            ICommand result = null;
            if (Events.ContainsKey(eventName))
                result = Events[eventName];
            return result;
        }

        public void Parse()
        {
            var xml = new XmlDocument();
            try
            {
                xml.Load(_config);
                XmlNode root = xml.DocumentElement;
                if (root != null && root.HasChildNodes)
                    for (var i = 0; i < root.ChildNodes.Count; i++)
                        if (root.ChildNodes[i].NodeType == XmlNodeType.Element)
                        {
                            var elm = (XmlElement) root.ChildNodes[i];
                            var elmName = root.ChildNodes[i].Name;

                            if (elmName.Equals("Event"))
                            {
                                var name = elm.GetAttribute("name");
                                if (name.Length > 0)
                                {
                                    var command = new EventsCommand();
                                    ReadContent(elm, command);
                                    _logger.Log("Append command: {" + name + "}");
                                    Events.Add(name, command);
                                }
                            }
                            else if (elmName.Equals("Init"))
                            {
                                ReadContent(elm, Init);
                            }
                        }
            }
            catch (Exception e)
            {
                _logger.Log("Error parsing XML at \"" + _config + "\"");
            }
        }

        private void ReadContent(XmlElement current, ICommand parent)
        {
            for (var i = 0; i < current.Attributes.Count; i++)
                parent.AddProperty(current.Attributes[i].Name, current.Attributes[i].Value);
            parent.AddProperty("@text", current.InnerText);
            if (current.HasChildNodes)
                for (var i = 0; i < current.ChildNodes.Count; i++)
                    if (current.ChildNodes[i].NodeType == XmlNodeType.Element)
                    {
                        var cmdName = current.ChildNodes[i].Name;
                        if (_registration.ContainsKey(cmdName))
                        {
                            var command = (ICommand) Activator.CreateInstance(_registration[cmdName]);
                            ReadContent((XmlElement) current.ChildNodes[i], command);
                            parent.AddChild(command);
                        }
                    }
        }
    }
}