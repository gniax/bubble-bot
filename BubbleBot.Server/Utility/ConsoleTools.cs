using System;
using System.Threading;

namespace BubbleBot.Server.Utility
{
    public class ConsoleTools
    {
        // Write to the console a success message
        public static void ConsoleWriteSuccess()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" => \u221A");
            Console.ResetColor();
        }

        // Write to the console an error message
        public static void ConsoleWriteError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" => " + 'X');
            Console.ResetColor();
        }
    }

    public class Spinner
    {
        static string[,] sequence = null;
        private readonly int totalSequences = 0;
        private int counter = 0;
        private string displayMsg = "";
        private int sequenceCode;
        private int Delay;
        private bool active;
        private readonly Thread thread;

        public Spinner(int delay = 100)
        {
            Delay = delay;
            sequence = new string[,] {
            { "/", "-", "\\", "|" },
            { ".", "o", "0", "o" },
            { "+", "x","+","x" },
            { "V", "<", "^", ">" },
            { ".   ", "..  ", "... ", "...." },
            { "=>   ", "==>  ", "===> ", "====>" },
            };
            totalSequences = sequence.GetLength(0);
            thread = new Thread(Spin);
        }

        public void Start(string msg = "", int code = 0)
        {
            displayMsg = msg;
            sequenceCode = code;
            active = true;
            //thread.Start();
        }

        public void Stop()
        {
            active = false;
        }

        private void Spin()
        {
            while (active)
            {
                Turn();
                Thread.Sleep(Delay);
            }
        }

        private void Turn()
        {
            counter++;

            sequenceCode = sequenceCode > totalSequences - 1 ? 0 : sequenceCode;

            int counterValue = counter % 4;

            string fullMessage = displayMsg + sequence[sequenceCode, counterValue];
            int msglength = fullMessage.Length;

            Console.Write(fullMessage);

            Console.SetCursorPosition(Console.CursorLeft - msglength, Console.CursorTop);
        }

    }
}
