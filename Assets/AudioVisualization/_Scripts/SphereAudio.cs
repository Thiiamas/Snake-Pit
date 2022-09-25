using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereAudio : MonoBehaviour
{
    public float _starScale, _maxScale;
    public bool _useBuffer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_useBuffer)
        {
            transform.localScale = new Vector3((AudioPeer._amplitudeBuffer * _maxScale) + _starScale, (AudioPeer._amplitudeBuffer * _maxScale) + _starScale, (AudioPeer._amplitudeBuffer * _maxScale) + _starScale);   
        }
        if (!_useBuffer)
        {
            transform.localScale = new Vector3((AudioPeer._amplitude * _maxScale) + _starScale, (AudioPeer._amplitude * _maxScale) + _starScale, (AudioPeer._amplitude * _maxScale) + _starScale);
        }
    }
}
