using System;
using System.Collections.Generic;
using WebAssembly.Runtime;

namespace GoWasmWrapper
{
    public abstract class GoInterface
    {
        public abstract void _start();
        public abstract void resume();
        public abstract UnmanagedMemory memory { get; }
    }
}