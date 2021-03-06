using BandoriBot.Handler;

namespace BandoriBot.Config
{
    public class BlacklistF : HashConfiguration<string>
    {
        public override string Name => "blacklistf.json";

        public bool InBlacklist(long group, object function)
        {
            if (function is HandlerHolder holder) function = holder.handler;
            return hash.Contains($"{group}.{function.GetType().Name}");
        }
    }
}