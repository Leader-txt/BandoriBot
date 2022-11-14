using System.Runtime.InteropServices;
using System.Text;
using WebAssembly.Runtime;

namespace GoWasmWrapper
{
    internal static class MemoryExtension
    {
        public static byte[] ReadBytes(this UnmanagedMemory mem, int offset, int count)
        {
            var res = new byte[count];
            Marshal.Copy(mem.Start + offset, res, 0, count);
            return res;
        }

        public static void WriteBytes(this UnmanagedMemory mem, byte[] bytes, int offset, int count)
        {
            Marshal.Copy(bytes, 0, mem.Start + offset, count);
        }

        public static T Read<T>(this UnmanagedMemory mem, int offset) where T : struct
        {
            return (T)Marshal.PtrToStructure(mem.Start + offset, typeof(T));
        }
        public static void Write<T>(this UnmanagedMemory mem, T t, int offset) where T : struct
        {
            Marshal.StructureToPtr(t, mem.Start + offset, true);
        }
        public static T[] Read<T>(this UnmanagedMemory mem, int offset, int count) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            var res = new T[count];
            for (int i = 0; i < count; ++i)
            {
                res[i] = mem.Read<T>(offset + i * size);
            }
            return res;
        }

        public static string ReadString(this UnmanagedMemory mem, int offset, int count)
        {
            return Encoding.UTF8.GetString(mem.ReadBytes(offset, count));
        }
    }
}