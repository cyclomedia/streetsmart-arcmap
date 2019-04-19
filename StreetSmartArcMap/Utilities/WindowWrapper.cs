using System;
using System.Windows.Forms;

namespace IntegrationArcMap.Utilities
{
    class WindowWrapper : IWin32Window
    {
        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        public IntPtr Handle { get; private set; }

        #endregion

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        public WindowWrapper(IntPtr handle)
        {
            Handle = handle;
        }

        public WindowWrapper(int handle)
        {
            Handle = new IntPtr(handle);
        }

        #endregion
    }
}