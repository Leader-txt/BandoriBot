using BandoriBot.Commands;
using BandoriBot.Config;
using BandoriBot.Handler;
using BandoriBot.Services;
using Native.Csharp.App.Terraria;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YukariToolBox.LightLog;
using BandoriBot.Models;
using Sora;
using Sora.Net.Config;
using Sora.EventArgs.SoraEvent;
using Sora.Enumeration.EventParamsType;

namespace BandoriBot
{
    class Program
    {
        public static object SekaiFile = new object();


        private static void PluginInitialize()
        {
            //MessageHandler.session = session;

            Configuration.Register<AntirevokePlus>();
            Configuration.Register<Activation>();
            Configuration.Register<MainServerConfig>();
            Configuration.Register<Delay>();
            Configuration.Register<MessageStatistic>();
            Configuration.Register<ReplyHandler>();
            Configuration.Register<Whitelist>();
            Configuration.Register<Admin>();
            Configuration.Register<Blacklist>();
            Configuration.Register<BlacklistF>();
            Configuration.Register<TitleCooldown>();
            //Configuration.Register<PCRConfig>();
            Configuration.Register<R18Allowed>();
            Configuration.Register<NormalAllowed>();
            Configuration.Register<AccountBinding>();
            Configuration.Register<ServerManager>();
            Configuration.Register<TimeConfiguration>();
            Configuration.Register<GlobalConfiguration>();
            Configuration.Register<Antirevoke>();
            Configuration.Register<SetuConfig>();
            Configuration.Register<Save>();
            Configuration.Register<CarTypeConfig>();
            Configuration.Register<SubscribeConfig>();
            Configuration.Register<PermissionConfig>();
            Configuration.Register<Pipe>();
            //Configuration.Register<PeriodRank>();
            Configuration.Register<GroupBlacklist>();

            MessageHandler.Register<CarHandler>();
            MessageHandler.Register(Configuration.GetConfig<ReplyHandler>());
            MessageHandler.Register<WhitelistHandler>();
            MessageHandler.Register<RepeatHandler>();
            MessageHandler.Register(Configuration.GetConfig<MessageStatistic>());
            MessageHandler.Register(Configuration.GetConfig<MainServerConfig>());

            MessageHandler.Register<YCM>();
            MessageHandler.Register<QueryCommand>();
            MessageHandler.Register<ReplyCommand>();
            //MessageHandler.Register<FindCommand>();
            MessageHandler.Register<DelayCommand>();
            MessageHandler.Register<AdminCommand>();
            MessageHandler.Register<AntirevokePlusCommand>();
            //MessageHandler.Register<SekaiCommand>();
            //MessageHandler.Register<SekaiPCommand>();
            MessageHandler.Register<WhitelistCommand>();
            MessageHandler.Register<GachaCommand>();
            //MessageHandler.Register<GachaListCommand>();
            MessageHandler.Register<Activate>();
            MessageHandler.Register<Deactivate>();
            MessageHandler.Register<BlacklistCommand>();
            MessageHandler.Register<TitleCommand>();
            //MessageHandler.Register<PCRRunCommand>();
            MessageHandler.Register<CarTypeCommand>();
            //MessageHandler.Register<SekaiLineCommand>();
            //MessageHandler.Register<SekaiGachaCommand>();
            MessageHandler.Register<PermCommand>();
            MessageHandler.Register<SendCommand>();

            /*MessageHandler.Register<DDCommand>();
            MessageHandler.Register<CDCommand>();
            MessageHandler.Register<CCDCommand>();
            MessageHandler.Register<SLCommand>();
            MessageHandler.Register<SCCommand>();
            MessageHandler.Register<TBCommand>();
            MessageHandler.Register<RCCommand>();
            MessageHandler.Register<CPMCommand>();*/

            CommandHelper.Register<AdditionalCommands.test>();
            CommandHelper.Register<AdditionalCommands.Wiki>();
            CommandHelper.Register<AdditionalCommands.泰拉商店>();
            CommandHelper.Register<AdditionalCommands.随机禁言>();
            CommandHelper.Register<AdditionalCommands.泰拉在线>();
            CommandHelper.Register<AdditionalCommands.泰拉资料>();
            CommandHelper.Register<AdditionalCommands.封>();
            CommandHelper.Register<AdditionalCommands.泰拉注册>();
            CommandHelper.Register<AdditionalCommands.泰拉每日在线排行>();
            CommandHelper.Register<AdditionalCommands.泰拉在线排行>();
            CommandHelper.Register<AdditionalCommands.泰拉物品排行>();
            CommandHelper.Register<AdditionalCommands.泰拉财富排行>();
            CommandHelper.Register<AdditionalCommands.泰拉渔夫排行>();
            CommandHelper.Register<AdditionalCommands.泰拉重生排行>();
            CommandHelper.Register<AdditionalCommands.泰拉玩家>();
            CommandHelper.Register<AdditionalCommands.泰拉背包>();
            CommandHelper.Register<AdditionalCommands.解>();
            CommandHelper.Register<AdditionalCommands.重置>();
            CommandHelper.Register<AdditionalCommands.泰拉切换>();
            CommandHelper.Register<AdditionalCommands.绑定>();
            CommandHelper.Register<AdditionalCommands.执行>();
            CommandHelper.Register<AdditionalCommands.解绑>();
            CommandHelper.Register<AdditionalCommands.开启前缀检测>();
            CommandHelper.Register<AdditionalCommands.关闭前缀检测>();
            CommandHelper.Register<AdditionalCommands.开启自动清人>();
            CommandHelper.Register<AdditionalCommands.关闭自动清人>();
            CommandHelper.Register<AdditionalCommands.加入黑名单>();
            CommandHelper.Register<AdditionalCommands.移除黑名单>();
            CommandHelper.Register<AdditionalCommands.黑名单列表>();
            CommandHelper.Register<AdditionalCommands.黑名单>();
            CommandHelper.Register<AdditionalCommands.查黑>();
            CommandHelper.Register<AdditionalCommands.服务器列表>();
            CommandHelper.Register<AdditionalCommands.解ip>();
            CommandHelper.Register<AdditionalCommands.封ip>();
            CommandHelper.Register<AdditionalCommands.saveall>();

            MessageHandler.Register<R18AllowedCommand>();
            MessageHandler.Register<NormalAllowedCommand>();
            MessageHandler.Register<SetuCommand>();
            MessageHandler.Register<ZMCCommand>();
            MessageHandler.Register<AntirevokeCommand>();
            //MessageHandler.Register<SubscribeCommand>();


            /*if (!Directory.Exists("Plugins")) Directory.CreateDirectory("Plugins");
            foreach (var type in new[] { Assembly.GetExecutingAssembly() }
                         .Concat(Directory.GetFiles("Plugins").Select(file => Assembly.LoadFrom(file)))
                         .SelectMany(asm => asm.GetTypes()))
            {
                if (type.IsAbstract) continue;

                object o = null;
                if (type.IsAssignableTo(typeof(Configuration)))
                {
                    Configuration.Register((Configuration)(o ??= System.Activator.CreateInstance(type)));
                    Utils.Log(LoggerLevel.Info, $"registering {o} to configuration");
                }

                if (type.IsAssignableTo(typeof(ICommand)))
                {
                    MessageHandler.Register((ICommand)(o ??= System.Activator.CreateInstance(type)));
                    Utils.Log(LoggerLevel.Info, $"registering {o} to command");
                }

                if (type.IsAssignableTo(typeof(IMessageHandler)))
                {
                    MessageHandler.Register((IMessageHandler)(o ??= System.Activator.CreateInstance(type)));
                    Utils.Log(LoggerLevel.Info, $"registering {o} to handler");
                }
            }*/

            Configuration.LoadAll();
            RecordDatabaseManager.InitDatabase();
            MessageHandler.SortHandler();

            /*foreach (var schedule in Configuration.GetConfig<TimeConfiguration>().t)
            {
                var s = schedule;
                ScheduleManager.QueueTimed(async () =>
                {
                    await session.SendGroupMessageAsync(s.group, await Utils.GetMessageChain(s.message, async p => await session.UploadPictureAsync(UploadTarget.Group, p)));
                }, s.delay);
            }*/

            GC.Collect();
            //MessageHandler.booted = true;
        }

