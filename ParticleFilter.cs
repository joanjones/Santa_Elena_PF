using System;
using System.Collections.Generic;
using System.Linq;

namespace RealPF
{
    public class ParticleFilter
    {
        public int NUMBER_OF_PARTICLES;
        public double Current_Time;
        public List<Particle> particleList = new List<Particle>();
        public Robot r1;
        public double NUMBER_OF_AUVS;
        public List<double> w1_list_x;
        public List<double> w1_list_y;

        public List<double> w2_list_x;
        public List<double> w2_list_y;

        public List<double> w3_list_x;
        public List<double> w3_list_y;

        public List<double> errorList;
        public List<double> regionalEntropy;

        public int sharkNumber;
        public int robotNumber;


        public ParticleFilter()
        {
            this.Current_Time = 0;
            this.NUMBER_OF_PARTICLES = 1000;
            this.r1 = new Robot();
            this.w1_list_x = new List<double>();
            this.w2_list_x = new List<double>();
            this.w3_list_x = new List<double>();
            this.w1_list_y = new List<double>();
            this.w2_list_y = new List<double>();
            this.w3_list_y = new List<double>();
            this.errorList = new List<double>();
        }



        public void create()
        {

            for (int i = 0; i < NUMBER_OF_PARTICLES; ++i)
            {
                particleList.Add(new Particle());
            }
            /* 
             Particle particle1 = new Particle();
             particle1.X = 45;
             particle1.Y = 45;
             particleList.Add(particle1);
             Particle particle2 = new Particle();
             particle2.X = 0;
             particle2.Y = 0;
             particleList.Add(particle2);
             Particle particle3 = new Particle();
             particle3.Y = -100;
             particle3.X = -100;
             particleList.Add(particle3);
             */

        }
        public void update()
        {
            // updates particles while simulated
            // returns new list of updated particles

            for (int i = 0; i < NUMBER_OF_PARTICLES; ++i)
            {
                particleList[i].updateParticles();
            }

        }
        public void update_weights(List<List<double>> real_range_list, int SharkNumber)
        {
            // normalize new weights for each new shark measurement

            //[1,2], [20000,5]]

            for (int i = 0; i < NUMBER_OF_PARTICLES; ++i)
            {
                double current_weight = 1;
                int robot_number = 0;
                foreach (double rangeError in real_range_list[SharkNumber])
                {
                    if (rangeError != 20000)
                    {
                        double particle_range = particleList[i].calc_particle_range(MyGlobals.robot_list[robot_number]);
                        current_weight *= particleList[i].weight(real_range_list[SharkNumber][robot_number], particle_range);
                    }
                    robot_number += 1;
                }
                particleList[i].W = current_weight;
            }

        }
        public void correct()
        {
            //corrects the particles, adding more copies of particles based on how high the weight is
            List<Particle> tempList = new List<Particle>();

            for (int i = 0; i < NUMBER_OF_PARTICLES; ++i)
            {
                if (particleList[i].W <= 0.333)
                {
                    Particle particle1 = particleList[i].DeepCopy();
                    tempList.Add(particle1);


                }
                else if (particleList[i].W <= 0.666)
                {
                    Particle particle1 = particleList[i].DeepCopy();
                    tempList.Add(particle1);
                    Particle particle2 = particleList[i].DeepCopy();
                    tempList.Add(particle2);

                }
                else
                {
                    Particle particle1 = particleList[i].DeepCopy();
                    tempList.Add(particle1);
                    Particle particle2 = particleList[i].DeepCopy();
                    tempList.Add(particle2);
                    Particle particle3 = particleList[i].DeepCopy();
                    tempList.Add(particle3);
                    Particle particle4 = particleList[i].DeepCopy();
                    tempList.Add(particle4);
                }

            }
            particleList = new List<Particle>();
            for (int i = 0; i < NUMBER_OF_PARTICLES; ++i)
            {
                int index = MyGlobals.random_num.Next(0, tempList.Count);
                Particle particleIndex = tempList[index].DeepCopy();
                particleList.Add(particleIndex);

            }
        }

