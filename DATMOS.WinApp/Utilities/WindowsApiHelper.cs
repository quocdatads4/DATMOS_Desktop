using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DATMOS.WinApp.Utilities
{
    public static class WindowsApiHelper
    {
        // Windows API functions for window manipulation
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessageW(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool IsWindowVisible(IntPtr hWnd);
        
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        
        // Windows Messages
        public const uint WM_CLOSE = 0x0010;
        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        public const uint WM_SYSCOMMAND = 0x0112;
        public const uint SC_CLOSE = 0xF060;
        
        // Virtual Key Codes
        public const int VK_CONTROL = 0x11;
        public const int VK_S = 0x53;
        public const int VK_RETURN = 0x0D;
        
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMAXIMIZED = 3;
        
        // Helper methods
        public static string GetWindowTitle(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return string.Empty;
            
            int length = GetWindowTextLength(hWnd);
            if (length == 0) return string.Empty;
            
            var builder = new StringBuilder(length + 1);
            GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }
        
        public static IntPtr GetWordWindowHandle()
        {
            return FindWindow("OpusApp", null); // Word window class name
        }
        
        public static bool SaveWordDocument(IntPtr wordWindow)
        {
            try
            {
                if (wordWindow == IntPtr.Zero) return false;
                
                // Bring Word window to foreground
                SetForegroundWindow(wordWindow);
                System.Threading.Thread.Sleep(200);
                
                // Send Ctrl+S to save
                System.Windows.Forms.SendKeys.SendWait("^(s)");
                System.Threading.Thread.Sleep(500);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving Word document: {ex.Message}");
                return false;
            }
        }
        
        public static bool CloseWordWindow(IntPtr wordWindow)
        {
            try
            {
                if (wordWindow == IntPtr.Zero) return false;
                
                // Send close message
                bool result = PostMessage(wordWindow, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                System.Threading.Thread.Sleep(500);
                
                // If still open, try graceful close
                if (IsWindowVisible(wordWindow))
                {
                    // Try to close via system menu
                    result = PostMessage(wordWindow, WM_SYSCOMMAND, (IntPtr)SC_CLOSE, IntPtr.Zero);
                    System.Threading.Thread.Sleep(500);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing Word window: {ex.Message}");
                return false;
            }
        }
    }
}
