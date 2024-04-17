using CorpseLib.Actions;

namespace TimerPlugin.Action
{
    public class StartTimerAction(TimerManager manager) : AAction(ms_Definition)
    {
        private static readonly ActionDefinition ms_Definition = new ActionDefinition("TimerStart")
            .AddArgument<string>("timer");

        private readonly TimerManager m_Manager = manager;

        public override object?[] Call(object?[] args)
        {
            m_Manager.StartTimer((string)args[0]!);
            return [];
        }
    }
}
