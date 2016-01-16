using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Runtime.InteropServices;
using SpeakUP.Controls;
using SpeakUP.Dependencies;

namespace SpeakUP
{
    public partial class MainForm : Form
    {
        private ChromiumWebBrowser browser;

        // P/Invoke constants
        private const int WM_SYSCOMMAND = 0x112;
        private const int MF_STRING = 0x0;
        private const int MF_SEPARATOR = 0x800;
        private string appVersion;
        private FormWindowState oldFormState = FormWindowState.Normal;

        // P/Invoke declarations
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InsertMenu(IntPtr hMenu, int uPosition, int uFlags, int uIDNewItem, string lpNewItem);


        // ID for the system menu
        private int SYSMENU_DEV_TOOLS = 0x1;
        private int SYSMENU_REPORT = 0x2;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            IntPtr hSysMenu = GetSystemMenu(this.Handle, false);
            AppendMenu(hSysMenu, MF_SEPARATOR, 0, string.Empty);
            AppendMenu(hSysMenu, MF_STRING, SYSMENU_DEV_TOOLS, "&Developer Tools");
            AppendMenu(hSysMenu, MF_STRING, SYSMENU_REPORT, "Report &Issue");
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if ((m.Msg == WM_SYSCOMMAND) && ((int)m.WParam == SYSMENU_DEV_TOOLS))
            {
                browser.ShowDevTools();
            }

            if ((m.Msg == WM_SYSCOMMAND) && ((int)m.WParam == SYSMENU_REPORT))
            {
                string url = Properties.Settings.Default.reportIssueUrl;
                System.Diagnostics.Process.Start(url);
            }

            if (m.Msg == NativeMethods.WM_SHOWSPEAKUP)
            {
                bringToFront();
            }
        }

