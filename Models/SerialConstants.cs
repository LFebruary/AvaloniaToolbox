using System.Collections.ObjectModel;

namespace AvaloniaToolbox.Models
{
    public static class SerialConstants
    {
        public enum FlowControl
        {
            Ctr_Rts,
            Dsr_Dtr,
            Xon_Xoff,
            None
        }

        public const string FlowControlCtsRts   = "CTS/RTS";
        public const string FlowControlDsrDtr   = "DSR/DTS";
        public const string FlowControlXonXoff  = "XON/XOFF";
        public const string FlowControlNone     = "None";
        public const string FlowControlDefault  = FlowControlNone;
        public static readonly IList<string> FlowControlValues = new ReadOnlyCollection<string>(
            new List<string> {
                FlowControlCtsRts,
                FlowControlDsrDtr,
                FlowControlXonXoff,
                FlowControlNone
            });

        #region Parity
        internal const string DefaultParity = NoParity;
        internal const string EvenParity    = "Even";
        internal const string OddParity     = "Odd";
        internal const string NoParity      = "None";
        public static readonly IList<string> ParityValues = new ReadOnlyCollection<string>(
            new List<string> {
                EvenParity,
                OddParity,
                NoParity
            });
        #endregion

        #region Misc
        internal static readonly int s_defaultTimeout = 1000;
        internal const string InvalidValue = "NAN";
        #endregion
    }
}
