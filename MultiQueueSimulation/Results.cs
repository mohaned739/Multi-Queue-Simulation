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
    public partial class Results : Form
    {
        public Results(SimulationSystem system)
        {
            InitializeComponent();
            this.system = system;
        }
        SimulationSystem system;
        private void Results_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Customer Number", typeof(int));
            dt.Columns.Add("Random Digits for Arrival", typeof(int));
            dt.Columns.Add("Time Between Arrivals", typeof(int));
            dt.Columns.Add("Clock Time of Arrival", typeof(int));
            dt.Columns.Add("Random Digits for Service", typeof(int));
            dt.Columns.Add("Server Time Service Begins", typeof(int));
            dt.Columns.Add("Server Service Time", typeof(int));
            dt.Columns.Add("Server Time Service Ends", typeof(int));
            dt.Columns.Add("Assigned Server", typeof(int));
            dt.Columns.Add("Time in Queue", typeof(int));
            var rand = new Random();
            int cust = 1;
            int timeInqueue = 0;
            int numWaited = 0;
            int simTime = 0;
            while(true){
                if ((int)system.StoppingCriteria==1&&cust>system.StoppingNumber){
                    break;
                }
                else if ((int)system.StoppingCriteria == 2 && simTime >= system.StoppingNumber){
                    break;
                }

                SimulationCase row = new SimulationCase();
                row.CustomerNumber = cust;
                row.RandomInterArrival = rand.Next(1, 100);
                row.InterArrival = Map_Interarrival(row.RandomInterArrival);
                if (cust==1){
                    row.ArrivalTime = 0;
                }
                else{
                    row.ArrivalTime = system.SimulationTable[cust - 2].ArrivalTime + row.InterArrival;
                }
                row.RandomService = rand.Next(1, 100);

                if ((int)system.SelectionMethod == 1){
                    numWaited= HighestPriority(row,numWaited);
                }
                else if ((int)system.SelectionMethod == 2){
                    numWaited = RandomChoice(row, numWaited,rand);
                }
                else if((int)system.SelectionMethod ==3){
                    numWaited = LeastUtilization(row, numWaited, simTime);
                }
              
                row.TimeInQueue = row.StartTime - row.ArrivalTime;
                system.SimulationTable.Add(row);
                timeInqueue += row.TimeInQueue;
                system.Servers[row.AssignedServer.ID-1].TotalWorkingTime += row.ServiceTime;
                simTime = Math.Max(simTime, system.Servers[row.AssignedServer.ID - 1].FinishTime);
                Fill_Grid(row, dt);
                cust++;
            }
            system.PerformanceMeasures.AverageWaitingTime = (decimal)timeInqueue / (cust - 1);
            system.PerformanceMeasures.WaitingProbability = (decimal)numWaited / (cust - 1);
            dataGridView1.DataSource = dt;

            system.PerformanceMeasures.MaxQueueLength = Get_max_Queue();
            
            Calc_Idle(simTime);
            Calc_Utilization(simTime);
            Calc_average_Service();

            string result = TestingManager.Test(system, Constants.FileNames.TestCase3);
            MessageBox.Show(result);
        }

        private void Fill_Grid(SimulationCase row, DataTable dt){
            dt.Rows.Add(row.CustomerNumber,row.RandomInterArrival,row.InterArrival,row.ArrivalTime,row.RandomService,row.StartTime,row.ServiceTime,row.EndTime,row.AssignedServer.ID,row.TimeInQueue);
        }
        private int HighestPriority(SimulationCase row,int numWaited){
            bool found = false;
            Server temp = new Server();
            temp.FinishTime = int.MaxValue;
            foreach (Server server in system.Servers){
                if (row.ArrivalTime >= server.FinishTime){
                    row.StartTime = row.ArrivalTime;
                    row.AssignedServer = server;
                    row.ServiceTime = Map_Server_Time(row.RandomService, row.AssignedServer.ID);
                    row.EndTime = row.StartTime + row.ServiceTime;
                    server.FinishTime = row.EndTime;
                    found = true;
                    break;
                }
                if (server.FinishTime < temp.FinishTime){
                    temp = server;
                }
            }
            if (!found){
                row.StartTime = temp.FinishTime;
                row.AssignedServer = temp;
                row.ServiceTime = Map_Server_Time(row.RandomService, row.AssignedServer.ID);
                row.EndTime = row.StartTime + row.ServiceTime;
                system.Servers[temp.ID - 1].FinishTime = row.EndTime;
                numWaited++;
            }
            return numWaited;
        }
        private int RandomChoice(SimulationCase row, int numWaited,Random rand){
            List<int> freeServers = new List<int>();
            Server temp = new Server();
            temp.FinishTime = int.MaxValue;
            foreach (Server server in system.Servers){
                if (row.ArrivalTime >= server.FinishTime){
                    freeServers.Add(server.ID);
                }
                if (server.FinishTime < temp.FinishTime){
                    temp = server;
                }
            }
            if (freeServers.Count!=0){
                int index = rand.Next(0, freeServers.Count - 1);
                int serverID = freeServers[index];
                row.StartTime = row.ArrivalTime;
                row.AssignedServer = system.Servers[serverID-1];
                row.ServiceTime = Map_Server_Time(row.RandomService, row.AssignedServer.ID);
                row.EndTime = row.StartTime + row.ServiceTime;
                system.Servers[serverID-1].FinishTime = row.EndTime;
            }
            else{
                row.StartTime = temp.FinishTime;
                row.AssignedServer = temp;
                row.ServiceTime = Map_Server_Time(row.RandomService, row.AssignedServer.ID);
                row.EndTime = row.StartTime + row.ServiceTime;
                system.Servers[temp.ID - 1].FinishTime = row.EndTime;
                numWaited++;
            }
            return numWaited;
        }
        private int LeastUtilization(SimulationCase row, int numWaited, int simTime)
        {
            bool found = false;
            Server temp = new Server();
            temp.Utilization = decimal.MaxValue;
            if (simTime!=0){
            Calc_Utilization(simTime);
            }
            foreach (Server server in system.Servers){
                if (row.ArrivalTime >= server.FinishTime){
                    row.StartTime = row.ArrivalTime;
                    row.AssignedServer = server;
                    row.ServiceTime = Map_Server_Time(row.RandomService, row.AssignedServer.ID);
                    row.EndTime = row.StartTime + row.ServiceTime;
                    server.FinishTime = row.EndTime;
                    found = true;
                    break;
                }
                if (server.Utilization<temp.Utilization){
                    temp = server;
                }
            }
            if (!found){
                row.StartTime = temp.FinishTime;
                row.AssignedServer = temp;
                row.ServiceTime = Map_Server_Time(row.RandomService, row.AssignedServer.ID);
                row.EndTime = row.StartTime + row.ServiceTime;
                system.Servers[temp.ID - 1].FinishTime = row.EndTime;
                numWaited++;
            }
            return numWaited;
        }
        private int Map_Interarrival(int randomNum){
            foreach (TimeDistribution t in system.InterarrivalDistribution){
                if (randomNum <= t.MaxRange){
                    return t.Time;
                }
            }
            return 0;
        }
        private int Map_Server_Time(int randomNum,int server){
            foreach (TimeDistribution t in system.Servers[server-1].TimeDistribution){
                if (randomNum <= t.MaxRange){
                    return t.Time;
                }
            }
            return 0;
        }
        private int Get_max_Queue(){
            int size = system.SimulationTable.Count;
            int []startTime=new int[system.Servers.Count];
            int[]endTime = new int[system.Servers.Count];
            int max = 0;
            for (int i = 0; i < size; i++){
                    int serverNum=system.SimulationTable[i].AssignedServer.ID;
                if (system.SimulationTable[i].TimeInQueue==0){
                    startTime[serverNum - 1] = system.SimulationTable[i].StartTime;
                    endTime[serverNum - 1] = system.SimulationTable[i].EndTime;   
                }
                else{
                    int count = 0;
                    for (int j = i; j < size; j++){
                        bool wait = true;
                        for (int k = 0; k < system.Servers.Count; k++){
                            if (endTime[k]<=system.SimulationTable[j].ArrivalTime){
                                wait = false;
                                break;
                            }
                        }
                        if (wait) {
                            count++;
                            max = Math.Max(count, max);
                        }
                        else{
                            startTime[serverNum - 1] = system.SimulationTable[i].StartTime;
                            endTime[serverNum - 1] = system.SimulationTable[i].EndTime;   
                            break;
                        }
                    }
                }
            }
            return max;
        }
        private void Calc_Utilization(int simTime){
            foreach (Server server in system.Servers){
                //server.Utilization = 1-server.IdleProbability;
                server.Utilization = (decimal)server.TotalWorkingTime / simTime;
            }
        }

        private void Calc_Idle(int simTime){
            foreach (Server server in system.Servers){
                server.IdleProbability = (decimal)(simTime-server.TotalWorkingTime)/simTime;
            }
        }

        private void Calc_average_Service(){
            int []custInServer=new int[system.Servers.Count];
            foreach (SimulationCase cust in system.SimulationTable){
                custInServer[cust.AssignedServer.ID - 1]++;
            }
            foreach (Server server in system.Servers){
                if (custInServer[server.ID-1]==0){
                    server.AverageServiceTime=0;
                    continue;
                }
                server.AverageServiceTime =(decimal) server.TotalWorkingTime/ custInServer[server.ID-1];
            }
        }
        private void Results_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServerChart chart = new ServerChart(system,1);
            //this.Hide();
            chart.Show();
        }
        
      
    }
}
