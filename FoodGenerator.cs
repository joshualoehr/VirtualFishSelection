using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace FishEvolutionGenetic
{
    public class Food
    {
        public double PositonX;
        public double PositonY;
        public double Energy;
    }

    public class FoodGenerator
    {
        public List<Food> FoodParticles;

        public static Random random = new Random();

        public FoodGenerator()
        {
            FoodParticles = new List<Food>();

        }

        public void AddFood()
        {
            Food food = new Food() { PositonX = (double)random.Next((int)MainWindow.Current.canvas.ActualWidth), Energy = 1000, PositonY = (double)random.Next((int)MainWindow.Current.canvas.ActualHeight) };
            FoodParticles.Add(food);
        }

        public void AddAtPosition(double PosX, double PosY, int num)
        {
            for (int i = 0; i < num; i++)
            {
                Food food = new Food() { PositonX = PosX, Energy = 1000, PositonY = PosY };
                FoodParticles.Add(food);
            }
        }

        public void RemoveFood(Food food)
        {
            FoodParticles.Remove(food);
        }

        private List<Line> shapes = new List<Line>();
        public IEnumerable<UIElement> GenerateFoodShapes()
        {
            shapes.Clear();

            foreach (Food food in FoodParticles)
            {
                shapes.Add(new Line() { X1 = food.PositonX, Y1 = food.PositonY, X2 = food.PositonX - 5, Y2 = food.PositonY - 5, Stroke = new SolidColorBrush(Colors.Green), StrokeThickness = 4 });
            }

            return shapes;
        }
    }
}
