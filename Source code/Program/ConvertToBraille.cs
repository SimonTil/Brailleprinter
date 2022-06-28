using System;
using System.Text;

using System.Text.RegularExpressions;

namespace Braille_plotter
{
    public partial class MainForm
    {
        int lineWidth = 28;
        private string convertToBraille(string inputText)
        {
            // Replace capital letters and numbers
            string[] regels = inputText.Split('\n');
            string samengesteld = "";
            for (int i = 0; i < regels.Length; i++)
            {
                samengesteld += replaceCaps(regels[i]);
                samengesteld = replaceNumbers(samengesteld);
                if (i + 1 != regels.Length)
                {
                    samengesteld += "\n";
                }
            }

            // Replace special characters and remove unknown characters
            samengesteld = replaceSpecialCharacters(samengesteld);
            samengesteld = removeUnknownCharacters(samengesteld);

            // Split long words
            regels = samengesteld.Split('\n');
            samengesteld = "";
            for (int i = 0; i < regels.Length; i++)
            {
                samengesteld += replaceLongWords(regels[i]);
                if (i + 1 != regels.Length)
                {
                    samengesteld += "\n";
                }
            }

            // Split to max line width
            samengesteld = splitToLineLength(samengesteld);

            // End of file
            samengesteld = (char)7 + samengesteld + (char)7; // ding dong, end of file!
            return samengesteld;
        }