        private void bringToFront()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            bool top = TopMost;
            TopMost = true;
            TopMost = top;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Upgrading settings...
            if (Properties.Settings.Default.appUpgraded)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.appUpgraded = false;
                Properties.Settings.Default.Save();
            }

            // Reading important settings
            string url = Properties.Settings.Default.baseUrl;
            string instancePath = Application.StartupPath;
            appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // assign handler for changing settings
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;

            // Setting form size, position and state
            int x = Properties.Settings.Default.formX;
            int y = Properties.Settings.Default.formY;
            int w = Properties.Settings.Default.formW;
            int h = Properties.Settings.Default.formH;
            bool isMaximized = Properties.Settings.Default.formMaximized;

            this.Left = x;
            this.Top = y;
            this.Width = w;
            this.Height = h;

            if (w > 100)
            {
                Width = w;
            }
            else
            {
                Width = 800;
            }

            if (h > 100)
            {
                Height = h;
            }
            else
            {
                Height = 480;
            }

            SetFormPos(x, y);
            if (isMaximized)
            {
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                WindowState = FormWindowState.Normal;
            }

            centerElements();

            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += timer_Tick;
            timer.Start();

            // Initializing browser and settings for browser
            var settings = new CefSettings();
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            settings.CefCommandLineArgs.Add("enable-usermedia-screen-capturing", "1");
            settings.CefCommandLineArgs.Add("disable-plugins-discovery", "1");

            settings.WindowlessRenderingEnabled = true;
            settings.CefCommandLineArgs.Add("enable-smooth-scrolling", "1");
            settings.CefCommandLineArgs.Add("enable-overlay-scrollbar", "1");

            settings.CachePath = instancePath + @"\cache\";
            settings.UserAgent = String.Format(
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{0} Safari/537.36 SpeakUPClient/{1}",
                Cef.ChromiumVersion,
                appVersion
                );

            Cef.Initialize(settings);

            // Checking command line arguments
            string[] argv = Environment.GetCommandLineArgs();
            string joinChannel = null;
            for (int i = 0; i < argv.Length; i++)
            {
                if (argv[i].IndexOf("speakup://") >= 0)
                {
                    joinChannel = argv[i].Replace("speakup://", "");
                }
            }

            if (!String.IsNullOrWhiteSpace(joinChannel))
            {
                url = url + "?room=" + joinChannel;
            }

            browser = new ChromiumWebBrowser(url)
            {
                Dock = DockStyle.Fill
            };

            browser.TitleChanged += OnBrowserTitleChanged;
            browser.LoadError += OnBrowserLoadError;

            browser.RegisterJsObject("speakup_client", new jsFunctions());

            Controls.Add(browser);

            // Checking for updates
            new Update();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            this.InvokeOnUiThreadIfRequired(() => this.MaximizeBox = true);
        }

        private void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "msgName"){
                return;
            }

            if (Properties.Settings.Default.msgName == "enableFullScreen")
            {
                Properties.Settings.Default.msgName = "";
                Properties.Settings.Default.msgValue = "";
                this.InvokeOnUiThreadIfRequired(() =>
                {
                    Focus();
                    MaximizeBox = false;
                    FormBorderStyle = FormBorderStyle.None;
                    oldFormState = WindowState;
                    WindowState = FormWindowState.Maximized;
                });
            }

            if (Properties.Settings.Default.msgName == "disableFullScreen")
            {
                Properties.Settings.Default.msgName = "";
                Properties.Settings.Default.msgValue = "";
                this.InvokeOnUiThreadIfRequired(() =>
                {
                    Focus();
                    MaximizeBox = true;
                    FormBorderStyle = FormBorderStyle.Sizable;
                    WindowState = oldFormState;
                });
            }
        }

        public class jsFunctions
        {
            public string appName
            {
                get
                {
                    return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                }
            }

            private bool isFullScreenEnabled = false;

            public bool isFullScreen
            {
                get
                {
                    return this.isFullScreenEnabled;
                }
            }

            public void appAlert(string msg, string type = "info")
            {
                MessageBoxIcon icon;
                switch (type)
                {
                    case "error": icon = MessageBoxIcon.Error; break;
                    case "warning": icon = MessageBoxIcon.Warning; break;
                    case "info":
                    default: icon = MessageBoxIcon.Information; break;
                }
                MessageBox.Show(msg, this.appName, MessageBoxButtons.OK, icon);
            }

            public void enableFullScreen()
            {
                this.isFullScreenEnabled = true;
                Properties.Settings.Default.msgValue = "";
                Properties.Settings.Default.msgName = "enableFullScreen";
            }

            public void disableFullScreen()
            {
                this.isFullScreenEnabled = false;
                Properties.Settings.Default.msgValue = "";
                Properties.Settings.Default.msgName = "disableFullScreen";
            }
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => Text = args.Title);
        }

        private void OnBrowserLoadError(object sender, LoadErrorEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                errLabelStatus.Text = "Connection error: " + args.ErrorCode.ToString();
                errPanel.Visible = true;
                browser.Visible = false;
            });
        }

        private void centerElements()
        {
            errPanel.Left = this.Width / 2 - errPanel.Width / 2;
            errPanel.Top = this.Height / 2 - errPanel.Height / 2;
        }

        private void saveSizePos(bool saveSettings = true)
        {
            bool formMaximized = (this.WindowState == FormWindowState.Maximized);
            Properties.Settings.Default.formMaximized = formMaximized;
            if (!formMaximized)
            {
                Properties.Settings.Default.formX = this.Left;
                Properties.Settings.Default.formY = this.Top;
                Properties.Settings.Default.formW = this.Width;
                Properties.Settings.Default.formH = this.Height;
            }
            if (saveSettings)
            {
                Properties.Settings.Default.Save();
            }
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            saveSizePos();
        }

        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            saveSizePos();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            saveSizePos(false);
            // save other settings...
            Properties.Settings.Default.Save();
        }

        private void errBtnReload_Click(object sender, EventArgs e)
        {
            errPanel.Visible = false;
            browser.Visible = true;
            browser.Reload();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            centerElements();
        }

        private void SetFormPos(int x, int y)
        {
            // requesting current screen
            Point mp = Cursor.Position;
            Screen currentScreen = Screen.FromPoint(mp);

            // determining X
            if (x != -1 && x < SystemInformation.VirtualScreen.Width)
            {
                this.Left = x;
            }
            else
            {
                this.Left = currentScreen.WorkingArea.Width / 2 - this.Width / 2;
            }
            // determining Y
            if (y != -1 && x < SystemInformation.VirtualScreen.Height)
            {
                this.Top = y;
            }
            else
            {
                this.Top = currentScreen.WorkingArea.Height / 2 - this.Height / 2;
            }
        }
    }
}
