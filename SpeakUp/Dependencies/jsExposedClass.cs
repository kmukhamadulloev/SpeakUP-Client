using System.Windows.Forms;

namespace SpeakUP.Dependencies
{
    public class jsExposedClass
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
}
