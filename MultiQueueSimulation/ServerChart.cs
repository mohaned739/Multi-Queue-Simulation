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
    public partial class ServerChart : Form{
        public ServerChart(SimulationSystem system,int servNum){
            InitializeComponent();
            this.system = system;
            this.servNum = servNum;
        }
        SimulationSystem system;
        int servNum;
        private void ServerChart_Load(object sender, EventArgs e){
            if (servNum==1){
                button1.Hide();
            }
            if (servNum == system.Servers.Count){
                button2.Hide();
            }
            string seriesName="Server "+servNum+" Usage";
            chart1.Titles.Add("Server Usage");
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = system.Servers[servNum - 1].FinishTime;
            chart1.ChartAreas[0].AxisX.Interval = 5;
            chart1.ChartAreas[0].AxisX.Name= "Time";
            chart1.ChartAreas[0].AxisX.IsMarginVisible = false;
            var chartSeries = chart1.Series.First();
            chartSeries.Name=seriesName;
            int lastused = 0;
            bool first = true;
            foreach (SimulationCase customer in system.SimulationTable){
                if (customer.AssignedServer.ID == servNum){
                    if (first){
                        chart1.Series[seriesName].Points.AddXY(customer.StartTime,1);
                        chart1.Series[seriesName].Points.AddXY(customer.EndTime, 1);
                        lastused = customer.EndTime;
                        first = false;
                    }
                    else if(lastused==customer.StartTime){
                        chart1.Series[seriesName].Points.AddXY(customer.StartTime, 1);
                        chart1.Series[seriesName].Points.AddXY(customer.EndTime, 1);
                        lastused = customer.EndTime;
                    }
                    else if (lastused != customer.StartTime){
                        chart1.Series[seriesName].Points.AddXY(lastused, 1);
                        for (int i = lastused; i <= customer.StartTime; i++){
                        chart1.Series[seriesName].Points.AddXY(i, 0);
                        }
                        chart1.Series[seriesName].Points.AddXY(customer.StartTime, 1);
                        chart1.Series[seriesName].Points.AddXY(customer.EndTime, 1);
                        lastused = customer.EndTime;
                    }
                }
            }
        }

        private void ServerChart_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e){
            servNum--;
            ServerChart chart = new ServerChart(system,servNum);
            this.Hide();
            chart.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            servNum++;
            ServerChart chart = new ServerChart(system, servNum);
            this.Hide();
            chart.Show();
        }
    }
}
