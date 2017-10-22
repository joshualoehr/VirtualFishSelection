using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace FishEvolutionGenetic
{
    public class FoodSensor
    {
        public List<double> FoodSensors;

        public Fish Fish;

        public static int FieldOfView = 270;
        public static int SensorCoeff = 1000;
        

        public FoodSensor(Fish fish)
        {
            Fish = fish;
            FoodSensors = new List<double>();
            for (int i = 0; i < FishChromozomes.NumInputNeurons; i++)
            {
                FoodSensors.Add(0);
            }
        }


        public void UpdateSensors(IEnumerable<Food> foodGen)
        {
            for (int i = 0; i < FoodSensors.Count; i++)
                FoodSensors[i] = 0;

            foreach (Food food in foodGen)
            {
                double foodangle = Math.Atan2(Fish.PositionY - food.PositonY, Fish.PositionX - food.PositonX);
                double distance = Math.Sqrt(((Fish.PositionX - food.PositonX) * (Fish.PositionX - food.PositonX)) + ((Fish.PositionY - food.PositonY) * (Fish.PositionY - food.PositonY)));
                double angle = foodangle - Fish.Angle;

                while (Math.Abs(angle) > Math.PI)
                {
                    if (angle > Math.PI)
                        angle = (angle - 2 * Math.PI);
                    else if (angle < Math.PI)
                        angle = (angle + 2 * Math.PI);
                }

                angle = angle * 180 / Math.PI;

                if (Math.Abs(angle) < FieldOfView / 2)
                {
                    int sensorindex = (int)(angle / (FieldOfView / FoodSensors.Count));
                    sensorindex = (sensorindex + (FoodSensors.Count / 2));
                    FoodSensors[sensorindex] += (SensorCoeff / distance);
                }
            }
        }
    }
}
