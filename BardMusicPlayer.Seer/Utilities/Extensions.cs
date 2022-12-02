﻿#region

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

#endregion

namespace BardMusicPlayer.Seer.Utilities
{
    internal static class SeerExtensions
    {
        private static readonly uint TOKEN_QUERY = 0x0008;

        internal static WindowsIdentity WindowsIdentity(this Process process)
        {
            var ph = IntPtr.Zero;
            WindowsIdentity wi;
            try
            {
                OpenProcessToken(process.Handle, TOKEN_QUERY, out ph);
                wi = new WindowsIdentity(ph);
            }
            finally
            {
                if (ph != IntPtr.Zero) CloseHandle(ph);
            }

            return wi;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);
    }
}