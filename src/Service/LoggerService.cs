using System;

namespace ghbin.Service
{
    public class LoggerService
    {
        public void Log(string text, ConsoleColor foreColor, ConsoleColor backColor, bool newLine = true)
        {
            var prevForeColor = Console.ForegroundColor;
            var prevBackColor = Console.BackgroundColor;

            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;

            if (newLine)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }

            Console.ForegroundColor = prevForeColor;
            Console.BackgroundColor = prevBackColor;
        }

        public void Log(string text, ConsoleColor foreColor, bool newLine = true)
        {
            Log(text, foreColor, Console.BackgroundColor, newLine);
        }

        public void Log(string text, bool newLine = true)
        {
            if (newLine)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }
        }

        public void Success(string text)
        {
            Log(text, ConsoleColor.Green);
        }

        public void Error(string text)
        {
            Log(text, ConsoleColor.Red);
        }

        public void Warn(string text, bool newLine = true)
        {
            Log(text, ConsoleColor.Yellow, newLine: newLine);
        }
    }
}