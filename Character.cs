using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Discord;
using WebSocketSharp;

namespace MudaeTrolling
{
    public class Character
{

        static private Regex DescMatcher =
            new Regex(
                @"(?<series>.*)\n(Claims: ?#(?<claim>\d*)\n)?(Likes: ?#(?<like>\d*)\n)?(\*\*(?<kakera>\d*)\*\*<:kakera:469835869059153940>\n)?",
                RegexOptions.Compiled | RegexOptions.Multiline);
        
        public string CharacterName { get; }
        public string Series { get; }
        public int Claims { get; }
        public int Likes { get; }
        public int Kakera { get; }
        public bool IsOwned = false;
        public bool hasData = false;
        public string CharacterArt { get; }
        public DiscordMessage Message;
        public DiscordEmbed Embed;
        public Boolean IsWished { get; } = false;
        public Boolean IsWishedByMe { get; } = false;
        public List<DiscordUser> Wishers { get; } = new List<DiscordUser>();


        private string GetFirstLine(string x)
        {
            return x.Substring(0, x.IndexOf('\n'));
        }

        public Character( DiscordMessage messageEmbed )
        {
            Message = messageEmbed;
            if ( Message.Embed == null || Message.Author.User.Id != 432610292342587392 || !Message.Embed.Description.EndsWith("React with any emoji to claim!") || Message.Embed.Footer.Text == "1 / 30")
                return;
            CharacterName = (Embed = messageEmbed.Embed).Author.Name;
            Series = GetFirstLine(Embed.Description);
            Console.WriteLine($"[MATCHER] {Embed.Description}");
            var Matched = DescMatcher.Match(Embed.Description);
            if (!Ext.IsNullOrEmpty(Matched.Groups["claim"].ToString()))
                Claims = Int32.Parse(Matched.Groups["claim"].ToString());
            if (!Ext.IsNullOrEmpty(Matched.Groups["like"].ToString()))
                Likes = Int32.Parse(Matched.Groups["like"].ToString());
            if (!Ext.IsNullOrEmpty(Matched.Groups["kakera"].ToString()))
                Kakera = Int32.Parse(Matched.Groups["kakera"].ToString());
            CharacterArt = Embed.Image.Url;
            IsWished = Message.Mentions.Count > 0;
            IsOwned = (Embed.Footer.Text ?? "aaaaw").ToLower().Contains("belongs");
            IsWishedByMe = Message.Mentions.Select(v => v.Id).Contains(Message.Client.User.Id);
            Wishers = Message.Mentions.ToList();
            hasData = Kakera > 0;
        }

        public void Take()
        {
            Message.AddReaction("❤");
            Program.Data.Claim.Usable = false;
        }

        public FetchedCharacter Fetch()
        {
            return new FetchedCharacter(CharacterName);
        }
        
    }
}