using System;
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
        internal static Fish MateFish(Fish parent1, Fish parent2, string name)
        {
            return new Fish(FishChromozomes.Mate(parent1.Chromozomes, parent2.Chromozomes), name);
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
                    fish[i] = FishSelect.MateFish(selectedFish.ElementAt(0), selectedFish.ElementAt(1), "Fish"+i);
                else if (i < fish.Count * 2 / 3)
                    fish[i] = FishSelect.MateFish(selectedFish.ElementAt(0), selectedFish.ElementAt(2), "Fish" + i);
                else
                    fish[i] = FishSelect.MateFish(selectedFish.ElementAt(1), selectedFish.ElementAt(2), "Fish" + i);
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
                    fish[i] = FishSelect.MateFish(selectedFish.ElementAt(0), selectedFish.ElementAt(1), "Fish" + i);
                else if (i < fish.Count * 2 / 3)
                    fish[i] = FishSelect.MateFish(selectedFish.ElementAt(0), selectedFish.ElementAt(2), "Fish" + i);
                else
                    fish[i] = FishSelect.MateFish(selectedFish.ElementAt(1), selectedFish.ElementAt(2), "Fish" + i);
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
                fish[i] = FishSelect.MateFish(pair[0], pair[1], "Fish" + i);
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
        private System.Random rand = new System.Random();

        public List<Fish> SelectFish(List<Fish> fish)
        {
            int totalFood = fish.Sum(f => f.NumFoodEaten);
            List<Fish> breedingPool = new List<Fish>(totalFood);
            fish.ForEach(f => populateBreedingPool(breedingPool, f));
            return breedingPool;
        }

        public void BreedFish(List<Fish> fish, List<Fish> selectedFish)
        {
            int parentIdx1, parentIdx2;

            for (int i = 0; i < fish.Count; i++)
            {
                parentIdx1 = rand.Next(0, selectedFish.Count);
                do
                {
                    parentIdx2 = rand.Next(0, selectedFish.Count);
                } while (parentIdx1 == parentIdx2);

                Fish parent1 = selectedFish.ElementAt(parentIdx1);
                Fish parent2 = selectedFish.ElementAt(parentIdx2);
                fish[i] = FishSelect.MateFish(parent1, parent2, "Fish" + i);
            }
        }

        private void populateBreedingPool(List<Fish> breedingPool, Fish fish)
        {
            int count = fish.NumFoodEaten;
            for (int i = 0; i < count; i++)
            {
                breedingPool.Add(fish);
            }
        }
    }

    class TournamentSelect : ISelectStrategy
    {
        // Hyperparameter - tune this manually
        private const int TOURNAMENT_SIZE = 2;

        System.Random rand = new System.Random();

        // Generate a list of parent fish by performing tournament selection
        // twice for every fish in the population
        public List<Fish> SelectFish(List<Fish> fish)
        {
            List<Fish> parents = new List<Fish>(2*fish.Count);
            for (int i = 0; i < 2*fish.Count; i++)
            {
                parents.Add(tournamentSelect(fish));
            }
            return parents;
        }

        // Mate parent fish which are adjacent within selectedFish
        public void BreedFish(List<Fish> fish, List<Fish> selectedFish)
        {
            for (int i = 0; i < fish.Count; i++)
            {
                Fish parent1 = selectedFish.ElementAt(2*i);
                Fish parent2 = selectedFish.ElementAt(2*i + 1);
                System.Diagnostics.Debug.Write(String.Format("({0}, {1}) ", parent1, parent2));
                fish[i] = FishSelect.MateFish(parent1, parent2, "Fish" + i);
            }
            System.Diagnostics.Debug.WriteLine("");
        }

        // Select the best fish out of a random sample of size TOURNAMENT_SIZE
        private Fish tournamentSelect(List<Fish> population)
        {
            Fish bestFish = null;
            for (int i = 0; i < TOURNAMENT_SIZE; i++)
            {
                int idx = rand.Next(0, population.Count);
                Fish fish = population.ElementAt(idx);
                if (bestFish == null || bestFish.NumFoodEaten < fish.NumFoodEaten)
                    bestFish = fish;
            }
            return bestFish;
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