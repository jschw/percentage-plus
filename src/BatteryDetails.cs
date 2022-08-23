using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace percentage_plus
{
    public partial class BatteryDetails : Form
    {
        public BatteryDetails(Dictionary<string, string> info, string title)
        {
            InitializeComponent();

            DgvBatteryInfo.Rows.Clear();

            // Read out battery information
            foreach (var item in info)
            {
                DgvBatteryInfo.Rows.Add(item.Key, item.Value);
            }

            this.Text = title;
        }
    }
}
