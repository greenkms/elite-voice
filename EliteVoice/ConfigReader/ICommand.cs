using System.Collections.Generic;

namespace EliteVoice.ConfigReader
{
    internal interface ICommand
    {
        LinkedList<ICommand> GetChilds();
        void AddChild(ICommand command);
        IDictionary<string, string> GetProperties();
        void AddProperty(string key, string value);

        int RunCommand(IDictionary<string, object> parameters);
    }
}