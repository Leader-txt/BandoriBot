using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BandoriBot.Handler;
using BandoriBot.Terraria;
using Newtonsoft.Json;

namespace BandoriBot.Config
{
    public class Guild
    {
        public long guild { get; set; }
        public long channel { get; set; }
    }
    [JsonObject]
    public class ServerConfig
    {
        public string host;
        public ushort port;
        public string format = string.Empty;
        public long[] groups = Array.Empty<long>();
        public Guild[] guilds = Array.Empty<Guild>();
        public uint owner_color;
        public uint admin_color;
        public uint member_color;
    }

    public class MainServerConfig : SerializableConfiguration<ServerConfig>, IMessageHandler
    {
        public override string Name => "trserver.json";
        private MainServer server;

        public override void LoadDefault()
        {
            t = new ServerConfig();
        }

        public override void LoadFrom(BinaryReader br)
        {
            base.LoadFrom(br);
            server = new MainServer(t);
            server.OnServerMessage += msg =>
            {
                msg = Regex.Replace(msg, @"\[c/.{6}:(.*?)\]", "$1");
                //msg = Regex.Replace(msg, @"\[i:([0-9]+)\]", $"[mirai:imagepath={Path.GetFullPath("items/Item_$1.png")}]");
                foreach (var group in t.groups)
                    MessageHandler.session.SendGroupMessage(group, msg).AsTask().Wait();
                foreach (var guild in t.guilds)
                {
                    MessageHandler.session.SendGuildMessage(guild.guild, guild.channel, msg);
                }
            };
        }

        public void SendMsg(string msg, Source sender)
        {
            server.SendMsg(sender, msg);
        }

        public override void Dispose()
        {
            server.Dispose();
        }
        public bool IgnoreCommandHandled => true;
        
        public async Task<bool> OnMessage(HandlerArgs args)
        {
            args.message = new Regex(@"&#(.*?);").Replace(args.message, x => "" + (char)ushort.Parse(x.Result("$1")));
            args.message = new Regex(@"\[mirai:at=(.*?)\]").Replace(args.message, x => "@" + Utils.GetGuildName(args.Sender.Session, args.Sender.FromGroup, long.Parse(x.Result("$1"))).Result);
            args.message = Regex.Replace(args.message, @"\[mirai:imagenew=(.*?)\]", "[图片(请在频道查看)]");
            args.message = Regex.Replace(args.message, @"\[mirai:face=(.*?)\]", x=>"["+(FaceID)int.Parse(x.Result("$1"))+"]");
            //args.message = args.message.Replace("&#91;", "[").Replace("&#93;", "]");
            if (t.groups.Contains(args.Sender.FromGroup))
            {
                SendMsg(args.message, args.Sender);
            }
            if (args.Sender.IsGuild)
            {
                var cache = MessageHandler.GetGroupCache(args.Sender.FromGroup);
                if (t.guilds.ToList().FindAll(x=>x.guild==cache.guild && x.channel==cache.channel).Any())
                {
                    SendMsg(args.message, args.Sender);
                }
            }

            return false;
        }
        enum FaceID
        {
            得意 = 4,
            流泪 = 5,
            睡 = 8,
            大哭 = 9,
            尴尬 = 10,
            调皮 = 12,
            微笑 = 14,
            酷 = 16,
            可爱 = 21,
            傲慢 = 23,
            饥饿 = 24,
            困 = 25,
            惊恐 = 26,
            流汗 = 27,
            憨笑 = 28,
            悠闲 = 29,
            奋斗 = 30,
            疑问 = 32,
            嘘 = 33,
            晕 = 34,
            敲打 = 38,
            再见 = 39,
            发抖 = 41,
            爱情 = 42,
            跳跳 = 43,
            拥抱 = 49,
            蛋糕 = 53,
            咖啡 = 60,
            玫瑰 = 63,
            爱心 = 66,
            太阳 = 74,
            月亮 = 75,
            赞 = 76,
            握手 = 78,
            胜利 = 79,
            飞吻 = 85,
            西瓜 = 89,
            冷汗 = 96,
            擦汗 = 97,
            抠鼻 = 98,
            鼓掌 = 99,
            糗大了 = 100,
            坏笑 = 101,
            左哼哼 = 102,
            右哼哼 = 103,
            哈欠 = 104,
            委屈 = 106,
            左亲亲 = 109,
            可怜 = 111,
            示爱 = 116,
            抱拳 = 118,
            拳头 = 120,
            爱你 = 122,
            NO = 123,
            OK = 124,
            转圈 = 125,
            挥手 = 129,
            喝彩 = 144,
            棒棒糖 = 147,
            茶 = 171,
            泪奔 = 173,
            无奈 = 174,
            卖萌 = 175,
            小纠结 = 176,
            喷血=177,
            doge = 179,
            惊喜 = 180,
            骚扰 = 181,
            笑哭 = 182,
            我最美 = 183,
            点赞 = 201,
            托脸 = 203,
            托腮 = 212,
            啵啵 = 214,
            蹭一蹭 = 219,
            抱抱 = 222,
            拍手 = 227,
            佛系 = 232,
            喷脸 = 240,
            甩头 = 243,
            加油抱抱 = 246,
            脑阔疼 = 262,
            捂脸 = 264,
            辣眼睛 = 265,
            哦哟 = 266,
            头秃 = 267,
            问号脸 = 268,
            暗中观察 = 269,
            emm = 270,
            吃瓜 = 271,
            呵呵哒 = 272, 
            我酸了 = 273,
            汪汪 = 277,
            汗 = 278,
            无眼笑 = 281,
            敬礼 = 282,
            面无表情 = 284,
            摸鱼 = 285,
            哦 = 287,
            睁眼 = 289,
            敲开心 = 290,
            摸锦鲤 = 293,
            期待 = 294,
            拜谢 = 297,
            元宝 = 298,
            牛啊 = 299,
            右亲亲 = 305,
            牛气冲天 = 306,
            喵喵 = 307,
            仔细分析 = 314,
            加油 = 315,
            崇拜 = 318,
            比心 = 319,
            庆祝 = 320,
            拒绝 = 322,
            吃糖 = 324,
            生气 = 326
        }
    }
}