        public void weight_list_x()
        {
            w1_list_x = new List<double>();
            w2_list_x = new List<double>();
            w3_list_x = new List<double>();
            for (int i = 0; i < NUMBER_OF_PARTICLES; ++i)
            {
                if (particleList[i].W <= 0.333)
                {
                    w1_list_x.Add(particleList[i].X);
                }
                else if (particleList[i].W <= 0.666)
                {
                    w2_list_x.Add(particleList[i].X);
                }
                else
                {
                    w3_list_x.Add(particleList[i].X);
                }

            }

        }
        public void weight_list_y()
        {
            w1_list_y = new List<double>();
            w2_list_y = new List<double>();
            w3_list_y = new List<double>();
            for (int i = 0; i < NUMBER_OF_PARTICLES; ++i)
            {
                if (particleList[i].W <= 0.333)
                {
                    w1_list_y.Add(particleList[i].Y);
                }
                else if (particleList[i].W <= 0.666)
                {
                    w2_list_y.Add(particleList[i].Y);
                }
                else
                {
                    w3_list_y.Add(particleList[i].Y);
                }
            }
        }
        public List<double> predicting_shark_location()
        {
            double particle_total_x = 0;
            double particle_total_y = 0;
            for (int i = 0; i < NUMBER_OF_PARTICLES; ++i)
            {
                particle_total_x += particleList[i].X;
                particle_total_y += particleList[i].Y;
            }
            double particle_mean_x = particle_total_x / NUMBER_OF_PARTICLES;
            double particle_mean_y = particle_total_y / NUMBER_OF_PARTICLES;
            List<double> mean_particle = new List<double>();
            mean_particle.Add(particle_mean_x);
            mean_particle.Add(particle_mean_y);
            return mean_particle;
        }
         public List<double> calculate_entropy()
        {
            /*maybe forloop going through particlelist if x and y between certain range
             add to regional list
             length of regionlist=omega
             for each region list
                create entropy list
                s=kTln(omega) */
            List<double> region1List = new List<double>();
            List<double> region2List = new List<double>();
            List<double> region3List = new List<double>();
            List<double> region4List = new List<double>();
            List<double> region5List = new List<double>();
            List<double> region6List = new List<double>();
            List<double> region7List = new List<double>();
            List<double> region8List = new List<double>();
            List<double> region9List = new List<double>();
            regionalEntropy = new List<double>(); 
           
            
            for (int i = 0; i < NUMBER_OF_PARTICLES; ++i)
            {
                if (particleList[i].X < INITIAL_PARTICLE_RANGE/3 && particleList[i].Y < INITIAL_PARTICLE_RANGE/3)
                {
                    region1List.Add(particleList[i].W);
                }
                if (particleList[i].X < INITIAL_PARTICLE_RANGE/3 && particleList[i].Y < 2*INITIAL_PARTICLE_RANGE/3)
                {
                    region2List.Add(particleList[i].W);
                }
                if (particleList[i].X < INITIAL_PARTICLE_RANGE/3 && particleList[i].Y < INITIAL_PARTICLE_RANGE)
                {
                    region3List.Add(particleList[i].W);
                }
                if (particleList[i].X < 2*INITIAL_PARTICLE_RANGE/3 && particleList[i].Y < INITIAL_PARTICLE_RANGE/3)
                {
                    region4List.Add(particleList[i].W);
                }
                if (particleList[i].X < 2*INITIAL_PARTICLE_RANGE/3 && particleList[i].Y < 2*INITIAL_PARTICLE_RANGE/3)
                {
                    region5List.Add(particleList[i].W);
                }
                if (particleList[i].X < 2*INITIAL_PARTICLE_RANGE && particleList[i].Y < INITIAL_PARTICLE_RANGE)
                {
                    region6List.Add(particleList[i].W);
                }
                if (particleList[i].X < INITIAL_PARTICLE_RANGE && particleList[i].Y < INITIAL_PARTICLE_RANGE/3)
                {
                    region7List.Add(particleList[i].W);
                }
                if (particleList[i].X < INITIAL_PARTICLE_RANGE && particleList[i].Y < 2*INITIAL_PARTICLE_RANGE/3)
                {
                    region8List.Add(particleList[i].W);
                }
                if (particleList[i].X < INITIAL_PARTICLE_RANGE && particleList[i].Y < INITIAL_PARTICLE_RANGE)
                {
                    region9List.Add(particleList[i].W);
                }
                
               /*
                
                if (particleList[i].X < INITIAL_PARTICLE_RANGE/3)
                {
                    if (particleList[i].Y < INITIAL_PARTICLE_RANGE/3)
                    {
                        Region1 += particleList[i].W;
                    }
                   if (particleList[i].Y < 2*INITIAL_PARTICLE_RANGE/3)
                    {
                        Region2 += particleList[i].W;
                    } 
                    if (particleList[i].Y < INITIAL_PARTICLE_RANGE)
                    {
                        Region3 += particleList[i].W;
                    }
                }
                if (particleList[i].X < 2*INITIAL_PARTICLE_RANGE/3)
                {
                    if (particleList[i].Y < INITIAL_PARTICLE_RANGE/3)
                    {
                        
                    }
                   if (particleList[i].Y < 2*INITIAL_PARTICLE_RANGE/3)
                    {
                    } 
                    if (particleList[i].Y < INITIAL_PARTICLE_RANGE)
                    {
                    }
                }
                if (particleList[i].X < INITIAL_PARTICLE_RANGE)
                {
                    if (particleList[i].Y < INITIAL_PARTICLE_RANGE/3)
                    {
                    }
                   if (particleList[i].Y < 2*INITIAL_PARTICLE_RANGE/3)
                    {
                    } 
                    if (particleList[i].Y < INITIAL_PARTICLE_RANGE)
                    {
                    }
                }*/
            }
            /*
            omegaList = new List<double>;
            omegaList.Add(region1List.Count)
            omegaList.Add(region2List.Count)
            omegaList.Add(region3List.Count)
            omegaList.Add(region4List.Count)
            omegaList.Add(region5List.Count)
            omegaList.Add(region6List.Count)
            omegaList.Add(region7List.Count)
            omegaList.Add(region8List.Count)
            omegaList.Add(region9List.Count)
            */
            double Sregion1 = 0;
            for (int i=0; i < region1List; ++i)
            {
                double k = 1.38*Math.Pow(10,-23);
                double S = -k*region1List[i]*log(region1List[i])
                Sregion1+=S;
            }
            regionalEntropy.Add(Sregion1)
            double Sregion2 = 0;
            for (int i=0; i < region2List; ++i)
            {
                double k = 1.38*Math.Pow(10,-23);
                double S = -k*region2List[i]*log(region2List[i])
                Sregion2+=S;
            }
            regionalEntropy.Add(Sregion2)
            double Sregion3 = 0;
            for (int i=0; i < region3List; ++i)
            {
                double k = 1.38*Math.Pow(10,-23);
                double S = -k*region3List[i]*log(region3List[i])
                Sregion3+=S;
            }
            regionalEntropy.Add(Sregion3)
            double Sregion4 = 0;
            for (int i=0; i < region4List; ++i)
            {
                double k = 1.38*Math.Pow(10,-23);
                double S = -k*region4List[i]*log(region4List[i])
                Sregion4+=S;
            }
            regionalEntropy.Add(Sregion4)
            double Sregion5 = 0;
            for (int i=0; i < region5List; ++i)
            {
                double k = 1.38*Math.Pow(10,-23);
                double S = -k*region5List[i]*log(region5List[i])
                Sregion5+=S;
            }
            regionalEntropy.Add(Sregion5)
            double Sregion6 = 0;
            for (int i=0; i < region6List; ++i)
            {
                double k = 1.38*Math.Pow(10,-23);
                double S = -k*region6List[i]*log(region6List[i])
                Sregion6+=S;
            }
            regionalEntropy.Add(Sregion6)
            double Sregion7 = 0;
            for (int i=0; i < region7List; ++i)
            {
                double k = 1.38*Math.Pow(10,-23);
                double S = -k*region7List[i]*log(region7List[i])
                Sregion7+=S;
            }
            regionalEntropy.Add(Sregion7)
            double Sregion8 = 0;
            for (int i=0; i < region8List; ++i)
            {
                double k = 1.38*Math.Pow(10,-23);
                double S = -k*region8List[i]*log(region8List[i])
                Sregion8+=S;
            }
            regionalEntropy.Add(Sregion8)
            double Sregion9 = 0;
            for (int i=0; i < region9List; ++i)
            {
                double k = 1.38*Math.Pow(10,-23);
                double S = -k*region9List[i]*log(region9List[i])
                Sregion9+=S;
            }
            this.regionalEntropy.Add(Sregion9)
            return regionalEntropy;
        }
    }


}