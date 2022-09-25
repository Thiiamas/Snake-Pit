using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class KochLine : KochGenerator
{
    LineRenderer _lineRenderer;
    [Range(0,1)]
    public float _lerpAmount;
    Vector3[] _lerpPosition;
    List<Vector2> _lerpPositionList;
    public float _generateMultiplier;

    public Material _material;
    public Color _color;
    private Material _matInstance;
    public float _emissionMultiplier;
    // Start is called before the first frame update
    void Start()
    {
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
        //_matInstance = new Material(_material);
        //_lineRenderer.material = _matInstance;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("ici 2d");
    }

    // Update is called once per frame
    void Update()
    {
        DrawGenerator();
        //_matInstance.SetColor("_EmissionColor",_color * AudioPeer._audioBandBuffer[_audioBandMaterial] * _emissionMultiplier);
        if (_generationCount != 0)
        {
            _lerpPosition = new Vector3[_position.Length];
            for (int i = 0; i < _position.Length; i++)
            {
                //change lerp arcording to _lerpAmount
                _lerpPosition[i] = Vector3.Lerp(_position[i], _targetPosition[i], _lerpAmount);
            }
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
