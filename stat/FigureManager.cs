using System;
using System.Text.RegularExpressions;

namespace MudaeTrolling.stat
{
    public class FigureManager
    {
        public Figures Claim;
        public Figures Roll;
        public Figures ReRoll;
        public Figures Daily;
        public Figures Dk;
        public Figures Vote;

        public static Regex TimeMatcher = new Regex(@"\*\*(?<val>[\dh ]*?)\*\* min", RegexOptions.Singleline | RegexOptions.Compiled);
        public static Regex IntMatcher = new Regex(@"have \*\*(?<val>\d*?)\*\*", RegexOptions.Singleline | RegexOptions.Compiled);

        TimeSpan GetTime(string x)
        {
            var b = TimeMatcher.Match(x);
            if (b.Length == 0)
                return new TimeSpan();
            return Utils.GetTimespanFromDate(Utils.ParseTime(b.Groups["val"].ToString()));
        }

        int GetInt(string x)
        {
            try
            {
                return Int32.Parse(IntMatcher.Match(x).Groups["val"].ToString());

            }
            catch
            {
                return 0;
            }
        }
        
        public FigureManager()
        {
            Claim = new Figures(new TimeSpan(), 0, false, false);
            Roll = new Figures(new TimeSpan(), 0, false, false);
            ReRoll = new Figures(new TimeSpan(), 0, false, false);
            Daily = new Figures(new TimeSpan(), 0, false, false);
            Dk = new Figures(new TimeSpan(), 0, false, false);
            Vote = new Figures(new TimeSpan(), 0, false, false);
        }

        public bool ParseData(string content)
        {
            var lines = content.Split('\n');
            if (lines.Length < 5)
                return false;
            if (lines[0].Contains("__can__"))
            {
                Claim.Usable = true;
            }
            if (lines[3].Contains("available"))
            {
                Daily.Usable = true;
            }
            if (lines[4].Contains("ready"))
            {
                Dk.Usable = true;
            }
            if (lines[5].Contains("may vote right"))
            {
                Dk.Usable = true;
            }

            Claim.TimeTillReset = GetTime(lines[0]);
            Roll.TimeTillReset = GetTime(lines[1]);
            Daily.TimeTillReset = GetTime(lines[2]);
            Dk.TimeTillReset = GetTime(lines[3]);
            Vote.TimeTillReset = GetTime(lines[4]);

            Roll.Usages = GetInt(lines[1]);
            ReRoll.Usages = GetInt(lines[2]);

            return true;
        }
    }
}