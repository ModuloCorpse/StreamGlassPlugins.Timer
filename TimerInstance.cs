using CorpseLib;
using CorpseLib.Placeholder;
using StreamGlass.Core;

namespace TimerPlugin
{
    public class TimerInstance(Timer timer) : TimedAction(250, (timer.Duration - 1) * 1000)
    {
        private readonly Timer m_Timer = timer;

        protected override void OnActionStart()
        {
            base.OnActionStart();
            OnActionUpdate(0);
        }

        protected override void OnActionUpdate(long elapsed)
        {
            TimeSpan remainingTime = TimeSpan.FromMilliseconds(Duration + 1000 - elapsed);
            StreamGlassContext streamGlassContext = new();
            Context context = new();
            context.AddVariable("h", remainingTime.Hours);
            context.AddVariable("hh", string.Format("{0:D2}", remainingTime.Hours));
            context.AddVariable("m", remainingTime.Minutes);
            context.AddVariable("mm", string.Format("{0:D2}", remainingTime.Minutes));
            context.AddVariable("s", remainingTime.Seconds);
            context.AddVariable("ss", string.Format("{0:D2}", remainingTime.Seconds));
            File.WriteAllText(m_Timer.FilePath, Converter.Convert(m_Timer.Format, streamGlassContext, context));
            base.OnActionUpdate(elapsed);
        }

        protected override void OnActionFinish()
        {
            StreamGlassContext streamGlassContext = new();
            if (!string.IsNullOrEmpty(m_Timer.FinishMessage))
                File.WriteAllText(m_Timer.FilePath, Converter.Convert(m_Timer.FinishMessage, streamGlassContext));
            base.OnActionFinish();
        }
    }
}
