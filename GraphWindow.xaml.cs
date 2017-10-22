using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FishEvolutionGenetic
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public GraphWindow()
        {
            InitializeComponent();

        }

        public void AddDataPoint(int x, int y)
        {
            Line line = new Line() { X1 = x * 2, X2 = x * 2, Y1 = graphCanvas.ActualHeight, Y2 = graphCanvas.ActualHeight - y, StrokeThickness = 2, Stroke = new SolidColorBrush(Colors.Black) };
            graphCanvas.Children.Add(line);
        }
    }
}
