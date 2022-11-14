using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WebAssembly.Runtime;

namespace GoWasmWrapper
{
    public partial class GoWrapper
    {
        private class JsValueManager
        {
            private Dictionary<int, object> idmap = new();
            private Dictionary<object, int> objmap = new();
            private Dictionary<int, int> refcount = new();
            private static readonly object NULL = new();
            public Dictionary<string, object> Global { get; }
            private UnmanagedMemory memory;

            public JsValueManager(UnmanagedMemory memory)
            {
                this.memory = memory;
                this.Global = new Dictionary<string, object>();
                Add(0, double.NaN);
                Add(1, 0.0);
                Add(2, NULL);
                Add(3, true);
                Add(4, false);
                Add(5, Global);
                Global["this"] = new Dictionary<string, object>();
                Add(6, Global["this"]);
            }

            private void Add(int id, object obj)
            {
                idmap[id] = obj;
                objmap[obj] = id;
            }

            [Flags]
            private enum TypeFlag
            {
                Object = 1,
                String = 2,
                Symbol = 3,
                Function = 4,
                Nan = 0x7FF80000,
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct obj
            {
                public int id;
                public TypeFlag flag;
            }

            public void Free(int offset)
            {
                FreeIndex(memory.Read<int>(offset));
            }

            private void FreeIndex(int id)
            {
                if (--refcount[id] == 0)
                {
                    objmap.Remove(idmap[id]);
                    idmap.Remove(id);
                    refcount.Remove(id);
                }
            }

            public void FreeObject(object o)
            {
                FreeIndex(objmap[o]);
            }

            public object Read(int offset)
            {
                var f = memory.Read<double>(offset);
                return double.IsNaN(f) ? idmap[memory.Read<int>(offset)] : f;
            }

            private void Write(int id, int offset)
            {
                if (idmap[id] is double f)
                {
                    memory.Write(f, offset);
                    return;
                }

                var obj = new obj
                {
                    flag = idmap[id] switch
                    {
                        Delegate => TypeFlag.Function,
                        string => TypeFlag.String,
                        _ => TypeFlag.Object
                    } | TypeFlag.Nan,
                    id = id
                };
                if (refcount.ContainsKey(id)) ++refcount[id];
                else refcount[id] = 1;
                memory.Write(obj, offset);
            }

            public void Write(object obj, int offset)
            {
                if (obj == null)
                {
                    Write(2, offset);
                    return;
                }

                if (objmap.TryGetValue(obj, out var id))
                {
                    Write(id, offset);
                    return;
                }

                int i = 0;
                while (idmap.ContainsKey(i)) ++i;
                Add(i, obj);
                Write(i, offset);
            }
        }
    }
}