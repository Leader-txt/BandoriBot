using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using WebAssembly;
using WebAssembly.Runtime;

namespace GoWasmWrapper
{
    using JsObject = Dictionary<string, object>;

    public partial class GoWrapper
    {
        private readonly JsValueManager values;

        private int fdwrite(Int32 fd, Int32 iovs_ptr, Int32 iovs_len, Int32 nwritten_ptr)
        {
            (fd == 0 ? Console.Out : Console.Error).Write(string.Concat(memory.Read<iov>(iovs_ptr, iovs_len).Select(
                iov => memory.ReadString(iov.ptr, iov.len))));
            return 0;
        }
        private void valueCall(Int32 ret_addr, Int32 v_addr, Int32 m_ptr, Int32 m_len, Int32 args_ptr, Int32 args_len, Int32 args_cap, Int32 syscall, Int32 js)
        {
            var function = (values.Read(v_addr) as Dictionary<string, object>)[memory.ReadString(m_ptr, m_len)] as Delegate;
            var param = new object[args_len];
            for (int i = 0; i < args_len; ++i)
                param[i] = values.Read(args_ptr + 4 * i);
            values.Write(function.DynamicInvoke(param), ret_addr);
            memory.Write((byte)1, ret_addr + 8);
        }
        private void valueGet(Int32 retval, Int32 v_addr, Int32 p_ptr, Int32 p_len, Int32 syscall, Int32 js)
        {
            values.Write((values.Read(v_addr) as Dictionary<string, object>)[memory.ReadString(p_ptr, p_len)], retval);
        }
        private void valueIndex(Int32 ret_addr, Int32 v_addr, Int32 i, Int32 syscall, Int32 js)
        {
            values.Write((values.Read(v_addr) as object[])[i], ret_addr);
        }
        private void valueNew(Int32 ret_addr, Int32 v_addr, Int32 args_ptr, Int32 args_len, Int32 args_cap, Int32 syscall, Int32 js)
        {
            var function = values.Read(v_addr) as Delegate;
            var param = new object[args_len];
            for (int i = 0; i < args_len; ++i)
                param[i] = values.Read(args_ptr + 4 * i);
            values.Write(function.DynamicInvoke(param), ret_addr);
            memory.Write((byte)1, ret_addr + 8);
        }
        private void valueSet(Int32 v_addr, Int32 p_ptr, Int32 p_len, Int32 x_addr, Int32 syscall, Int32 js)
        {
            var obj = (values.Read(v_addr) as Dictionary<string, object>);
            if (obj == null)
                return;
            obj[memory.ReadString(p_ptr, p_len)] = values.Read(x_addr);
        }
        private void valueSetIndex(Int32 syscall, Int32 js, Int32 v_addr, Int32 i, Int32 x_addr)
        {
        }
        private void stringVal(Int32 ret_ptr, Int32 value_ptr, Int32 value_len, Int32 syscall, Int32 js)
        {
            values.Write(memory.ReadString(value_ptr, value_len), ret_ptr);
        }
        private Int32 valueLength(Int32 v_addr, Int32 syscall, Int32 js)
        {
            return (values.Read(v_addr) as object[]).Length;
        }
        private void valuePrepareString(Int32 ret_addr, Int32 v_addr, Int32 syscall, Int32 js)
        {
            var str = values.Read(v_addr) as string;
            var b = Encoding.UTF8.GetBytes(str);
            values.Write(b, ret_addr);
            memory.Write(b.Length, ret_addr + 8);
        }
        private void valueLoadString(Int32 v_addr, Int32 slice_ptr, Int32 slice_len, Int32 slice_cap, Int32 syscall, Int32 js)
        {
            var b = values.Read(v_addr) as byte[];
            memory.WriteBytes(b, slice_ptr, Math.Min(Math.Min(slice_cap, slice_len), b.Length));
        }
        private void finalizeRef(Int32 v_addr, Int32 syscall, Int32 js)
        {
            values.Free(v_addr);
        }

        private ImportDictionary CreateImportDict()
        {
            var result = new ImportDictionary();
            result.Add("wasi_unstable", "fd_write", new FunctionImport(new Func<int, int, int, int, int>(fdwrite)));
            result.Add("env", "syscall/js.valueCall", new FunctionImport(new Action<int, int, int, int, int, int, int, int, int>(valueCall)));
            result.Add("env", "syscall/js.valueGet", new FunctionImport(new Action<int, int, int, int, int, int>(valueGet)));
            result.Add("env", "syscall/js.valueIndex", new FunctionImport(new Action<int, int, int, int, int>(valueIndex)));
            result.Add("env", "syscall/js.valueNew", new FunctionImport(new Action<int, int, int, int, int, int, int>(valueNew)));
            result.Add("env", "syscall/js.valueSet", new FunctionImport(new Action<int, int, int, int, int, int>(valueSet)));
            result.Add("env", "syscall/js.valueSetIndex", new FunctionImport(new Action<int, int, int, int, int>(valueSetIndex)));
            result.Add("env", "syscall/js.stringVal", new FunctionImport(new Action<int, int, int, int, int>(stringVal)));
            result.Add("env", "syscall/js.valueLength", new FunctionImport(new Func<int, int, int, int>(valueLength)));
            result.Add("env", "syscall/js.valuePrepareString", new FunctionImport(new Action<int, int, int, int>(valuePrepareString)));
            result.Add("env", "syscall/js.valueLoadString", new FunctionImport(new Action<int, int, int, int, int, int>(valueLoadString)));
            result.Add("env", "syscall/js.finalizeRef", new FunctionImport(new Action<int, int, int>(finalizeRef)));
            return result;
        }

        private readonly UnmanagedMemory memory;
        private readonly Instance<GoInterface> instance;
        private readonly JsObject @this;

        public JsObject Global => values.Global;

        public GoWrapper(Module module)
        {
            instance = module.Compile<GoInterface>()(CreateImportDict());
            this.memory = instance.Exports.memory;
            values = new(instance.Exports.memory);
            this.@this = Global["this"] as JsObject;
            @this["_makeFuncWrapper"] = new Func<object, Action>(_ => () => { });
            Global["Object"] = new Action(() => new JsObject());
            Global["Array"] = new Action(() => new List<object>());
            instance.Exports._start();
        }

        public object RunEvent(int id, object[] args)
        {
            var evt = new JsObject
            {
                ["id"] = (double)id,
                ["this"] = @this,
                ["args"] = args
            };
            @this["_pendingEvent"] = evt;
            instance.Exports.resume();
            values.FreeObject(evt["args"]);
            values.FreeObject(evt);
            return evt["result"];
        }
    }
}