        private string replaceCaps(string input)
        {
            // Permanent cap modus
            bool capmodus = false;
            string[] words = input.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                if (words.Length > 1 && permanentCaps(words[i]))
                {
                    if (!capmodus)
                    {
                        if (singleWordCaps(words[i]) && permanentCaps(words[i]))
                        {
                            if (words[i].Length > 1)
                            {
                                words[i] = "P" + words[i].ToLower();
                            }
                            else
                            {
                                if (isUpper(words[i]))
                                {
                                    words[i] = "C" + words[i].ToLower();
                                }
                                else
                                {
                                    words[i] = words[i].ToLower();
                                }
                            }
                            if (i + 2 < words.Length)
                            {
                                bool isNumbersOnly = true;
                                if (number(words[i + 1]) || number(words[i + 2]))
                                {
                                    for (int j = i + 2; j < words.Length; j++)
                                    {
                                        isNumbersOnly = number(words[j]) ? true : false;
                                        if (!isNumbersOnly)
                                        {
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    isNumbersOnly = false;
                                }
                                if (permanentCaps(words[i + 1]) && permanentCaps(words[i + 2]))
                                {
                                    capmodus = true;
                                    words[i] = "P" + words[i];
                                }
                            }
                        }
                        // else
                        // {
                        //     words[i] = convertCapsInWord(words[i]);
                        // }
                    }
                    else if (capmodus)
                    {
                        if (i + 1 < words.Length)
                        {
                            bool isNumbersOnly = true;
                            if (number(words[i + 1]))
                            {
                                isNumbersOnly = true;
                                for (int j = i + 1; j < words.Length; j++)
                                {
                                    isNumbersOnly = number(words[j]) ? true : false;
                                    if (!isNumbersOnly)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                isNumbersOnly = false;
                            }
                            
                            if (isNumbersOnly)
                            {
                                words[i] = "P" + words[i].ToLower();
                                capmodus = false;
                            }
                            else
                            {
                                if (!permanentCaps(words[i + 1]))
                                {
                                    words[i] = "P" + words[i].ToLower();
                                    capmodus = false;
                                }
                                else
                                {
                                    words[i] = words[i].ToLower();
                                }
                            }
                        }
                        else
                        {
                            words[i] = "P" + words[i].ToLower();
                            capmodus = false;
                        }
                    }
                }
                else
                {
                    // not entire caps, but possible more than 1 cap, loop through word
                    capmodus = false;
                    words[i] = convertCapsInWord(words[i]);
                }
            }
            string output = string.Join(" ", words);
            return output;
        }
        private string replaceNumbers(string input)
        {
            var pattern = @"(\d[\d.,]*)([a-j€$©])?";
            return Regex.Replace(input, pattern, Callback);
        }
        private string Callback(Match match)
        {
            var output = "N";
            foreach (var c in match.Groups[1].Value)
            {
                output += (char)(char.IsDigit(c) ? (c == '0' ? 'j' : c + 'a' - '1') : c);
            }
            if (match.Groups[2].Value.Length != 0)
            {
                output += "R" + match.Groups[2].Value;
            }

            return output;
        }
        private string replaceSpecialCharacters(string input)
        {
            string output = "";
            for (int i = 0; i < input.Length; i++)
            {
                // Add key "K" before special characters
                Regex rg = new Regex(@"[\\#~©®™<>\{\}]");
                if (rg.IsMatch(input[i].ToString()))
                {
                    output += "K";
                }

                output += input[i];
            }

            // Add omen "O" before special character
            output = output.Replace("°", "O°");

            // Replace currency scripts
            output = Regex.Replace(output, @"\x20*€\x20*", " e");
            output = Regex.Replace(output, @"\x20*\$\x20*", " d");
            output = Regex.Replace(output, @"\x20*£\x20*", " p");
            output = Regex.Replace(output, @"\x20*¥\x20*", " y");

            // Replace superscript characters
            output = output.Replace("²", "^Nb");
            output = output.Replace("³", "^Nc");

            // Replace special characters
            output = output.Replace("±", "+-");
            output = output.Replace("‰", "%%");
            output = output.Replace("™", "tm");
            output = output.Replace("©", "c");
            output = output.Replace("®", "r");

            // Add alphabet change "A" before non-Dutch character
            output = output.Replace("Cł", "ACl");
            output = output.Replace("ł", "Al");
            output = output.Replace("ñ", "An");
            output = output.Replace("ß", "ss");
            output = output.Replace("α", "Aa");
            output = output.Replace("β", "Ab");
            output = output.Replace("γ", "Ac");
            output = output.Replace("π", "Ap");
            output = output.Replace("Cω", "ACz");

            // Replace smart quotation marks
            output = Regex.Replace(output, @"[`´‘’]", "\'");
            output = Regex.Replace(output, @"[“”]", "\"");

            // Replace non-used diacritic accents
            output = Regex.Replace(output, @"[ò]", "o");
            output = Regex.Replace(output, @"[ýÿ]", "y");
            return output;
        }
        private string replaceLongWords(string input)
        {
            string[] words = input.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > lineWidth)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < words[i].Length; j++)
                    {
                        if (j % (lineWidth - 1) == 0)
                        {
                            sb.Append("K\n");
                        }

                        sb.Append(words[i][j]);
                    }
                    words[i] = sb.ToString();
                    words[i] = words[i].Remove(0, 2);
                }
            }
            string output = string.Join(" ", words);
            return output;
        }
        private string removeUnknownCharacters(string input)
        {
            string output = "";
            Regex rg = new Regex(@"[\x20-\x7Eäáàâç©ëéèê€ïíìîöóô§®üúùû\n]");
            for (int i = 0; i < input.Length; i++)
            {
                if (rg.IsMatch(input[i].ToString()))
                {
                    output += input[i];
                }
            }
            return output;
        }
        private string convertToBrailleCharacters(string input)
        {
            string brailleAlphabet = "⠁⠃⠉⠙⠑⠋⠛⠓⠊⠚⠅⠇⠍⠝⠕⠏⠟⠗⠎⠞⠥⠧⠺⠭⠽⠵";
            for (int i = 0; i < input.Length; i++)
            {
                if ((int)input[i] >= 0x61 && (int)input[i] <= 0x7A)
                {
                    input = input.Replace(input[i], brailleAlphabet[input[i] - 0x61]);
                }
            }

            // Convert tokens
            input = input.Replace("A", "⠰");
            input = input.Replace("C", "⠨");
            input = input.Replace("K", "⠐");
            input = input.Replace("N", "⠼");
            input = input.Replace("O", "⠈");
            input = input.Replace("P", "⠘");
            input = input.Replace("R", "⠠");

            // Convert special characters
            input = input.Replace(' ', '⠀'); // convert to unicode braille SP
            input = input.Replace(',', '⠂');
            input = input.Replace('\x27', '⠄'); // single quotation mark
            input = input.Replace(';', '⠆');
            input = Regex.Replace(input, @"[í/]", "⠌");
            input = input.Replace(':', '⠒');
            input = input.Replace('*', '⠔');
            input = Regex.Replace(input, @"[\+!]", "⠖");
            input = Regex.Replace(input, @"[@ä]", "⠜");
            input = input.Replace('â', '⠡');
            input = Regex.Replace(input, @"[~\?]", "⠢");
            input = input.Replace('ê', '⠣');
            input = input.Replace('-', '⠤');
            input = Regex.Replace(input, @"[\(×·]", "⠦");
            input = input.Replace('î', '⠩');
            input = input.Replace('ö', '⠪');
            input = input.Replace('ë', '⠫');
            input = Regex.Replace(input, @"[ó§\^]", "⠬");
            input = input.Replace('è', '⠮');
            input = input.Replace('&', '⠯');
            input = input.Replace('û', '⠱');
            input = input.Replace('.', '⠲');
            input = input.Replace('ü', '⠳');
            input = input.Replace('(', '⠴');
            input = Regex.Replace(input, @"[\x22=]", "⠶");
            input = Regex.Replace(input, @"[áà\[]", "⠷");
            input = input.Replace('|', '⠹');
            input = input.Replace('ï', '⠻');
            input = Regex.Replace(input, @"[úù\]]", "⠷");
            input = Regex.Replace(input, @"[é%]", "⠷");
            return input;
        }
        private string splitToLineLength(string input)
        {
            int currentLineLength = 0;

            // Split entire line to words, regels by enter, space or hyphen
            string[] regelsString = Regex.Split(input, @"(?<=[\n\-\s⠤⠀])");
            string output = "";

            for (int i = 0; i < regelsString.Length; i++)
            {
                int wordLength = Regex.Replace(regelsString[i], @"[⠀\n\s]+", "").Length;
                if (currentLineLength + wordLength <= lineWidth)
                {
                    output += regelsString[i];
                    currentLineLength += wordLength;
                    if (regelsString[i].Length != wordLength)
                    {
                        currentLineLength++;
                    }
                }
                else
                {
                    output += "\n" + regelsString[i];
                    currentLineLength = regelsString[i].Length;
                }
                if (output[output.Length - 1] == '\n')
                {
                    currentLineLength = 0;
                }
            }
            return output;
        }
        private bool isUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!char.IsUpper(input[i]))
                {
                    return false;
                }
            }
            return true;
        }
        private bool permanentCaps(string input)
        {
            // check if input string is not empty
            if (input.Length <= 0)
            {
                return false;
            }

            // remove some characters and numbers
            input = Regex.Replace(input, @"[\.,!?;:/*\-\+]", string.Empty);
            input = Regex.Replace(input, @"\d*", string.Empty);

            // ignore quotation marks and hyphens
            input = input.Replace("\'", string.Empty);
            input = input.Replace("\"", string.Empty);
            input = input.Replace("-", string.Empty);

            // if word is all caps
            return isUpper(input);
        }

        private bool singleWordCaps(string input)
        {
            // check if input string is not empty
            if (input.Length <= 0)
            {
                return false;
            }

            // remove some characters
            input = Regex.Replace(input, @"[\.,;:/*\+]", string.Empty);

            // ignore quotation marks and hyphens
            input = input.Replace("\'", string.Empty);
            input = input.Replace("\"", string.Empty);

            // if word is all caps
            return isUpper(input);
        }
        private bool number(string input)
        {
            if (input.Length == 0)
            {
                return false;
            }

            Regex regex = new Regex(@"^\d*(\.|,)?\d*$");
            return regex.IsMatch(input);
        }
        private string convertCapsInWord(string input)
        {
            string output = "";
            bool capmodus = false;

            if (input.Length > 0)
            {
                for (int c = 0; c < input.Length; c++)
                {
                    if (!capmodus)
                    {
                        if (c + 1 < input.Length)
                        {
                            if (char.IsUpper(input[c]) && char.IsUpper(input[c + 1]))
                            {
                                output += "P";
                                capmodus = true;
                            }
                            else if (char.IsUpper(input[c]))
                            {
                                output += "C";
                            }
                        }
                        else if (char.IsUpper(input[c]))
                        {
                            output += "C";
                        }
                        output += Char.ToLower(input[c]);
                    }
                    else
                    {
                        output += Char.ToLower(input[c]);
                        if (c + 1 < input.Length && !char.IsUpper(input[c + 1]))
                        {
                            if (!char.IsDigit(input[c + 1]) && input[c + 1] != '-')
                            {
                                Regex rg = new Regex(@"[\.,\!\?]");
                                Regex rg2 = new Regex(@"[A-Z]");
                                if (c + 2 < input.Length && !rg.IsMatch(input[c + 1].ToString()) && !rg2.IsMatch(input[c + 2].ToString()))
                                {
                                    output += "R";
                                }
                            }
                            capmodus = false;
                        }
                    }
                }
            }
            return output;
        }
        private char convertNumber(char input)
        {
            uint myInput = (uint)input;
            uint output = 0;
            if (myInput == 48)
            {
                output = 106;
            }

            if (myInput >= 49 && myInput <= 57)
            {
                output = myInput + 48;
            }
            return (char)output;
        }
    }
}