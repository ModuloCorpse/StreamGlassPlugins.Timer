using StreamGlass.Core;

namespace TimerPlugin
{
    public class TimerManager
    {
        private readonly Dictionary<string, Timer> m_Timers = [];
        private readonly Dictionary<string, TimerInstance> m_Instances = [];

        public void RegisterTimer(Timer timer) => m_Timers[timer.ID] = timer;

        public bool StartTimer(string timerID)
        {
            if (m_Timers.TryGetValue(timerID, out Timer? timer))
            {
                StartTimer(timer);
                return true;
            }
            return false;
        }

        public bool StopTimer(string family)
        {
            if (m_Instances.TryGetValue(family, out TimerInstance? instance))
            {
                instance.Stop();
                instance.Clear();
                return true;
            }
            return false;
        }

        public void StartTimer(Timer timer)
        {
            if (m_Instances.TryGetValue(timer.Family, out TimerInstance? oldInstance))
                oldInstance.Stop();
            if (!string.IsNullOrEmpty(timer.Scene))
                StreamGlassCLI.ExecuteCommand(string.Format("OBSScene {0}", timer.Scene));
            TimerInstance instance = new(timer);
            instance.OnFinish += (sender, e) => m_Instances.Remove(timer.Family);
            instance.OnStop += (sender, e) => m_Instances.Remove(timer.Family);
            m_Instances[timer.Family] = instance;
            instance.Start();
            Timer.AdsInfo? ads = timer.Ads;
            if (ads != null)
            {
                string adCommand = string.Format("TwitchAd {0}", ads.Duration);
                if (ads.Delay > 0)
                    Task.Delay(ads.Delay * 1000).ContinueWith(t => StreamGlassCLI.ExecuteCommand(adCommand));
                else
                    StreamGlassCLI.ExecuteCommand(adCommand);
            }
        }
        public void Test(string testPath)
        {
            StartTimer(new Timer("__TestID__", "__TestFamily__", testPath, "${mm}:${ss}", "Test timer finished", string.Empty, 30, null));
        }

        ~TimerManager()
        {
            foreach (var pair in m_Instances)
                pair.Value.Stop();
        }
    }
}
