using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EliteVoice
{
    internal class CommandProcessor
    {
        private readonly ConfigReader.ConfigReader _config;
        //private JavaScriptSerializer json = new JavaScriptSerializer();

        private readonly TextLogger _logger = TextLogger.Instance;

        public CommandProcessor(ConfigReader.ConfigReader config)
        {
            _config = config;
            Init();
        }

        public void Init()
        {
            _config.Init.RunCommand(new Dictionary<string, object>());
        }

        public void Process(string jsonStr)
        {
            _logger.Log("Receive json: " + jsonStr);
            IDictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr);
            if (values.ContainsKey("event"))
            {
                var eventName = (string) values["event"];
                var command = _config.GetEvent(eventName);
                if (command != null)
                {
                    _logger.Log("Command successfully found for event: " + eventName);
                    try
                    {
                        foreach (var rp in EventContext.Instance.Replacers)
                            rp.Replace(values);
                    }
                    catch (Exception e)
                    {
                        _logger.Log("Replace error result: " + e.Message);
                    }
                    command.RunCommand(values);
                }
                else
                {
                    _logger.Log("No command found for event: " + eventName);
                }
            }
            else
            {
                _logger.Log("No event found!!!");
            }
        }
    }
}