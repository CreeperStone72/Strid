using System;
using System.Threading.Tasks;

namespace Strid.Utility {
    public static class TaskUtils {
        public static async Task WaitUntil(Func<bool> predicate, int sleep = 50) { while (!predicate()) await Task.Delay(sleep); }
    }
}