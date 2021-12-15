using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MudaeTrolling.stat
{
    public class Figures
    {

        public TimeSpan TimeTillReset = new TimeSpan();
        public int Usages  = 0;
        public bool Usable = false;
        public bool HasTimer  = false;

        public delegate void DeUsable();
        public event DeUsable UponUsable;


        
        public Figures(TimeSpan ttr,int u, bool us, bool t)
        {
            TimeTillReset = ttr;
            Usages = u;
            Usable = us;
            HasTimer = t;
            Task.Factory.StartNew(WaitUponUsable);
        }
        public void WaitUponUsable()
        {
            var be = TimeTillReset;
            bool com = false;
            while (true)
            {
                if (com && be == TimeTillReset)
                {
                    Task.Delay(TimeSpan.FromSeconds(3)).Wait();
                    continue;
                }
                Task.Delay(TimeTillReset).Wait();
                UponUsable?.Invoke();
                be = TimeTillReset;
                com = true;
            }
        }

    }
}