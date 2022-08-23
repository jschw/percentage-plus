using System.Runtime.InteropServices;
using System.Management;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Forms.Application;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection.Emit;

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
        KeyboardHook hook = new KeyboardHook();

        private ManagementClass wmi;
        private ManagementObjectCollection allBatteries;
        private int battCount = 0;

        SettingsInterface settingsInterface;


        public TrayIcon()
        {
            wmi = new ManagementClass("Win32_Battery");
            allBatteries = wmi.GetInstances();

            settingsInterface = new SettingsInterface();

            BuildContextMenu();

            // Turn off detailed battery info if only one battery is present
            if (battCount < 2)
            {
                settingsInterface.settings.contextShowBatteries = false;
                settingsInterface.settings.tooltipShowBatteries = false;
            }

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 2000;
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();

            // Register the Hotkey Ctrl + B
            settingsInterface.settings.enableHotkey = true;
            if (settingsInterface.settings.enableHotkey)
            {
                hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
                hook.RegisterHotKey(ModifierKeys.Control, Keys.B);
            }

            RefreshBatteryData();
            ToggleMenuItems();
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

        private void BuildContextMenu()
        {
            contextMenu = new ContextMenuStrip();
            ToolStripMenuItem itemBattInfo = new ToolStripMenuItem("Battery info");

            // Check count of installed batteries and add menu items
            foreach (var battery in allBatteries)
            {
                ToolStripMenuItem menuItemPercentage = new ToolStripMenuItem("Battery: " + (battCount + 1).ToString() + "0 %");
                menuItemPercentage.Enabled = false;
                menuItemPercentage.Tag = "batt_single";
                contextMenu.Items.Add(menuItemPercentage);

                // Add dropdown menu item to info section
                ToolStripMenuItem tmpItem = new ToolStripMenuItem("Battery " + (battCount + 1).ToString());
                tmpItem.Click += new EventHandler(MenuItemBattInfoClick);
                tmpItem.Tag = battCount.ToString();
                itemBattInfo.DropDownItems.Add(tmpItem);

                battCount++;
            }

            contextMenu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem itemAvgCharge = new ToolStripMenuItem("Charge remaining: ");
            itemAvgCharge.Enabled = false;
            itemAvgCharge.Tag = "avg";
            contextMenu.Items.Add(itemAvgCharge);

            ToolStripMenuItem itemTime = new ToolStripMenuItem("Time remaining: ");
            itemTime.Enabled = false;
            itemTime.Tag = "time";
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
        }

        private void ToggleMenuItems()
        {
            // Turn on or off the menu itema display the battery charge
            // according to settings

            foreach(ToolStripItem item in contextMenu.Items)
            {
                if (item.Tag != null)
                {
                    if (item.Tag.Equals("batt_single") && !settingsInterface.settings.contextShowBatteries)
                    {
                        item.Visible = false;
                        continue;
                    }
                    else item.Visible = true;

                    if (item.Tag.Equals("avg") && !settingsInterface.settings.contextShowOverallCharge)
                    {
                        item.Visible = false;
                        continue;
                    }
                    else item.Visible = true;

                    if (item.Tag.Equals("time") && !settingsInterface.settings.contextShowRemainingTime)
                    {
                        item.Visible = false;
                        continue;
                    }
                    else item.Visible = true;
                }
            }

            // Turn off separator if no single battery data should be displayed
            if (!settingsInterface.settings.contextShowBatteries) contextMenu.Items[battCount].Visible = false;
            else contextMenu.Items[battCount].Visible = true;
        }

        private void RefreshBatteryData()
        {
            PowerStatus pw = SystemInformation.PowerStatus;
            allBatteries = wmi.GetInstances();

            string toolTipText = "";

            int i = 1;
            foreach (var battery in allBatteries)
            {
                int estimatedChargeRemaining = Convert.ToInt32(battery["EstimatedChargeRemaining"])+1;

                contextMenu.Items[i - 1].Text = "Battery " + i.ToString() + ":" + "  " + estimatedChargeRemaining + " %";

                // Add to tooltip
                if (settingsInterface.settings.tooltipShowBatteries)
                {
                    toolTipText += "Battery " + i.ToString() + ":" + "  " + estimatedChargeRemaining + " %\r\n";
                }

                i++;
            }

            i++; // Separator

            // Separator in tooltip
            if (settingsInterface.settings.tooltipShowBatteries) toolTipText += "--------------------\r\n";

            // Get power state
            bool isCharging = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;

            // Get average percentage
            int avgPercentage = (int)Math.Round(pw.BatteryLifePercent * 100);
            contextMenu.Items[i - 1].Text = "Charge remaining:" + "  " + avgPercentage.ToString() + " %";
            i++;

            // Add to tooltip
            if (settingsInterface.settings.tooltipShowOverallCharge) toolTipText += "Charge:" + "  " + avgPercentage.ToString() + " %\r\n";

            // TODO
            // Color tray icon according to battery level
            // string Status = "";
            // if (estimatedChargeRemaining < 15) Status = "Critical";
            // else if (estimatedChargeRemaining < 35) Status = "Low";
            // else if (estimatedChargeRemaining < 60) Status = "Regular";
            // else if (estimatedChargeRemaining < 90) Status = "High";
            // else Status = "Complete";

            // Get remaining time
            int timeReadout = (int)pw.BatteryLifeRemaining;
            TimeSpan timeRemaining = TimeSpan.FromSeconds(timeReadout);
            string timeRemainingStr = string.Format("{0:D2} h : {1:D2} m", timeRemaining.Hours, timeRemaining.Minutes);

            contextMenu.Items[i - 1].Text = "Time remaining:" + "  " + (isCharging ? " -- (Charging)" : timeRemainingStr);

            // Add time to tooltip
            if (settingsInterface.settings.tooltipShowRemainingTime) toolTipText += "Time: " + "  " + (isCharging ? "(Charging)" : timeRemainingStr);

            // Set Notify icon
            string notifyIconValue = (isCharging && settingsInterface.settings.showPowerstate) ? avgPercentage.ToString() + "+" : avgPercentage.ToString();
            SetNotifyIcon(notifyIconValue, notifyIcon);

            // Set tooltip
            notifyIcon.Text = toolTipText;
        }

        private void MenuItemExitClick(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Exit();
        }

        private void MenuItemSettingsClick(object sender, EventArgs e)
        {
            Settings settingsDialog = new Settings(settingsInterface.settings, battCount);
            settingsDialog.ShowDialog();

            // Copy modified struct
            if (settingsDialog.DialogResult == DialogResult.OK)
            {
                settingsInterface.settings = settingsDialog.settings;
                bool settingsOk = settingsInterface.SaveSettings();

                if (settingsOk) MessageBox.Show("Saving the changes successful.", "Save settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show("Saving the changes was not successful", "Save settings", MessageBoxButtons.OK, MessageBoxIcon.Error);

                RefreshBatteryData();
                ToggleMenuItems();
            }
            
        }

        private void MenuItemBattInfoClick(object sender, EventArgs e)
        {
            ToolStripMenuItem m = (ToolStripMenuItem)sender;
            int battNum = Convert.ToInt16(m.Tag);
            string winTitle = "Detailed Information Battery " + (battNum+1).ToString();

            Dictionary<string, string> battInfo = new Dictionary<string, string>();

            int i = 0;
            foreach (var battery in allBatteries)
            {
                int estimatedChargeRemaining = Convert.ToInt32(battery["EstimatedChargeRemaining"]);

                if (i == battNum)
                {
                    battInfo.Add("Availability", (IsNull(battery["Availability"]) ? "" : battery["Availability"]).ToString());
                    battInfo.Add("BatteryRechargeTime", (IsNull(battery["BatteryRechargeTime"]) ? "" : battery["BatteryRechargeTime"]).ToString());
                    battInfo.Add("BatteryStatus", (IsNull(battery["BatteryStatus"]) ? "" : battery["BatteryStatus"]).ToString());
                    battInfo.Add("Caption", (IsNull(battery["Caption"]) ? "" : battery["Caption"]).ToString());
                    battInfo.Add("Chemistry", (IsNull(battery["Chemistry"]) ? "" : battery["Chemistry"]).ToString());
                    battInfo.Add("ConfigManagerErrorCode", (IsNull(battery["ConfigManagerErrorCode"]) ? "" : battery["ConfigManagerErrorCode"]).ToString());
                    battInfo.Add("InstallDate", (IsNull(battery["InstallDate"]) ? "" : battery["InstallDate"]).ToString());
                    battInfo.Add("ConfigManagerUserConfig", (IsNull(battery["ConfigManagerUserConfig"]) ? "" : battery["ConfigManagerUserConfig"]).ToString());
                    battInfo.Add("CreationClassName", (IsNull(battery["CreationClassName"]) ? "" : battery["CreationClassName"]).ToString());
                    battInfo.Add("Description", (IsNull(battery["Description"]) ? "" : battery["Description"]).ToString());
                    battInfo.Add("DesignCapacity", (IsNull(battery["DesignCapacity"]) ? "" : battery["DesignCapacity"]).ToString());
                    battInfo.Add("DesignVoltage", (IsNull(battery["DesignVoltage"]) ? "" : battery["DesignVoltage"]).ToString());
                    battInfo.Add("DeviceID", (IsNull(battery["DeviceID"]) ? "" : battery["DeviceID"]).ToString());
                    battInfo.Add("ErrorCleared", (IsNull(battery["ErrorCleared"]) ? "" : battery["ErrorCleared"]).ToString());
                    battInfo.Add("ErrorDescription", (IsNull(battery["ErrorDescription"]) ? "" : battery["ErrorDescription"]).ToString());
                    battInfo.Add("EstimatedChargeRemaining", (IsNull(battery["EstimatedChargeRemaining"]) ? "" : battery["EstimatedChargeRemaining"]).ToString());
                    battInfo.Add("EstimatedRunTime", (IsNull(battery["EstimatedRunTime"]) ? "" : battery["EstimatedRunTime"]).ToString());
                    battInfo.Add("ExpectedBatteryLife", (IsNull(battery["ExpectedBatteryLife"]) ? "" : battery["ExpectedBatteryLife"]).ToString());
                    battInfo.Add("ExpectedLife", (IsNull(battery["ExpectedLife"]) ? "" : battery["ExpectedLife"]).ToString());
                    battInfo.Add("FullChargeCapacity", (IsNull(battery["FullChargeCapacity"]) ? "" : battery["FullChargeCapacity"]).ToString());
                    battInfo.Add("LastErrorCode", (IsNull(battery["LastErrorCode"]) ? "" : battery["LastErrorCode"]).ToString());
                    battInfo.Add("MaxRechargeTime", (IsNull(battery["MaxRechargeTime"]) ? "" : battery["MaxRechargeTime"]).ToString());
                    battInfo.Add("Name", (IsNull(battery["Name"]) ? "" : battery["Name"]).ToString());
                    battInfo.Add("PNPDeviceID", (IsNull(battery["PNPDeviceID"]) ? "" : battery["PNPDeviceID"]).ToString());
                    battInfo.Add("PowerManagementSupported", (IsNull(battery["PowerManagementSupported"]) ? "" : battery["PowerManagementSupported"]).ToString());
                    battInfo.Add("SmartBatteryVersion", (IsNull(battery["SmartBatteryVersion"]) ? "" : battery["SmartBatteryVersion"]).ToString());
                    battInfo.Add("Status", (IsNull(battery["Status"]) ? "" : battery["Status"]).ToString());
                    battInfo.Add("TimeToFullCharge", (IsNull(battery["TimeToFullCharge"]) ? "" : battery["TimeToFullCharge"]).ToString());
                    battInfo.Add("TimeOnBattery", (IsNull(battery["TimeOnBattery"]) ? "" : battery["TimeOnBattery"]).ToString());
                    UInt16[] PowerManagement = (UInt16[])battery["PowerManagementCapabilities"];
                    foreach (uint version in PowerManagement)
                    {
                        battInfo.Add("PowerManagementCapabilities", version.ToString());
                    }

                    break;
                }

                i++;
            }

            BatteryDetails battInfoDialog = new BatteryDetails(battInfo, winTitle);
            battInfoDialog.Show();
        }

        private void ClickNotifyIcon(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;

            int itemCount = contextMenu.Items.Count;

            if (me.Button == MouseButtons.Left)
            {
                // Hide last four items (left click)
                contextMenu.Items[itemCount - 1].Visible = false;
                contextMenu.Items[itemCount - 2].Visible = false;
                contextMenu.Items[itemCount - 3].Visible = false;
                contextMenu.Items[itemCount - 4].Visible = false;
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                if (mi != null) mi.Invoke(notifyIcon, null);
            }else if (me.Button == MouseButtons.Right)
            {
                // Show last four items (right click)
                contextMenu.Items[itemCount - 1].Visible = true;
                contextMenu.Items[itemCount - 2].Visible = true;
                contextMenu.Items[itemCount - 3].Visible = true;
                contextMenu.Items[itemCount - 4].Visible = true;
            }
            
        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            int itemCount = contextMenu.Items.Count;

            // Hide last four items (similar left click)
            contextMenu.Items[itemCount - 1].Visible = false;
            contextMenu.Items[itemCount - 2].Visible = false;
            contextMenu.Items[itemCount - 3].Visible = false;
            contextMenu.Items[itemCount - 4].Visible = false;
            MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
            if (mi != null) mi.Invoke(notifyIcon, null);
        }

        private void SetNotifyIcon(string bitmapText, NotifyIcon notIcon)
        {
            using (Bitmap bitmap = new Bitmap(GetTextBitmap(bitmapText, new Font(font, fontSize), Color.White)))
            {
                System.IntPtr intPtr = bitmap.GetHicon();
                try
                {
                    using (Icon icon = Icon.FromHandle(intPtr))
                    {
                        notIcon.Icon = new Icon(icon, SystemInformation.SmallIconSize);
                    }
                }
                finally
                {
                    DestroyIcon(intPtr);
                }
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            RefreshBatteryData();
        }
    
        private bool IsNull(object obj)
        {
            if (obj == null) return true;
            else return false;
        }
    }
}
