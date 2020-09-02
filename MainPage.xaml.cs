//Descendent of MainPage.xaml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Plugin.SimpleAudioPlayer;

namespace MetronomeApp
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        ISimpleAudioPlayer tickSound = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        bool soundLoaded = false;
        bool stopped = true;

        public void Handle_Metronome(double tempo, double sync)
        {
            DateTime syncDateTime = DateTime.UtcNow;
            if (sync == 0)
            {
                //Sync Code is just date as a number
                SyncBox.Text = Convert.ToString(syncDateTime.ToOADate());
            }
            else
            {
                //Delays the first click by the remaining time until the next tick from the given Sync Code
                int initialDelay = Convert.ToInt32((60000 / tempo) - ((syncDateTime.ToOADate() - sync) * 86400000) % (60000 / tempo));
                System.Threading.Thread.Sleep(initialDelay);
            }
            //Begins a loop that ticks according to the tempo (60000/tempo = ms/beat)
            stopped = false;
            MetronomeTick(60000/tempo);
        }

        private async Task MetronomeTick(msDelay)
        {
            //Prevents needing to load the sound every time
            if (soundLoaded == false)
            {
                tickSound.Load("Click.mp3");
                soundLoaded = true;
            }
            //Makes the delay happen asynchronously
            await Task.Delay(msDelay);
            if (stopped == false)
            {
                tickSound.Play();
                await MetronomeTick(msDelay);
            }
        }

        public void Handle_Start(object sender, System.EventArgs e)
        {
            if (stopped == true)
            {
                string tempoString = TempoBox.Text;
                //Makes sure tempo is a number
                if (double.TryParse(tempoString, out double tempoNum) == true & tempoNum != 0)
                {
                    string syncString = SyncBox.Text;
                    if (syncString == "")
                    {
                        //Makes the function output a new Sync Code, used for the instructor
                        Handle_Metronome(tempoNum, 0);
                    }
                    else if (double.TryParse(syncString, out double syncNum) == true)
                    {
                        Handle_Metronome(tempoNum, syncNum);
                    }
                    else
                    {
                        ErrorBox.Text = "Please input a valid Sync Code.";
                    }
                }
                else
                {
                    ErrorBox.Text = "Please input a valid tempo.";
                }
            }
        }

        public void Handle_Stop(object sender, System.EventArgs e)
        {
            stopped = true;
        }
    }
}
