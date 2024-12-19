using UnityEngine;

/// <summary>
/// Spawns Gameobject in a circle and scales GameObject based on Audio Sampler
/// </summary>
public class ReactiveAudio_GOWaveformScaleTest : MonoBehaviour
{
    public float SpawnRadius = 10;
    [Range(10,360)] public int SegmentNum = 360;

    [SerializeField] AudioSampler audioSampler;

    GameObject[] gos;
    Vector3 center;

    void Start ()
    {
        center = transform.position;

        Spawn();
    }

    void Spawn()
    {
        var diameter = 2 * SpawnRadius;
        var circumference = Mathf.PI * diameter;
        gos = new GameObject[SegmentNum];
        for (var i = 0; i < SegmentNum; i++)
        {
            gos[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gos[i].transform.position = new Vector3(center.x + SpawnRadius * Mathf.Cos(i * (360f / SegmentNum) * Mathf.Deg2Rad), center.y, center.z + SpawnRadius * Mathf.Sin(i * (360f / SegmentNum) * Mathf.Deg2Rad));
            gos[i].transform.localScale = new Vector3(circumference / SegmentNum, 0.2f, 0.2f);
            gos[i].transform.LookAt(center);
        }
    }
	
	void Update ()
    {
        SetScales();
    }

    void SetScales()
    {
        if (!audioSampler) return;

        for (int i = 0; i < SegmentNum && i < audioSampler.AudioSamples.Length; i++)
        {
            Vector3 currentscale = gos[i].transform.localScale;
            Vector3 currentpos = gos[i].transform.position;
            
            gos[i].transform.position = new Vector3(currentpos.x, center.y + audioSampler.AudioSamples[i] * 100 / 2, currentpos.z);
            gos[i].transform.localScale = new Vector3(currentscale.x, 0.2f + audioSampler.AudioSamples[i] * 100, currentscale.z);
        }
    }
}