using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Gateway;

namespace MudaeTrolling
{
    public class FetchedCharacter
    {
        
        static private Regex DescMatcher =
            new Regex(
                @"(?<series>.*?)<:(?<gender>female|male):\d*>\n.*· \*\*(?<kakera>\d*)\*\*<:kakera:469835869059153940>\nClaim Rank: #(?<claim>\d*)\nLike Rank: #(?<like>\d*)",
                RegexOptions.Compiled | RegexOptions.Multiline);
        
        private bool Waiting = true;
        
        private DiscordMessage msg;
        public string CharacterName { get; } 
        public string Series { get; }
        public int LikeRank { get; }
        public int ClaimRank { get; }
        public bool Gender { get; }
        public int Kakera { get; }
        public string AvatarUrl { get; }
        public FetchedCharacter(string chrName)
        {
            TextChannel lookupChannel = Program.mainChannel;
            lookupChannel?.SendMessage($"$im {chrName}");
            Program.client.OnMessageReceived += Resolver;
            WaitForMessage().Start();
            WaitForMessage().Wait();
            Program.client.OnMessageReceived -= Resolver;
            var Embed = msg.Embed;
            CharacterName = Embed.Author.Name;
            var Matched = DescMatcher.Match(Embed.Description);
            Series = Matched.Groups["series"].ToString();
            Gender = Matched.Groups["gender"].ToString() == "male";
            Kakera = Int32.Parse(Matched.Groups["kakera"].ToString());
            LikeRank =Int32.Parse(Matched.Groups["like"].ToString());
            ClaimRank =Int32.Parse(Matched.Groups["claim"].ToString());

            AvatarUrl = Embed.Image.Url;
        }

        private async Task WaitForMessage()
        {
            while (Waiting)
            {
                
                await Task.Delay(25);
            }
        }

        private void Resolver(DiscordSocketClient client, MessageEventArgs args)
        {
            if (args.Message.Author.User.Id != Program.mudaeId || args.Message.Channel.Id != 886671522180263947)
                return;
            Waiting = false;
            msg = args.Message;
        }
        
    }
}