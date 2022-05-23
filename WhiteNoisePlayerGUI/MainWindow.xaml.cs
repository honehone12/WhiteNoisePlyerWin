using System;
using System.Windows;
using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;

namespace WhiteNoisePlayerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //set right path when release
        const string fileName = "noise.wav";

        readonly WasapiOut soundOut;

        public MainWindow()
        {
            IWaveSource waveSource;
            try
            {
                waveSource = CodecFactory.Instance.GetCodec(fileName)
                .ToMono()
                .Loop();
            }
            catch (Exception e)
            {
                // better way to show error message ??
                Console.WriteLine("Please make sure noise.wav is in same folder.");
                throw e;
            }

            MMDevice device;
            using (var mmDeviceEnumerator = new MMDeviceEnumerator())
            {
                device = mmDeviceEnumerator.GetDefaultAudioEndpoint(
                    DataFlow.Render,
                    Role.Multimedia
                );
            }
            
            soundOut = new WasapiOut()
            {
                Latency = 100,
                Device = device
            };

            soundOut.Initialize(waveSource);
            soundOut.Play();

            InitializeComponent();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            soundOut.Volume = (float)e.NewValue;
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
        private void PlayerWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            soundOut.Stop();
            soundOut.Dispose();
        }
    }
}
