using System;
namespace Compiler
{
    public class FatalException : Exception { }

    public class LexicalAnalyzer
    {
        public const byte
            star = 21, // *
            slash = 60, // /
            equal = 16, // =
            comma = 20, // ,
            semicolon = 14, // ;
            colon = 5, // :
            point = 61,	// .
            arrow = 62,	// ^
            leftpar = 9,	// (
            rightpar = 4,	// )
            lbracket = 11,	// [
            rbracket = 12,	// ]
            flpar = 63,	// {
            frpar = 64,	// }
            later = 65,	// <
            greater = 66,	// >
            laterequal = 67,	//  <=
            greaterequal = 68,	//  >=
            latergreater = 69,	//  <>
            plus = 70,	// +
            minus = 71,	// –
            lcomment = 72,	//  (*
            rcomment = 73,	//  *)
            assign = 51,	//  :=
            twopoints = 74,	//  ..
            ident = 2,	// идентификатор
            floatc = 82,	// вещественная константа
            charc = 75, // символьная константа
            intc = 15,	// целая константа
            casesy = 31,
            elsesy = 32,
            filesy = 57,
            gotosy = 33,
            thensy = 52,
            typesy = 34,
            untilsy = 53,
            dosy = 54,
            withsy = 37,
            ifsy = 56,
            insy = 100,
            ofsy = 101,
            orsy = 102,
            tosy = 103,
            endsy = 104,
            varsy = 105,
            divsy = 106, // возвращает целую часть от деления двух целых чисел
            andsy = 107,
            notsy = 108,
            forsy = 109,
            modsy = 110, // вычисляет остаток, полученный при выполнении целочисленного деления
            nilsy = 111,
            setsy = 112,
            beginsy = 113,
            whilesy = 114,
            arraysy = 115,
            constsy = 116,
            labelsy = 117,
            downtosy = 118,
            packedsy = 119,
            recordsy = 120,
            repeatsy = 121,
            programsy = 122,
            functionsy = 123,
            procedurensy = 124,
            integersy = 125,
            quoteOne = 126, // '
            quoteTwo = 127, // "
            comment = 128, // comment
            stringc = 129,
            writelnsy = 130,
            readlnsy = 131,
            stringsy = 132, // string
            floatsy = 133, // float
            booleansy = 133, // boolean
            charsy = 134, // char
            equalStar = 136, // *=
            equalSlash = 137, // /=
            equalMinus = 138, // -=
            equalPlus = 139, // +=
            truesy = 140, // +=
            falsesy = 141; // +=

        public static CToken symbol; // код символа
        public static TextPosition token; // позиция символа
        string addrName; // адрес идентификатора в таблице имен
        static int nmb_int; // значение целой константы
        static float nmb_float; // значение вещественной константы
        static char one_symbol; // значение символьной константы
        static int count = 0;
        
