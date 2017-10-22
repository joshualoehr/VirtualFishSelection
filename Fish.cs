using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNetworks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FishEvolutionGenetic
{
    public class Fish
    {
        public double PositionX;
        public double PositionY;

        public double Angle;
        public double HeadingAngle;

        public double Age;
        public double Energy;
        public bool IsDead;

        public int Size;
        public double AerodynamicEfficienty;

        public int NumFoodEaten;

        public static int InitialEnergy = 1000;

        public String Name;

        public BackPropogationNetwork FishNeural;

        public FishChromozomes Chromozomes;
        public FoodSensor Sensor;
        public FishLearn FishLearn;

        public class DeadEventArgs : EventArgs{}
        public class FeedEventArgs : EventArgs{}
        public class EnergyMaxEventArgs : EventArgs{}

        public delegate void DeadEventHandler(object sender, DeadEventArgs e);
        public event DeadEventHandler Dead;

        public delegate void FeedEventHandler(object sender, FeedEventArgs e);
        public event FeedEventHandler Feed;

        public Fish(FishChromozomes chromozomes, String name)
        {
            Chromozomes = chromozomes;
            FishNeural = new BackPropogationNetwork(Chromozomes.MentalChromozome);
            FishLearn = new FishEvolutionGenetic.FishLearn(this);
            Sensor = new FoodSensor(this);
            Name = name;
            ResetFish();

            Feed += Fish_Feed;
        }

        public Fish(FishChromozomes chromozomes) : this(chromozomes, "UnnamedFish") { }

 
        public Fish(String name)
        {
            Chromozomes = new FishChromozomes();
            Chromozomes.PhysicalChromozome = FishChromozomes.InititizeFishPhysicalChromosome();
            Chromozomes.MentalChromozome =  FishChromozomes.InitializeFishMentalChromosone();
            FishNeural = new BackPropogationNetwork(Chromozomes.MentalChromozome);
            FishLearn = new FishEvolutionGenetic.FishLearn(this);
            Sensor = new FoodSensor(this);
            Name = name;
            ResetFish();

            Feed += Fish_Feed;
        }

        public override String ToString()
        {
            return String.Format("{0}[{1}]", Name, NumFoodEaten);
        }

        void Fish_Feed(object sender, Fish.FeedEventArgs e)
        {
            FishLearn.LearnPreviousSteps(10);
        }

        public void ResetFish()
        {
            PositionX = 100;
            PositionY = 100;
            Energy = InitialEnergy;
            Age = 0;
            NumFoodEaten = 0;
            Angle = 0;
            IsDead = false;
            HeadingAngle = 0;
            Size = (int)((Chromozomes.PhysicalChromozome.Genes.Find(r => r.Name == "Size").Value +0.5) * 10);
            AerodynamicEfficienty = (Chromozomes.PhysicalChromozome.Genes.Find(r => r.Name == "AerodynamicEfficiency").Value +0.5) / 2; ;
        }

        public List<Line> lines = new List<Line>();
        public virtual IEnumerable<UIElement> GenerateFishShapes()
        {
            lines.Clear();

            double line1x = (Size * Math.Cos(Angle)) + PositionX;
            double line1y = (Size * Math.Sin(Angle)) + PositionY;
            double line2x = PositionX + (Size * Math.Cos(HeadingAngle + Angle + Math.PI));
            double line2y = PositionY + (Size * Math.Sin(HeadingAngle + Angle + Math.PI));

            byte red = (byte)(Chromozomes.PhysicalChromozome.Genes.Find(r=> r.Name == "Color").Value*255);


            lines.Add(new Line() { X1 = PositionX, Y1 = PositionY, X2 = line1x, Y2 = line1y, Stroke = new SolidColorBrush(new Color {A= 255, B = 128, R = red, G = red }), StrokeThickness = 2 });
            lines.Add(new Line() { X1 = PositionX, Y1 = PositionY, X2 = line2x, Y2 = line2y, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 4 });

            return lines;
        }


        public void Live(FoodGenerator foodGen, IEnumerable<Fish> fishes)
        {
            Sensor.UpdateSensors(foodGen.FoodParticles);
            FishNeural.ApplyInput(Sensor.FoodSensors);
            FishNeural.CalculateOutput();

            CheckFood(foodGen);

            Age += 0.005;

            if (Age > 10)
            {
                IsDead = true;
                if (Dead != null)
                    Dead(this, new DeadEventArgs() { });
            }

            IEnumerable<double> Outputs = FishNeural.ReadOutput();

            FishLearn.AddStep(Sensor.FoodSensors, Outputs);
            HeadingAngle = (Math.Atan(Outputs.ElementAt(1)/Outputs.ElementAt(2))-(Math.PI/4));
            MoveForward(Outputs.ElementAt(0));
        }

        public void MoveForward(double param = 0)
        {
            double line2x = PositionX;
            double line2y = PositionY;

            PositionX = PositionX + (Size * AerodynamicEfficienty *param * Math.Cos(HeadingAngle + Angle + Math.PI));
            PositionY = PositionY + (Size * AerodynamicEfficienty * param* Math.Sin(HeadingAngle + Angle + Math.PI));

            Angle = Math.Atan2((line2y - PositionY), (line2x - PositionX));

            Energy -= ((Size*0.1) - AerodynamicEfficienty*0.05);

            if (Energy <= 0)
            {
                IsDead = true;
                if (Dead != null)
                    Dead(this, new DeadEventArgs() { });
            }

            CheckWalls();
        }

        private void CheckWalls()
        {
            if (PositionX < 0)
            {
                Angle += Math.PI / 2;
                PositionX = 0;
            }
            else if (PositionX > MainWindow.Current.canvas.ActualWidth)
            {
                Angle += Math.PI / 2;
                PositionX = MainWindow.Current.canvas.ActualWidth;
            }

            if (PositionY < 0)
            {
                Angle += Math.PI / 2;
                PositionY = 0;
            }
            else if (PositionY > MainWindow.Current.canvas.ActualHeight)
            {
                PositionY = MainWindow.Current.canvas.ActualHeight;
                Angle += Math.PI / 2;
            }
        }

        private List<Food> removeFood = new List<Food>();
        private void CheckFood(FoodGenerator foodGen)
        {
            removeFood.Clear();
            foreach (Food food in foodGen.FoodParticles)
            {
                if (Math.Sqrt(((PositionX - food.PositonX) * (PositionX - food.PositonX)) + ((PositionY - food.PositonY) * (PositionY - food.PositonY))) < 10)
                {
                    if (Feed != null)
                        Feed(this, new FeedEventArgs() { });

                    NumFoodEaten += 1;

                    Energy += (int)food.Energy;
                    removeFood.Add(food);
                }
            }

            foreach (Food food in removeFood)
            {
                foodGen.AddFood();
                foodGen.RemoveFood(food);
                
            }
        }
    }
}
