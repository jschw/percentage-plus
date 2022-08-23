
namespace percentage_plus
{
    public partial class Settings : Form
    {
        public ProgramSettings settings;
        bool lastStateHotkey;
        bool firstCycle = true;
        public Settings(ProgramSettings set, int numBatt)
        {
            InitializeComponent();

            settings = set;

            // Load actual settings
            checkContextSingleBatt.Checked = settings.contextShowBatteries;
            checkContextOverall.Checked = settings.contextShowOverallCharge;
            checkContextTime.Checked = settings.contextShowRemainingTime;

            checkTooltipSingleBatt.Checked = settings.tooltipShowBatteries;
            checkTooltipOverall.Checked = settings.tooltipShowOverallCharge;
            checkTooltipTime.Checked = settings.tooltipShowRemainingTime;

            checkEnableHotkey1.Checked = settings.enableHotkey;
            checkShowPowerState.Checked = settings.showPowerstate;

            // Store actual states
            lastStateHotkey = checkEnableHotkey1.Checked;

            // Toggle show all batteries if only one battery is present
            if (numBatt < 2)
            {
                checkContextSingleBatt.Checked = false;
                checkContextSingleBatt.Enabled = false;
                settings.contextShowBatteries = false;

                checkTooltipSingleBatt.Checked = false;
                checkTooltipSingleBatt.Enabled = false;
                settings.tooltipShowBatteries = false;
            }

            firstCycle = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Copy settings from UI to struct
            settings.contextShowBatteries = checkContextSingleBatt.Checked;
            settings.contextShowOverallCharge = checkContextOverall.Checked;
            settings.contextShowRemainingTime = checkContextTime.Checked;

            settings.tooltipShowBatteries = checkTooltipSingleBatt.Checked;
            settings.tooltipShowOverallCharge = checkTooltipOverall.Checked;
            settings.tooltipShowRemainingTime = checkTooltipTime.Checked;

            settings.enableHotkey = checkEnableHotkey1.Checked;
            settings.showPowerstate = checkShowPowerState.Checked;

            if (lastStateHotkey != checkEnableHotkey1.Checked & !firstCycle) MessageBox.Show("Changing the Hotkey settings requires a restart of the application.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
        }
    }
}

