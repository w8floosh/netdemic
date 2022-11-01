using System;
using System.Linq;
using System.Security;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.U2D;
using static SimulationManager;
using static SimulationVisualizer;
using GaussianDistribution = MathNet.Numerics.Distributions.Normal;

// usare le gaussiane come timbri. usare gaussiane con campana stretta perché così evidenziano la frequenza fondamentale. media = freq fondamentale, varianza = influenza della fondamentale sulle vicine
// 1) creo gaussiana sulla base di media e varianza
// 2) faccio array di intensità frequenze (22000) per l'archetipo e per i nodi normali sommo un delta a ogni cella

[Serializable]
public class Waveform : MonoBehaviour
{
    //private float[] _spectrum;                                                   // array di NumberOfFrequencies elementi in cui ogni cella indica quante volte la frequenza di indice i è presente
    private double[] _spectrum;                                                    // rappresenta lo spettro della waveform
    [Range(0.1f, 2f)] public float NoiseGenerationRange;

    private void GenerateFromExistingWaveform(Waveform baseWave)
    {
        Array.Copy(baseWave._spectrum, _spectrum, baseWave._spectrum.Length);    // duplica la wave base inserendola nel nuovo oggetto
        double noise = UnityEngine.Random.Range(-NoiseGenerationRange / 2, NoiseGenerationRange / 2);
        for (int f = 0; f < NumberOfFrequencies; f++)
        {
            //if (_spectrum[f] != 0f) 
            _spectrum[f] = Math.Clamp(_spectrum[f] + noise, 0d, 100d);
            //_spectrum[f] = Mathf.Clamp(_spectrum[f] + noise, 0f, 100f);
        }
    }

    private double[] GenerateNewWaveform(double mean, double variance)
    {
        double[] samples = SampleFromGaussianDistribution(mean, variance);
        return GenerateProbabilityDensityFunction(samples);
    }

