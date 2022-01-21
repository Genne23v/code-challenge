using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicalInstrument
{
    public partial class Form1 : Form
    {
        SignalGenerator sine = new SignalGenerator()
        {
            Type = SignalGeneratorType.Sin,
            Gain = 0.2
        };
   
        WaveOutEvent player = new WaveOutEvent();

        public Form1()
        {
            InitializeComponent();

            player.Init(sine);

            trackFrequency.ValueChanged += (s, e) => sine.Frequency = trackFrequency.Value;
            trackFrequency.Value = 600;

            trackVolume.ValueChanged += (s, e) => player.Volume = trackVolume.Value / 100F;
            trackVolume.Value = 50;
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {

        }

        private System.Drawing.Point CursorPositionOnMouseDown;
        private bool ButtonIsDown = false;
        private void MouseUp(object sender, MouseEventArgs e)
        {
            player.Stop();
            ButtonIsDown = false;
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            player.Play();
            CursorPositionOnMouseDown = e.Location;
            ButtonIsDown = true;
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            var dx = e.X - CursorPositionOnMouseDown.X;
            var volume = player.Volume + (dx / 1000F);
            var dy = CursorPositionOnMouseDown.Y - e.Y;
            var frequency = sine.Frequency + dy;

            if (ButtonIsDown)
            {
                player.Volume = (volume > 0) ? (volume < 1) ? volume : 1 : 0;
                sine.Frequency = (frequency > 100) ? (frequency < 1000) ? frequency : 1000 :  100;

                trackVolume.Value = (int)Math.Round(player.Volume *100);
                trackFrequency.Value = (int)Math.Round(sine.Frequency);
            }
        }
    }
}
