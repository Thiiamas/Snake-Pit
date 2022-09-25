using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiate256Cubes : MonoBehaviour
{
    public GameObject _sampleCubePrefab;
    public float _maxScale;
    GameObject[] _sampleCube = new GameObject[512];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 256; i++)
        {
            GameObject _instanceSampleCube = (GameObject)Instantiate(_sampleCubePrefab);
            _instanceSampleCube.transform.position = this.transform.position;
            _instanceSampleCube.transform.parent = transform;
            _instanceSampleCube.name = "SampleCube" + i;
            this.transform.eulerAngles = new Vector3(0, -0.703125f * i, 0);
            _instanceSampleCube.transform.position = Vector3.forward * 100;
            _sampleCube[i] = _instanceSampleCube;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i =0; i < _sampleCube.Length; i++)
        {
            if (_sampleCube != null)
            {
                _sampleCube[i].transform.localScale = new Vector3(10, (AudioPeer._samples[i] * _maxScale) + 2, 10);
            }
        }
    }
}
