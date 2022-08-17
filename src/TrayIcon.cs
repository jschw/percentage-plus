using System.Runtime.InteropServices;
using System.Management;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Forms.Application;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace percentage_plus
{
    class TrayIcon
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool DestroyIcon(IntPtr handle);

        private const int fontSize = 14;
        private const string font = "Segoe UI Variable";

        ContextMenuStrip contextMenu;

        private NotifyIcon notifyIcon;

        private ManagementClass wmi;
        private ManagementObjectCollection allBatteries;

        public TrayIcon()
        {
            contextMenu = new ContextMenuStrip();
            ToolStripMenuItem itemBattInfo = new ToolStripMenuItem("Battery info");

            wmi = new ManagementClass("Win32_Battery");

            allBatteries = wmi.GetInstances();

            // Check count of installed batteries and add menu items
            int i = 1;
            foreach (var battery in allBatteries)
            {
                ToolStripMenuItem menuItemPercentage = new ToolStripMenuItem("Battery: " + i.ToString() + "0 %");
                menuItemPercentage.Enabled = false;
                contextMenu.Items.Add(menuItemPercentage);

                // Add dropdown menu item to info section
                ToolStripMenuItem tmpItem = new ToolStripMenuItem("Battery " + i.ToString());
                tmpItem.Click += new EventHandler(MenuItemBattInfoClick);
                tmpItem.Tag = (i - 1).ToString();
                itemBattInfo.DropDownItems.Add(tmpItem);

                i++;
            }

            ToolStripMenuItem itemTime = new ToolStripMenuItem("Remaining time: ");
            itemTime.Enabled = false;
            contextMenu.Items.Add(itemTime);

            contextMenu.Items.Add(new ToolStripSeparator());

            contextMenu.Items.AddRange(new ToolStripMenuItem[] { itemBattInfo });

            ToolStripMenuItem itemSettings = new ToolStripMenuItem("Settings");
            itemSettings.Click += new EventHandler(MenuItemSettingsClick);
            contextMenu.Items.Add(itemSettings);

            ToolStripMenuItem itemExit = new ToolStripMenuItem("Exit");
            itemExit.Click += new EventHandler(MenuItemExitClick);
            contextMenu.Items.Add(itemExit);


            notifyIcon = new NotifyIcon();

            notifyIcon.ContextMenuStrip = contextMenu;
            notifyIcon.Click += ClickNotifyIcon;
            notifyIcon.Visible = true;

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();
        }

        private Bitmap GetTextBitmap(String text, Font font, Color fontColor)
        {
            SizeF imageSize = GetStringImageSize(text, font);
            Bitmap bitmap = new Bitmap((int)imageSize.Width, (int)imageSize.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.FromArgb(0, 0, 0, 0));
                using (Brush brush = new SolidBrush(fontColor))
                {
                    graphics.DrawString(text, font, brush, 0, 0);
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    graphics.Save();
                }
            }
            return bitmap;
        }

        private static SizeF GetStringImageSize(string text, Font font)
        {
            using (System.Drawing.Image image = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(image))
                return graphics.MeasureString(text, font);
        }

        private void MenuItemExitClick(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Exit();
        }

        private void MenuItemSettingsClick(object sender, EventArgs e)
        {
            // TODO
            MessageBox.Show(contextMenu.Items.Count.ToString());
        }

        private void MenuItemBattInfoClick(object sender, EventArgs e)
        {
            // TODO
            ToolStripMenuItem m = (ToolStripMenuItem)sender;
            MessageBox.Show(m.Tag.ToString());
        }

        private void ClickNotifyIcon(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;

            if (me.Button == MouseButtons.Left)
            {
                // Hide last four items (left click)
                contextMenu.Items[6].Visible = false;
                contextMenu.Items[5].Visible = false;
                contextMenu.Items[4].Visible = false;
                contextMenu.Items[3].Visible = false;
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                if (mi != null) mi.Invoke(notifyIcon, null);
            }else if (me.Button == MouseButtons.Right)
            {
                // Show last four items (right click)
                contextMenu.Items[6].Visible = true;
                contextMenu.Items[5].Visible = true;
                contextMenu.Items[4].Visible = true;
                contextMenu.Items[3].Visible = true;
            }
            
        }
 

        private void TimerTick(object sender, EventArgs e)
        {
            PowerStatus powerStatus = SystemInformation.PowerStatus;
            String percentage = (powerStatus.BatteryLifePercent * 100).ToString();
            bool isCharging = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;

            // String bitmapText = isCharging ? percentage + "*" : percentage;
            String bitmapText = isCharging ? percentage : percentage;

            using (Bitmap bitmap = new Bitmap(GetTextBitmap(bitmapText, new Font(font, fontSize), Color.White)))
            {
                System.IntPtr intPtr = bitmap.GetHicon();
                try
                {
                    using (Icon icon = Icon.FromHandle(intPtr))
                    {
                        // notifyIcon.Icon = icon;
                        notifyIcon.Icon = new Icon(icon, SystemInformation.SmallIconSize);
                        String toolTipText = percentage + "%" + (isCharging ? " Charging" : "");
                        notifyIcon.Text = toolTipText;
                    }
                }
                finally
                {
                    DestroyIcon(intPtr);
                }
            }
        }
    }
}
