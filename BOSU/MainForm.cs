using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;

namespace BOSU
{
    public partial class MainForm : Form
    {
        private System.Windows.Forms.ContextMenu contextMenu;

        private System.Windows.Forms.MenuItem menuItem;

        private const int CP_NOCLOSE_BUTTON = 0x200;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams pars = base.CreateParams;
                pars.ClassStyle |= CP_NOCLOSE_BUTTON;
                return pars;
            }
        }

        private void ShowMessageBox(string message, MessageBoxIcon icon)
        {
            const string caption = "Block OS Update";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.OK,
                                         icon);
        }

        public void SendMessageToDialogBox(string message)
        {
            String timeStamp = SystemHardwareInfo.GetCurrentTime();
            string content = timeStamp + ": " + message;
            richTextBox.AppendText(content);
            richTextBox.AppendText(Environment.NewLine);
        }

        public MainForm()
        {
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.menuItem = new System.Windows.Forms.MenuItem();

            this.contextMenu.MenuItems.AddRange(
                    new System.Windows.Forms.MenuItem[] { this.menuItem });

            this.menuItem.Index = 0;
            this.menuItem.Text = "E&xit";
            this.menuItem.Click += new System.EventHandler(this.MenuItem_Click);
            if (!PermissionsValidator.IsAdministrator)
            {
                const string message = "BOSU needs to be run as administrator.";
                ShowMessageBox(message, MessageBoxIcon.Error);
                this.Close();
            }
            InitializeComponent();
            new ServiceInterrupter(this).Start();
        }

        private void MenuItem_Click(object Sender, EventArgs e)
        {
            this.Close();
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(2000);
            }
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Hide();
            notifyIcon1.ContextMenu = this.contextMenu;
            notifyIcon1.BalloonTipText = "Double click on icon to open";
            notifyIcon1.BalloonTipTitle = "Іщураз на дротє";
        }

        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.Show();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RichTextBox_TextChanged(object sender, EventArgs e) {  }

        private void CreateStartupTask(string serviceName)
        {
            using (var taskService = new TaskService())
            {
                var task = taskService.NewTask();
                task.Actions.Add(new ExecAction("cmd.exe",
                    string.Format("/c start " + Application.ExecutablePath.ToString() + " && exit", serviceName)));
                task.Triggers.Add(new LogonTrigger());
                task.Principal.RunLevel = TaskRunLevel.Highest;
                taskService.RootFolder.RegisterTaskDefinition(serviceName + " Startup", task);
            }
        }

        private void RemoveStartupTask(string serviceName)
        {
            using (var taskService = new TaskService())
            {
                var task = taskService.NewTask();
                taskService.RootFolder.DeleteTask(serviceName + " Startup", false);
            }
        }

        private void UncheckOtherToolStripMenuItems(ToolStripMenuItem selectedMenuItem)
        {
            selectedMenuItem.Checked = true;

            // Select the other MenuItens from the ParentMenu(OwnerItens) and unchecked this,
            // The current Linq Expression verify if the item is a real ToolStripMenuItem
            // and if the item is a another ToolStripMenuItem to uncheck this.
            foreach (var ltoolStripMenuItem in (from object
                                                    item in selectedMenuItem.Owner.Items.OfType<ToolStripMenuItem>()
                                                let ltoolStripMenuItem = item as ToolStripMenuItem
                                                where ltoolStripMenuItem != null
                                                where !item.Equals(selectedMenuItem)
                                                select ltoolStripMenuItem))
                (ltoolStripMenuItem).Checked = false;

            //selectedMenuItem.Owner.Show();
        }

        private bool IsStartupItem()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (registryKey.GetValue(SystemHardwareInfo.AssemblyProduct) == null)
                return false;
            else
                return true;
        }

        private void EnableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);

            //RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (!IsStartupItem())
            {
                RemoveStartupTask(SystemHardwareInfo.AssemblyProduct);
                CreateStartupTask(SystemHardwareInfo.AssemblyProduct);
                //registryKey.SetValue(SystemHardwareInfo.AssemblyProduct, Application.ExecutablePath.ToString());
                string message = "Be aware current location will be saved to registry" + Environment.NewLine +
                                "and if you change " + SystemHardwareInfo.AssemblyProduct + " path it wont startup";
                ShowMessageBox(message, MessageBoxIcon.Warning);
            }
        }

        private void DisableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);

            RemoveStartupTask(SystemHardwareInfo.AssemblyProduct);

            //RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (IsStartupItem())
            {
                //registryKey.DeleteValue(SystemHardwareInfo.AssemblyProduct, false);
                string message = SystemHardwareInfo.AssemblyProduct + " entry removed from registry!";
                ShowMessageBox(message, MessageBoxIcon.Information);
            }
        }
    }
}
