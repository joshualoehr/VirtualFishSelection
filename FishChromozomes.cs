using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNetworks;

namespace FishEvolutionGenetic
{
    public class FishChromozomes
    {
        public class Gene
        {
            public string Name;
            public double Value;
            public double MutationProbability;
        }
        public class Chromozome
        {
            public List<Gene> Genes;
            public void MutateRandom()
            {
                foreach (Gene gene in Genes)
                {
                    if (gene.MutationProbability > RandomProvider.Random.NextDouble())
                        gene.Value = RandomProvider.Random.NextDouble();
                }
            }
        }


        public Chromozome PhysicalChromozome;
        public NetworkData MentalChromozome;

        public static int NumInputNeurons = 15;
        public static int NumOutputNeurons = 3;
        public static int NumHiddenNeurons = 10;


        public static Chromozome InititizeFishPhysicalChromosome()
        {
            Chromozome chromozome = new Chromozome();
            chromozome.Genes = new List<Gene>();

            chromozome.Genes.Add(new Gene() { Name = "Size", MutationProbability = 0.01, Value = RandomProvider.Random.NextDouble() });
            chromozome.Genes.Add(new Gene() { Name = "AerodynamicEfficiency", MutationProbability = 0.01, Value = RandomProvider.Random.NextDouble() });
            chromozome.Genes.Add(new Gene() { Name = "Color", MutationProbability = 0.01, Value = RandomProvider.Random.NextDouble() });

            return chromozome;
        }
        public static NetworkData InitializeFishMentalChromosone()
        {
            return new BackPropogationNetwork(NumInputNeurons, NumOutputNeurons, NumHiddenNeurons).GetNetworkData();
        }

        public static FishChromozomes Mate(FishChromozomes one, FishChromozomes two)
        {
            FishChromozomes guppie = new FishChromozomes();
            guppie.PhysicalChromozome = new Chromozome();

            guppie.MentalChromozome = MixNeuralUp(one.MentalChromozome, two.MentalChromozome);
            guppie.PhysicalChromozome.Genes = MixGenesUp(one.PhysicalChromozome.Genes, two.PhysicalChromozome.Genes);

            guppie.PhysicalChromozome.MutateRandom();
            MutateNetwork(guppie.MentalChromozome);
            MutateNetwork(guppie.MentalChromozome);
            MutateNetwork(guppie.MentalChromozome);

            return guppie;
        }

        public static void MutateNetwork(NetworkData network)
        {
            int connetionId = RandomProvider.Random.Next(network.Connections.Count);
            if (RandomProvider.Random.NextDouble() > 0.5)
                network.Connections[connetionId].Weight = RandomProvider.Random.NextDouble();
            else
                network.Connections[connetionId].Weight = RandomProvider.Random.NextDouble();
        }

        public static List<Gene> MixGenesUp(List<Gene> one, List<Gene> two)
        {
            int crossoverpoint = RandomProvider.Random.Next(one.Count);
            List<Gene> genes = new List<Gene>();
            for (int i = 0; i < one.Count; i++)
            {
                if (i <= crossoverpoint)
                    genes.Add(new Gene() { Name = one[i].Name, Value = one[i].Value });
                else
                    genes.Add(new Gene() { Name = two[i].Name, Value = two[i].Value });
            }

            return genes;
        }

        public static NetworkData MixNeuralUp(NetworkData one, NetworkData two)
        {
            int crossoverpoint1 = RandomProvider.Random.Next(one.Layers[one.InputLayerId].NumNeuron + 1);
            bool frontback = false;

            if (RandomProvider.Random.NextDouble() > 0.5)
                frontback = true;

            BackPropogationNetwork guppienet = new BackPropogationNetwork(one.Layers[one.InputLayerId].NumNeuron,
                                                                          one.Layers[one.OutputLayerId].NumNeuron, 
                                                                          one.Layers[1].NumNeuron);
            NetworkData guppiedata = guppienet.GetNetworkData();
            if (frontback)
            {
                foreach (ConnectionData c in guppiedata.Connections.Where(r => r.From.Layer == guppiedata.InputLayerId))
                    c.Weight = one.Connections.Find(r => (r.From.Layer == c.From.Layer) 
                                                      && (r.From.Node == c.From.Node)
                                                      && (r.To.Layer == c.To.Layer)
                                                      && (r.To.Node == c.To.Node)).Weight;

                foreach (ConnectionData c in guppiedata.Connections.Where(r => r.To.Layer == guppiedata.OutputLayerId
                                                                            && r.To.Node < crossoverpoint1))
                    c.Weight = one.Connections.Find(r => (r.From.Layer == c.From.Layer)
                                                      && (r.From.Node == c.From.Node) 
                                                      && (r.To.Layer == c.To.Layer) 
                                                      && (r.To.Node == c.To.Node)).Weight;

                foreach (ConnectionData c in guppiedata.Connections.Where(r => r.To.Layer == guppiedata.OutputLayerId
                                                                            && r.To.Node >= crossoverpoint1))
                    c.Weight = two.Connections.Find(r => (r.From.Layer == c.From.Layer) 
                                                      && (r.From.Node == c.From.Node) 
                                                      && (r.To.Layer == c.To.Layer) 
                                                      && (r.To.Node == c.To.Node)).Weight;
            }

            else
            {
                foreach (ConnectionData c in guppiedata.Connections.Where(r => r.From.Layer == guppiedata.InputLayerId))
                    c.Weight = two.Connections.Find(r => (r.From.Layer == c.From.Layer)
                                                      && (r.From.Node == c.From.Node) 
                                                      && (r.To.Layer == c.To.Layer) 
                                                      && (r.To.Node == c.To.Node)).Weight;

                foreach (ConnectionData c in guppiedata.Connections.Where(r => r.To.Layer == guppiedata.OutputLayerId
                                                                            && r.To.Node < crossoverpoint1))
                    c.Weight = two.Connections.Find(r => (r.From.Layer == c.From.Layer)
                                                      && (r.From.Node == c.From.Node) 
                                                      && (r.To.Layer == c.To.Layer) 
                                                      && (r.To.Node == c.To.Node)).Weight;

                foreach (ConnectionData c in guppiedata.Connections.Where(r => r.To.Layer == guppiedata.OutputLayerId
                                                                            && r.To.Node >= crossoverpoint1))
                    c.Weight = one.Connections.Find(r => (r.From.Layer == c.From.Layer) 
                                                      && (r.From.Node == c.From.Node) 
                                                      && (r.To.Layer == c.To.Layer) 
                                                      && (r.To.Node == c.To.Node)).Weight;
            }
            return guppiedata;
        }
    }

    public class RandomProvider
    {
        public static Random Random = new Random();
    }
}
