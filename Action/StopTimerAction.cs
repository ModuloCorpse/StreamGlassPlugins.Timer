using CorpseLib.Actions;
using StreamGlass.Core;

namespace TimerPlugin.Action
{
    public class StopTimerAction(TimerManager manager) : AStreamGlassAction(ms_Definition)
    {
        private static readonly ActionDefinition ms_Definition = new ActionDefinition("TimerStop", "Stop a timer family")
            .AddArgument<string>("family", "Timer family to stop");
        public override bool AllowDirectCall => true;
        public override bool AllowCLICall => true;
        public override bool AllowScriptCall => true;
        public override bool AllowRemoteCall => true;

        private readonly TimerManager m_Manager = manager;

        public override object?[] Call(object?[] args)
        {
            m_Manager.StopTimer((string)args[0]!);
            return [];
        }
    }
}
