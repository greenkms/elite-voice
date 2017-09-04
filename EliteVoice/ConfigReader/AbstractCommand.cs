using System.Collections.Generic;

namespace EliteVoice.ConfigReader
{
    internal abstract class AbstractCommand : ICommand
    {
        private readonly IDictionary<string, string> _properties = new Dictionary<string, string>();

        protected TextLogger Logger = TextLogger.Instance;
        public ICommand Parent { protected set; get; }
        protected LinkedList<ICommand> Commands { get; } = new LinkedList<ICommand>();

        public void AddChild(ICommand command)
        {
            ((AbstractCommand) command).Parent = this;
            Commands.AddLast(command);
        }

        public virtual void AddProperty(string key, string value)
        {
            _properties.Add(key, value);
        }

        public LinkedList<ICommand> GetChilds()
        {
            return Commands;
        }

        public IDictionary<string, string> GetProperties()
        {
            return _properties;
        }

        public abstract int RunCommand(IDictionary<string, object> parameters);

        protected void RunChilds(IDictionary<string, object> parameters)
        {
            foreach (var command in Commands)
                if (command.RunCommand(parameters) < 0)
                    break;
        }
    }
}