    private double[] GenerateProbabilityDensityFunction(double[] gaussianSamples)
    {
        int[] integerSamples = new int[NumberOfFrequencies];
        long mean = 0;
        for (int i = 0; i < NumberOfFrequencies; i++)
        {
            integerSamples[i] = (int)Math.Floor(gaussianSamples[i]);
            mean += integerSamples[i];
        }
        mean /= NumberOfFrequencies;
        double[] frequencyProbabilities = new double[NumberOfFrequencies];
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
                //frequencyProbabilities[f] = Math.Round((double)frequencyTuple.Count / NumberOfFrequencies * 100, 6);
                frequencyProbabilities[f] = (double)frequencyTuple.Count / NumberOfFrequencies * 100d;
                if (frequencyProbabilities[f] < 0.005d) frequencyProbabilities[f] = 0;
                //Debug.Log(frequencyTuple.Frequency + " Hz: " + frequencyProbabilities[f]);

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

    //private void GenerateFromExistingWaveform(Waveform baseWave)
    //{
    //    Array.Copy(baseWave._spectrum, _spectrum, baseWave._spectrum.Length);    // duplica la wave base inserendola nel nuovo oggetto
    //    float noise = UnityEngine.Random.Range(-NoiseGenerationRange / 2, NoiseGenerationRange / 2);
    //    for (int f = 0; f < NumberOfFrequencies; f++)
    //    {
    //        //if (_spectrum[f] != 0f) 
    //        _spectrum[f] = Mathf.Clamp(_spectrum[f] + noise, 0f, 100f);
    //    }
    //}

    //private float[] GenerateNewWaveform(double mean, double variance)
    //{
    //    double[] samples = SampleFromGaussianDistribution(mean, variance);
    //    return GenerateProbabilityDensityFunction(samples);
    //}

    //private float[] GenerateProbabilityDensityFunction(double[] gaussianSamples)
    //{
    //    int[] integerSamples = new int[NumberOfFrequencies];
    //    long mean = 0;
    //    for (int i = 0; i < NumberOfFrequencies; i++)
    //    {
    //        integerSamples[i] = (int)Math.Floor(gaussianSamples[i]);
    //        mean += integerSamples[i];
    //    }
    //    mean /= NumberOfFrequencies;
    //    float[] frequencyProbabilities = new float[NumberOfFrequencies];
    //    var counters = from frequency in integerSamples
    //                   group frequency by frequency into frequencyCounter
    //                   let count = frequencyCounter.Count()
    //                   let freq = frequencyCounter.Key
    //                   orderby freq ascending
    //                   select new { Frequency = frequencyCounter.Key, Count = count };

    //    for (int f = counters.First().Frequency - 1; f < counters.Last().Frequency; f++)
    //    {
    //        var frequencyTuple = counters.FirstOrDefault(c => c.Frequency == f);
    //        if (frequencyTuple != null)
    //        {
    //            frequencyProbabilities[f] = (float)Math.Round((float)frequencyTuple.Count / NumberOfFrequencies * 100, 4);
    //            //Debug.Log(frequencyTuple.Frequency + " Hz: " + frequencyProbabilities[f]);

    //        }
    //    }
    //    return frequencyProbabilities;
    //}

    //private double[] SampleFromGaussianDistribution(double mean, double variance)                   // genera una gaussiana partendo da media e varianza per poi campionare x valori da essa
    //{
    //    double[] gaussianSamples = new double[NumberOfFrequencies];
    //    GaussianDistribution.Samples(gaussianSamples, mean, Math.Sqrt(variance));                   // campiono 22000 valori double dalla gaussiana                                                          // li converto in interi arrotondandoli all'intero più vicino
    //    return gaussianSamples;
    //}

    public double[] GetSpectrum(GameObject caller)
    {
        if (caller.GetComponent<Region>() != null || caller.GetComponent<SimulationVisualizer>() != null || caller.GetComponent<Node>().Tone == this) 
            return _spectrum;
        else throw new SecurityException("Attempted security violation: waveform spectrum access restricted to the owner node and its proper region only");
    }
    public void SetupWaveform(Region regionData)
    {
        double mean, variance;
        if (regionData == null)                                                 // se è vero sto generando l'archetipo del virus
        {
            mean = UnityEngine.Random.Range(0, NumberOfFrequencies);
            variance = UnityEngine.Random.Range(200f, 600f);
            Array.Copy(GenerateNewWaveform(mean, variance), _spectrum, NumberOfFrequencies);
            return;
        }                                                 
        if (regionData.RegionWaveform != null)                                  // genera una waveform sulla base di un archetipo
        {
            GenerateFromExistingWaveform(regionData.RegionWaveform);            // introduce il rumore di generazione nella nuova waveform
        }
        else                                                                    // se no genera un nuovo archetipo vuoto e riempilo
        {
            // devo generare una media tale che il cerchio della regione che costruisco
            // (con centro (media, y)) non intersechi con altre regioni
            // i possibili centri sono fissati in base al numero di regioni
            // splitFactor serve a rendere equidistanti le medie delle gaussiane delle varie regioni
            float splitFactor = SimulationManagerInstance.ActiveRegions / (float)(SimulationManagerInstance.NumberOfRegions+1);
            mean = Mathf.Lerp(0, NumberOfFrequencies, splitFactor);
            //Debug.Log("active regions: " + SimulationManagerInstance.ActiveRegions + " split factor: " + splitFactor + " media: " + mean);


            // la media distingue bene gli archetipi in base al numero di regioni presenti

            variance = UnityEngine.Random.Range(200f, 600f);                                                      // la varianza è di massimo 50*3 frequenze in quanto vogliamo una curva molto appuntita per definire bene l'archetipo
            Array.Copy(GenerateNewWaveform(mean, variance), _spectrum, NumberOfFrequencies);                 // crea un nuovo spettro e lo inserisce dentro l'oggetto
        }
        //Debug.Log("Waveform set up successfully");
    }

    public static void InterpolateWaveforms(Waveform source, Waveform toEdit, byte step) {
        bool changed = false;
        Node victim = toEdit.gameObject.GetComponent<Node>();
        for (int i=0; i<source._spectrum.Length; i++)
        {
            if (source._spectrum[i] != victim.Tone._spectrum[i])
            {
                changed = true;
                victim.Tone._spectrum[i] = Lerp(toEdit._spectrum[i], source._spectrum[i], step/(float)victim.ResistanceLimit);
            }
        }
        if (changed && SimulationVisualizerInstance.RegionPanel.activeInHierarchy && victim.RegionData == SimulationVisualizerInstance.VisualizingRegion.GetComponent<Region>())
        {
            SimulationVisualizerInstance.OnShowRegionInfo.Invoke(victim.RegionData);
        }
        if (changed && SimulationVisualizerInstance.NodePanel.activeInHierarchy)
        {
            SimulationVisualizerInstance.UpdateWaveformGraphicsAsync(true);
        }
    }

    public (int frequency, double probability) GetFundamental()
    {
        int frequency = Array.IndexOf(_spectrum, _spectrum.Max());
        (int frequency, double probability) fundamental = (frequency, _spectrum[frequency]);
        return fundamental;
    }

    //public Waveform Interpolate(Waveform victim, Waveform attacker, byte step) { }
    private void Awake()
    {
        _spectrum = new double[NumberOfFrequencies];
        NoiseGenerationRange = UnityEngine.Random.Range(0.1f, 2f);

        //  Debug.Log("created Waveform component");
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
