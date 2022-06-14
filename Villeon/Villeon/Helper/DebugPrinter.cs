using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Helper
{
    public class DebugPrinter
    {
        private static Queue<string> _stringBuffer = new Queue<string>();

        private static float _time = 0;

        public static void Print(string text)
        {
            _stringBuffer.Enqueue(text);
        }

        public static void PrintToConsole(float time)
        {
            _time += time;
            if (_time < 0.1f)
                return;
            _time = 0;

            foreach (string item in _stringBuffer)
            {
                Console.WriteLine(item);
            }

            _stringBuffer.Clear();
        }
    }
}
