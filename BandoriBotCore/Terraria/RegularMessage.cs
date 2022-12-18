using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using static BandoriBot.Commands.AdditionalCommands;

namespace BandoriBot.Terraria
{
    public class RegularMessage
    {
        public class Config
        {
            private const string path = "RegularMessage.json";
            private static Config instance = null;
            public static Config Context
            {
                get
                {
                    if (instance == null)
                    {
                        if (!File.Exists(path))
                        {
                            instance = new Config();
                            File.WriteAllText(path, JsonConvert.SerializeObject(instance, Formatting.Indented));
                        }
                        else
                            instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
                    }
                    return instance;
                }
            }
            public Dictionary<long, long> Channel { get; set; } = new Dictionary<long, long>() { { 63998841636727701, 1344229 } };
            public int Timer { get; set; }
        }
        public static void Start()
        {
            var config = Config.Context;
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        foreach (var channel in config.Channel)
                        {
                            var info = 泰拉在线.Text();
                            Console.WriteLine($"send to [{channel.Key}::{channel.Value}] {info}");
                            MessageHandler.session.SendGuildMessage(channel.Key, channel.Value, info);
                        }
                        Thread.Sleep(1000 * config.Timer);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                    }
                }
            })
            { IsBackground = true }.Start();
        }
    }
}
