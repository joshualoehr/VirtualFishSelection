using System.Collections.Generic;
using System.Linq;

namespace FishEvolutionGenetic
{
    public class FishSelect
    {
        public const int TOP_3_SELECT = 0;
        public const int BOTTOM_3_SELECT = 1;
        public const int ABOVE_AVERAGE_SELECT = 2;
        public const int ROULETTE_SELECT = 3;
        public const int TOURNAMENT_SELECT = 4;
        public const int STEADY_STATE_SELECT = 5;
        public const int ELITISM_SELECT = 6;

        private ISelectStrategy SelectStrategy;

        public FishSelect(int strategyType)
        {
            switch (strategyType)
            {
                case TOP_3_SELECT:
                    SelectStrategy = new Top3Select();
                    break;
                case BOTTOM_3_SELECT:
                    SelectStrategy = new Bottom3Select();
                    break;
                case ABOVE_AVERAGE_SELECT:
                    SelectStrategy = new AboveAverageSelect();
                    break;
                case ROULETTE_SELECT:
                    SelectStrategy = new RouletteSelect();
                    break;
                case TOURNAMENT_SELECT:
                    SelectStrategy = new TournamentSelect();
                    break;
                case STEADY_STATE_SELECT:
                    SelectStrategy = new SteadyStateSelect();
                    break;
                case ELITISM_SELECT:
                    SelectStrategy = new ElitismSelect();
                    break;
                default:
                    SelectStrategy = new Top3Select();
                    break;
            }
        }

        public void NextGeneration(List<Fish> fish)
        {
            List<Fish> selected = SelectStrategy.SelectFish(fish);
            SelectStrategy.BreedFish(fish, selected);
        }

        // Convenience method for mating two fish
        internal static Fish MateFish(Fish parent1, Fish parent2)
        {
            return new Fish(FishChromozomes.Mate(parent1.Chromozomes, parent2.Chromozomes));
        }
    }

    public interface ISelectStrategy
    {
        List<Fish> SelectFish(List<Fish> fish);
        void BreedFish(List<Fish> fish, List<Fish> selectedFish);
    }


    class Top3Select : ISelectStrategy
    {
        // Selects the 3 best fish in the population
        public List<Fish> SelectFish(List<Fish> fish)
        {
            List<Fish> topFish = fish.OrderByDescending(r => r.NumFoodEaten).Take(3).ToList();
            return topFish;
        }

        // Breeds evenly among the top 3 fish
        public void BreedFish(List<Fish> fish, List<Fish> selectedFish)
        {
            for (int i = 0; i < fish.Count; i++)
            {
                if (i < fish.Count / 3)
                    fish[i] = FishSelect.MateFish(selectedFish.ElementAt(0), selectedFish.ElementAt(1));
                else if (i < fish.Count * 2 / 3)
                    fish[i] = FishSelect.MateFish(selectedFish.ElementAt(0), selectedFish.ElementAt(2));
                else
                    fish[i] = FishSelect.MateFish(selectedFish.ElementAt(1), selectedFish.ElementAt(2));
            }
        }
    }

    class Bottom3Select : ISelectStrategy
    {
        // Selects the 3 worst fish in the population
        public List<Fish> SelectFish(List<Fish> fish)
        {
            List<Fish> topFish = fish.OrderBy(r => r.NumFoodEaten).Take(3).ToList();
            return topFish;
        }

        // Breeds evenly among the bottom 3 fish
        public void BreedFish(List<Fish> fish, List<Fish> selectedFish)
        {
            for (int i = 0; i < fish.Count; i++)
            {
                if (i < fish.Count / 3)
                    fish[i] = FishSelect.MateFish(selectedFish.ElementAt(0), selectedFish.ElementAt(1));
                else if (i < fish.Count * 2 / 3)
                    fish[i] = FishSelect.MateFish(selectedFish.ElementAt(0), selectedFish.ElementAt(2));
                else
                    fish[i] = FishSelect.MateFish(selectedFish.ElementAt(1), selectedFish.ElementAt(2));
            }
        }
    }

    class AboveAverageSelect : ISelectStrategy
    {
        // Selects those fish who consumed an above average amount of food
        public List<Fish> SelectFish(List<Fish> fish)
        {
            int averageFitness = fish.Sum(f => f.NumFoodEaten) / fish.Count;
            List<Fish> topFish = fish.Where(f => f.NumFoodEaten >= averageFitness).ToList();
            return topFish;
        }

        // Breeds repeating permutations of selectedFish pairs, starting with the best overall
        public void BreedFish(List<Fish> fish, List<Fish> selectedFish)
        {
            List<Fish[]> fishPairs = PairFish(selectedFish);
            
            for (int i = 0; i < fish.Count; i++)
            {
                Fish[] pair = fishPairs.ElementAt(i % fishPairs.Count);
                fish[i] = FishSelect.MateFish(pair[0], pair[1]);
            }
        }

        // Generates all permutations of fish pairs
        private List<Fish[]> PairFish(List<Fish> selectedFish)
        {
            selectedFish = selectedFish.OrderByDescending(f => f.NumFoodEaten).ToList();

            List<Fish[]> pairs = new List<Fish[]>();
            for (int j = 0; j < selectedFish.Count; j++)
            {
                Fish parent1 = selectedFish.ElementAt(j);
                for (int k = j + 1; k < selectedFish.Count; k++)
                {
                    Fish parent2 = selectedFish.ElementAt(k);
                    pairs.Add(new Fish[] { parent1, parent2 });
                }
            }
            return pairs;
        }
    }

    class RouletteSelect : ISelectStrategy
    {
        public List<Fish> SelectFish(List<Fish> fish)
        {
            throw new System.NotImplementedException();
        }

        public void BreedFish(List<Fish> fish, List<Fish> selectedFish)
        {
            throw new System.NotImplementedException();
        }
    }

    class TournamentSelect : ISelectStrategy
    {
        public List<Fish> SelectFish(List<Fish> fish)
        {
            throw new System.NotImplementedException();
        }

        public void BreedFish(List<Fish> fish, List<Fish> selectedFish)
        {
            throw new System.NotImplementedException();
        }
    }

    class SteadyStateSelect : ISelectStrategy
    {
        public List<Fish> SelectFish(List<Fish> fish)
        {
            throw new System.NotImplementedException();
        }

        public void BreedFish(List<Fish> fish, List<Fish> selectedFish)
        {
            throw new System.NotImplementedException();
        }
    }

    class ElitismSelect : ISelectStrategy
    {
        public List<Fish> SelectFish(List<Fish> fish)
        {
            throw new System.NotImplementedException();
        }

        public void BreedFish(List<Fish> fish, List<Fish> selectedFish)
        {
            throw new System.NotImplementedException();
        }
    }

   
}