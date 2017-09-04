using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using SpeechLib;

namespace EliteVoice
{
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ISpeechObjectTokens audios;
        private readonly Thread lProcessor;
        private readonly Speech speech;
        private readonly Thread tProcessor;

        public MainWindow()
        {
            InitializeComponent();

            var logger = TextLogger.Instance;
            logger.Output = logTextBox;

            speech = Speech.Instance;
            audios = speech.speech.GetAudioOutputs();
            int idx = 0, i = 0;
            foreach (SpObjectToken audio in audios)
            {
                audioDevices.Items.Add(audio.GetDescription());
                if (audio.Equals(speech.speech.AudioOutput))
                    idx = i;
                i++;
            }
            audioDevices.SelectedIndex = idx;
            /*
             * 
             */
            logger.Log("Found voices:");
            foreach (ISpeechObjectToken voice in speech.speech.GetVoices())
                logger.Log(voice.GetDescription());
            /*
                * 
                */
            var config = new ConfigReader.ConfigReader("config/config.xml");
            config.Parse();
            /*
             * 
             */
            var commands = new CommandProcessor(config);
            var files = new List<FileDescription>();
            fileGrid.ItemsSource = files;
            var processor = new FileProcessor(files, commands);
            tProcessor = new Thread(processor.DirectoryRead);
            tProcessor.Start();
            lProcessor = new Thread(processor.ProcessCurrentFile);
            lProcessor.Start();
        }

        private void audioDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (audioDevices.SelectedIndex > -1)
                speech.speech.AudioOutput = audios.Item(audioDevices.SelectedIndex);
        }


        private void fileGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            fileGrid.SelectedIndex = fileGrid.Items.Count - 1;
            logTextBox.AppendText("Here\n");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                tProcessor.Interrupt();
            }
            catch (Exception e1)
            {
                // ignored
            }
            try
            {
                lProcessor.Interrupt();
            }
            catch (Exception e2)
            {
                // ignored
            }
        }

        private void logTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            logTextBox.ScrollToEnd();
        }
    }
}