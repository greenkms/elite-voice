using System.Collections.Generic;
using SpeechLib;

namespace EliteVoice.ConfigReader.Commands
{
    internal class TextToSpeechCommand : AbstractCommand
    {
        public override int RunCommand(IDictionary<string, object> parameters)
        {
            var sp = Speech.Instance;

            /*
             * Store values
             */
            var prevVoice = sp.speech.Voice;
            var prevVolume = sp.speech.Volume;
            var prevRate = sp.speech.Rate;


            foreach (var key in GetProperties().Keys)
            {
                var value = GetProperties()[key];
                switch (key)
                {
                    case "voice":
                        var voice = sp.GetVoice(value);
                        if (voice != null)
                            sp.speech.Voice = (SpObjectToken) voice;
                        break;
                    case "volume":
                        var volume = int.Parse(value);
                        if (volume > -1 && volume < 101)
                            sp.speech.Volume = volume;
                        break;
                    case "rate":
                        var rate = int.Parse(value);
                        if (rate > -11 && rate < 11)
                            sp.speech.Rate = rate;
                        break;
                }
            }
            RunChilds(parameters);
            if (Commands.Count > 0)
            {
                sp.speech.Voice = prevVoice;
                sp.speech.Volume = prevVolume;
                sp.speech.Rate = prevRate;
            }
            return 0;
        }
    }
}