using BandoriBot.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BandoriBot.Services;
using Microsoft.EntityFrameworkCore.Query;
using Sora.Entities.Base;
using Sora.Enumeration.EventParamsType;

namespace BandoriBot.Commands
{
    public class FindCommand : ICommand
    {
        private enum PermitType
        {
            /// <summary>
            /// 成员
            /// </summary>
            None = 1,
            /// <summary>
            /// 管理
            /// </summary>
            Manage = 2,
            /// <summary>
            /// 群主
            /// </summary>
            Holder = 3
        }
        private class GroupMemberInfo
        {
            /// <summary>
            /// 获取或设置一个值, 指示成员所在群
            /// </summary>
            public long GroupId { get; set; }
            /// <summary>
            /// 获取或设置一个值, 指示成员QQ
            /// </summary>
            public long QQId { get; set; }
            /// <summary>
            /// 获取或设置一个值, 指示成员最后发言时间
            /// </summary>
            //public DateTime LastDateTime { get; set; }
            /// <summary>
            /// 获取或设置一个值, 指示成员在此群的权限
            /// </summary>
            public PermitType PermitType { get; set; }
        }
        private class GroupInfo
        {
            /// <summary>
            /// 群号码
            /// </summary>
            public long Id;
            /// <summary>
            /// 群名字
            /// </summary>
            public string Name;
        }

        private static async Task<List<GroupMemberInfo>> GetMemberList(SoraApi session, long groupId)
        {
            return (await session.GetGroupMemberList(groupId)).groupMemberList
                .Select(info => new GroupMemberInfo
                {
                    GroupId = groupId,
                    QQId = info.UserId,
                    PermitType = info.Role switch
                    {
                        MemberRoleType.Owner => PermitType.Holder,
                        MemberRoleType.Admin => PermitType.Manage,
                        _ => PermitType.None
                    }
                }).ToList();
        }

        private List<GroupMemberInfo> infos = new List<GroupMemberInfo>();
        private List<GroupInfo> groups = new List<GroupInfo>();
        public List<string> Alias => new List<string>
        {
            "/find"
        };

        public async Task Run(CommandArgs args)
        {
            string[] splits = args.Arg.Trim().Split(' ');
            if (splits.Length < 1)
            {
                await args.Callback("/find <refresh/count/id> ...");
                return;
            }
            switch (splits[0])
            {
                case "refresh":
                    if (!await args.Source.HasPermission("management.find.refresh", -1))
                    {
                        await args.Callback("Access denied!");
                        return;
                    }
                    infos.Clear();
                    await args.Callback($"refreshing...please wait.");
                    groups = new();
                    foreach (var group in (await MessageHandler.GetGroupList()).GroupBy(t => t.Item1)
                             .Select(g => g.First()))
                    {
                        foreach (var member in await GetMemberList(group.Item2, group.Item1) ?? new List<GroupMemberInfo>())
                            infos.Add(member);
                        var inf = (await group.Item2.GetGroupInfo(group.Item1)).groupInfo;
                        groups.Add(new GroupInfo()
                        {
                            Id = inf.GroupId,
                            Name = inf.GroupName
                        });
                    }
                    var idhash = new HashSet<long>(groups.Select((group) => group.Id));
                    var groupfile = Path.Combine("groups.json");
                    if (File.Exists(groupfile))
                    {
                        try
                        {
                            await args.Callback("reading from groups.json...");
                            var json = JArray.Parse(File.ReadAllText(groupfile));
                            foreach (JObject group in json)
                            {
                                GroupInfo info = new GroupInfo
                                {
                                    Id = (long)group["id"],
                                    Name = (string)group["name"]
                                };
                                if (!idhash.Contains(info.Id))
                                {
                                    groups.Add(info);
                                    idhash.Add(info.Id);
                                }
                                foreach (JObject member in group["members"])
                                    infos.Add(new GroupMemberInfo
                                    {
                                        QQId = (long)member["qq"],
                                        PermitType = (PermitType)Enum.Parse(typeof(PermitType), (string)member["position"]),
                                        GroupId = info.Id
                                    });

                            }
                        }
                        catch (Exception e)
                        {
                            await args.Callback(e.ToString());
                        }
                    }
                    await args.Callback($"Member info has refreshed succussfully. ({infos.Count} records in {groups.Count} groups)");
                    break;
                case "count":
                    var users = new HashSet<long>();
                    DateTime active;
                    try
                    {
                        active = DateTime.Parse(splits[1]);
                    }
                    catch
                    {
                        active = new DateTime(0);
                    }
                    foreach (var member in infos)
                        users.Add(member.QQId);
                    await args.Callback(@$"{
                        new HashSet<long>(infos
                            .Select((info) => info.QQId)).Count
                        } members in total {(active.Ticks == 0 ? "" : "is active after " + active.ToString())} (counting in {groups.Count} groups).");
                    break;
                case "id":
                    if (splits.Length < 2)
                    {
                        await args.Callback("Invalid argument count.");
                        return;
                    }
                    if (long.TryParse(splits[1], out long qq))
                    {
                        if (!await args.Source.HasPermission("management.find.id", -1))
                        {
                            await args.Callback("Access denied.");
                            return;
                        }
                        var total = 0;
                        var list = string.Concat(infos.Where((member) => member.QQId == qq)
                            .Select((member) => @$"{++total}. {
                                groups.Where((group) => group.Id == member.GroupId)
                                .FirstOrDefault()?.Name ?? "<未找到群信息>"}({member.GroupId}) {
                                member.PermitType switch
                                {
                                    PermitType.Holder => "(群主)",
                                    PermitType.Manage => "(管理)",
                                    _ => "",
                                }}
"));
                        await args.Callback($"{qq}所在的群(共{total}个):\n{list}");
                    }
                    break;

            }
        }
    }
}
