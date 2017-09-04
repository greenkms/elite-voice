using System.Collections.Generic;
using System.Threading;

namespace EliteVoice.ConfigReader.Commands
{
    internal class StopCommand : AbstractCommand
    {
        private int _fade;
        private string _name;

        public override void AddProperty(string key, string value)
        {
            base.AddProperty(key, value);
            switch (key)
            {
                case "name":
                    _name = value;
                    break;
                case "fade":
                    _fade = int.Parse(value);
                    if (_fade < 0)
                        _fade = 0;
                    break;
            }
        }

        public override int RunCommand(IDictionary<string, object> parameters)
        {
            var thread = new Thread(RunThreads);
            thread.Start();
            return 0;
        }

        private void RunThreads()
        {
            var players = EventContext.Instance.GetPlayersByName(_name);

            var threads = new List<Thread>();

            foreach (var player in players)
            {
                player.FadeMills = _fade;
                var thread = new Thread(player.Fade);
                threads.Add(thread);
                thread.Start();
            }

            var allStoped = false;
            while (!allStoped)
            {
                allStoped = true;
                foreach (var thread in threads)
                    if (thread.IsAlive)
                    {
                        allStoped = false;
                        break;
                    }
                if (!allStoped)
                    Thread.Sleep(100);
            }

            Logger.Log("Stop Play Threads");
        }
    }
}