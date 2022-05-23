using System;
using System.Threading;
using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;

namespace WhiteNoisePlayer
{
    internal class Program
    {
        //set right path when release
        const string fileName = "noise.wav";

        static void Main()
        {
            IWaveSource waveSource;
            try
            {
                waveSource = CodecFactory.Instance.GetCodec(fileName)
                .ToMono()
                .Loop();
            }
            catch (Exception)
            {
                Console.WriteLine("Please make sure noise.wav is in same folder.");
                // to show error message
                while (true)
                {
                    Thread.Sleep(100);
                }
            }
            
            MMDevice device;
            using (var mmDeviceEnumerator = new MMDeviceEnumerator())
            {
                device = mmDeviceEnumerator.GetDefaultAudioEndpoint(
                    DataFlow.Render,
                    Role.Multimedia
                );
            }
            var soundOut = new WasapiOut()
            {
                Latency = 100,
                Device = device
            };

            soundOut.Initialize(waveSource);
            soundOut.Play();
            double currentVolume = soundOut.Volume;

            Console.WriteLine("Welcome !! This is WhiteNoisePlayer !!");
            Console.WriteLine("Press ...");
            Console.WriteLine("[q] to quit.");
            Console.WriteLine("[w] to up volume");
            Console.WriteLine("[s] to down volume");

            var appLoop = true;
            while (appLoop)
            {
                Console.WriteLine($"Current volume: {currentVolume} (0-1)");
                var ctl = Console.ReadLine();
                switch (ctl)
                {
                    case "q":
                        appLoop = false;
                        break;
                    case "w":
                        if(currentVolume < 1.0)
                        {
                            currentVolume += 0.1;
                            if(currentVolume > 1.0)
                            {
                                currentVolume = 1.0;
                            }
                            soundOut.Volume = (float)currentVolume;
                        }
                        break;
                    case "s":
                        if (currentVolume > 0.0)
                        {
                            currentVolume -= 0.1;
                            if (currentVolume < 0.0)
                            {
                                currentVolume = 0.0;
                            }
                            soundOut.Volume = (float)currentVolume;
                        }
                        break;
                }

                Thread.Sleep(500);
            }

            soundOut.Stop();
            soundOut.Dispose(); 
            Console.WriteLine("Thank you !! Byebye !!");
        }
    }
}
