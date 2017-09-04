using SpeechLib;

namespace EliteVoice
{
    internal class Speech
    {
        private readonly ISpeechObjectTokens _voices;

        private Speech()
        {
            _voices = speech.GetVoices();
        }

        public static Speech Instance { get; } = new Speech();
        public SpVoice speech { get; } = new SpVoice();

        private void SetDefaults()
        {
            //speech.Voice = 
        }

        public ISpeechObjectToken GetVoice(string name)
        {
            ISpeechObjectToken result = null;

            foreach (ISpeechObjectToken voice in _voices)
                if (voice.GetDescription().Equals(name))
                {
                    result = voice;
                    break;
                }

            return result;
        }

        public void Speak(string text)
        {
            speech.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
            speech.WaitUntilDone(-1);
        }
    }
}