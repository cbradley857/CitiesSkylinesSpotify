using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdGetter
{
    public class Program
    {
        static void Main(string[] args)
        {
            Process[] SpotifyProcesses = Process.GetProcessesByName("spotify");//.Where(w => w.MainWindowTitle != string.Empty).FirstOrDefault();

            int? appId = null;
            IntPtr windowHandle = new IntPtr();
            bool isOpen = false;
            
            foreach (var p in SpotifyProcesses)
            {
                if (p.MainWindowTitle == string.Empty)
                    continue;

                if (p.MainWindowTitle == null)
                    continue;

                if (p.MainWindowTitle.Length > 6)
                {
                    isOpen = true;
                    appId = p.Id;
                    windowHandle = p.MainWindowHandle;
                    break;
                }
            }

            Console.Write(isOpen);

            if (isOpen)
            {
                Console.Write(":");
                Console.Write(appId.Value);
                Console.Write(":");
                Console.Write(windowHandle);
            }

            System.Environment.Exit(1);
        }
    }
}
