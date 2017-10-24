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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NeuralNetworks;
using System.Windows.Threading;

namespace FishEvolutionGenetic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
	const int STRATEGY = FishSelect.STEADY_STATE_SELECT;

        int generation = 0;
        public static int NumPopulation = 25;
        public static int NumFood = 3;

        List<Fish> fishes;
        public static MainWindow Current;
        FoodGenerator foodgen;

        static FishSelect fishSelection;

        DispatcherTimer timer = new DispatcherTimer();

        GraphWindow graphWindow = new GraphWindow();
        public MainWindow()
        {   
            InitializeComponent();
            graphWindow.Show();
            Current = this;
            fishes = new List<Fish>();

            for (int i = 0; i < NumPopulation; i++)
                fishes.Add(new Fish("Fish" + i));

            fishSelection = new FishSelect(STRATEGY);

            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Start();
        }

        int counter = 1;
        void timer_Tick(object sender, EventArgs e)
        {
            foreach (Fish fish in fishes.Where(r => r.IsDead == false))
            {
                fish.Live(foodgen, fishes.Where(r=>r.IsDead==false));
            }
            if (fishes.Where(r => r.IsDead == false).Count() == 0 || counter > 10000)
            {
                counter = 0;
                UpdateTotalFitness(++generation);
                fishSelection.NextGeneration(fishes);
            }
            counter++;
            RefreshCanvas();
        }

        public void UpdateTotalFitness(int generation)
        {
            int totalfood = 0;
            fishes.ForEach(r => totalfood += r.NumFoodEaten);

            status_text_block.Text = "Generation: " + generation + ", consumed " + totalfood + " food";
            System.Diagnostics.Debug.WriteLine(status_text_block.Text);
            graphWindow.AddDataPoint(generation, totalfood);
        }

        public void RefreshCanvas()
        {
            canvas.Children.Clear();
            IEnumerable<Fish> Alive = fishes.Where(r => r.IsDead == false);
            foreach (Fish fish in Alive)
            {
                foreach (UIElement e in fish.GenerateFishShapes())
                    canvas.Children.Add(e);
            }
            if (foodgen != null)
            {
                foreach (UIElement e in foodgen.GenerateFoodShapes())
                    canvas.Children.Add(e);
            }
        }
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            foodgen = new FoodGenerator();
            for (int i = 0; i < NumFood; i++) 
                foodgen.AddFood();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            graphWindow.Close();
        }
    }
}
