using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static SimulationManager;
using GaussianDistribution = MathNet.Numerics.Distribution.Normal;

public class Waveform : MonoBehaviour
{
    public int[] SamplesArray { get; private set; }

    public const byte NoiseGenerationRange = 255;

    public Waveform(Region region = null)
    {
        if (region != null)                                                                                 // genera una waveform sulla base di un archetipo
        {
            GenerateFromExistingWaveform(region.RegionWaveform);                                            // introduce il rumore di generazione nella nuova waveform
        }
        else
        {
            SamplesArray = new int[SampleRatePerSecond];                                                    // genera un nuovo archetipo vuoto
        }
    }

    private void GenerateFromExistingWaveform(Waveform baseWave)
    {
        Waveform newWaveform = new();
        Array.Copy(baseWave.SamplesArray, newWaveform.SamplesArray, baseWave.SamplesArray.Length); // duplica la wave base in un nuovo oggetto e lavoro sulla copia
        foreach (int s in newWaveform.SamplesArray)
        {
            newWaveform.SamplesArray.SetValue(
                newWaveform.SamplesArray[s] + UnityEngine.Random.Range(-NoiseGenerationRange / 2, (NoiseGenerationRange + 1) / 2), s);
        }
        Array.Copy(newWaveform.SamplesArray, this.SamplesArray, SampleRatePerSecond);

    }



    public static int[] GaussianDistributor(double mean, double variance)
    {
        double[] GaussianSamplesNormalized = new double[SampleRatePerSecond];
        int[] GaussianSamples = new int[SampleRatePerSecond];
        GaussianDistribution.Samples(GaussianSamplesNormalized, mean, variance); // crea 44100 valori nel range (0,1) dalla distribuzione gaussiana e li mette nell'array di double 
        for (int s = 0; s < SampleRatePerSecond; s++)
        {
            GaussianSamples[s] = (short)Math.Round(GaussianSamplesNormalized[s] * (double)ushort.MaxValue) - short.MaxValue; // denormalizza dal range (0,1) al range (-32768, 32767)
        }
        return GaussianSamples; // ritorna l'array dei sample interi per poi copiarli a riga 30 nell'archetipo della regione
    }

    public void DisplayWaveform() { }

    public Waveform Interpolate(Waveform victim, Waveform attacker, byte step) { }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
