using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Get_Good
{
    public partial class Form1 : Form
    {
        private string pythonScriptPath;
        private NotifyIcon _notifyIcon;
        private Point lastLocation;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        public Form1()
        {
            InitializeNotifyIcon();
            InitializeComponent();
            ExtractResources();

            // Load saved text box values from settings
            txtVoice.Text = Properties.Settings.Default.VoiceInput;
            txtChat.Text = Properties.Settings.Default.ChatInput;

            // Configure form to have no border and no control box
            this.FormBorderStyle = FormBorderStyle.None;
            this.ControlBox = false;

            // Set background color
            this.BackColor = Color.FromArgb(51, 51, 51); // This corresponds to #333 (dark gray)

            // Initialize last location for dragging
            lastLocation = new Point();

            // Create rounded corners for the form
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            // Style buttons
            StyleButtons();

            // Add exit button
            AddExitButton();

            maybeAutoStart();
        }

        private void maybeAutoStart() {
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.VoiceInput) && !string.IsNullOrWhiteSpace(Properties.Settings.Default.ChatInput))
            {
                btnStart.Text = "Starting...";
                StartProcesses();
            }
        }

        private void InitializeNotifyIcon()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Properties.Resources.icon_circle;
            _notifyIcon.Text = "Get Good";
            _notifyIcon.Click += notifyIcon1_Click;
        }

        private void ExtractResources()
        {
            pythonScriptPath = ExtractResource("Get_Good.server.exe", "app-server.exe");
        }

        private string ExtractResource(string resourceName, string targetName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = Path.Combine(Path.GetTempPath(), targetName);

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Resource not found: " + resourceName);
                }

                using (var fileStream = new FileStream(resourcePath, FileMode.Create))
                {
                    stream.CopyTo(fileStream);
                }
            }

            return resourcePath;
        }

        private void StartProcesses()
        {
            var pythonProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pythonScriptPath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    WorkingDirectory = Path.GetDirectoryName(pythonScriptPath)
                }
            };

            pythonProcess.Start();

            Task.Run(() =>
            {
                Client.StartClient();
            });

            btnStart.Text = "Running";
        }

        private async void TerminateProcesses()
        {
            async Task<bool> TryClose(string processName)
            {
                var processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName));

                if (processes.Length == 0)
                    return true;

                var success = true;
                foreach (var process in processes)
                {
                    try
                    {
                        process.CloseMainWindow();
                        await Task.Delay(5000); // Adjust delay as needed

                        if (!process.HasExited)
                        {
                            process.Kill();
                            success = false;
                        }
                    }
                    catch
                    {
                        success = false;
                    }
                }

                return success;
            }

            var tasks = new List<Task<bool>>
            {
                TryClose("app-server")
            };

            var timeoutTask = Task.Delay(10000); // 10 seconds timeout
            var completedTask = await Task.WhenAny(Task.WhenAll(tasks), timeoutTask);

            Application.Exit();

            if (completedTask == timeoutTask)
            {
                Application.Exit();
                // MessageBox.Show("Timeout occurred while stopping processes.");
            }
        }

        #region Dragging Form

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                lastLocation = e.Location;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastLocation.X;
                this.Top += e.Y - lastLocation.Y;
            }
        }

        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                _notifyIcon.Visible = true;
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            _notifyIcon.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                _notifyIcon.Visible = true;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                _notifyIcon.Visible = false;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtVoice.Text) || string.IsNullOrWhiteSpace(txtChat.Text))
            {
                MessageBox.Show("Please enter text in both Voice Input and Chat Input fields.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Call your method to start processes here
                btnStart.Text = "Starting...";
                Properties.Settings.Default.VoiceInput = txtVoice.Text;
                Properties.Settings.Default.ChatInput = txtChat.Text;
                Properties.Settings.Default.Save();
                StartProcesses();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Text = "Stopping...";
            TerminateProcesses();
        }

        // Dispose method override to dispose managed resources
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources (e.g., components)
                _notifyIcon.Dispose(); // Dispose the NotifyIcon component if necessary
            }

            // Dispose unmanaged resources

            // Call base.Dispose(disposing) to ensure base class resources are properly disposed
            base.Dispose(disposing);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
