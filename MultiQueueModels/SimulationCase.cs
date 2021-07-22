using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class SimulationCase
    {
        public SimulationCase()
        {
            this.AssignedServer = new Server();
        }

        public int CustomerNumber { get; set; }
        public int RandomInterArrival { get; set; }//rkm mn 1 l 100 h3mlo map m3 el range bta3 el interarrival  
        public int InterArrival { get; set; }// mapping ll random arrival
        public int ArrivalTime { get; set; }//sum between arrival time bta3 el 2ble m3 el interarrival bta3e
        public int RandomService { get; set; }
        public int ServiceTime { get; set; }
        public Server AssignedServer { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int TimeInQueue { get; set; }
    }
}
