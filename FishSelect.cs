using System.Collections.Generic;
using System.Linq;

namespace FishEvolutionGenetic
{
    public class FishSelect
    {
        public const int TOP_3_SELECT = 0;
        public const int BOTTOM_3_SELECT = 1;
        public const int FITNESS_PROP_SELECT = 2;
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
                case FITNESS_PROP_SELECT:
                    SelectStrategy = new FitnessPropSelect();
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

    }

    class Top3Select : ISelectStrategy
    {
        public List<Fish> SelectFish(List<Fish> fish)
        {
            List<Fish> topFish = fish.OrderByDescending(r => r.NumFoodEaten).Take(3).ToList();
            return topFish;
        }

        public void BreedFish(List<Fish> fish, List<Fish> selectedFish)
        {
            for (int i = 0; i < fish.Count; i++)
            {
                if (i < fish.Count / 3)
                    fish[i] = new Fish(FishChromozomes.Mate(selectedFish.ElementAt(0).Chromozomes, selectedFish.ElementAt(1).Chromozomes));
                else if (i < fish.Count * 2 / 3)
                    fish[i] = new Fish(FishChromozomes.Mate(selectedFish.ElementAt(0).Chromozomes, selectedFish.ElementAt(2).Chromozomes));
                else
                    fish[i] = new Fish(FishChromozomes.Mate(selectedFish.ElementAt(1).Chromozomes, selectedFish.ElementAt(2).Chromozomes));
            }
        }
    }

    class Bottom3Select : ISelectStrategy
    {
        public List<Fish> SelectFish(List<Fish> fish)
        {
            List<Fish> topFish = fish.OrderBy(r => r.NumFoodEaten).Take(3).ToList();
            return topFish;
        }

        public void BreedFish(List<Fish> fish, List<Fish> selectedFish)
        {
            for (int i = 0; i < fish.Count; i++)
            {
                if (i < fish.Count / 3)
                    fish[i] = new Fish(FishChromozomes.Mate(selectedFish.ElementAt(0).Chromozomes, selectedFish.ElementAt(1).Chromozomes));
                else if (i < fish.Count * 2 / 3)
                    fish[i] = new Fish(FishChromozomes.Mate(selectedFish.ElementAt(0).Chromozomes, selectedFish.ElementAt(2).Chromozomes));
                else
                    fish[i] = new Fish(FishChromozomes.Mate(selectedFish.ElementAt(1).Chromozomes, selectedFish.ElementAt(2).Chromozomes));
            }
        }
    }

    class FitnessPropSelect : ISelectStrategy
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

    public interface ISelectStrategy
    {
        List<Fish> SelectFish(List<Fish> fish);
        void BreedFish(List<Fish> fish, List<Fish> selectedFish);
    }
}