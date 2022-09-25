using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class KochLineAudio : KochGenerator
{
    LineRenderer _lineRenderer;
    EdgeCollider2D _collider;
    [Range(0, 1)]
    public float _lerpAmount;
    Vector3[] _lerpPosition;
    List<Vector2> _lerpPositionList;
    public float _generateMultiplier;
    private float[] _lerpAudio;

    [Header("audio")]
    public AudioPeer _audioPeer;
    public int[] _audioBand;
    public Material _material;
    public Color _color;
    private Material _matInstance;
    public int _audioBandMaterial;
    public float _emissionMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<EdgeCollider2D>();
        if (_collider)
        {
            _collider.enabled = true;
        }
        _lerpAudio = new float[_initiatorPointAmount];
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = true;
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.loop = true;
        _lineRenderer.positionCount = _position.Length;
        _lineRenderer.SetPositions(_position);
        _lerpPosition = new Vector3[_position.Length];
        //List for collider
        _lerpPositionList = new List<Vector2>();
        //apply material
        _matInstance = new Material(_material);
        //_lineRenderer.material = _matInstance;

    }

    void setPoints()
    {
        _collider.SetPoints(_lerpPositionList);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("ici 2d");
    }

    // Update is called once per frame
    void Update()
    {
        //_matInstance.SetColor("_EmissionColor",_color * AudioPeer._audioBandBuffer[_audioBandMaterial] * _emissionMultiplier);
        if (_generationCount != 0)
        {
            int count = 0;
            //_lerpPositionList.Clear();
            for (int i = 0; i < _initiatorPointAmount; i++)
            {
                _lerpAudio[i] = AudioPeer._audioBandBuffer[_audioBand[i]];
                for (int j = 0; j < (_position.Length - 1) / _initiatorPointAmount; j++)
                {
                    _lerpPosition[count] = Vector3.Lerp(_position[count], _targetPosition[count], _lerpAudio[i]);
                    //_lerpPositionList.Add(_lerpPosition[count]);
                    count++;
                }
            }
            _lerpPosition[count] = Vector3.Lerp(_position[count], _targetPosition[count], _lerpAudio[_initiatorPointAmount - 1]);
            //setPoints();
            //for(int i = 0; i < _position.Length; i++)
            //{
            //    //change lerp arcording to _lerpAmount
            //    //_lerpPosition[i] = Vector3.Lerp(_position[i], _targetPosition[i], _lerpAmount);
            //    _lerpPosition[i] = Vector3.Lerp(_position[i], _targetPosition[i], AudioPeer._audioBandBuffer[_audioBand]);
            //}
            if (_useBezierCurves)
            {
                _bezierPosition = BezierCurve(_lerpPosition, _bezierVertexCount);
                _lineRenderer.positionCount = _bezierPosition.Length;
                _lineRenderer.SetPositions(_bezierPosition);
            }
            else
            {
                _lineRenderer.positionCount = _lerpPosition.Length;
                _lineRenderer.SetPositions(_lerpPosition);
            }
        }

        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    Debug.Log("O");
        //    KochGenerate(_targetPosition, true, _generateMultiplier);
        //    _lerpPosition = new Vector3[_position.Length];
        //    _lineRenderer.positionCount = _position.Length;
        //    _lineRenderer.SetPositions(_position);
        //    _lerpAmount = 0;
        //}
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    Debug.Log("i");
        //    KochGenerate(_targetPosition, false, _generateMultiplier);
        //    _lerpPosition = new Vector3[_position.Length];
        //    _lineRenderer.positionCount = _position.Length;
        //    _lineRenderer.SetPositions(_position);
        //    _lerpAmount = 0;
        //}

    }

}
