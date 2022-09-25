using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCubes : MonoBehaviour
{
    public GameObject _sampleCubePrefab;
    public float _maxScale;
    public int _size;
    public float _radius;
    public bool _turn;
    GameObject[] _sampleCube;


    // Start is called before the first frame update
    void Start()
    {
        _sampleCube = new GameObject[_size];
        for (int i = 0; i < _size; i++)
        {
            GameObject _instanceSampleCube = (GameObject)Instantiate(_sampleCubePrefab);
            _instanceSampleCube.transform.position = this.transform.position;
            _instanceSampleCube.transform.parent = transform;
            _instanceSampleCube.name = "SampleCube" + i;
            float angle = (360f / (float)_size) * (float)i;
            print("angle = " + angle);
            this.transform.eulerAngles = new Vector3(0, angle, 0);
            _instanceSampleCube.transform.position = Vector3.forward * _radius;
            _sampleCube[i] = _instanceSampleCube;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _sampleCube.Length; i++)
        {
            if (_sampleCube != null)
            {
                _sampleCube[i].transform.localScale = new Vector3(10, (AudioPeer._samples[i] * _maxScale) + 2, 10);
            }
        }

        if (_turn)
        {
            this.transform.Rotate(new Vector3(0, 0.15f, 0));
        }
    }
}
