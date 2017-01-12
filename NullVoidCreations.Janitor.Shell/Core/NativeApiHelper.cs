using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using NullVoidCreations.Janitor.Shared.Helpers;
using System.Windows.Media;
using System.Windows.Controls;

namespace NullVoidCreations.Janitor.Shell.Core
{

    class NativeApiHelper
    {

        /// <summary>Enumeration of the different ways of showing a window using ShowWindow</summary>
        [Flags]
        enum WindowShowStyle : int
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,

            /// <summary>Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,

            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,

            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,

            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,

            /// <summary>Displays a window in its most recent size and position. This value is similar to "ShowNormal", except the window is not actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,

            /// <summary>Activates the window and displays it in its current size and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,

            /// <summary>Minimizes the specified window and activates the next top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,

            /// <summary>Displays the window as a minimized window. This value is similar to "ShowMinimized", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,

            /// <summary>Displays the window in its current size and position. This value is similar to "Show", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,

            /// <summary>Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,

            /// <summary>Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,

            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread that owns the window is hung. This flag should only be used when minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }

        const int CSIDL_COMMON_STARTMENU = 0x16;  // All Users\Start Menu

        [DllImport("shell32.dll")]
        static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(string cls, string win);

        [DllImport("user32")]
        static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32")]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32")]
        static extern bool OpenIcon(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowCaret(IntPtr hWnd);

        static NativeApiHelper _instance;

        private NativeApiHelper()
        {

        }

        #region properties

        public static NativeApiHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NativeApiHelper();

                return _instance;
            }
        }

        #endregion

        public void ActivateOtherWindow(string title)
        {
            var other = FindWindow(null, title);
            if (other != IntPtr.Zero)
            {
                Show(other);
                if (IsIconic(other))
                    OpenIcon(other);
            }
        }

        public IntPtr GetHandle(Visual element)
        {
            var source = (HwndSource)HwndSource.FromVisual(element);
            return source.Handle;
        }

        public bool ToggleCaret(TextBox textBox, bool showCaret)
        {
            var handle = GetHandle(textBox);
            if (showCaret)
                return ShowCaret(handle);
            else
                return HideCaret(handle);
        }

        public IntPtr GetWindowHandle(Window window)
        {
            return new WindowInteropHelper(window).Handle;
        }

        public void Hide(IntPtr hWnd)
        {
            ShowWindow(hWnd, (int)WindowShowStyle.Hide);
        }

        public void Minimize(IntPtr hWnd)
        {
            ShowWindow(hWnd, (int)WindowShowStyle.Minimize);
        }

        public void Show(IntPtr hWnd)
        {
            ShowWindow(hWnd, (int)WindowShowStyle.Restore);
        }

        public string GetStartMenuDirectory()
        {
            var path = new StringBuilder(260);
            SHGetSpecialFolderPath(IntPtr.Zero, path, CSIDL_COMMON_STARTMENU, false);
            return path.ToString();
        }

        public void CreateShortcut(string lnkPath, string executable, string arguments, string workingDirectory, string iconPath, bool isMinimized)
        {
            // delete existing link
            FileSystemHelper.Instance.DeleteFile(lnkPath);

            // Windows Script Host Shell Object
            var type = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")); 
            var shell = Activator.CreateInstance(type);
            try
            {
                var link = type.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, new object[] { lnkPath });
                try
                {
                    type.InvokeMember("TargetPath", BindingFlags.SetProperty, null, link, new object[] { executable });
                    if (!string.IsNullOrEmpty(arguments))
                        type.InvokeMember("Arguments", BindingFlags.SetProperty, null, link, new object[] { arguments });
                    type.InvokeMember("IconLocation", BindingFlags.SetProperty, null, link, new object[] { iconPath });
                    type.InvokeMember("WindowStyle", BindingFlags.SetProperty, null, link, new object[] { isMinimized ? 7 : 1 });
                    type.InvokeMember("Save", BindingFlags.InvokeMethod, null, link, null);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    Marshal.FinalReleaseComObject(link);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Marshal.FinalReleaseComObject(shell);
            }
        }
    }
}
