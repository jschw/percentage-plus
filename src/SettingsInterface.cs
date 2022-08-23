using System.Configuration;
using System.Diagnostics;

namespace percentage_plus
{
    public struct ProgramSettings
    {
        public ProgramSettings()
        {
            contextShowBatteries = true;
            contextShowOverallCharge = true;
            contextShowRemainingTime = true;
            tooltipShowBatteries = false;
            tooltipShowOverallCharge = true;
            tooltipShowRemainingTime = true;
            showPowerstate = false;
            enableHotkey = false;
        }

        public bool contextShowBatteries { get; set; }
        public bool contextShowOverallCharge { get; set; }
        public bool contextShowRemainingTime { get; set; }
        public bool tooltipShowBatteries { get; set; }
        public bool tooltipShowOverallCharge { get; set; }
        public bool tooltipShowRemainingTime { get; set; }
        public bool showPowerstate { get; set; }
        public bool enableHotkey { get; set; }
    }

    internal class SettingsInterface
    {
        private string storePath;
        private string folderPath;
        private string configFilename;

        public ProgramSettings settings;

        System.Configuration.Configuration configFile;

        bool saveErr = false;

        public SettingsInterface()
        {
            folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\percentage-plus";
            configFilename = "app-settings.config";
            storePath = folderPath + "\\" + configFilename;

            Debug.WriteLine(storePath);

            // Check if folder exists
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            settings = new ProgramSettings();

            if (File.Exists(storePath))
            {
                // Get the configuration file
                ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = storePath };
                configFile = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                LoadSettings();
            }
            else
            {
                // Create and map a new configuration file
                configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configFile.SaveAs(storePath, ConfigurationSaveMode.Full);

                ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = storePath };
                configFile = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

                // Init config file with default values if keys are missing
                AddUpdateAppSettings("contextShowBatteries", settings.contextShowBatteries.ToString());
                AddUpdateAppSettings("contextShowOverallCharge", settings.contextShowOverallCharge.ToString());
                AddUpdateAppSettings("contextShowRemainingTime", settings.contextShowRemainingTime.ToString());
                AddUpdateAppSettings("tooltipShowBatteries", settings.tooltipShowBatteries.ToString());
                AddUpdateAppSettings("tooltipShowOverallCharge", settings.tooltipShowOverallCharge.ToString());
                AddUpdateAppSettings("tooltipShowRemainingTime", settings.tooltipShowRemainingTime.ToString());
                AddUpdateAppSettings("showPowerstate", settings.showPowerstate.ToString());
                AddUpdateAppSettings("enableHotkey", settings.enableHotkey.ToString());
            }
        }

        public bool LoadSettings()
        {
            if (GetAppConfiguration("contextShowBatteries") == null) AddUpdateAppSettings("contextShowBatteries", settings.contextShowBatteries.ToString());
            else settings.contextShowBatteries = Convert.ToBoolean(GetAppConfiguration("contextShowBatteries"));

            if (GetAppConfiguration("contextShowOverallCharge") == null) AddUpdateAppSettings("contextShowOverallCharge", settings.contextShowOverallCharge.ToString());
            else settings.contextShowOverallCharge = Convert.ToBoolean(GetAppConfiguration("contextShowOverallCharge"));

            if (GetAppConfiguration("contextShowRemainingTime") == null) AddUpdateAppSettings("contextShowRemainingTime", settings.contextShowRemainingTime.ToString());
            else settings.contextShowRemainingTime = Convert.ToBoolean(GetAppConfiguration("contextShowRemainingTime"));

            if (GetAppConfiguration("tooltipShowBatteries") == null) AddUpdateAppSettings("tooltipShowBatteries", settings.tooltipShowBatteries.ToString());
            else settings.tooltipShowBatteries = Convert.ToBoolean(GetAppConfiguration("tooltipShowBatteries"));

            if (GetAppConfiguration("tooltipShowOverallCharge") == null) AddUpdateAppSettings("tooltipShowOverallCharge", settings.tooltipShowOverallCharge.ToString());
            else settings.tooltipShowOverallCharge = Convert.ToBoolean(GetAppConfiguration("tooltipShowOverallCharge"));

            if (GetAppConfiguration("tooltipShowRemainingTime") == null) AddUpdateAppSettings("tooltipShowRemainingTime", settings.tooltipShowRemainingTime.ToString());
            else settings.tooltipShowRemainingTime = Convert.ToBoolean(GetAppConfiguration("tooltipShowRemainingTime"));

            if (GetAppConfiguration("showPowerstate") == null) AddUpdateAppSettings("showPowerstate", settings.showPowerstate.ToString());
            else settings.showPowerstate = Convert.ToBoolean(GetAppConfiguration("showPowerstate"));

            if (GetAppConfiguration("enableHotkey") == null) AddUpdateAppSettings("enableHotkey", settings.enableHotkey.ToString());
            else settings.enableHotkey = Convert.ToBoolean(GetAppConfiguration("enableHotkey"));

            return true;
        }
        public bool SaveSettings()
        {
            // Init config file with default values if keys are missing
            saveErr = false;
            AddUpdateAppSettings("contextShowBatteries", settings.contextShowBatteries.ToString());
            AddUpdateAppSettings("contextShowOverallCharge", settings.contextShowOverallCharge.ToString());
            AddUpdateAppSettings("contextShowRemainingTime", settings.contextShowRemainingTime.ToString());
            AddUpdateAppSettings("tooltipShowBatteries", settings.tooltipShowBatteries.ToString());
            AddUpdateAppSettings("tooltipShowOverallCharge", settings.tooltipShowOverallCharge.ToString());
            AddUpdateAppSettings("tooltipShowRemainingTime", settings.tooltipShowRemainingTime.ToString());
            AddUpdateAppSettings("showPowerstate", settings.showPowerstate.ToString());
            AddUpdateAppSettings("enableHotkey", settings.enableHotkey.ToString());
            if (!saveErr) return true;
            else return false;
        }

        public string GetAppConfiguration(string key)
        {
            try
            {
                return configFile.AppSettings.Settings[key].Value;
            }
            catch (NullReferenceException excpt)
            {
                return null;
            }
            catch (ConfigurationErrorsException excpt)
            {
                return null;
            }

        }

        public bool AddUpdateAppSettings(string key, string value)
        {
            try
            {
                KeyValueConfigurationCollection settings = configFile.AppSettings.Settings;

                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

                return true;
            }
            catch (ConfigurationErrorsException excpt)
            {
                saveErr = true;
                return false;
            }
        }
    }
}
