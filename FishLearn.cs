using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FishEvolutionGenetic
{
    public class FishLearn
    {
        public Fish Fish;
        public Queue<NeuralNetworks.DataSet> LearnQueue;
        public BackgroundWorker worker;

        public FishLearn(Fish fish)
        {
            Fish = fish;
            LearnQueue = new Queue<NeuralNetworks.DataSet>(15);

            worker = new BackgroundWorker();

            worker.DoWork += ((object e, DoWorkEventArgs w) =>
            {
                Fish.FishNeural.BatchBackPropogate(LearnQueue.ToArray(),
                                                  (int)w.Argument, 0.1, 0.1, worker);
            });
        }

        public void AddStep(IEnumerable<double> neuralInputs, IEnumerable<double> neuralOutputs)
        {
            NeuralNetworks.DataSet fishio = new NeuralNetworks.DataSet() {
                                                    Inputs = neuralInputs.ToArray(),
                                                    Outputs = neuralOutputs.ToArray()
                                            };
            try
            {
                LearnQueue.Enqueue(fishio);
            }
            catch
            {
                LearnQueue.Dequeue();
                LearnQueue.Enqueue(fishio);
            }
        }

        public void LearnPreviousSteps(int iterations)
        {
            worker.WorkerReportsProgress = true;
            if(!worker.IsBusy)
                worker.RunWorkerAsync(iterations);
        }
    }
}
