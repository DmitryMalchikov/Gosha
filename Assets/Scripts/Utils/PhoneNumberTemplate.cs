using System.Text;
using System.Text.RegularExpressions;

public class PhoneNumberTemplate
{
    private const char defaultCharacter = '_';
    private const string charactersType = @"\d";
    private const char placeholder = '#';

    private string _result = string.Empty;
    private byte _previousLength = 0;
    private string _previousInput;

    public string ChangePhoneCharacters(string input, string regionTemplate)
    {
        if (input == _result)
        {
            return input;
        }
        bool isDeleting = _previousLength > input.Length;
        char deletedCharacter = ' ';
        _previousLength = (byte)input.Length;
        if (isDeleting && !string.IsNullOrEmpty(_previousInput))
        {
            deletedCharacter = _previousInput[_previousInput.Length - 1];
        }
        _previousInput = input;

        string template = regionTemplate.Replace(defaultCharacter, placeholder);

        StringBuilder builder = new StringBuilder(template);
        Regex reg = new Regex(charactersType);
        var matches = reg.Matches(input);
        int index = -1;

        for (int i = 0; i < matches.Count; i++)
        {
            index = -1;
            for (int j = 0; j < builder.Length; j++)
            {
                if (builder[j] == placeholder)
                {
                    index = j;
                    break;
                }
            }

            if (index > -1)
            {
                builder.Replace(placeholder, matches[i].Value[0], index, 1);
            }
        }


        for (int i = builder.Length - 1; builder.Length > 0 && i >= 0; i--)
        {
            if (builder[i] == placeholder)
            {
                builder.Remove(i, 1);
            }
            else if (!Regex.IsMatch(builder[i].ToString(), charactersType))
            {
                if (i > 0)
                {
                    if (isDeleting)
                    {
                        if (!Regex.IsMatch(builder[i].ToString(), charactersType))
                        {
                            if (Regex.IsMatch(builder[i - 1].ToString(), charactersType))
                            {
                                if (!Regex.IsMatch(deletedCharacter.ToString(), charactersType))
                                {
                                    builder.Remove(i - 1, 2);
                                    i--;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                builder.Remove(i, 1);
                            }
                        }
                        else
                        {
                            builder.Remove(i, 1);
                        }
                    }
                    else if (Regex.IsMatch(builder[i - 1].ToString(), charactersType))
                    {
                        break;
                    }
                    else
                    {
                        builder.Remove(i, 1);
                    }
                }
                else
                {
                    builder.Remove(i, 1);
                }
            }
            else
            {
                break;
            }
        }

        _result = builder.ToString();
        return _result;
    }
}
