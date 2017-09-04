using System.Collections.Generic;
using EliteVoice.ConfigReader;
using EliteVoice.ConfigReader.Commands;

namespace EliteVoice
{
    internal class EventContext
    {
        private readonly List<PlayCommand> _players = new List<PlayCommand>();
        public static EventContext Instance { get; } = new EventContext();

        public List<Replacer> Replacers { get; } = new List<Replacer>();

        public List<PlayCommand> GetPlayersByName(string name = null)
        {
            List<PlayCommand> result;
            if (name != null)
            {
                result = new List<PlayCommand>();
                name = name.ToLower();
                foreach (var player in _players)
                    if (name.Equals(player.Name))
                        result.Add(player);
            }
            else
            {
                result = _players;
            }

            return result;
        }

        public void AddPlayer(PlayCommand player)
        {
            _players.Add(player);
        }
    }
}