using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Discord;
using Discord.Gateway;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Humanizer;
using MudaeTrolling.stat;
using WebSocketSharp;
using MessageEventArgs = Discord.Gateway.MessageEventArgs;

namespace MudaeTrolling
{
    class Program
    {
        enum State
        {
            FetchingData,
            Rolling,
            Waiting,
        }
        public static ulong mudaeId = 432610292342587392;
        public static ulong GuildId = 886571875893903390;
        public static FigureManager Data = new FigureManager();

        public static DiscordSocketClient client;
        public static TextChannel mainChannel;

        private static bool Started = false;
        private static State currentState = State.FetchingData;
        static void Main(string[] args)
        {
            client = new DiscordSocketClient(new DiscordSocketConfig() {});
            Console.Write("Token: ");
            client.Login(Console.ReadLine());
            Console.Clear();
            client.OnLoggedIn += Client_LoggedIn;
            client.OnMessageReceived += Client_MessageRecieved;
            Thread.Sleep(-1);
            Console.WriteLine("ok");
        }

        public static void FetchData()
        {
            mainChannel.SendMessage("$tu");
        }
        
        private static void Client_LoggedIn(DiscordSocketClient client, LoginEventArgs args)
        {
            Console.WriteLine($"Logged into {args.User.Username}");
            mainChannel = client.GetChannel(886571875893903393) as TextChannel;
            FetchData();
            Task.Delay(TimeSpan.FromSeconds(2)).Wait();
            Console.WriteLine(Data.Claim.Usable);
            Data.Roll.UponUsable += UponRollReset;
            Data.Vote.UponUsable += VoteMudae;
            Data.Dk.UponUsable += () =>
            {
                mainChannel.SendMessage("$dk");
            };
            Data.Daily.UponUsable+= () =>
            {
                mainChannel.SendMessage("$daily");
            };

        }
        private static void Client_MessageRecieved(DiscordSocketClient client, MessageEventArgs args)
        {
            if (args.Message.Author.User.Id != mudaeId || args.Message.Guild.Id != GuildId)
                return;
            Console.WriteLine("New message from mudae.");
            var Character = new Character(args.Message);

            if (Character.hasData)
            {
                Console.WriteLine($"{Character.CharacterName} - {Character.IsWishedByMe}. L {Character.Likes}. C {Character.Claims}. K {Character.Kakera}. S {Character.Series}. W {String.Join(" | ",Character.Wishers.Select(v=>v.Username) )}");
                if (Character.IsWishedByMe || Character.Likes < 600 || Character.Claims < 600)
                {
                    Console.WriteLine("Would of taken.");
                    Console.WriteLine(args.Message.Embed.Description);
                    // Character.Take();
                    if (Character.IsWishedByMe)
                    {
                        Console.WriteLine("Captured a wish listed character, removing it from the wish list.");
                        args.Message.Channel.SendMessage($"$wr {Character.CharacterName}");
                    }
                }
            }

            if (currentState == State.FetchingData && args.Message.Content.StartsWith($"**{client.User.Username}**"))
            {
                Data.ParseData(args.Message.Content);
            }
        }

        private static void VoteMudae()
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();

            startInfo.WorkingDirectory = @$"{Environment.CurrentDirectory}\MudaeAutoVote";
            startInfo.FileName = "node";
            startInfo.Arguments = "./dist";
            startInfo.RedirectStandardOutput = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            
            process.StartInfo = startInfo;

            process.Start();
            process.WaitForExit();
        }
        private static void UponRollReset()
        {
            if (!Data.Claim.Usable)
            {
                Console.WriteLine("Unable to claim, so ignoring.");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < Data.Roll.Usages; i++)
                {
                    if (!Data.Claim.Usable)
                    {
                        Console.WriteLine("Unable to claim mid way through rolling, assumed claimed so stopping.");
                        break;
                    }
                    mainChannel.SendMessage("$wa");
                    Task.Delay(TimeSpan.FromSeconds(2)).Wait();
                }

            }).Wait();
            
            Console.WriteLine("Used all rolls (?)");

        }
    }
}