        private static async Task Testing()
        {

        }

        public static void Main(string[] args)
        {
            Log.SetLogLevel(LogLevel.Fatal);
            Testing().Wait();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            new Thread(() => Apis.Program.Main2(args)).Start();
            var tasks = new List<Task>();

            PluginInitialize();

            foreach (var line in File.ReadAllLines("cqservers.txt"))
            {
                var s = line.Split(":");
                var service = SoraServiceFactory.CreateService(new ClientConfig()
                {
                    Host = s[0],
                    Port = ushort.Parse(s[1])
                });

                service.Event.OnClientConnect += Event_OnClientConnect;
                service.Event.OnFriendRequest += Event_OnFriendRequest;
                service.Event.OnGroupMessage += Event_OnGroupMessage;
                service.Event.OnPrivateMessage += Event_OnPrivateMessage;
                service.Event.OnGroupRequest += Event_OnGroupRequest;
                service.Event.OnGuildMessage += Event_OnGuildMessage;

                Console.WriteLine("connected to server");

                tasks.Add(service.StartService().AsTask());
            }

            Task.WaitAll(tasks.ToArray());
            Thread.Sleep(-1);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Assembly.LoadFrom(Path.Combine(root, $"{new AssemblyName(args.Name).Name}.dll"));
            }
            catch
            {
                return null;
            }
        }

