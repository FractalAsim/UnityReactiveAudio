using UnityEngine;

public class ReactiveAudio_ShaderTest : MonoBehaviour
{
    public AudioSampler audioSampler;
    Renderer[] rends;

    public Texture2D patterntex;

    Texture2D soundbuffertex;

    void Start ()
    {
        rends = FindObjectsOfType<Renderer>();
        foreach (Renderer r in rends)
        {
            var patterntex = r.material.mainTexture;
            r.material.SetTexture("_PatternTex", patterntex);
        }
    }
	
	void Update ()
    {
        soundbuffertex = AudioSampler.BufferToTexture(audioSampler.AudioBandbuffer);
        foreach (Renderer r in rends)
        {
            r.material.SetTexture("_SoundBufferTex", soundbuffertex);
        }
    }
}
