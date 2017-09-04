using System.Collections.Generic;
using System.IO;
using System.Threading;
using NAudio.Wave;

namespace EliteVoice.ConfigReader.Commands
{
    internal class PlayCommand : AbstractCommand
    {
        private bool _async;
        private AudioFileReader _audioFileReader;

        private float _volume = 0.5f;
        private readonly IWavePlayer _waveOutDevice = new WaveOut();
        public string Name { get; private set; }
        public bool IsOpen { get; private set; }

        public int FadeMills { set; private get; }

        private void InitializeParameters()
        {
            if (GetProperties().ContainsKey("name"))
                Name = GetProperties()["name"].ToLower();


            if (GetProperties().ContainsKey("async"))
                _async = "true".Equals(GetProperties()["async"]);

            var volume = -1;
            if (GetProperties().ContainsKey("volume"))
            {
                volume = int.Parse(GetProperties()["volume"]);
                if (volume < 0 || volume > 100)
                    volume = -1;
            }
            if (GetProperties().ContainsKey("file"))
            {
                var filename = GetProperties()["file"];
                Logger.Log("Try to play file: \"" + filename + "\"");
                if (File.Exists(filename))
                {
                    _audioFileReader = new AudioFileReader(filename);
                    if (volume > -1)
                    {
                        _volume = volume / 100.0f;
                        _audioFileReader.Volume = _volume;
                    }
                    _waveOutDevice.Init(_audioFileReader);
                    _waveOutDevice.PlaybackStopped += Player_PlayStateChange;
                }
                else
                {
                    Logger.Log("File NOT Found: \"" + filename + "\"!");
                }
            }

            EventContext.Instance.AddPlayer(this);
        }

        private void Player_PlayStateChange(object sender, StoppedEventArgs e)
        {
            //logger.log("Stop Playing");
            IsOpen = false;
        }

        public override int RunCommand(IDictionary<string, object> parameters)
        {
            if (_audioFileReader == null)
                InitializeParameters();
            else
                _audioFileReader.Position = 0;

            if (_audioFileReader != null)
            {
                IsOpen = true;
                _waveOutDevice.Play();

                while (!_async && IsOpen)
                    Thread.Sleep(500);
                //logger.log("Continue Playing");
                //isOpen = false;
            }
            return 0;
        }

        public void Fade()
        {
            //logger.log("Open state " + isOpen);

            if (IsOpen)
            {
                var oldVolume = _audioFileReader.Volume;
                if (FadeMills > 0)
                {
                    var steps = 10;
                    var volStep = oldVolume / steps;
                    var sleep = 1 + FadeMills / steps;
                    for (var i = 0; i < steps; i++)
                    {
                        _audioFileReader.Volume -= volStep;
                        Thread.Sleep(sleep);
                    }
                }
                _waveOutDevice.Stop();
                IsOpen = false;
                _audioFileReader.Volume = oldVolume;
            }
        }
    }
}