using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Application = System.Windows.Application;
using Cursor = System.Windows.Forms.Cursor;

namespace BlackOut
{
    public partial class App : Application
    {

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private HwndSource _source;
        private const int HOTKEY1_ID = 9000;
        private const int HOTKEY2_ID = 9100;
        private const int HOTKEY3_ID = 9200;
        private const int HOTKEY4_ID = 9300;
        private const int HOTKEY5_ID = 9400;
        private const string RegesterKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        Screen[] screens = Screen.AllScreens;
        Dictionary<Screen, MainWindow> windows = new Dictionary<Screen, MainWindow>();
        private ContextMenuStrip _contextMenu = new ContextMenuStrip();
        private readonly NotifyIcon _notifyIcon;

        public App()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Resource.Icon;
            _notifyIcon.Visible = true;
            _notifyIcon.BalloonTipTitle = "BlackOut";
            _notifyIcon.DoubleClick += NotifyIconDoubleClick;
            CreateIconMenuStructure("Exit");
            _notifyIcon.ContextMenuStrip = _contextMenu;
        }

        private void NotifyIconDoubleClick(object? sender, EventArgs e)
        {
            OnHotKeyPressedShowAllWindow();
        }
        public void CreateIconMenuStructure(string caption)
        {
            _contextMenu.Items.Add(caption, null, OnClick);
        }

        private void OnClick(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            switch (menuItem.Text)
            {
                case "Exit":
                    Application.Current.Shutdown(0);
                    break;

            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {

            RegistryKey key = Registry.CurrentUser.OpenSubKey(RegesterKey, true);
            key.SetValue("BackOut", Process.GetCurrentProcess().MainModule.FileName);
            base.OnStartup(e);
            foreach (var screen in screens)
            {
                var window = new MainWindow();
                window.Top = screen.Bounds.Top;
                window.Left = screen.Bounds.Left;
                window.Height = screen.Bounds.Height;
                window.Width = screen.Bounds.Width;
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Show();
                windows.Add(screen, window);
                window.Hide();
            }
            var helper = new WindowInteropHelper(windows[screens[0]]);

            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            base.OnExit(e);
        }

        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(windows[screens[0]]);
            const uint VK_F10 = 0x79;
            const uint MOD_ALT = 0x1;
            const uint MOD_CTRL = 0x0002;
            const uint VK_H = 0x48; // H key
            const uint VK_S = 0x53; // S key
            const uint VK_A = 0x41; // A key
            const uint VK_Q = 0x51; // Q key
            const uint VK_F2 = 0x71; // F2 key

            if (!RegisterHotKey(helper.Handle, HOTKEY1_ID, MOD_ALT | MOD_CTRL, VK_H))
            {
                // handle error
            }
            if (!RegisterHotKey(helper.Handle, HOTKEY2_ID, MOD_ALT | MOD_CTRL, VK_S))
            {
                // handle error
            }
            if (!RegisterHotKey(helper.Handle, HOTKEY3_ID, MOD_ALT | MOD_CTRL, VK_A))
            {
                // handle error
            }
            if (!RegisterHotKey(helper.Handle, HOTKEY4_ID, MOD_ALT | MOD_CTRL, VK_Q))
            {
                // handle error
            }
            if (!RegisterHotKey(helper.Handle, HOTKEY5_ID, MOD_ALT | MOD_CTRL, VK_F2))
            {
                // handle error
            }
        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(windows[screens[0]]);
            UnregisterHotKey(helper.Handle, HOTKEY1_ID);
            UnregisterHotKey(helper.Handle, HOTKEY2_ID);
            UnregisterHotKey(helper.Handle, HOTKEY3_ID);
            UnregisterHotKey(helper.Handle, HOTKEY4_ID);
            UnregisterHotKey(helper.Handle, HOTKEY5_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY1_ID:
                            OnHotKeyPressedHideWindow();
                            handled = true;
                            break;
                        case HOTKEY2_ID:
                            OnHotKeyPressedShowWindow();
                            handled = true;
                            break;
                        case HOTKEY3_ID:
                            OnHotKeyPressedShowAllWindow();
                            handled = true;
                            break;
                        case HOTKEY4_ID:
                            OnHotKeyPressedHideAllWindow();
                            handled = true;
                            break;
                        case HOTKEY5_ID:
                            OnHotKeyPressedShowAllExceptCurrentWindow();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressedShowAllExceptCurrentWindow()
        {
            Screen s = Screen.FromPoint(Cursor.Position);
            foreach (var window in windows)
            {
                if (!window.Key.Equals(s))
                    window.Value.Show();
                else
                    window.Value.Hide();
            }
        }

        private void OnHotKeyPressedHideAllWindow()
        {
            foreach (var window in windows)
            {
                window.Value.Hide();
            }
        }

        private void OnHotKeyPressedShowAllWindow()
        {
            foreach (var window in windows)
            {
                window.Value.Show();
            }
        }

        private void OnHotKeyPressedShowWindow()
        {
            Screen s = Screen.FromPoint(Cursor.Position);
            if (windows.ContainsKey(s))
            {
                windows[s].Show();
            }
        }
        private void OnHotKeyPressedHideWindow()
        {
            Screen s = Screen.FromPoint(Cursor.Position);
            if (windows.ContainsKey(s))
            {
                windows[s].Hide();
            }

        }

    }
}
