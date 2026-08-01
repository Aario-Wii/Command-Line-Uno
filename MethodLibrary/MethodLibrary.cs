using System;
namespace MethodLibrary
{
    public static class Read
    {
        public static string String()
        {
            while (true)
            {
                string? CheckString = Console.ReadLine();
                if (CheckString != null || CheckString?.Trim() != null)
                {
                    return CheckString;
                }
                else
                {
                    Console.WriteLine("Please enter characters.");
                }
            }
        }
        public static int Int()
        {
            while (true)
            {
                string CheckString = String();
                string DigitString = new string(CheckString.Where(Char.IsDigit).ToArray());
                if (DigitString == null || DigitString == "")
                {
                    Console.WriteLine("Please only enter numbers");
                }
                else
                {
                    return int.Parse(DigitString);
                }
            }

        }
    }
    public static class ConsoleUtilities
    {
        
        public static void ClearLines(int clearnum)
        {
            for (int i = Console.CursorTop; i >= clearnum; i--)
            {
                try
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write(new string(' ', Console.BufferWidth));
                }
                catch (ArgumentOutOfRangeException)
                { }
            }
        }
        public static void WriteLine(string text, StringWriter writer)
        {
            Console.SetOut(writer);
            Console.WriteLine(text);
            Console.OpenStandardOutput();
        }
    }
}