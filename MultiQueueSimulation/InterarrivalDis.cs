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
    public partial class InterarrivalDis : Form
    {
        public InterarrivalDis(SimulationSystem system)
        {
            InitializeComponent();
            this.system = system;
        }
        SimulationSystem system;
        private void Form1_Load(object sender, EventArgs e)
        {
            DataTable d1 = new DataTable();
            d1.Columns.Add("Interarrival Time", typeof(int));
            d1.Columns.Add("Probability", typeof(decimal));
            d1.Columns.Add("Cumulative Probability", typeof(decimal));
            d1.Columns.Add("MinRang", typeof(int));
            d1.Columns.Add("MaxRang", typeof(int));
            for (int i = 0; i < system.InterarrivalDistribution.Count; i++)
            {
                d1.Rows.Add(system.InterarrivalDistribution[i].Time, system.InterarrivalDistribution[i].Probability, system.InterarrivalDistribution[i].CummProbability, system.InterarrivalDistribution[i].MinRange, system.InterarrivalDistribution[i].MaxRange);

            }
            dataGridView1.DataSource = d1;
        }

    }
}
