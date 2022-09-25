using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sample : MonoBehaviour
{
    public enum _wave
    {
        Sin,
        Square,
        SawTooth
    }
    AudioSource aud;
    public _wave wave = new _wave();
    public int position = 0;
    public int samplerate = 44100;
    public float _startFreq;
    public float frequency = 440f;

    public void setFrequency(float pFreq)
    {
        frequency = pFreq;
    }
    // Start is called before the first frame update
    void Start()
    {
        _startFreq = frequency;
        AudioClip myClip = AudioClip.Create("MySinusoid" + Random.Range(0,10), samplerate * 2, 1, samplerate, true, OnAudioRead);
        aud = GetComponent<AudioSource>();
        aud.clip = myClip;
        aud.loop = true;
        //aud.Play();
    }

    public void readSound(float pFreq)
    {
        //aud.Stop();
        frequency = pFreq;
        AudioClip myClip = AudioClip.Create("MySinusoid" + Random.Range(0, 10), samplerate * 2, 1, samplerate, true, OnAudioRead,OnAudioSetPosition);
        aud.clip = myClip;
        aud.loop = true;
        aud.Play();
    }

    void OnAudioRead(float[] data)
    {
        int count = 0;
        while (count < data.Length)
        {
            switch (wave)
            {
                case _wave.Sin:
                    data[count] = Mathf.Sin(2 * Mathf.PI * frequency * position / samplerate);
                    break;
                case _wave.Square:
                    data[count] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * frequency * position / samplerate)) *0.5f;
                    break;
                case _wave.SawTooth:
                    data[count] = (Mathf.PingPong(frequency * position / samplerate, 0.5f));
                    break;
            }
            position++;
            count++;
            if (position == 2 * samplerate)
            {
                Debug.Log("loop");
            }
        }
    }

    void OnAudioSetPosition(int newPosition)
    {
        position = newPosition;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
