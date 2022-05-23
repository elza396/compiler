﻿using System;
namespace Compiler
{
    class LexicalAnalyzer
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
            divsy = 106,
            andsy = 107,
            notsy = 108,
            forsy = 109,
            modsy = 110,
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
            procedurensy = 124;

        public static byte symbol; // код символа
        public static TextPosition token; // позиция символа
        string addrName; // адрес идентификатора в таблице имен
        static int nmb_int; // значение целой константы
        static float nmb_float; // значение вещественной константы
        static char one_symbol; // значение символьной константы
        static int count = 0;
        
        public static byte NextSym()
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
                                nmb_float += (InputOutput.Ch - '0') * (float)Math.Pow(0.1, count++);
                                isFloat = true;
                                InputOutput.NextCh();
                            }
                        }
                    }

                    if (!isFloat)
                    {
                        symbol = intc;
                    }
                    else
                    {
                        symbol = floatc;
                    }
                    break;

                case char word when ((word >= 'a' && word <= 'z') || (word >= 'A' && word <= 'Z')):
                    string name = "";
                    while  ((InputOutput.Ch >= 'a' && InputOutput.Ch <= 'z') ||
                            (InputOutput.Ch >= 'A' && InputOutput.Ch <= 'Z') ||
                            (InputOutput.Ch >= '0' && InputOutput.Ch <= '9'))
                            {
                                name += InputOutput.Ch;
                                InputOutput.NextCh();
                            }
                    symbol = ident;

                    if (name.Length >= 2 && name.Length <= 9)
                    {
                        Keywords keywordsLib = new Keywords();
                        if (keywordsLib.Kw[(byte) name.Length].ContainsKey(name))
                        {
                            symbol = keywordsLib.Kw[(byte) name.Length][name];
                        }
                    }
                    break;
                case '/':
                    symbol = slash;
                    InputOutput.NextCh();
                    break;
                case '<':
                   InputOutput.NextCh();
                   if (InputOutput.Ch == '=')
                   {
                        symbol = laterequal; InputOutput.NextCh();
                   }
                   else
                    if (InputOutput.Ch == '>')
                    {
                        symbol = latergreater; InputOutput.NextCh();
                    }
                    else
                        symbol = later;
                    break;
                case '>':
                   InputOutput.NextCh();
                   if (InputOutput.Ch == '=')
                   {
                        symbol = greaterequal; InputOutput.NextCh();
                   }
                   else
                   {
                       symbol = greater;
                   }
                   break;
                case ':':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        symbol = assign; InputOutput.NextCh();
                    }
                    else
                        symbol = colon;
                    break;
                case ';':
                    symbol = semicolon;
                    InputOutput.NextCh();
                    break;
                case '.':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '.')
                    {
                        symbol = twopoints; InputOutput.NextCh();
                    }
                    else symbol = point;
                    break;
                case '*':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == ')')
                    {
                        symbol = rcomment; InputOutput.NextCh();
                    }
                    else
                    {
                        symbol = star;
                    }
                    break;
                case '(':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '*')
                    {
                        symbol = lcomment; InputOutput.NextCh();
                    }
                    else
                    {
                        symbol = leftpar;
                    }
                    break;
                case ')':
                    symbol = rightpar;
                    InputOutput.NextCh();
                    break;
                case '[':
                    symbol = lbracket;
                    InputOutput.NextCh();
                    break;
                case ']':
                    symbol = rbracket;
                    InputOutput.NextCh();
                    break;
                case '=':
                    symbol = equal;
                    InputOutput.NextCh();
                    break;
                case ',':
                    symbol = comma;
                    InputOutput.NextCh();
                    break;
                case '^':
                    symbol = arrow;
                    InputOutput.NextCh();
                    break;
                case '{':
                    symbol = flpar;
                    InputOutput.NextCh();
                    break;
                case '}':
                    symbol = frpar;
                    InputOutput.NextCh();
                    break;
                case '+':
                    symbol = plus;
                    InputOutput.NextCh();
                    break;
                case '-':
                    symbol = minus;
                    InputOutput.NextCh();
                    break;
            }

            return symbol;
        }
    }
}