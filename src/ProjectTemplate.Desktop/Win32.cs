using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ProjectTemplate.Desktop
{
    internal static class Win32
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;

        public static void DisableMinimizeMaximizeButtons(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var value = GetWindowLong(hwnd, GWL_STYLE);
            value = value & ~WS_MAXIMIZEBOX;
            value = value & ~WS_MINIMIZEBOX;
            SetWindowLong(hwnd, GWL_STYLE, value);
        }
    }
}
