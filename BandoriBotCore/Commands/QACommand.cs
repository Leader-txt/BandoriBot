using System;
using BandoriBot.Config;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AssetsTools;
using BandoriBot.Services;
using Newtonsoft.Json.Linq;
using PCRApi;
using PCRApi.Controllers;
using PCRApi.Models.Db;

namespace BandoriBot.Commands
{
    public class QACommand : ICommand
    {
        private AssetManager mgr = new ();

        public List<string> Alias => new List<string> { "/qa" };
        public async Task Run(CommandArgs args)
        {
            var a = args.Arg.Trim().Split(' ');
            switch (a[0])
            {
                case "query":
                    var now = DateTime.Now;
                    await args.Callback("����������:\n" + string.Join("\n", ISchedule.Schedules.SelectMany(sch => sch.AsEnumerable()
                        .Where(s => DateTime.Parse(s.StartTime) > now).Select(s => $"{s.StartTime}-{s.EndTime} {s.Description}"))));
                    break;
                case "update":
                    var client = new AssetController.PCRClient();
                    client.urlroot = "http://l3-qa2-all-gs-gzlj.bilibiligame.net/";
                    var manifest = client.Callapi("source_ini/get_resource_info", new JObject { ["viewer_id"] = "0" });

                    await mgr.Initialize(a[1],
                        (string)manifest["movie_ver"],
                        (string)manifest["sound_ver"], manifest["resource"][0].ToString());
                    var ab = await mgr.ResolveAssetsBundle("a/masterdata_master.unity3d", "master_data.unity3d");
                    var af = ab.Files[0].ToAssetsFile();
                    await File.WriteAllBytesAsync("Data/master.db", af.Objects[0].Data.Skip(16).ToArray());
                    masterContextCache.instance = new masterContext();
                    await args.Callback($"manifest updated to {a[1]}");
                    break;
            }
        }
    }
}
