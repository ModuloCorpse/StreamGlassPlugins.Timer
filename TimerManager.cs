using StreamGlass.Core;

namespace TimerPlugin
{
    public class TimerManager
    {
        private readonly Dictionary<string, Timer> m_Timers = [];
        private readonly Dictionary<string, Timer> m_PluginTimers = [];
        private readonly Dictionary<string, TimerInstance> m_Instances = [];
        private readonly Dictionary<string, TimerInstance> m_LastInstances = [];

        public void RegisterTimer(Timer timer)
        {
            m_Timers[timer.ID] = timer;
            StreamGlassContext.CreateStringSource(timer.StringSource);
            StreamGlassContext.UpdateStringSource(timer.StringSource, string.Empty);
        }

        public void RegisterPluginTimer(Timer timer)
        {
            m_PluginTimers[timer.ID] = timer;
            StreamGlassContext.CreateStringSource(timer.StringSource);
            StreamGlassContext.UpdateStringSource(timer.StringSource, string.Empty);
        }

        public bool StartTimer(string timerID)
        {
            if (m_Timers.TryGetValue(timerID, out Timer? timer) ||
                m_PluginTimers.TryGetValue(timerID, out timer))
            {
                StartTimer(timer);
                return true;
            }
            return false;
        }

        public bool StopTimer(string family)
        {
            if (m_Instances.TryGetValue(family, out TimerInstance? instance) ||
                m_LastInstances.TryGetValue(family, out instance))
            {
                instance.Stop();
                instance.Clear();
                return true;
            }
            return false;
        }

        private void OnInstanceFinish(TimerInstance instance, Timer timer)
        {
            StreamGlassCanals.Emit("timer_family_ended", timer.Family);
            m_LastInstances[timer.Family] = instance;
            m_Instances.Remove(timer.Family);
        }

        public void StartTimer(Timer timer)
        {
            if (m_Instances.TryGetValue(timer.Family, out TimerInstance? oldInstance))
                oldInstance.Stop();
            if (!string.IsNullOrEmpty(timer.Scene))
                StreamGlassActions.Call("ChangeScene", timer.Scene);
            TimerInstance instance = new(timer);
            instance.OnFinish += (sender, e) => OnInstanceFinish(instance, timer);
            instance.OnStop += (sender, e) => OnInstanceFinish(instance, timer);
            m_Instances[timer.Family] = instance;
            instance.Start();
            Timer.AdsInfo? ads = timer.Ads;
            if (ads != null)
            {
                if (ads.Delay > 0)
                    Task.Delay(ads.Delay * 1000).ContinueWith(_ => StreamGlassActions.Call("TwitchAd", ads.Duration));
                else
                    StreamGlassActions.Call("TwitchAd", ads.Duration);
            }
        }

        ~TimerManager()
        {
            foreach (var pair in m_Instances)
                pair.Value.Stop();
        }
    }
}
