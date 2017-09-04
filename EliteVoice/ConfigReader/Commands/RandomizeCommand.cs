using System;
using System.Collections.Generic;
using System.Linq;

namespace EliteVoice.ConfigReader.Commands
{
    internal class RandomizeCommand : AbstractCommand
    {
        public override int RunCommand(IDictionary<string, object> parameters)
        {
            var childs = GetChilds();
            var priority = new LinkedList<int>();
            var scale = 0;
            foreach (var child in childs)
            {
                if (child.GetProperties().ContainsKey("priority"))
                {
                    var pr = int.Parse(child.GetProperties()["priority"]);
                    if (pr < 1)
                        pr = 1;
                    scale += pr;
                }
                else
                {
                    scale += 1;
                }
                priority.AddLast(scale);
            }

            var rand = new Random();

            var check = rand.Next(scale);
            var idx = 0;
            foreach (var pr in priority)
            {
                if (pr > check)
                    break;
                idx++;
            }

            childs.ElementAt(idx).RunCommand(parameters);

            return 0;
        }
    }
}