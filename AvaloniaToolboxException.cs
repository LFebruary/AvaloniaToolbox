using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaToolbox
{
    /// <summary>
    /// A custom exception class used to wrap *expected* exceptions.
    /// </summary>
    public partial class AvaloniaToolboxException : Exception
    {
        public AvaloniaToolboxException(string message) : base(message)
        {
        }

        public AvaloniaToolboxException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
