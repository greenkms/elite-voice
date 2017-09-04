using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace EliteVoice
{
    internal class FileProcessor
    {
        private static readonly string _homeDir = Environment.ExpandEnvironmentVariables("%USERPROFILE%") +
                                                  "\\Saved Games\\Frontier Developments\\Elite Dangerous\\";

        private readonly List<FileDescription> _files;
        private readonly TextLogger _logger = TextLogger.Instance;
        private readonly CommandProcessor _processor;
        private StreamReader _reader;

        public FileProcessor(List<FileDescription> files, CommandProcessor processor)
        {
            _files = files;
            _processor = processor;
        }

        public void DirectoryRead()
        {
            try
            {
                var reg = new Regex("^.*/([^/]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                var lastCount = 0;
                while (true)
                {
                    var filenames = Directory.GetFiles(_homeDir, "*.log");
                    if (filenames.Length != lastCount)
                    {
                        _files.Clear();
                        var filesTmp = new List<FileDescription>();
                        foreach (var filename in filenames)
                            filesTmp.Add(new FileDescription(reg.Replace(filename.Replace('\\', '/'), "$1"),
                                File.GetCreationTime(filename).ToString()));
                        lastCount = filenames.Length;
                        filesTmp.Sort(CompareByCreateTime);
                        filesTmp.ForEach(delegate(FileDescription fd) { _files.Add(fd); });
                        /*
                         * get reader
                         */
                        var skip = _reader == null;
                        _reader = null;
                        var fn = _homeDir + _files[_files.Count - 1].Name;
                        _logger.Log("Using journal file: " + fn);
                        var fs = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        _reader = new StreamReader(fs, Encoding.UTF8);
                        if (skip)
                            _reader.ReadToEnd();
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e1)
            {
                // ignored
            }
        }

        private static int CompareByCreateTime(FileDescription a, FileDescription b)
        {
            return File.GetCreationTime(_homeDir + a.Name).CompareTo(File.GetCreationTime(_homeDir + b.Name));
            //return a.createDate.CompareTo(b.createDate);
        }

        public void ProcessCurrentFile()
        {
            try
            {
                while (true)
                {
                    while (_reader == null)
                        Thread.Sleep(500);

                    string line;

                    while ((line = _reader.ReadLine()) != null)
                    {
                        _logger.Log("Read event line from journal file.");
                        _processor.Process(line);
                    }

                    Thread.Sleep(500);
                }
            }
            catch (Exception e1)
            {
                // ignored
            }
        }
    }
}