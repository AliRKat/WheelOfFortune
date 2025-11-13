using UnityEngine;

namespace Code.Core {
    public static class GameLogger {
        public static bool Enabled = true;

        private const string SYS_COLOR = "#8080FF"; // blue
        private const string METHOD_COLOR = "#C586C0"; // purple
        private const string CTX_COLOR = "#CE9178"; // orange
        private const string INFO_COLOR = "#9CDCFE"; // cyan
        private const string WARN_COLOR = "#F9D342"; // amber
        private const string ERROR_COLOR = "#FF6B6B"; // red

        private static string Tag(string color, string text)
            => $"<color={color}>[{text}]</color>";

        private static string Compose(string system, string method, string context, string message) {
            return $"{Tag(SYS_COLOR, system)}" +
                   $"{Tag(METHOD_COLOR, method)}" +
                   $"{Tag(CTX_COLOR, context)} " +
                   $"{message}";
        }

        // INFO ----------------------------------------------------------

        public static void Log(string system, string method, string context, string message) {
            if (!Enabled) return;
            Debug.Log($"{Tag(INFO_COLOR, "INFO")} {Compose(system, method, context, message)}");
        }

        public static void Log(object sender, string method, string context, string message) {
            if (!Enabled) return;
            string sys = sender.GetType().Name;
            Debug.Log($"{Tag(INFO_COLOR, "INFO")} {Compose(sys, method, context, message)}");
        }

        // WARN ----------------------------------------------------------

        public static void Warn(string system, string method, string context, string message) {
            if (!Enabled) return;
            Debug.LogWarning($"{Tag(WARN_COLOR, "WARN")} {Compose(system, method, context, message)}");
        }

        public static void Warn(object sender, string method, string context, string message) {
            if (!Enabled) return;
            string sys = sender.GetType().Name;
            Debug.LogWarning($"{Tag(WARN_COLOR, "WARN")} {Compose(sys, method, context, message)}");
        }

        // ERROR ---------------------------------------------------------

        public static void Error(string system, string method, string context, string message) {
            if (!Enabled) return;
            Debug.LogError($"{Tag(ERROR_COLOR, "ERROR")} {Compose(system, method, context, message)}");
        }

        public static void Error(object sender, string method, string context, string message) {
            if (!Enabled) return;
            string sys = sender.GetType().Name;
            Debug.LogError($"{Tag(ERROR_COLOR, "ERROR")} {Compose(sys, method, context, message)}");
        }
    }
}
