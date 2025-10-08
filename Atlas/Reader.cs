using GameHelper;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Atlas
{
    public static class Reader
    {
        public static IntPtr ProcessHandle { get; set; }

        public static T ReadMemory<T>(IntPtr address) where T : unmanaged
        {
            if (address == IntPtr.Zero) return default;
            CheckProcessHandle();

            T result = default;
            ProcessMemoryUtilities.Managed.NativeWrapper.ReadProcessMemory(ProcessHandle, address, ref result);
            return result;
        }

        public static string ReadWideString(nint address, int stringLength)
        {
            if (address == IntPtr.Zero || stringLength <= 0) return string.Empty;
            CheckProcessHandle();

            byte[] result = new byte[stringLength * 2];
            ProcessMemoryUtilities.Managed.NativeWrapper.ReadProcessMemoryArray(ProcessHandle, address, result);
            return Encoding.Unicode.GetString(result).Split('\0')[0];
        }

        [DllImport("user32.dll")]
        public static extern nint GetForegroundWindow();

        private static void CheckProcessHandle()
        {
            if (ProcessHandle == 0)
                ProcessHandle = ProcessMemoryUtilities.Managed.NativeWrapper.OpenProcess(ProcessMemoryUtilities.Native.ProcessAccessFlags.Read, (int)Core.Process.Pid);
        }
    }
}
