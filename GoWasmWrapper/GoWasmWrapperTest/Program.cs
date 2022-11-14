using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using GoWasmWrapper;
using WebAssembly.Runtime;
using WebAssembly.Runtime.Compilation;
using Module = WebAssembly.Module;

namespace GoWasmWrapperTest
{
    using JsObject = Dictionary<string, object>;
    internal class Program
    {
        private static GoWrapper wrapper;

        private static double calcHash(string str)
        {
            var text = Encoding.ASCII.GetBytes(str);
            uint _0x473e93, _0x5d587e;
            for (_0x473e93 = 0x1bf52, _0x5d587e = (uint)text.Length; _0x5d587e != 0;)
                _0x473e93 = 0x309 * _0x473e93 ^ text[--_0x5d587e];
            return _0x473e93 >> 0x3;
        }

        private static double myHash(string str)
        {
            var text = Encoding.ASCII.GetBytes(str);
            uint _0x473e93, _0x5d587e;
            for (_0x473e93 = 0x202, _0x5d587e = (uint)text.Length; _0x5d587e != 0;)
                _0x473e93 = 0x72 * _0x473e93 ^ text[--_0x5d587e];
            return _0x473e93 >> 0x3;
        }

        private static JsObject evt;
        
        static void Main(string[] args)
        {
            var wrapper = new GoWrapper(Module.ReadFromBinary("pcrd.wasm"));

            var @this = wrapper.Global["this"] as JsObject;
            wrapper.Global["myhash"] = new Func<string, double>(myHash);
            wrapper.Global["location"] = new JsObject
            {
                ["host"] = "pcrdfans.com",
                ["hostname"] = "pcrdfans.com",

            };
            
            while (true)
            {
                Console.WriteLine(wrapper.RunEvent(1, new object[]
                {
                    $"{{\"def\":[111001,100801,112201,102801,103001],\"language\":0,\"nonce\":\"6n3m3yrc8ik2iqv4\",\"page\":1,\"region\":1,\"sort\":1,\"ts\":1639143316}}",
                    "6n3m3yrc8ik2iqv4",
                    calcHash("6n3m3yrc8ik2iqv4")
                }));
            }
        }
    }
}
