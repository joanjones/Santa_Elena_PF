using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace RealPF
{
    public partial class Form1 : Form
    {
        private Thread cpuThread;
        List<double> w1xList_pf1 = new List<double>();
        List<double> w1yList_pf1 = new List<double>();
        List<double> w2xList_pf1 = new List<double>();
        List<double> w2yList_pf1 = new List<double>();
        List<double> w3xList_pf1 = new List<double>();
        List<double> w3yList_pf1 = new List<double>();

        List<double> w1xList_pf2 = new List<double>();
        List<double> w1yList_pf2 = new List<double>();
        List<double> w2xList_pf2 = new List<double>();
        List<double> w2yList_pf2 = new List<double>();
        List<double> w3xList_pf2 = new List<double>();
        List<double> w3yList_pf2 = new List<double>();
        List<double> errorList1 = new List<double>();
        List<double> errorList2 = new List<double>();

        public int NUMBER_OF_SHARKS;
        public int NUMBER_OF_ROBOTS;
        public int NUMBER_OF_PARTICLEFILTERS;
        public List<List<ParticleFilter>> particleFilterList = new List<List<ParticleFilter>>();
        public List<List<Simulation>> simulationList = new List<List<Simulation>>();
        
        public Form1()
        {
            InitializeComponent();
        }
        public void create_real_range_list()
        {
            foreach (Shark s1 in MyGlobals.shark_list)
            {
                List<double> newList = new List<double>();
                foreach (Robot r1 in MyGlobals.robot_list)
                {
                    newList.Add(20000);
                }
                MyGlobals.real_range_list.Add(newList);
            }
        }

        public void create_and_initialize_sharks()
        {
            int SharkIndex = 0;
            for (int i = 0; i < NUMBER_OF_SHARKS; ++i)
            {
                Shark s1 = new Shark();
                s1.SHARKNUMBER = SharkIndex;
                MyGlobals.shark_list.Add(s1);
                SharkIndex += 1;
            }
        }
        public void create_and_initialize_robots()
        {
            int RobotIndex = 0;
            for (int i = 0; i < NUMBER_OF_ROBOTS; ++i)
            {
                Robot r1 = new Robot();
                r1.ROBOTNUMBER = RobotIndex;
                MyGlobals.robot_list.Add(r1);
                RobotIndex += 1;
            }
        }

        public void create_and_initialize_particle_filter()
        {
            for (int s = 0; s < NUMBER_OF_SHARKS; ++s)
            {
                List<ParticleFilter> partylist = new List<ParticleFilter>();
                for (int r = 0; r < NUMBER_OF_ROBOTS; ++r)
                {
                    ParticleFilter p1 = new ParticleFilter();
                    p1.create();
                    partylist.Add(p1);
                }
                particleFilterList.Add(partylist);
            }
        }

        public void create_simulation()
        {
            foreach (Shark s1 in MyGlobals.shark_list)
            {
                List<Simulation> currentSharkSim = new List<Simulation>();
                foreach (Robot r1 in MyGlobals.robot_list)
                {
                    double rangeError = s1.calc_range_error(r1);
                    Simulation sim = new Simulation(rangeError, s1, r1);
                    currentSharkSim.Add(sim);

                }
                simulationList.Add(currentSharkSim);
            }
        }
        //[[Sim1, Sim2], [Sim1, Sim2]]
        public void update_simulation_list(int sharkNumber, int robotNumber)
        {
            simulationList[sharkNumber][robotNumber].rangeError = MyGlobals.shark_list[sharkNumber].calc_range_error(MyGlobals.robot_list[robotNumber]);
        }
        public void update_robots()
        {
            foreach (Robot r1 in MyGlobals.robot_list)
            {
                r1.update_robot_position();
            }
        }
        public void update_sharks()
        {
            foreach (Shark s1 in MyGlobals.shark_list)
            {
                s1.update_shark();
            }
        }

        public void update_pfs()
        {
            foreach (List<ParticleFilter> pflist in particleFilterList)
            {
                foreach (ParticleFilter pf1 in pflist)
                {
                    pf1.update();
                }

            }
        }
        public void clear_real_range_list()
        {
            List<List<double>> newList = new List<List<double>>();
            MyGlobals.real_range_list = newList;
            create_real_range_list();
        }

        public bool get_simulation()
        {
            if (MyGlobals.random_num.Next(0, 11) > 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public double calc_range_error(List<double> mean_particle, int sharkNum1)
        {

            double auvRange = Math.Sqrt(Math.Pow((mean_particle[1] - MyGlobals.shark_list[sharkNum1].Y), 2) + Math.Pow((mean_particle[0] - MyGlobals.shark_list[sharkNum1].X), 2));
            return auvRange;
        }
        private void update_weight_lists(int index, ParticleFilter pf)
        {
            if (index == 0)
            {
                w1xList_pf1 =pf.w1_list_x;
                w1yList_pf1 = pf.w1_list_y;
                w2xList_pf1 = pf.w2_list_x;
                w2yList_pf1 = pf.w2_list_y;
                w3yList_pf1 = pf.w3_list_y;
                w3xList_pf1 = pf.w3_list_x;
            }
            if (index == 1)
            {
                w1xList_pf2 = pf.w1_list_x;
                w1yList_pf2 = pf.w1_list_y;
                w2xList_pf2 = pf.w2_list_x;
                w2yList_pf2 = pf.w2_list_y;
                w3yList_pf2 = pf.w3_list_y;
                w3xList_pf2 = pf.w3_list_x;
            }

        }
        
        private double calculateRangeError(Shark currentShark, List<double> meanList)
        { 
            Particle meanParticle = new Particle();
            meanParticle.X = meanList[0];
            meanParticle.Y = meanList[1];
            double particleRange = 0;
            double realRange = 0;
            foreach (Robot r1 in MyGlobals.robot_list)
            {
                realRange += (currentShark.calc_range_error(r1))/2;
                particleRange += (meanParticle.calc_particle_range(r1))/2;
            }
            double rangeError = realRange - particleRange;
            return rangeError;
        }
        //want create_range_error to return the errorList
        private List<double> create_range_error(Shark currentShark, ParticleFilter pf)
        {
            List<double> predictedList = pf.predicting_shark_location();
            double currentError = Math.Abs(calculateRangeError(currentShark, predictedList));
            List<double> errorList = new List<double>();
            errorList.Add(currentError);
            return errorList;
        }
        private void UpdateMap()
        {

            chart1.Series["Weight1"].Points.Clear();
            chart1.Series["Weight2"].Points.Clear();
            chart1.Series["Weight3"].Points.Clear();
            chart1.Series["Weight1_2"].Points.Clear();
            chart1.Series["Weight2_2"].Points.Clear();
            chart1.Series["Weight3_2"].Points.Clear();
            chart1.Series["Shark"].Points.Clear();
            chart1.Series["Shark2"].Points.Clear();
            chart1.Series["AUV1"].Points.Clear();
            chart1.Series["AUV2"].Points.Clear();
            //chart1.Series["AUV3"].Points.Clear();
            chart1.Series["Shark"].Points.AddXY(MyGlobals.shark_list[0].X, MyGlobals.shark_list[0].Y);
            chart1.Series["Shark2"].Points.AddXY(MyGlobals.shark_list[1].X, MyGlobals.shark_list[1].Y);
            //add in one of pf2
            for (int i = 0; i < w1xList_pf1.Count; ++i)
            {
                chart1.Series["Weight1"].Points.AddXY(w1xList_pf1[i], w1yList_pf1[i]);
            }
            for (int i = 0; i < w2xList_pf1.Count; ++i)
            {
                chart1.Series["Weight2"].Points.AddXY(w2xList_pf1[i], w2yList_pf1[i]);
            }
            for (int i = 0; i < w3xList_pf1.Count; ++i)
            {
                chart1.Series["Weight3"].Points.AddXY(w3xList_pf1[i], w3yList_pf1[i]);

            }


            for (int i = 0; i < w1xList_pf2.Count; ++i)
            {
                chart1.Series["Weight1_2"].Points.AddXY(w1xList_pf2[i], w1yList_pf2[i]);
            }
            for (int i = 0; i < w2xList_pf2.Count; ++i)
            {
                chart1.Series["Weight2_2"].Points.AddXY(w2xList_pf2[i], w2yList_pf2[i]);
            }
            for (int i = 0; i < w3xList_pf2.Count; ++i)
            {
                chart1.Series["Weight3_2"].Points.AddXY(w3xList_pf2[i], w3yList_pf2[i]);
            }

            //double hey = particle_filter.r1.robot_list_x.Count;
            //double yes = particle_filter.r1.robot_list_y[0];
            chart1.Series["AUV1"].Points.AddXY(MyGlobals.robot_list[0].X, MyGlobals.robot_list[0].Y);
            chart1.Series["AUV2"].Points.AddXY(MyGlobals.robot_list[1].X, MyGlobals.robot_list[1].Y);
            //chart1.Series["AUV3"].Points.AddXY(MyGlobals.robot_list[2].X, MyGlobals.robot_list[2].Y);

        }

       
        private void getParticleCoordinates()
        { 
            
            NUMBER_OF_ROBOTS = 2;
            NUMBER_OF_SHARKS = 2;
            NUMBER_OF_PARTICLEFILTERS = NUMBER_OF_ROBOTS * NUMBER_OF_SHARKS;
            create_and_initialize_sharks();
            create_and_initialize_robots();
            create_and_initialize_particle_filter();

            
            create_simulation();
            create_real_range_list();
            double count = 0;
            while (true)
            {
                count += 10;
                update_robots();
               

                if (get_simulation())
                {
                    update_simulation_list(0, 0);
                    update_simulation_list(0, 1);

                    if (get_simulation())
                    {
                        update_simulation_list(1, 0);
                        update_simulation_list(1, 1);

                    }

                }
                clear_real_range_list();
                foreach (List<Simulation> simlist in simulationList)
                {
                    foreach (Simulation sim in simlist)
                    {
                        sim.update_real_range_list();
                    }
                }
                update_pfs();
                int sharkNum = 0;
                foreach (List<ParticleFilter> pflist in this.particleFilterList)
                {
                    foreach (ParticleFilter pf in pflist)
                    {
                        //pf.update();
                        pf.update_weights(MyGlobals.real_range_list, sharkNum);
                        
                        if (count % 80 == 0)
                        {
                            pf.correct();
                            update_sharks();
                        }
                        
                    }

                    sharkNum += 1;
                }
                int sharkNum1 = 0;
                foreach (List<ParticleFilter> pflist in this.particleFilterList)
                {
                    int robotNum = 0;
                    foreach (ParticleFilter pf in pflist)
                    {
                        pf.weight_list_x();
                        pf.weight_list_y();

                        if (sharkNum1 == 0 & robotNum == 0)
                        {
                            int index = 0;
                            update_weight_lists(index, pf);
                        }
                        else if (sharkNum1 == 1 & robotNum == 0)
                        {
                            int index = 1;
                            update_weight_lists(index, pf);
                        }
                    }
                    sharkNum1 += 1;
                }
                errorList1 = create_range_error(MyGlobals.shark_list[0], particleFilterList[0][0]);
                errorList2 = create_range_error(MyGlobals.shark_list[1], particleFilterList[1][0]);

                if (chart1.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { UpdateMap(); });
                }
                else
                {
                    //......
                }
                if (chart2.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { UpdateChart1(); });
                }
                else
                {
                    //......
                }

                Thread.Sleep(100);
            }
        }
        private void UpdateChart1()
        {
            for (int i = 0; i < errorList1.Count; ++i)
            {
                chart2.Series["RangeError1"].Points.AddY(errorList1[i]);
                //chart2.Series["RangeError1"].Points.AddY(5.0);
            }
            for (int i = 0; i < errorList2.Count; ++i)
            {
                chart2.Series["RangeError2"].Points.AddY(errorList2[i]);
                //chart2.Series["Range Error 2"].Points.AddY(3.0);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            cpuThread = new Thread(new ThreadStart(this.getParticleCoordinates));
            cpuThread.IsBackground = true;
            cpuThread.Start();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }
    }
}
