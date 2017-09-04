using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EliteVoice.ConfigReader
{
    internal class Replacer
    {
        private readonly TextLogger _logger = TextLogger.Instance;

        private readonly Regex _match;

        private readonly string _replace;

        private readonly Regex _source;

        private readonly string _target;

        public Replacer(string match, string replace, string source, string target, string ingnoreCase)
        {
            if (replace == null || match == null && source == null || match == null && source != null && target == null)
            {
                _logger.Log("Ignore one or more Replace commands... Not enough arguments!!!");
            }
            else
            {
                IsValid = true;

                _target = target;
                _replace = replace;

                if (source != null)
                    try
                    {
                        _source = new Regex(source, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    }
                    catch (ArgumentException e1)
                    {
                        _source = null;
                        IsValid = false;
                        _logger.Log("Error parsing regexp \"" + source + "\"");
                    }
                if (match != null)
                    try
                    {
                        var options = !"false".Equals(ingnoreCase)
                            ? RegexOptions.IgnoreCase | RegexOptions.Compiled
                            : RegexOptions.Compiled;
                        _match = new Regex(match, options);
                    }
                    catch (ArgumentException e1)
                    {
                        _match = null;
                        IsValid = false;
                        _logger.Log("Error parsing regexp \"" + source + "\"");
                    }
            }
        }

        public bool IsValid { get; }


        public void Replace(IDictionary<string, object> parameters)
        {
            if (IsValid)
            {
                var keys = new List<string>(parameters.Keys);
                foreach (var key in keys)
                {
                    var value = parameters[key].ToString();

                    var restarget = _target == null ? key : _target;

                    if (_source == null || _source.IsMatch(key))
                        if (_match != null && _match.IsMatch(value))
                        {
                            value = _match.Replace(value, _replace);
                            parameters[restarget] = value;
                        }
                        else if (_match == null)
                        {
                            parameters[restarget] = _replace;
                        }
                }
            }
        }

        public string Replace(string text)
        {
            var result = text;
            if (IsValid && _match != null && _source == null && _target == null)
                result = _match.Replace(text, _replace);
            return result;
        }
    }
}