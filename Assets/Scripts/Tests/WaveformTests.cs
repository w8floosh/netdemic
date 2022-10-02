using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using GaussianDistribution = MathNet.Numerics.Distributions.Normal;

public class WaveformTests
{
    int testSamples = 22000;
    // A Test behaves as an ordinary method
    [Test]
    public void GenerateNewWaveformTest() //private
    {
        var watch = Stopwatch.StartNew();
        
        double[] gaussianSamples = NewWaveform(300, 5);
        int[] integerSamples = new int[testSamples];
        long mean = 0;
        for (int i = 0; i < testSamples; i++){
            integerSamples[i] = (int)Math.Floor(gaussianSamples[i]);
            mean += integerSamples[i];
        }
        watch.Stop();
        UnityEngine.Debug.Log(watch.ElapsedMilliseconds);

        watch.Start();

        mean /= testSamples;
        UnityEngine.Debug.Log("Media: " + mean + " | Media iniziale: 300");
        float[] frequencyProbabilities = new float[testSamples];
        var counters = from frequency in integerSamples
                       group frequency by frequency into frequencyCounter
                       let count = frequencyCounter.Count()
                       let freq = frequencyCounter.Key
                       orderby freq ascending
                       select new { Frequency = frequencyCounter.Key, Count = count };

        watch.Stop();
        UnityEngine.Debug.Log(watch.ElapsedMilliseconds);

        watch.Start();
        for (int f = counters.First().Frequency - 1; f < counters.Last().Frequency; f++)
        {
            var frequencyTuple = counters.FirstOrDefault(c => c.Frequency == f);
            if (frequencyTuple != null)
            {
                frequencyProbabilities[f] = (float)Math.Round((float)frequencyTuple.Count / testSamples * 100, 4);
                UnityEngine.Debug.Log(frequencyTuple.Frequency + " Hz: " + frequencyProbabilities[f]);
            }
        }

        watch.Stop();
        UnityEngine.Debug.Log(watch.ElapsedMilliseconds);

        UnityEngine.Debug.Log("le altre celle sono inizializzate a " + frequencyProbabilities[0] + ", la somma è " + frequencyProbabilities.Sum());
        if (Convert.ToInt32(frequencyProbabilities.Sum()) == 100)           Assert.Pass();
        else                                                                Assert.Fail();
        // Test results: 90-120 ms
    }

    private double[] NewWaveform(double mean, double variance)
    {
        double[] gaussianSamples = new double[testSamples];
        GaussianDistribution.Samples(gaussianSamples, mean, Math.Sqrt(variance));
        UnityEngine.Debug.Log("SORTED");
        return gaussianSamples;
    }
}
