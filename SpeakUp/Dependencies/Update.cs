using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace SpeakUP.Dependencies
{
    class Update
    {
        private string versionInfoUrl = "https://speakup.cf/client/version.txt";
        public Update()
        {
            Thread thread = new Thread(new ThreadStart(CheckUpdate));
            thread.Start();
        }

        public void DownloadUpdate(string url, string hash)
        {
            string tmpDir = Path.GetTempPath();
            string tmpName = tmpDir + "SpeakUP_Update.exe";
            WebClient Client = new WebClient();
            Client.DownloadFile(url, tmpName);

            HashCheck hashCheck = new HashCheck(tmpName, hash);
            bool verifiedHash = hashCheck.Result();

            if (verifiedHash)
            {
                System.Diagnostics.Process.Start(tmpName, "/SILENT /CLOSEAPPLICATIONS /NOICONS /SP-");
                Application.Exit();
            }
            else
            {
                string msg = String.Format(
                    "Update failed! Can't verify updated setup file.\nComputed Hash: {0}\nRequired Hash: {1}",
                    hashCheck.HashString(),
                    hash);
                MessageBox.Show(
                    msg,
                    "SpeakUP :: Update",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            return;
        }

        public void CheckUpdate()
        {
            try
            {
                WebClient client = new WebClient();
                string content = string.Empty;
                Stream stream;

                try
                {
                    stream = client.OpenRead(versionInfoUrl);
                    StreamReader reader = new StreamReader(stream);
                    content = reader.ReadToEnd();
                }
                catch (WebException)
                {
                    // MessageBox.Show(ex.Message);
                    return;
                }

                string[] strContent = content.Split(';');
                if (strContent.Length != 3)
                {
                    return;
                }

                Version updVersion = new Version(strContent[0]);
                string updLink = strContent[1];
                string hashStr = strContent[2];

                Version curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                if (updVersion > curVersion)
                {
                    string msg = String.Format(
                        "New version available! Click 'OK' to update.\n{0} => {1}",
                        curVersion,
                        updVersion);
                    DialogResult res = MessageBox.Show(
                        msg,
                        "SpeakUP :: Update",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    DownloadUpdate(updLink, hashStr);
                }
                return;
            }
            catch
            {
                return;
            }
        }
    }
}
