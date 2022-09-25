using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Strid.Utility {
    /// <summary>Utility class to support easy logging</summary>
    public abstract class Loggable {
        protected abstract string ToLog();

        public void Log() { Debug.Log(ToLog()); }

        public static void LogAll(IEnumerable<Loggable> items) { Debug.Log(string.Join(", ", items.Select(item => item.ToLog()))); }
    }
}