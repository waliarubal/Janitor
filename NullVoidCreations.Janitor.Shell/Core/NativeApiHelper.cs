using System;
using System.Runtime.InteropServices;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class NativeApiHelper
    {
        [DllImport("user32", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(string cls, string win);

        [DllImport("user32")]
        static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32")]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32")]
        static extern bool OpenIcon(IntPtr hWnd);

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
                SetForegroundWindow(other);
                if (IsIconic(other))
                    OpenIcon(other);
            }
        }
    }
}
