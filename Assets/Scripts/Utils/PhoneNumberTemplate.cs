using System.Text;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Utils
{
    public class PhoneNumberTemplate
    {
        public const char DefaultCharacter = '_';
        public const string CharactersType = @"\d";

        private string _result = string.Empty;
        private readonly string _regionTemplate;
        private readonly Regex _regex;
        private bool _isDeleting;
        private char _deletedCharacter;
        private StringBuilder _builder;
        private MatchCollection _matches;

        public PhoneNumberTemplate(string regionTemplate)
        {
            _regionTemplate = regionTemplate;
            _regex = new Regex(CharactersType);
        }

        public string ChangeCharacters(string input)
        {
            if (input == _result)
            {
                return input;
            }

            InitParameters(input);
            int index;

            ReplacePlaceholders(out index);
            CutEnd(index);

            _result = _builder.ToString();
            return _result;
        }

        public void InitParameters(string input)
        {
            _isDeleting = _result.Length > input.Length;
            if (_isDeleting && !string.IsNullOrEmpty(_result))
            {
                _deletedCharacter = _result[_result.Length - 1];
            }

            _builder = new StringBuilder(_regionTemplate);
            _matches = _regex.Matches(input);
        }

        public void CutEnd(int index)
        {
            if (_matches.Count == 0)
            {
                _builder.RemoveToEnd(0);
                return;
            }

            if (index == -1)
            {
                return;
            }

            if (_isDeleting && !_regex.IsMatch(_deletedCharacter.ToString()))
            {
                _builder.RemoveToEnd(index);
            }
            else if (index < _builder.Length - 1 && _builder[index + 1] == DefaultCharacter)
            {
                _builder.RemoveToEnd(index + 1);
            }
            else if (index < _builder.Length - 2)
            {
                _builder.RemoveToEnd(index + 2);
            }
        }

        public void ReplacePlaceholders(out int index)
        {
            index = -1;
            for (int i = 0; i < _matches.Count; i++)
            {
                index = _builder.IndexOf(DefaultCharacter);
                if (index > -1)
                {
                    _builder.Replace(DefaultCharacter, _matches[i].Value[0], index, 1);
                }
            }
        }
    }
}
