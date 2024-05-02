using CorpseLib.Actions;
using StreamGlass.Core;

namespace TimerPlugin.Action
{
    public class StartTimerAction(TimerManager manager) : AStreamGlassAction(ms_Definition)
    {
        private static readonly ActionDefinition ms_Definition = new ActionDefinition("TimerStart", "Start a timer")
            .AddArgument<string>("timer", "ID of the timer to start");
        public override bool AllowDirectCall => true;
        public override bool AllowCLICall => true;
        public override bool AllowScriptCall => true;
        public override bool AllowRemoteCall => true;

        private readonly TimerManager m_Manager = manager;

        public override object?[] Call(object?[] args)
        {
            m_Manager.StartTimer((string)args[0]!);
            return [];
        }
    }
}
