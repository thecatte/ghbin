using System;

namespace ghbin.Service
{
    public class LoggerService
    {
        public void Log(string text, ConsoleColor foreColor, ConsoleColor backColor) {
            var prevForeColor = Console.ForegroundColor;
            var prevBackColor = Console.BackgroundColor;

            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
            
            Console.WriteLine(text);

            Console.ForegroundColor = prevForeColor;
            Console.BackgroundColor = prevBackColor;
        }

        public void Log(string text, ConsoleColor foreColor) {
            Log(text, foreColor, Console.BackgroundColor);
        }

        public void Log(string text) {
            Console.WriteLine(text);
        }

        public void Success(string text) {
            Log(text, ConsoleColor.Green);
        }

        public void Error(string text) {
            Log(text, ConsoleColor.Red);
        }

        public void Warn(string text) {
            Log(text, ConsoleColor.Yellow);
        }
    }
}