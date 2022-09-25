using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    AudioSource _audioSource;
    public static float[] _samples = new float[512];
    public static float[]  _freqBand = new float[8];
    public static float[] _bandBuffer = new float[8];
    private float[] _bufferDecrease = new float[8];

    public float[] _freqBandHighest = new float[8];
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];

    public static float _amplitude, _amplitudeBuffer;
    float _amplitudeHighest;
    public float _audioProfile;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioProfile(_audioProfile);
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBand();
        BandBuffer();
        CreateAudioBands();
        GetAmplitude();
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < 8; i++)
        {
            _freqBandHighest[i] = audioProfile;
        }
    }
    void GetAmplitude()
    {
        float currentAmplitude = 0;
        float currentAmplitudeBuffer = 0;

        for (int i = 0; i < 8; i++)
        {
            currentAmplitude += _audioBand[i];
            currentAmplitudeBuffer += _audioBandBuffer[i];
        }
        if (currentAmplitude > _amplitudeHighest)
        {
            _amplitudeHighest = currentAmplitude;
        }
        _amplitude = currentAmplitude / _amplitudeHighest;
        _amplitudeBuffer = currentAmplitudeBuffer / _amplitudeHighest;
    }

    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBand[i];
            }
            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }

    void BandBuffer()
    {
        for (int g = 0; g < 8; ++g)
        {
            if (_freqBand[g] > _bandBuffer[g])
            {
                _bandBuffer[g] = _freqBand[g];
                _bufferDecrease[g] = 0.005f;
            }
            if (_freqBand[g] < _bandBuffer[g])
            {
                _bandBuffer[g] -= _bufferDecrease[g];
                _bufferDecrease[g] *= 1.05f;
            }
        }
    }

    void MakeFrequencyBand()
    {
        /*
         * 22050 / 512 = 43 Hz per sample
         * 
         * 20 - 60 Hz
         * 60 - 250 Hz
         * 250 - 500 Hz
         * 500 - 2000 Hz
         * 2000 - 4000 Hz
         * 4000 - 60000 Hz
         * 6000 - 20000 Hz
         * 
         * sample per band :
         * 0 - 2 sample = 86 Hz
         * 1 - 4 = 172 Hz => 87 - 258 Hz
         * 2 - 8 = 344 Hz => 259 - 602 Hz
         * 3 - 16
         * 4 - 32
         * 5 - 64 
         * 6 - 128
         * 7 - 256
         *  => 510 sample
         */
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                //to arrive to 512
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= count;
            _freqBand[i] = average * 10;
        }
    }
}
