using System;
using System.Windows.Controls.Primitives;

namespace EliteVoice
{
    internal class TextLogger
    {
        private TextLogger()
        {
        }

        public static TextLogger Instance { get; } = new TextLogger();

        public TextBoxBase Output { set; get; }

        public void Log(string value)
        {
            if (Output != null)
                if (!Output.CheckAccess())
                {
                    Output.Dispatcher.Invoke(new Action<string>(Log), value + "\n");
                }
                else
                {
                    Output.AppendText(value);
                    Output.AppendText("\n");
                }
        }
    }
}