        private static async ValueTask Event_OnGuildMessage(string eventType, GuildMessageEventArgs eventArgs)
        {
            //eventArgs.SoraApi.SendGuildMessage(eventArgs.Guild, eventArgs.Channel, "guild::rep");
            await MessageHandler.OnMessage(eventArgs.SoraApi, Utils.GetCQMessage(eventArgs.Message), new Source
            {
                Session = eventArgs.SoraApi,
                FromGroup = MessageHandler.HashGroupCache(eventArgs.Guild, eventArgs.Channel),
                IsGuild = true,
                FromQQ = eventArgs.SenderInfo.UserId,
                time = eventArgs.Time
            });
        }

        private static async ValueTask Event_OnGroupRequest(string type, AddGroupRequestEventArgs eventArgs)
        {
            if (eventArgs.SubType == GroupRequestType.Invite)
                await eventArgs.Accept();
        }

        private static async ValueTask Event_OnPrivateMessage(string type, PrivateMessageEventArgs eventArgs)
        {
            await MessageHandler.OnMessage(eventArgs.SoraApi, Utils.GetCQMessage(eventArgs.Message), new Source
            {
                Session = eventArgs.SoraApi,
                FromGroup = 0,
                FromQQ = eventArgs.SenderInfo.UserId,
                time = eventArgs.Time
            });
        }

        private static async ValueTask Event_OnGroupMessage(string type, GroupMessageEventArgs eventArgs)
        {
            await MessageHandler.OnMessage(eventArgs.SoraApi, Utils.GetCQMessage(eventArgs.Message), new Source
            {
                Session = eventArgs.SoraApi,
                FromGroup = eventArgs.SourceGroup.Id,
                FromQQ = eventArgs.SenderInfo.UserId,
                time = eventArgs.Time
            });
        }

        private static async ValueTask Event_OnFriendRequest(string type, FriendRequestEventArgs eventArgs)
        {
            await eventArgs.SoraApi.SetFriendAddRequest(eventArgs.RequestFlag, true);
        }

        private static async ValueTask Event_OnClientConnect(string type, Sora.EventArgs.SoraEvent.ConnectEventArgs eventArgs)
        {
            lock (MessageHandler.bots)
            {
                if (MessageHandler.bots.ContainsKey(eventArgs.LoginUid))
                    MessageHandler.bots.Remove(eventArgs.LoginUid);
                MessageHandler.bots.Add(eventArgs.LoginUid, eventArgs.SoraApi);
                MessageHandler.selfids.Add(eventArgs.LoginUid);
            }
            MessageHandler.booted = true;
        }

        private static void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            var ex = e.Exception;
            //while (ex is AggregateException e2) ex = e2.InnerException;
            //if (ex is ApiException) return;
            //if (ex is IOException) return;

            Console.WriteLine(e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            RecordDatabaseManager.Close();
            Console.WriteLine(e.ExceptionObject);
        }
    }
}
