using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

// Frequency Modulation Synthesizer
public class AudioSynthFM : MonoBehaviour
{
    [Range(20, 4000)]  //Creates a slider in the inspector
    public float frequency; // main note frequency
    [Range(0, 20)]  
    public float carrierMultiplier; // carrier frequency = frequency * carrierMultiplier
    [Range(0, 20)]  
    public float modularMultiplier; // modular frequency = frequency * modularMultiplier
    public float sampleRate = 44100;

    [Range(0.1f, 2)]  //Creates a slider in the inspector
    public float amplitude;
    AudioSource audioSource;
    int timeIdx = 0;
    public float envelope;
    public GameObject monkey;
    float phase = 0;
    void Awake()
    {
        monkey = GameObject.Find("Colobus_LOD0");
    }
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.Stop(); //avoids audiosource from starting to play automatically
        frequency = 180; // init
        carrierMultiplier = 1.4f;
        modularMultiplier = 0.5f;
        amplitude = 0.2f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // trigger of sound synth
        {
            if (!audioSource.isPlaying)
            {
                timeIdx = 0;  //resets timer before playing sound
                audioSource.Play();
            }
        }
        // turn off the audio when the envelope is small enough.
        if (timeIdx > 1000 && Envelope(timeIdx) < 0.001)
        {
            audioSource.Stop();
            timeIdx = 0;
        }
        carrierMultiplier = Mathf.Exp(Input.GetAxis("Mouse X"));
        modularMultiplier = Mathf.Exp(Input.GetAxis("Mouse Y"));
            Debug.Log(carrierMultiplier);
        // Graphic Part: cube reacts to the audio
        monkey.transform.position = new Vector3(0, Envelope(timeIdx) * 5, 0);
        monkey.transform.localScale = new Vector3(modularMultiplier + Envelope(timeIdx) * 5, modularMultiplier + Envelope(timeIdx) * 5, carrierMultiplier + Envelope(timeIdx) * 6);
        monkey.transform.rotation = new Quaternion(carrierMultiplier * (1 + Envelope(timeIdx)), 90, carrierMultiplier * (1 + Envelope(timeIdx)), 0);
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            phase += 2 * Mathf.PI * frequency / sampleRate;
            data[i] = amplitude * Envelope(timeIdx) * FM(timeIdx, phase, carrierMultiplier, modularMultiplier);
            data[i + 1] = data[i];
            timeIdx++;
            if (phase >= 2 * Mathf.PI)
            {
                phase -= 2 * Mathf.PI;
            }
        }
    }
    // Compute frequency in angular frequency
    public float ComputeFreq(float frequency)
    {   // why? http://hplgit.github.io/primer.html/doc/pub/diffeq/._diffeq-solarized002.html#:~:text=Mathematically%2C%20the%20oscillations%20are%20described,means%2044100%20samples%20per%20second.
        return 2 * Mathf.PI * frequency / sampleRate; // e.g. 2*pi*440/44100
    }
    // Frequency Modulation computation
    public float FM(int timeIdx, float phase, float carMul, float modMul)
    {
        return Mathf.Sin((ComputeFreq(phase * carMul) * timeIdx + Mathf.Sin(ComputeFreq(phase * modMul) * timeIdx))); // fluctuating FM
    }
    public float Envelope(int timeIdx)
    {   // should have something looks like..: /\__
        // https://www.sciencedirect.com/topics/engineering/envelope-function
        float a = 0.13f;
        float b = 0.45f;
        float tempo = 1000f;// timeIdx is an integer increasing rapidly so calm down
        return Mathf.Abs(Mathf.Exp(-a * (timeIdx)/tempo) - Mathf.Exp(-b * (timeIdx) / tempo));
    } 
}