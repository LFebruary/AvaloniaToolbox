using Avalonia;
using Avalonia.Platform;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AvaloniaToolbox.Functions
{
    internal static partial class CustomDebug
    {
        /// <summary>
        /// Custom debug method that provides additional detail about origin caller and file.
        /// </summary>
        /// <param name="debugMessage">Message to write to console</param>
        /// <param name="file">Name of file from where the Debug is called from</param>
        /// <param name="member">Member that calls the Debug method</param>
        /// <param name="line">Line number in file from where the Debug method is called</param>
        [Conditional("DEBUG")]
        internal static void WriteLine(object? debugMessage, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Debug.WriteLine($"{Path.GetFileName(file)} - {member}({line}): {debugMessage}");
        }

        public static bool IsDebug =>
#if DEBUG
                true;
#else
                false;
#endif

        public static bool IsRelease => IsDebug == false;

        public static OperatingSystemType CurrentPlatform => AvaloniaLocator.Current.GetService<IRuntimePlatform>()?.GetRuntimeInfo().OperatingSystem 
            ?? throw new NullReferenceException("Null value encountered for runtime platform service");

    }
}
