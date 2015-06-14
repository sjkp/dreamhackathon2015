using BeatDetectorCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatDector
{
    class Program
    {
        static void Main(string[] args)
        {
            BeatDetector detector = BeatDetector.Instance();

            detector.loadSystem();
            detector.LoadSong(512, @"C:\Users\Simon Pedersen\Downloads\Player In C-UaR8hmeugc8.mp3");
           
            detector.setStarted(true);
            
            
            var lastBeat = detector.getLastBeat();
            
            var tick = DateTime.Now.Ticks;
            while(true)
            {
                if (DateTime.Now.Ticks > tick)
                {
                    tick = DateTime.Now.Ticks;
                    detector.update();
                    var newbeat = detector.getLastBeat();
                    if (lastBeat != newbeat)
                    {
                        lastBeat = newbeat;
                        Console.WriteLine("BEAT");
                    }
                }
            }

            Console.Read();
        }
    }
}
