using System;
using System.Collections.Generic;
using System.IO;

namespace Compiler
{
    public struct TextPosition
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
        static StreamReader File { get;  set; }
        public static StreamReader Filedict { get; set; }

        static uint errCount = 0;
        public static Dictionary<int, string> Dicterrors = new Dictionary<int, string>(); // создание словаря с ошибками

        public static bool permission = true;

        static public void Begin()
        {
            File = new StreamReader("Text.txt");
            dictionaryErrors();
            positionNow = new TextPosition();
            ReadNextLine();
            Ch = line[0];
            Syntaxis.Programme();
        }
        static public void Scan() // работа лексического анализатора
        {
            File = new StreamReader("Text.txt");
            positionNow = new TextPosition();
            ReadNextLine();
            Ch = line[0];
            StreamWriter streamWriter = new StreamWriter("Lex.txt", true);

            while (permission)
            {
                LexicalAnalyzer.NextSym();
                Console.WriteLine(LexicalAnalyzer.symbol);
                streamWriter.WriteLine(LexicalAnalyzer.symbol);
            }

            streamWriter.Close();
        }

        static public void NextCh()
        {
            if (!permission)
            {
                End();
                Environment.Exit(0);
            }
            lastInLine = (byte)line.Length;
            if (positionNow.charNumber == lastInLine - 1)
            {
                ListThisLine();
                if (err.Count > 0)
                {
                    ListErrors();
                }
                ReadNextLine();
                positionNow.lineNumber += 1;
                positionNow.charNumber = 0;
            }
            else ++positionNow.charNumber;
            Ch = line[positionNow.charNumber];
        }

        private static void ListThisLine()
        {
            string text = $"   {positionNow.lineNumber + 1}  {line}";
            Console.WriteLine(text);
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
                permission = false;
            }
        }

        static void End()
        {
            Console.WriteLine($"Компиляция завершена: ошибок — {errCount}!");
        }

        static void ListErrors()
        {
            int pos = 8 - $"{positionNow.charNumber} ".Length;
            string s;
            foreach (Err item in err)
            {
                ++errCount;
                s = "**";
                if (errCount < 10) s += "0";
                s += $"{errCount}**";
                while (s.Length < pos + item.errorPosition.charNumber) s += " ";
                s += $"^ ошибка код {item.errorCode} ({Dicterrors[item.errorCode]} ) на {item.errorPosition.lineNumber + 1} строке;";
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

        static public void dictionaryErrors()
        {
            Filedict = new StreamReader("ErrorCodes.txt");
            while (!Filedict.EndOfStream)
            {
                var lines = Filedict.ReadLine();
                string[] walye = lines.Split(':');
                if (walye.Length == 2)
                {
                    Dicterrors.Add(Int32.Parse(walye[0]), walye[1]);
                }


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