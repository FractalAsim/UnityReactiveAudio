using UnityEngine;

public class AudioSampler : MonoBehaviour
{
    public AudioSource audioSource;
    public FFTWindow sampleAlgo = FFTWindow.BlackmanHarris;

    int NumAudioSamples;

    [Range(5,12)]//minmaxcap
    public int TotalAudioBands = 8;
    public float AudioBandsDecreaseRate = 0.0001f;
    public float AmpScale = 1;


    public float[] AudioSamples;
    float[] freqBands;
    public float[] AudioBandbuffer;
    float[] freqBandsbufferdecrease;

    void Start()
    {
        NumAudioSamples = (int)Mathf.Pow(2, TotalAudioBands + 1);
        AudioSamples = new float[NumAudioSamples];

        //Audiobands
        freqBands = new float[TotalAudioBands];
        AudioBandbuffer = new float[TotalAudioBands];
        freqBandsbufferdecrease = new float[TotalAudioBands];
    }   
    void Update ()
    {
        if (audioSource)
        {
            audioSource.GetSpectrumData(AudioSamples, 0, sampleAlgo);
        }
        ApplyAudioBands(AudioSamples);
    }

    void ApplyAudioBands(float[] audioFFTSamples)
    {
        int samplecounter = 0;
        for (int i = 0; i < TotalAudioBands; i++)
        {
            float average = 0;
            int targetsamplecountforfreqband = (int)Mathf.Pow(2, i + 1);

            for (int j = 0; j < targetsamplecountforfreqband; j++)
            {
                average += audioFFTSamples[samplecounter] * (samplecounter + 1); // fancy way to get avg
                samplecounter++;
            }
            average /= samplecounter; // fancy way to get avg

            freqBands[i] = average * AmpScale; // just to increase the value a bit
        }

        for (int k = 0; k < TotalAudioBands; k++)
        {
            if (freqBands[k] > AudioBandbuffer[k])
            {
                AudioBandbuffer[k] = freqBands[k];
                freqBandsbufferdecrease[k] = AudioBandsDecreaseRate;
            }
            else if (freqBands[k] < AudioBandbuffer[k])
            {
                AudioBandbuffer[k] -= freqBandsbufferdecrease[k];//decrease buffer
                freqBandsbufferdecrease[k] *= 1.2f;
            }
        }
    }
    public static Texture2D BufferToTexture(float[] buffer)
    {
        Texture2D tex = new Texture2D(buffer.Length, 1, TextureFormat.RGBA32, false);
        for (int i = 0; i < buffer.Length; i++)
        {
            tex.SetPixel(i, 1, new Color((buffer[i]), 0, 0, 0));
        }
        tex.Apply();
        return tex;
    }
}
