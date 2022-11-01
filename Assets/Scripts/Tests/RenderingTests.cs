using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
public class RenderingTests
{
    // A Test behaves as an ordinary method
    public List<ArraySegment<float>> GetChunksTest(float[] waveform) //private
    {
        var watch = Stopwatch.StartNew();

        int chunkStartIndex = -1, chunkEndIndex;
        bool inChunk = false;
        float min = waveform.Min();
        UnityEngine.Debug.Log("min: " + min);
        List<ArraySegment<float>> notMinValuesChunks = new();
        for (int i = 0; i < waveform.Length; i++)
        {
            if (waveform[i] == min)
            {
                if (inChunk)
                {
                    chunkEndIndex = i - 1;
                    ArraySegment<float> chunk = new(waveform, chunkStartIndex, chunkEndIndex - chunkStartIndex + 1);
                    watch.Stop();
                    UnityEngine.Debug.Log("found chunk after " + watch.ElapsedMilliseconds + " from index " + chunkStartIndex + " to " + chunkEndIndex);
                    watch.Start();
                    notMinValuesChunks.Add(chunk);
                    inChunk = false;
                }
                else continue;
            }
            else
            {
                if (!inChunk)
                {
                    
                    chunkStartIndex = i;
                    inChunk = true;
                }
                else continue;
            }
        }
        UnityEngine.Debug.Log("found " + notMinValuesChunks.Count + " chunks after " + watch.ElapsedMilliseconds);
        watch.Stop();
        UnityEngine.Debug.Log(watch.ElapsedMilliseconds);
        return notMinValuesChunks;
        // Test results:
    }
    [Test]
    public void GetChunksWrapperTest()
    {
        float[] waveform = new float[22000];
        Array.Clear(waveform, 0, waveform.Length);
        for (int i=0; i<waveform.Length; i+=11000) 
        {
            int rand = UnityEngine.Random.Range(0, 5000);
            for (int j = rand ; j < rand + 20; j++)
            {
                waveform[i + j] = UnityEngine.Random.Range(0.1f, 0.7f);
                UnityEngine.Debug.Log("Hz " + (i + j) + ": " + waveform[i + j]);
            }
        }
        List<ArraySegment<float>> result = GetChunksTest(waveform);
        if (result.Count == 2) Assert.Pass();
    }
}
