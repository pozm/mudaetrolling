using System;
using System.Threading;

namespace MudaeTrolling
{
    public class Utils
    {
        public static DateTime ParseTime(string x)
        {
            int h = 0;
            int m = 0;
            if (x.Contains("h"))
            {
                var tillH = x.IndexOf("h");
                h = int.Parse(x.Substring(0, tillH));
                m = int.Parse(x.Substring(tillH + 1).Trim());
            }
            else
            {
                m = int.Parse(x);
            }
            
            DateTime newDate = DateTime.Now;
            newDate = newDate.AddHours(h);
            newDate = newDate.AddMinutes(m);
            return newDate;

        }
        public static long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        public static TimeSpan GetTimespanFromDate(DateTime d)
        {
            var now = ToUnixTime(DateTime.Now);
            var then = ToUnixTime(d);
            var diff = then - now;
            return TimeSpan.FromSeconds((int) (diff > 0 ? diff : 0)).Add(TimeSpan.FromSeconds(2));
        }
    }
}