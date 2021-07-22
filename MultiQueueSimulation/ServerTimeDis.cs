using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueModels;
using MultiQueueTesting;

namespace MultiQueueSimulation
{
    public partial class ServerTimeDis : Form
    {
        public ServerTimeDis(SimulationSystem system)
        {
            InitializeComponent();
            this.system = system;
        }
        SimulationSystem system;
        private void ServerTimeDis_Load(object sender, EventArgs e)
        {
            DataTable d2 = new DataTable();

            d2.Columns.Add("Server ID", typeof(int));
            d2.Columns.Add("Service Time", typeof(decimal));
            d2.Columns.Add("Probability", typeof(decimal));
            d2.Columns.Add("Cumulative Probability", typeof(decimal));
            d2.Columns.Add("MinRang", typeof(int));
            d2.Columns.Add("MaxRang", typeof(int));
            for (int i = 0; i < system.Servers.Count; i++){
                for (int j = 0; j < system.Servers[i].TimeDistribution.Count; j++){
                    d2.Rows.Add(system.Servers[i].ID, system.Servers[i].TimeDistribution[j].Time, system.Servers[i].TimeDistribution[j].Probability, system.Servers[i].TimeDistribution[j].CummProbability, system.Servers[i].TimeDistribution[j].MinRange, system.Servers[i].TimeDistribution[j].MaxRange);
                }
            }
            dataGridView1.DataSource = d2;
        }
    }
}
