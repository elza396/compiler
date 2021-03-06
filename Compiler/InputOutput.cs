using System;
using System.Collections.Generic;
using System.IO;

namespace Compiler
{
    struct TextPosition
    {
        public uint lineNumber; // номер строки
        public byte charNumber; // номер позиции в строке

        public TextPosition(uint ln = 0, byte c = 0)
        {
            lineNumber = ln;
            charNumber = c;
        }
    }

    struct Err
    {
        public TextPosition errorPosition;
        public byte errorCode;

        public Err(TextPosition errorPosition, byte errorCode)
        {
            this.errorPosition = errorPosition;
            this.errorCode = errorCode;
        }
    }


    class InputOutput
    {
        const byte ERRMAX = 9;
        public static char? Ch;
        public static TextPosition positionNow;
        static string line;
        static byte lastInLine = 0;
        public static List<Err> err;
        static StreamReader File { get;  set; } // ?
        static uint errCount = 0;

        public static bool permission = true;
        // public static SemanticAnalyzer.Scope localScope = new SemanticAnalyzer.Scope();
        // public static SemanticAnalyzer.TypeRec charType, intType, realType, boolType, textType, nilType;
        // public static SyntaxisAnalyzer.ListRec varList;
        // public static SyntaxisAnalyzer.WithStack localWith = null;

        static public void Begin()
        {
            File = new StreamReader("Text.txt");
            positionNow = new TextPosition();
            ReadNextLine();
            Ch = line[0];
            while (permission)
            {
                SyntaxisAnalyzer.Start();
            }
        }
        static public void Scan()
        {
            File = new StreamReader("Text.txt");
            positionNow = new TextPosition();
            ReadNextLine();
            Ch = line[0];
            StreamWriter streamWriter = new StreamWriter("Lex.txt", true);

            while (permission)
            {
                LexicalAnalyzer.NextSym();
                //Console.WriteLine(LexicalAnalyzer.symbol);
                streamWriter.WriteLine(LexicalAnalyzer.symbol);
            }

            streamWriter.Close();
        }

        static public void NextCh()
        {
            lastInLine = (byte)line.Length;
            if (positionNow.charNumber == lastInLine - 1)
            {
                ListThisLine();
                if (err.Count > 0)
                        ListErrors();
                ReadNextLine();
                positionNow.lineNumber += 1;
                positionNow.charNumber = 0;
            }
            else ++positionNow.charNumber;
            Ch = line[positionNow.charNumber];
        }

        private static void ListThisLine()
        {
            Console.WriteLine(line);
        }

        private static void ReadNextLine()
        {
            if (!File.EndOfStream)
            {
                line = File.ReadLine();
                err = new List<Err>();
            }
            else
            {
                End();
            }
        }

        static void End()
        {
            permission = false;
            Console.WriteLine($"Компиляция завершена: ошибок — {errCount}!");
        }

        static void ListErrors()
        {
            int pos = 6 - $"{positionNow.lineNumber} ".Length;
            string s;
            foreach (Err item in err)
            {
                ++errCount;
                s = "**";
                if (errCount < 10) s += "0";
                s += $"{errCount}**";
                while (s.Length - 1 < pos + item.errorPosition.charNumber) s += " ";
                s += $"^ ошибка код {item.errorCode}; {positionNow.lineNumber} : {positionNow.charNumber}";
                Console.WriteLine(s);
            }
        }

        static public void Error(byte errorCode, TextPosition position)
        {
            Err e;
            if (err.Count <= ERRMAX)
            {
                e = new Err(position, errorCode);
                err.Add(e);
            }
        }

        static public void Error(byte errorCode)
        {
            Err e;
            if (err.Count <= ERRMAX)
            {
                e = new Err(LexicalAnalyzer.token, errorCode);
                err.Add(e);
            }
        }
    }
}