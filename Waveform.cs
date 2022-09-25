using System;
using System.Linq;
using System.Security;
using UnityEngine;
using static SimulationManager;
using GaussianDistribution = MathNet.Numerics.Distributions.Normal;



// usare le gaussiane come timbri. usare gaussiane con campana stretta perché così evidenziano la frequenza fondamentale. media = freq fondamentale, varianza = influenza della fondamentale sulle vicine
// 1) creo gaussiana sulla base di media e varianza
// 2) faccio array di intensità frequenze (22000) per l'archetipo e per i nodi normali sommo un delta a ogni cella

[Serializable]
public class Waveform : MonoBehaviour
{
    private float[] _spectrum;                                                   // array di NumberOfFrequencies elementi in cui ogni cella indica quante volte la frequenza di indice i è presente
                                                                                 // rappresenta lo spettro della waveform
    [Range(0.1f, 2f)] public float NoiseGenerationRange;

    public Waveform(Region regionData = null)
    {
        if (regionData.RegionWaveform != null)                                                                          // genera una waveform sulla base di un archetipo
        {
            GenerateFromExistingWaveform(regionData.RegionWaveform);                                                    // introduce il rumore di generazione nella nuova waveform
        }
        else                                                                                                            // se no genera un nuovo archetipo vuoto e riempilo
        {
            double mean = NumberOfFrequencies * (regionData.RegionID / NumberOfRegions);                                // la media distingue bene gli archetipi in base al numero di regioni presenti
            double variance = UnityEngine.Random.Range(5f, 50f);                                                        // la varianza è di massimo 50 frequenze in quanto vogliamo una curva molto appuntita per definire bene l'archetipo
            Array.Copy(GenerateNewWaveform(mean, Math.Sqrt(variance)), _spectrum, NumberOfFrequencies);                 // crea un nuovo spettro e lo inserisce dentro l'oggetto
        }
    }

    private void GenerateFromExistingWaveform(Waveform baseWave)
    {
        Array.Copy(baseWave._spectrum, _spectrum, baseWave._spectrum.Length);    // duplica la wave base inserendola nel nuovo oggetto
        for (int f = 0; f < NumberOfFrequencies; f++)
        {
            _spectrum[f] += UnityEngine.Random.Range(-NoiseGenerationRange / 2, NoiseGenerationRange / 2);
        }
    }

    private float[] GenerateNewWaveform(double mean, double variance)
    {
        double[] samples = SampleFromGaussianDistribution(mean, variance);
        return GenerateProbabilityDensityFunction(samples);
    }

    private float[] GenerateProbabilityDensityFunction(double[] gaussianSamples)
    {
        int[] integerSamples = new int[NumberOfFrequencies];
        long mean = 0;
        for (int i = 0; i < NumberOfFrequencies; i++)
        {
            integerSamples[i] = (int)Math.Floor(gaussianSamples[i]);
            mean += integerSamples[i];
        }
        mean /= NumberOfFrequencies;
        float[] frequencyProbabilities = new float[NumberOfFrequencies];
        var counters = from frequency in integerSamples
                       group frequency by frequency into frequencyCounter
                       let count = frequencyCounter.Count()
                       let freq = frequencyCounter.Key
                       orderby freq ascending
                       select new { Frequency = frequencyCounter.Key, Count = count };

        for (int f = counters.First().Frequency - 1; f < counters.Last().Frequency; f++)
        {
            var frequencyTuple = counters.FirstOrDefault(c => c.Frequency == f);
            if (frequencyTuple != null)
            {
                frequencyProbabilities[f] = (float)Math.Round((float)frequencyTuple.Count / NumberOfFrequencies * 100, 4);
                UnityEngine.Debug.Log(frequencyTuple.Frequency + " Hz: " + frequencyProbabilities[f]);
            }
        }
        return frequencyProbabilities;
    }

    private double[] SampleFromGaussianDistribution(double mean, double variance)                   // genera una gaussiana partendo da media e varianza per poi campionare x valori da essa
    {
        double[] gaussianSamples = new double[NumberOfFrequencies];
        GaussianDistribution.Samples(gaussianSamples, mean, Math.Sqrt(variance));                   // campiono 22000 valori double dalla gaussiana                                                          // li converto in interi arrotondandoli all'intero più vicino
        return gaussianSamples;
    }

    public float[] GetRegionWaveformSpectrum(GameObject caller)
    {
        if (caller.GetComponent<Region>() != null || caller.GetComponent<Node>().Tone == this) return _spectrum;
        else throw new SecurityException("Attempted security violation: waveform spectrum access restricted to the owner node and its proper region only");
    }
    public void DisplayWaveform() { }

    //public Waveform Interpolate(Waveform victim, Waveform attacker, byte step) { }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
