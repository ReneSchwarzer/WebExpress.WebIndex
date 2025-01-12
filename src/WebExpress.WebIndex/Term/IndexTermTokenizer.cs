using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WebExpress.WebIndex.Term
{
    /// <summary>
    /// A whitespace tokenizer for breaking down a document into terms.
    /// </summary>
    public static class IndexTermTokenizer
    {
        /// <summary>
        /// Enumeration of separators.
        /// </summary>
        private static char[] Delimiters { get; } =
        [
            (char)0, (char)1,(char)2, (char)3, (char)4, (char)5, (char)6,
            (char)7, (char)8, (char)9, (char)10, (char)11, (char)12, (char)13,
            (char)14, (char)15, (char)16, (char)17, (char)18, (char)19,
            (char)20, (char)21, (char)22, (char)23, (char)24, (char)25,
            (char)26, (char)27, (char)28, (char)29, (char)30, (char) 31,
            '?', '!', '&', ':', ';', '<', '>', '=', '_', '~', '^', '#', '+',
            '|', '%', '(', ')', '[', ']', '{', '}', '"', '“', '”', '/', '§',
            '$', '◦', '•', '●', '■', '-', '—', '.', '\'', '@', '→', '¥',
            '★','*', '€', '¢', '£', '¥', '©', '░', '▒', '▓', '│', '┤', '├',
            '╣', '╠', '╬', '╩', '╦', '╚', '╔', '╗', '╝', '╜', '╛', '╙', '╘',
            '╕', '╖', '╓', '╒', '║', '═', '╏', '╎', '╍', '╌', '╋', '╊', '╉',
            '╈', '╇', '╆', '╅', '╄', '╃', '╂', '╁', '╀', '╹', '╸', '╷', '╶',
            '╵', '╴', '╳', '╲', '╱', '╰', '╯', '╮', '╭', '╫', '╪', '╨', '╧',
            '╥', '╤', '╢', '╡', '╟', '╞', '┑', '┏', '┎', '┛', '┓', '┐', '└',
            '┘', '┌', '┗', '┖', '┕', '┒', '┍', '┋', '┴', '┳', '┲', '┱', '┰',
            '┯', '┮', '┭', '┬', '┫', '┪', '┩', '┨', '┧', '┦', '┥', '┤', '┣',
            '┢', '┡', '┠', '┟', '┞', '┝', '┚', '┙', '┬', '─', '┼', '®', '¶',
            '„', '“', '◊', '†', '', '·', '¦', '➔', '➜', '➝', '➞', '➟',
            '➠', '➡', '➢', '➣', '➤', '➥', '➦', '➧', '➨', '➩', '➪',
            '➫', '➬', '➭', '➮', '➯', '➱', '➲', '➳', '➴', '➵', '➶',
            '➷', '➸', '➹', '➺', '➻', '➼', '➽', '➾', '➿', '⤴', '⤵',
            '⤶', '⤷', '⤸', '⤹', '⤺', '⤻', '⤼', '⤽', '⤾', '⤿', '⥀', '⥁',
            '⥂', '⥃', '⥄', '⥅', '⥆', '⥇', '⥈', '⥉', '⥊', '⥋', '⥌', '⥍',
            '⥎', '⥏', '⥐', '⥑', '±', '∓', '×', '÷', '∗', '∙', '√', '∛', '∜',
            '∝', '∟', '∠', '∡', '∢', '∣', '∤', '∥', '∦', '∧', '∨', '∩',
            '∪', '∫', '∬', '∭', '∮', '∯', '∰', '∱', '∲', '∳', '∴', '∵',
            '∶', '∷', '∸', '∹', '∺', '∻', '∼', '∽', '∾', '∿', '≀', '≁',
            '≂', '≃', '≄', '≅', '≆', '≇', '≈', '≉', '≊', '≋', '≌', '≍',
            '≎', '≏', '≐', '≑', '≒', '≓', '≔', '≕', '≖', '≗', '≘', '≙',
            '≚', '≛', '≜', '≝', '≞', '≟', '≠', '≡', '≢', '≣', '≤', '≥',
            '≦', '≧', '≨', '≩', '≪', '≫', '≬', '≭', '≮', '≯', '≰', '≱',
            '≲', '≳', '≴', '≵', '≶', '≷', '≸', '≹', '≺', '≻', '≼', '≽',
            '≾', '≿', '⊀', '⊁', '⊂', '⊃', '⊄', '⊅', '⊆', '⊇', '⊈', '⊉',
            '⊊', '⊋', '⊌', '⊍', '⊎', '⊏', '⊐', '⊑', '⊒', '⊓', '⊔', '⊕',
            '⊖', '⊗', '⊘', '⊙', '⊚', '⊛', '⊜', '⊝', '⊞', '⊟', '⊠', '⊡',
            '⊢', '⊣', '⊤', '⊥', '⊦', '⊧', '⊨', '⊩', '⊪', '⊫', '⊬', '⊭',
            '⊮', '⊯', '⊰', '⊱', '⊲', '⊳', '⊴', '⊵', '⊶', '⊷', '⊸', '⊹',
            '⊺', '⊻', '⊼', '⊽', '⊾', '⊿', '⋀', '⋁', '⋂', '⋃', '⋄', '⋅',
            '⋆', '⋇', '⋈', '⋉', '⋊', '⋋', '⋌', '⋍', '⋎', '⋏', '⋐', '⋑',
            '⋒', '⋓', '⋔', '⋕', '⋖', '⋗', '⋘', '⋙', '⋚', '⋛', '⋜', '⋝',
            '⋞', '⋟', '⋠', '⋡', '⋢', '⋣', '⋤', '⋥', '⋦', '⋧', '⋨', '⋩',
            '⋪', '⋫', '⋬', '⋭', '⋮', '⋯', '⋰', '⋱', '⋲', '⋳', '⋴', '⋵',
            '⋶', '⋷', '⋸', '⋹', '⋺', '⋻', '⋼', '⋽', '⋾', '⋿', '⌀', '⌁',
            '⌂', '⌃', '⌄', '⌅', '⌆', '⌇', '⌈', '⌉', '⌊', '⌋', '⌌', '⌍',
            '⌎', '⌏', '⌐', '⌑', '⌒', '⌓', '⌔', '⌕', '⌖', '⌗', '⌘', '⌙',
            '⌚', '⌛', '⌜', '⌝', '⌞', '⌟', '⌠', '⌡', '⌢', '⌣', '⌤', '⌥',
            '⌦', '⌧', '⌨', '〈', '〉', '⌫', '⌬', '⌭', '⌮', '⌯', '⌰', '⌱',
            '⌲', '⌳', '⌴', '⌵', '⌶', '⌷', '⌸', '⌹', '⌺', '⌻', '⌼', '⌽',
            '⌾', '⌿', '⍀', '⍁', '⍂', '⍃', '⍄', '⍅', '⍆', '⍇', '⍈', '⍉',
            '⍊', '⍋', '⍌', '⍍', '⍎', '⍏', '⍐', '⍑', '⍒', '⍓', '⍔', '⍕',
            '⍖', '⍗', '⍘', '⍙', '⍚', '⍛', '⍜', '⍝', '⍞', '⍟', '⍠', '⍡',
            '⍢', '⍣', '⍤', '⍥', '⍦', '⍧', '⍨', '⍩', '⍪', '⍫', '⍬', '⍭',
            '⍮', '⍯', '⍰', '⍱', '⍲', '⍳', '⍴', '⍵', '⍶', '⍷', '⍸', '⍹',
            '⍺', '⍻', '⍼', '⍽', '⍾', '⍿', '⎀', '⎁', '⎂', '⎃', '⎄', '⎅',
            '⎆', '⎇', '⎈', '⎉', '⎊', '⎋', '⎌', '⎍', '⎎', '⎏', '⎐', '⎑',
            '⎒', '⎓', '⎔', '⎕', '⎖', '⎗', '⎘', '⎙', '⎚', '⎛', '⎜', '⎝',
            '⎞', '⎟', '⎠', '⎡', '⎢', '⎣', '⎤', '⎥', '⎦', '⎧', '⎨', '⎩',
            '⎪', '⎫', '⎬', '⎭', '⎮', '⎯', '⎰', '⎱', '⎲', '⎳', '⎴', '⎵',
            '⎶', '⎷', '⎸', '⎹', '⎺', '⎻', '⎼', '⎽', '⎾', '⎿', '⏀', '⏁',
            '⏂', '⏃', '⏄', '⏅', '⏆', '⏇', '⏈', '⏉', '⏊', '⏋', '⏌', '⏍',
            '⏎', '⏏', '⏐', '⏑', '⏒', '⏓', '⏔', '⏕', '⏖', '⏗', '⏘', '⏙',
            '⏚', '⏛', '⏜', '⏝', '⏞', '⏟', '⏠', '⏡', '⏢', '⏣', '⏤', '⏥',
            '⏦', '⏧', '⏨', '⏩', '⏪', '⏫', '⏬', '⏭', '⏮', '⏯', '⏰', '⏱',
            '⏲', '⏳', '⏴', '⏵', '⏶', '⏷', '⏸', '⏹', '⏺', '⏻', '⏼', '⏽',
            '⏾', '⏿', '␀', '␁', '␂', '␃', '␄', '␅', '␆', '␇', '␈', '␉',
            '␊', '␋', '␌', '␍', '␎', '␏', '␐', '␑', '␒', '␓', '␔', '␕',
            '␖', '␗', '␘', '␙', '␚', '␛', '␜', '␝', '␞', '␟', '␠', '␡',
            '␢', '␣', '␤', '␥', '␦', '␧', '␨', '␩', '␪', '␫', '␬', '␭',
            '␮', '␯', '␰', '␱', '␲', '␳', '␴', '␵', '␶', '␷', '␸', '␹',
            '␺', '␻', '␼', '␽', '␾', '␿', '⑀', '⑁', '⑂', '⑃', '⑄', '⑅',
            '⑆', '⑇', '⑈', '⑉', '⑊', '⑋', '⑌', '⑍', '⑎', '⑏', '⑐', '⑑',
            '⑒', '⑓', '⑔', '⑕', '⑖', '⑗', '⑘', '⑙', '⑚', '⑛', '⑜', '⑝',
            '⑞', '⑟', '①', '②', '③', '④', '⑤', '⑥', '⑦', '⑧', '⑨', '⑩',
            '⑪', '⑫', '⑬', '⑭', '⑮', '⑯', '⑰', '⑱', '⑲', '⑳', '⑴', '⑵',
            '⑶', '⑷', '⑸', '⑹', '⑺', '⑻', '⑼', '⑽', '⑾', '⑿', '⒀', '⒁',
            '⒂', '⒃', '⒄', '⒅', '⒆', '⒇', '⒈', '⒉', '⒊', '⒋', '⒌', '⒍',
            '⒎', '⒏', '⒐', '⒑', '⒒', '⒓', '⒔', '⒕', '⒖', '⒗', '⒘', '⒙',
            '⒚', '⒛', '⒜', '⒝', '⒞', '⒟', '⒠', '⒡', '⒢', '⒣', '⒤', '⒥',
            '⒦', '⒧', '⒨', '⒩', '⒪', '⒫', '⒬', '⒭', '⒮', '⒯', '⒰', '⒱',
            '⒲', '⒳', '⒴', '⒵', 'Ⓐ', 'Ⓑ', 'Ⓒ', 'Ⓓ', 'Ⓔ', 'Ⓕ', 'Ⓖ', 'Ⓗ',
            'Ⓘ', 'Ⓙ', 'Ⓚ', 'Ⓛ', 'Ⓜ', 'Ⓝ', '⹄', 'Ⓟ', 'Ⓠ', 'Ⓡ', 'Ⓢ', 'Ⓣ',
            'Ⓤ', 'Ⓥ', 'Ⓦ', 'Ⓧ', 'Ⓨ', 'Ⓩ', 'ⓐ', 'ⓑ', 'ⓒ', 'ⓓ', 'ⓔ', 'ⓕ',
            'ⓖ', 'ⓗ', 'ⓘ', 'ⓙ', 'ⓚ', 'ⓛ', 'ⓜ', 'ⓝ', 'ⓞ', 'ⓟ', 'ⓠ', 'ⓡ',
            'ⓢ', 'ⓣ', 'ⓤ', 'ⓥ', 'ⓦ', 'ⓧ', 'ⓨ', 'ⓩ', '⓪', '⓫', '⓬', '⓭',
            '⓮', '⓯', '⓰', '⓱', '⓲', '⓳', '⓴', '⓵', '⓶', '⓷', '⓸', '⓹',
            '⓺', '⓻', '⓼', '⓽', '⓾', '⓿', '❶', '❷', '❸', '❹', '❺', '❻',
            '❼', '❽', '❾', '❿', '➀', '➁', '➂', '➃', '➄', '➅', '➆', '➇',
            '➈', '➉', '➊', '➋', '➌', '➍', '➎', '➏', '➐', '➑', '➒', '➓',
            '➕', '➖', '➗', '➘', '➙', '➚', '➛', '←', '↑', '→', '↓', '↔',
            '↕', '↖', '↗', '↘', '↙', '↚', '↛', '↜', '↝', '↞', '↟', '↠',
            '⟵', '⟶', '⟷', '↡', '↢', '↣', '↤', '↥', '↦', '↧', '↨', '↩',
        ];

        /// <summary>
        /// Enumeration of wildcards.
        /// </summary>
        public static char[] Wildcards { get; } = { '?', '*' };

        /// <summary>
        /// Tokenize an input string into an enumeration of terms.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="wildcards">An enumeration of wildcards.</param>
        /// <returns>An enumeration of terms.</returns>
        public static IEnumerable<IndexTermToken> Tokenize(string input, CultureInfo culture, char[] wildcards = null)
        {
            if (string.IsNullOrEmpty(input))
            {
                yield break;
            }

            var currentToken = new StringBuilder();
            var position = 0u;
            var except = new HashSet<char>(Delimiters.Except(wildcards ?? new char[0]));
            var isString = false;
            var isNumber = false;
            var hasDecimal = false;
            var hasExponent = false;
            var decimalSeparator = culture.NumberFormat.NumberDecimalSeparator[0];
            var groupSeparator = culture.NumberFormat.NumberGroupSeparator[0];
            var infinitySymbol = culture.NumberFormat.PositiveInfinitySymbol[0];
            var positiveSign = culture.NumberFormat.PositiveSign[0];
            var negativeSign = culture.NumberFormat.NegativeSign[0];

            for (int i = 0; i < input.Length; i++)
            {
                var current = input[i];
                var last = i > 0 ? input[i - 1] : (char)0;
                var next = i + 1 < input.Length ? input[i + 1] : (char)0;

                if (!isString && (char.IsDigit(current) ||
                    (isNumber && (current == decimalSeparator && !hasDecimal ||
                    current == groupSeparator || current == ',' ||
                    current == 'e' || current == 'E')) ||
                    (current == negativeSign && (currentToken.Length == 0 || hasExponent) &&
                    (char.IsDigit(next) || next == infinitySymbol)) ||
                    (current == positiveSign && (currentToken.Length == 0 || hasExponent) &&
                    (char.IsDigit(next) || next == infinitySymbol)) ||
                    current == infinitySymbol))
                {
                    if (!isNumber)
                    {
                        if (currentToken.Length > 0)
                        {
                            yield return new IndexTermToken
                            {
                                Position = position,
                                Value = Convert(currentToken, false, culture)
                            };
                            currentToken.Clear();
                        }
                        isNumber = true;
                        hasDecimal = current == decimalSeparator;
                        hasExponent = current == 'e' || current == 'E';
                    }
                    else if (current == decimalSeparator)
                    {
                        if (!hasDecimal)
                        {
                            hasDecimal = true;
                        }
                        else
                        {
                            yield return new IndexTermToken
                            {
                                Position = position,
                                Value = Convert(currentToken, true, culture)
                            };
                            currentToken.Clear();
                            isNumber = false;
                            hasDecimal = false;
                            hasExponent = false;
                            position++;
                            continue;
                        }
                    }
                    else if (current == groupSeparator)
                    {
                        if (!char.IsDigit(last) || !char.IsDigit(next) || hasDecimal)
                        {
                            yield return new IndexTermToken
                            {
                                Position = position,
                                Value = Convert(currentToken, true, culture)
                            };
                            currentToken.Clear();
                            isNumber = false;
                            hasDecimal = false;
                            hasExponent = false;
                            position++;
                        }
                        continue;
                    }
                    else if (current == 'e' || current == 'E')
                    {
                        if (!hasExponent)
                        {
                            hasExponent = true;
                        }
                        else
                        {
                            yield return new IndexTermToken
                            {
                                Position = position,
                                Value = Convert(currentToken, true, culture)
                            };
                            currentToken.Clear();
                            isNumber = false;
                            hasDecimal = false;
                            hasExponent = false;
                            position++;
                            continue;
                        }
                    }
                    currentToken.Append(current);
                }
                else
                {
                    if (isNumber)
                    {
                        yield return new IndexTermToken
                        {
                            Position = position,
                            Value = Convert(currentToken, true, culture)
                        };
                        currentToken.Clear();
                        isNumber = false;
                        hasDecimal = false;
                        hasExponent = false;
                        position++;
                    }

                    if (char.IsWhiteSpace(current) || except.Contains(current))
                    {
                        if (currentToken.Length > 0)
                        {
                            yield return new IndexTermToken
                            {
                                Position = position,
                                Value = Convert(currentToken, false, culture)
                            };
                            currentToken.Clear();
                            isString = false;
                        }
                        position++;
                    }
                    else
                    {
                        currentToken.Append(current);
                        isString = true;
                    }
                }
            }

            if (currentToken.Length > 0)
            {
                yield return new IndexTermToken
                {
                    Position = position,
                    Value = Convert(currentToken, isNumber, culture)
                };
            }
        }

        /// <summary>
        /// Converts the content to its appropriate data type.
        /// </summary>
        /// <param name="sb">The object containing the string to convert.</param>
        /// <param name="isNumber">A flag indicating whether the string represents a numeric value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// Returns a double if the string represents a numeric value and conversion is successful; otherwise, returns the string itself.
        /// </returns>
        private static object Convert(StringBuilder sb, bool isNumber, CultureInfo culture)
        {
            var res = sb.ToString();

            if (isNumber)
            {
                if (double.TryParse(res, NumberStyles.Any, culture, out double number))
                {
                    return number;
                }
                else if (double.TryParse(res, NumberStyles.Any, CultureInfo.InvariantCulture, out double number1))
                {
                    return number1;
                }
            }

            return res;
        }
    }
}
