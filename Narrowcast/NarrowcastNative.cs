using System;
using System.Runtime.InteropServices;

namespace Narrowcast
{
    internal class NarrowcastNative
    {
        // for single instance of the application.
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_SHOWNARROWCAST = RegisterWindowMessage("WM_SHOWNARROWCAST");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        // static definitions used throughout.
        public const int MAX_FILE_DUMP_SIZE = 16000;
        public const int SEGMENT_MAX_SIZE = 63;
        public const int TEXT_BLOCK_SIZE = 32;
        public const int BLOCK_SIZE = 16;
        public const int SERVER_TIME_SYNC_SECONDS = 3600; // sync time (update offset time) every X seconds.
        public const int SKIP_WINDOW_FINAL_SECONDS = 5; // if in last X seconds of a freqeuncy window wait to send output until the next window to avoid it being lost.
        public const string STATUS_PREFIX = "*** ";
        public const string BASE_ALPHABET = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const char BASE_PAD = '_';
        public const string BASE64_LPAD = "0-";
        public static readonly string[] BASE64_REMAP = {
            "=", "-0",
            "/", "-1",
            "+", "-2"
        };
        public enum ControlCodes : byte
        {
            SAY  = 0x01,
            JOIN = 0x02,
            PART = 0x03, // unused, placeholder for future support
            QUIT = 0x04 // unused, placeholder for future support
        };
    }
}
