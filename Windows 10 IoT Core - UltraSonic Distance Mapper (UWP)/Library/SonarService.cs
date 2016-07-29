using org.alljoyn.example.Sonar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.Foundation;

namespace SonarScope.Library
{
    public class SonarService : ISonarService
    {

        MainPage mp;

        public SonarService(MainPage mp)
        {
            this.mp = mp;
        }

        public IAsyncOperation<SonarSpeechResult> SpeechAsync(AllJoynMessageInfo info)
        {
            Task<SonarSpeechResult> task = new Task<SonarSpeechResult>(() =>
            {
                Debug.WriteLine("Speech");
                mp.Speech();
                return SonarSpeechResult.CreateSuccessResult();
            });

            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<SonarStartResult> StartAsync(AllJoynMessageInfo info)
        {
            Task<SonarStartResult> task = new Task<SonarStartResult>(() =>
            {
                Debug.WriteLine("Start");
                mp.Start();
                return SonarStartResult.CreateSuccessResult();
            });

            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<SonarStopResult> StopAsync(AllJoynMessageInfo info)
        {
            Task<SonarStopResult> task = new Task<SonarStopResult>(() =>
            {
                Debug.WriteLine("Stop");
                mp.Stop();
                return SonarStopResult.CreateSuccessResult();
            });

            task.Start();
            return task.AsAsyncOperation();
        }
    }
}
