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

namespace SpeakUP
{
    public partial class MainForm : Form
    {
        private ChromiumWebBrowser browser;

        // P/Invoke constants
        private const int WM_SYSCOMMAND = 0x112;
        private const int MF_STRING = 0x0;
        private const int MF_SEPARATOR = 0x800;

        // P/Invoke declarations
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InsertMenu(IntPtr hMenu, int uPosition, int uFlags, int uIDNewItem, string lpNewItem);


        // ID for the system menu
        private int SYSMENU_DEV_TOOLS = 0x1;

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
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if ((m.Msg == WM_SYSCOMMAND) && ((int)m.WParam == SYSMENU_DEV_TOOLS))
            {
                browser.ShowDevTools();
            }

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
            string appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

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

            // Initializing browser and settings for browser
            var settings = new CefSettings();
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");
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

            Controls.Add(browser);

            // Checking for updates
            new Dependencies.Update();
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
            System.Drawing.Point mp = Cursor.Position;
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
