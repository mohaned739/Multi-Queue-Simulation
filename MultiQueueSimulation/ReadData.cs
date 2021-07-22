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

namespace MultiQueueSimulation
{
    public partial class ReadData : Form
    {
        public ReadData()
        {
            InitializeComponent();
            this.system = new SimulationSystem();
        }
        SimulationSystem system;
        private void Form2_Load(object sender, EventArgs e)
        {   

            string[] lines = System.IO.File.ReadAllLines("../../TestCases/TestCase3.txt");

            int serverID=1;
            for (int i = 0; i < lines.Length;i++){
                if (lines[i] == "") { continue; }
                else if (lines[i]=="NumberOfServers"){
                    system.NumberOfServers = int.Parse(lines[i + 1]);
                    i++;
                }
                else if (lines[i] == "StoppingNumber"){
                    system.StoppingNumber = int.Parse(lines[i + 1]);
                    i++;
                }
                else if (lines[i] == "StoppingCriteria"){
                    if (lines[i + 1]=="1"){
                        system.StoppingCriteria = Enums.StoppingCriteria.NumberOfCustomers;
                    }
                    else{
                        system.StoppingCriteria = Enums.StoppingCriteria.SimulationEndTime;
                    }
                    i++;
                }
                else if (lines[i] == "SelectionMethod"){
                    if (lines[i + 1]=="1"){
                        system.SelectionMethod = Enums.SelectionMethod.HighestPriority;
                        }
                    else if (lines[i + 1] == "2"){
                        system.SelectionMethod = Enums.SelectionMethod.Random;
                    }
                    else{
                        system.SelectionMethod = Enums.SelectionMethod.LeastUtilization;
                    }
                    i++;
                }
                else if (lines[i] == "InterarrivalDistribution"){
                    i++;
                    decimal cumProb = 0;
                    int min = 1;
                    int max;
                    while (lines[i] != "")
                    {
                        string[] line = lines[i].Split(',');
                        TimeDistribution t = new TimeDistribution();
                        t.Time = int.Parse(line[0]);
                        t.Probability = decimal.Parse(line[1].TrimStart());
                        cumProb += t.Probability;
                        t.CummProbability = cumProb;
                        t.MinRange = min;
                        max = (int)(cumProb * 100);
                        t.MaxRange = max;
                        min = max + 1;
                        system.InterarrivalDistribution.Add(t);
                        i++;
                    }
                }
                else if (lines[i].Substring(0, 7) == "Service"){
                    Server server = new Server();
                    server.ID = serverID;
                    i++;
                    decimal cumProb = 0;
                    int min = 1;
                    int max;
                    while (i<lines.Length&&lines[i] != ""){
                        string[] line = lines[i].Split(',');
                        TimeDistribution t = new TimeDistribution();
                        t.Time = int.Parse(line[0]);
                        t.Probability = decimal.Parse(line[1].TrimStart());
                        cumProb += t.Probability;
                        t.CummProbability = cumProb;
                        t.MinRange = min;
                        max = (int)(cumProb * 100);
                        t.MaxRange = max;
                        min = max + 1;
                        server.TimeDistribution.Add(t);
                        i++;
                    }
                    serverID++;
                    system.Servers.Add(server);
                }

            }
            DataTable dt = new DataTable();
            dt.Columns.Add("Number of Servers", typeof(int));
            dt.Columns.Add("Stopping Number", typeof(int));
            dt.Columns.Add("Stopping Criteria", typeof(int));
            dt.Columns.Add("Selection Method", typeof(int));
            dt.Rows.Add(system.NumberOfServers,system.StoppingNumber,(int)system.StoppingCriteria,(int)system.SelectionMethod);
            dataGridView1.DataSource = dt;

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Interarrival Time", typeof(int));
            dt2.Columns.Add("Probability", typeof(decimal));
            for (int i = 0; i < system.InterarrivalDistribution.Count; i++){
                dt2.Rows.Add(system.InterarrivalDistribution[i].Time,system.InterarrivalDistribution[i].Probability);                
            }
            dataGridView2.DataSource = dt2;
            DataTable dt3 = new DataTable();
            dt3.Columns.Add("Server ID", typeof(int));
            dt3.Columns.Add("Service Time", typeof(int));
            dt3.Columns.Add("Probability", typeof(decimal));
            for (int i = 0; i < system.Servers.Count; i++){
                for (int j = 0; j < system.Servers[i].TimeDistribution.Count; j++){
                    dt3.Rows.Add(system.Servers[i].ID, system.Servers[i].TimeDistribution[j].Time,system.Servers[i].TimeDistribution[j].Probability);
                }
                }
            dataGridView3.DataSource = dt3;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Results res = new Results(system);
            this.Hide();
            res.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InterarrivalDis inter = new InterarrivalDis(system);
            inter.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ServerTimeDis std = new ServerTimeDis(system);
            std.Show();
        }

    }
}