        public static CToken NextSym()
        {
            while (InputOutput.Ch == ' ') InputOutput.NextCh();
            token.lineNumber = InputOutput.positionNow.lineNumber;
            token.charNumber = InputOutput.positionNow.charNumber;

            switch (InputOutput.Ch)
            {
                case char num when (num >= '0' && num <= '9'):
                    byte digit;
                    Int16 maxint = Int16.MaxValue; // 32 767
                    nmb_int = 0;
                    nmb_float = 0;
                    bool isFloat = false;
                    while ((InputOutput.Ch >= '0' && InputOutput.Ch <= '9') || InputOutput.Ch == '.')
                    {
                        digit = (byte)(InputOutput.Ch - '0');
                        if (nmb_int < maxint / 10 ||
                            (nmb_int == maxint / 10 && digit <= maxint % 10))
                        {
                            nmb_int = 10 * nmb_int + digit;
                        }
                        else
                        {
                            // константа превышает предел
                            InputOutput.Error(203, InputOutput.positionNow);
                            nmb_int = 0;
                            while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9') InputOutput.NextCh(); // ?
                        }
                        InputOutput.NextCh();
                        if (InputOutput.Ch == '.')
                        {
                            InputOutput.NextCh();
                            count++;
                            nmb_float = nmb_int;
                            while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
                            {
                                nmb_float += ((char)InputOutput.Ch - '0') * (float)Math.Pow(0.1, count++);
                                isFloat = true;
                                InputOutput.NextCh();
                            }
                        }
                    }

                    if (!isFloat)
                    {
                        symbol = new CConstToken(token, intc, new CIntVariant(nmb_int));;
                    }
                    else
                    {
                        symbol = new CConstToken(token, floatc, new CRealVariant(nmb_float));;
                    }
                    break;

                case char word when ((word >= 'a' && word <= 'z') || (word >= 'A' && word <= 'Z')):
                    string name = "";
                    while  (InputOutput.Ch >= 'a' && InputOutput.Ch <= 'z' ||
                            InputOutput.Ch >= 'A' && InputOutput.Ch <= 'Z' ||
                            InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
                            {
                                name += InputOutput.Ch;
                                InputOutput.NextCh();
                            }
                    symbol = new CIdentToken(token, ident, name);

                    if (name.Length >= 2 && name.Length <= 9)
                    {
                        Keywords keywordsLib = new Keywords();
                        if (keywordsLib.Kw[(byte) name.Length].ContainsKey(name))
                        {
                            symbol = new CKeywordToken(token, keywordsLib.Kw[(byte) name.Length][name]);
                        }
                    }
                    break;
                case '\'':
                    string str = "";
                    InputOutput.NextCh();
                    while (InputOutput.Ch.HasValue && InputOutput.Ch != '\'')
                    {
                        str += InputOutput.Ch;
                        InputOutput.NextCh();
                    }
                    symbol = new CConstToken(token, charc, new CStringVariant(str));
                    InputOutput.NextCh();
                    break;
                case '/':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        symbol = new CKeywordToken(token, equalSlash);
                        InputOutput.NextCh();
                    }
                    else
                    {

                        symbol = new CKeywordToken(token, slash);
                    }
                    break;
                case '<':
                   InputOutput.NextCh();
                   if (InputOutput.Ch == '=')
                   {
                        symbol = new CKeywordToken(token, laterequal);
                        InputOutput.NextCh();
                   }
                   else if (InputOutput.Ch == '>')
                   {
                       symbol = new CKeywordToken(token, latergreater);
                        InputOutput.NextCh();
                   }
                   else
                   {
                       symbol = new CKeywordToken(token, later);
                   }
                   break;
                case '>':
                   InputOutput.NextCh();
                   if (InputOutput.Ch == '=')
                   {
                       symbol = new CKeywordToken(token, greaterequal);
                       InputOutput.NextCh();
                   }
                   else
                   {
                       symbol = new CKeywordToken(token, greater);
                   }
                   break;
                case ':':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        symbol = new CKeywordToken(token, assign);
                        InputOutput.NextCh();
                    }
                    else
                    {

                        symbol = new CKeywordToken(token, colon);
                    }
                    break;
                case ';':
                    symbol = new CKeywordToken(token, semicolon);
                    InputOutput.NextCh();
                    break;
                case '.':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '.')
                    {
                        symbol = new CKeywordToken(token, twopoints);
                        InputOutput.NextCh();
                    }
                    else
                    {
                        symbol = new CKeywordToken(token, point);
                    }
                    break;
                case '*':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == ')')
                    {
                        symbol = new CKeywordToken(token, rcomment);
                        InputOutput.NextCh();
                    }
                    else if (InputOutput.Ch == '=')
                    {
                        symbol = new CKeywordToken(token, equalStar);
                        InputOutput.NextCh();
                    }
                    else
                    {
                        symbol = new CKeywordToken(token, star);
                    }
                    break;
                case '(':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '*')
                    {
                        symbol = new CKeywordToken(token, lcomment);
                        InputOutput.NextCh();
                    }
                    else
                    {
                        symbol = new CKeywordToken(token, leftpar);
                    }
                    break;
                case ')':
                    symbol = new CKeywordToken(token, rightpar);
                    InputOutput.NextCh();
                    break;
                case '[':
                    symbol = new CKeywordToken(token, lbracket);
                    InputOutput.NextCh();
                    break;
                case ']':
                    symbol = new CKeywordToken(token, rbracket);
                    InputOutput.NextCh();
                    break;
                case '=':
                    symbol = new CKeywordToken(token, equal);
                    InputOutput.NextCh();
                    break;
                case ',':
                    symbol = new CKeywordToken(token, comma);
                    InputOutput.NextCh();
                    break;
                case '^':
                    symbol = new CKeywordToken(token, arrow);
                    InputOutput.NextCh();
                    break;
                case '{':
                    symbol = new CKeywordToken(token, flpar);
                    InputOutput.NextCh();
                    break;
                case '}':
                    symbol = new CKeywordToken(token, frpar);
                    InputOutput.NextCh();
                    break;
                case '+':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        symbol = new CKeywordToken(token, equalPlus);
                        InputOutput.NextCh();
                    }
                    else
                    {

                        symbol = new CKeywordToken(token, plus);
                    }
                    break;
                case '-':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        symbol = new CKeywordToken(token, equalMinus);
                        InputOutput.NextCh();
                    }
                    else
                    {

                        symbol = new CKeywordToken(token, minus);
                    }
                    break;
                default:
                    InputOutput.Error(6, token);
                    InputOutput.NextCh();
                    throw new FatalException();
            }

            return symbol;
        }
    }
}