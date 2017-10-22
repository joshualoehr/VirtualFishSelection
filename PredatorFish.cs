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
    public class PredatorFish :Fish
    {

        public bool IsPredator = true;



        public override IEnumerable<UIElement> GenerateFishShapes()
        {
            lines.Clear();

            double line1x = (Size * Math.Cos(Angle)) + PositionX;
            double line1y = (Size * Math.Sin(Angle)) + PositionY;
            double line2x = PositionX + (Size * Math.Cos(HeadingAngle + Angle + Math.PI));
            double line2y = PositionY + (Size * Math.Sin(HeadingAngle + Angle + Math.PI));

            byte red = (byte)(Chromozomes.PhysicalChromozome.Genes.Find(r => r.Name == "Color").Value * 255);

            lines.Add(new Line() { X1 = PositionX, Y1 = PositionY, X2 = line1x, Y2 = line1y, Stroke = new SolidColorBrush(new Color { A = 255, B = 128, R = red, G = red }), StrokeThickness = 4 });
            lines.Add(new Line() { X1 = PositionX, Y1 = PositionY, X2 = line2x, Y2 = line2y, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 8 });

            return lines;
        }

    }
}
