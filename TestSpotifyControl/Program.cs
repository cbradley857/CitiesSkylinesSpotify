using CitiesSkylinesSpotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSpotifyControl
{
    public class Program
    {
        static SpotifyAPI api;

        static void Main(string[] args)
        {
            api = new SpotifyAPI();

            while (true)
            {
                Console.WriteLine("Enter option:\n1. Pause/Unpause\n2.Exit");
                int option = int.Parse(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        bool temp = api.PausePlay();
                        Console.WriteLine("Successful: " + temp + "\n");
                        break;
                    case 2:
                        System.Environment.Exit(1);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
