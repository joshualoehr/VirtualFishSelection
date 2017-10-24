/* ************************************************************************* */
/* Program: FishSelect.cs                                                    */
/* Description: Provide various selection strategies for genetic algorithm   */
/* Author: Josh Loehr                                                        */
/* Last Date Modified: October 22, 2017                                      */
/*                                                                           */
/* NOTES:                                                                    */
/* 1. Algorithms implemented (adapted from [2]):                             */
/*     a. Top 3 Selection (Default)                                          */
/*     b. Bottom 3 Selection                                                 */
/*     c. Above Average Selection                                            */
/*     d. Roulette Selection                                                 */
/*     e. Tournament Selection                                               */
/*     f. Steady State Selection                                             */
/*     g. Steady State w/ Elitism                                            */
/* 2. Hyperparameters:                                                       */
/*     a. TOURNAMENT_SIZE (in TournamentSelect)                              */
/*     b. NUM_BREED and NUM_REMOVE (in SteadyStateSelect)                    */
/*     c. USE_ELITISM (in SteadyStateSelect)                                 */
/* 3. This program is based on [1] with modifications on                     */
/*    MainWindow.NextGeneration()                                            */
/*                                                                           */
/* References:                                                               */
/* [1] User hemanthk119. AI: Genetic Evolution of Virtual Fish. Retrieved    */
/*     October 23, 2017, from Code Project site:                             */
/*     https://www.codeproject.com/Articles/1074915/AI-Genetic-Evolution-of- */
/*     Virtual-Fish                                                          */
/* [2] Stuart Russell and Peter Norvig, “Artificial Intelligence: A Modern   */
/*     Approach”, Third Edition, Prentice Hall, 2010, 1132 pages             */
/* ************************************************************************* */

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

	// Adds one instance of each Fish to the breedingPool for each food consumed
        public List<Fish> SelectFish(List<Fish> fish)
        {
            int totalFood = fish.Sum(f => f.NumFoodEaten);
            List<Fish> breedingPool = new List<Fish>(totalFood);
            fish.ForEach(f => populateBreedingPool(breedingPool, f));
            return breedingPool;
        }

	// Randomly selects from breeding pool
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
        private const int TOURNAMENT_SIZE = 3;

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
        Random rand = new Random();

        // Hyperparameters - tune manually
        private const int NUM_BREED = 5;
        private const int NUM_REMOVE = 5;
        private const bool USE_ELITISM = true;

        // Select the top NUM_BREED fish from the population
        public List<Fish> SelectFish(List<Fish> fish)
        {
            return fish.OrderByDescending(f => f.NumFoodEaten).Take(NUM_BREED).ToList();
        }

        public void BreedFish(List<Fish> fish, List<Fish> selectedFish)
        {
            // Sort the population in ascending order, in place
            fish.Sort((f1, f2) => f1.NumFoodEaten.CompareTo(f2.NumFoodEaten));

            // Replace worst NUM_REMOVE fish with offspring of the best
            for (int i = 0; i < NUM_REMOVE; i++)
            {
                fish[i] = RandomMate(selectedFish, "Fish" + i);
            }

            // Mate remaining fish with themselves to allow for mutations
            for (int j = NUM_REMOVE; j < fish.Count; j++)
            {
                // If using elitism, leave the top NUM_BREED fish untouched
                if (j >= fish.Count - NUM_BREED && USE_ELITISM)
                    break;
                fish[j] = FishSelect.MateFish(fish[j], fish[j], "Fish" + j);
            }
        }

        // Randomly select two fish from the breedingPool to produce an offspring
        private Fish RandomMate(List<Fish> breedingPool, String name)
        {
            int idx1 = rand.Next(0, breedingPool.Count);
            int idx2;
            do
            {
                idx2 = rand.Next(0, breedingPool.Count);
            } while (idx1 == idx2);

            Fish parent1 = breedingPool.ElementAt(idx1);
            Fish parent2 = breedingPool.ElementAt(idx2);
            return FishSelect.MateFish(parent1, parent2, name);
        }
    }   
}
