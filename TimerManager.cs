using StreamGlass.Core;

namespace TimerPlugin
{
    public class TimerManager
    {
        private readonly Dictionary<string, Timer> m_Timers = [];
        private readonly Dictionary<string, Timer> m_PluginTimers = [];
        private readonly Dictionary<string, TimerInstance> m_Instances = [];

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

        public async Task<bool> StartTimer(string timerID)
        {
            if (m_Timers.TryGetValue(timerID, out Timer? timer) ||
                m_PluginTimers.TryGetValue(timerID, out timer))
            {
                await StartTimer(timer);
                return true;
            }
            return false;
        }

        public async Task<bool> StopTimer(string family)
        {
            if (m_Instances.TryGetValue(family, out TimerInstance? instance))
            {
                await instance.Stop();
                instance.Clear();
                return true;
            }
            return false;
        }

        private async Task OnInstanceFinish(Timer timer)
        {
            await StreamGlassCanals.Emit("timer_family_ended", timer.Family);
            m_Instances.Remove(timer.Family);
        }

        public async Task StartTimer(Timer timer)
        {
            if (m_Instances.TryGetValue(timer.Family, out TimerInstance? instance))
            {
                await instance.Stop();
                instance.SetTimer(timer);
            }
            else
            {
                instance = new(timer);
                instance.OnFinish += async () => await OnInstanceFinish(timer);
                m_Instances[timer.Family] = instance;
            }

            if (!string.IsNullOrEmpty(timer.Scene))
                await StreamGlassActions.Call("ChangeScene", timer.Scene);
            await instance.Start();
            Timer.AdsInfo? ads = timer.Ads;
            if (ads != null)
            {
                if (ads.Delay > 0)
                    await Task.Delay(ads.Delay * 1000).ContinueWith(_ => StreamGlassActions.Call("TwitchAd", ads.Duration));
                else
                    await StreamGlassActions.Call("TwitchAd", ads.Duration);
            }
        }

        ~TimerManager()
        {
            foreach (var pair in m_Instances)
                _ = pair.Value.Stop();
        }
    }
}
