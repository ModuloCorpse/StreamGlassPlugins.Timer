using CorpseLib;
using CorpseLib.Placeholder;
using StreamGlass.Core;

namespace TimerPlugin
{
    public class TimerInstance(Timer timer) : TimedAction(TimeSpan.FromMilliseconds(250), TimeSpan.FromSeconds(timer.Duration - 1))
    {
        private Timer m_Timer = timer;

        public void Clear() => StreamGlassContext.UpdateStringSource(m_Timer.StringSource, string.Empty);

        public void SetTimer(Timer timer)
        {
            m_Timer = timer;
            SetDuration(TimeSpan.FromSeconds(timer.Duration - 1));
        }

        protected override async Task OnActionStart()
        {
            await base.OnActionStart();
            await OnActionUpdate(TimeSpan.Zero);
        }

        protected override async Task OnActionUpdate(TimeSpan elapsed)
        {
            TimeSpan remainingTime = Duration - elapsed;
            StreamGlassContext streamGlassContext = new();
            Context context = new();
            context.AddVariable("h", remainingTime.Hours);
            context.AddVariable("hh", string.Format("{0:D2}", remainingTime.Hours));
            context.AddVariable("m", remainingTime.Minutes);
            context.AddVariable("mm", string.Format("{0:D2}", remainingTime.Minutes));
            context.AddVariable("s", remainingTime.Seconds);
            context.AddVariable("ss", string.Format("{0:D2}", remainingTime.Seconds));
            StreamGlassContext.UpdateStringSource(m_Timer.StringSource, Converter.Convert(m_Timer.Format, streamGlassContext, context));
            await base.OnActionUpdate(elapsed);
        }

        protected override async Task OnActionFinish()
        {
            StreamGlassContext streamGlassContext = new();
            if (!string.IsNullOrEmpty(m_Timer.FinishMessage))
                StreamGlassContext.UpdateStringSource(m_Timer.StringSource, Converter.Convert(m_Timer.FinishMessage, streamGlassContext));
            await base.OnActionFinish();
        }
    }
}
