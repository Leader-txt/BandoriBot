using BandoriBot.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandoriBot.Commands
{
    public class AdminCommand : HashCommand<Admin, long>, ICommand
    {
        public override List<string> Alias => new List<string>
        {
            "/admin"
        };

        protected override string Permission => throw new NotImplementedException();

        string ICommand.Permission => "management.admin";

        public override async Task Run(CommandArgs args)
        {
            await base.Run(args);
        }
    }
}
