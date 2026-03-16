using Lumos.Agent.Models;
using System;
using System.Runtime.InteropServices;

namespace Lumos.Agent.Collectors
{
    public static class UserSessionCollector
    {
        private const int WTS_CURRENT_SERVER_HANDLE = 0;

        public static UserSessionInfo GetLoggedUser()
        {
            IntPtr sessionInfoPtr = IntPtr.Zero;
            int sessionCount = 0;

            try
            {
                if (!WTSEnumerateSessions(
                        WTS_CURRENT_SERVER_HANDLE,
                        0,
                        1,
                        out sessionInfoPtr,
                        out sessionCount))
                {
                    return null;
                }

                int dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
                long current = sessionInfoPtr.ToInt64();

                for (int i = 0; i < sessionCount; i++)
                {
                    var sessionInfo = Marshal.PtrToStructure<WTS_SESSION_INFO>(
                        new IntPtr(current));

                    if (sessionInfo.State == WTS_CONNECTSTATE_CLASS.WTSActive)
                    {
                        string user = QuerySessionString(
                            sessionInfo.SessionID,
                            WTS_INFO_CLASS.WTSUserName);

                        string domain = QuerySessionString(
                            sessionInfo.SessionID,
                            WTS_INFO_CLASS.WTSDomainName);

                        if (!string.IsNullOrWhiteSpace(user))
                        {
                            return new UserSessionInfo
                            {
                                SessionId = sessionInfo.SessionID.ToString(),
                                UserName = user,
                                Domain = domain
                            };
                        }
                    }

                    current += dataSize;
                }

                return null;
            }
            finally
            {
                if (sessionInfoPtr != IntPtr.Zero)
                {
                    WTSFreeMemory(sessionInfoPtr);
                }
            }
        }

        private static string QuerySessionString(
            int sessionId,
            WTS_INFO_CLASS infoClass)
        {
            IntPtr buffer = IntPtr.Zero;
            int bytesReturned;

            try
            {
                if (WTSQuerySessionInformation(
                        WTS_CURRENT_SERVER_HANDLE,
                        sessionId,
                        infoClass,
                        out buffer,
                        out bytesReturned)
                    && buffer != IntPtr.Zero)
                {
                    return Marshal.PtrToStringAnsi(buffer);
                }

                return null;
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    WTSFreeMemory(buffer);
                }
            }
        }

        #region WTS API

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSEnumerateSessions(
            int hServer,
            int Reserved,
            int Version,
            out IntPtr ppSessionInfo,
            out int pCount);

        [DllImport("wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQuerySessionInformation(
            int hServer,
            int sessionId,
            WTS_INFO_CLASS wtsInfoClass,
            out IntPtr ppBuffer,
            out int pBytesReturned);

        [StructLayout(LayoutKind.Sequential)]
        private struct WTS_SESSION_INFO
        {
            public int SessionID;

            [MarshalAs(UnmanagedType.LPStr)]
            public string pWinStationName;

            public WTS_CONNECTSTATE_CLASS State;
        }

        private enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive = 0,
            WTSConnected = 1,
            WTSConnectQuery = 2,
            WTSShadow = 3,
            WTSDisconnected = 4,
            WTSIdle = 5,
            WTSListen = 6,
            WTSReset = 7,
            WTSDown = 8,
            WTSInit = 9
        }

        private enum WTS_INFO_CLASS
        {
            WTSUserName = 5,
            WTSDomainName = 7
        }

        #endregion
    }
}
