using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GoWasmWrapper
{
    public partial class GoWrapper
    {

        [StructLayout(LayoutKind.Sequential)]
        private struct iov
        {
            public int ptr, len;
        }
    }
}
