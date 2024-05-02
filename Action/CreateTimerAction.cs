using CorpseLib.Actions;
using StreamGlass.Core;
using static TimerPlugin.Timer;

namespace TimerPlugin.Action
{
    public class CreateTimerAction(TimerManager manager) : AStreamGlassAction(ms_Definition)
    {
        private static readonly ActionDefinition ms_Definition = new ActionDefinition("TimerCreate", "Create a new timer")
            .AddArgument<int>("duration", "Duration of the timer")
            .AddArgument<string>("id", "ID of the timer")
            .AddArgument<string>("family", "Timer family of the timer")
            .AddArgument<string>("string_source", "String source where to write the timer")
            .AddOptionalArgument("end", "Message to display at the end of the timer", string.Empty)
            .AddOptionalArgument("format", "Format of the timer", "${mm}:${ss}")
            .AddOptionalArgument("scene", "OBS scene to switch to when starting the timer", string.Empty)
            .AddOptionalArgument("ads_duration", "Duration of Twitch ad when starting timer", -1)
            .AddOptionalArgument("ads_delay", "Delay before starting Twitch ad after starting timer", -1);
        public override bool AllowDirectCall => true;
        public override bool AllowCLICall => true;
        public override bool AllowScriptCall => true;
        public override bool AllowRemoteCall => true;

        private readonly TimerManager m_Manager = manager;

        public override object?[] Call(object?[] args)
        {
            AdsInfo? ads = null;
            int adsDuration = (int)args[7]!;
            int adsDelay = (int)args[8]!;
            if (adsDuration > 0)
            {
                if (adsDelay > 0)
                    ads = new(adsDuration, adsDelay);
                else
                    ads = new(adsDuration);
            }
            m_Manager.RegisterPluginTimer(new((string)args[1]!, (string)args[2]!, (string)args[3]!, (string)args[5]!, (string)args[4]!, (string)args[6]!, (int)args[0]!, ads));
            return [];
        }
    }
}
