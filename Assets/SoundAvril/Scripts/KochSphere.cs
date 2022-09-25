using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class KochSphere : KochGenerator
{
    LineRenderer _lineRenderer;
    EdgeCollider2D _collider;
    public GenerationsManager _genManager;
    List<int> _listGen;
    protected int _savedGenerationCount;

    Sample _sample;
    public GameObject _sphere;
    public float _loopTime = 3;
    float _moveTime;
    public bool _isMoving = false;
    bool _stopMovement = false;
    public int _moveIndex;
    public int _startGenSavedSize;

    

    public bool _wake;
    public bool _awake;
    public float _generateMultiplier;
    [Range(0, 1)]
    public float _lerpAmount;
    public float _lerpIncr;
    Vector3[] _lerpPosition;
    public Material _material;
    public Color _color;
    private Material _matInstance;
    public float _emissionMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _collider = GetComponent<EdgeCollider2D>();
        _sample = GetComponent<Sample>();
        _lineRenderer.enabled = true;
        _collider.enabled = true;
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.loop = true;
        _lineRenderer.positionCount = _position.Length;
        _lineRenderer.SetPositions(_position);
        _startGenSavedSize = _startGen.Length;
        _lerpPosition = new Vector3[_position.Length];
        //List for collider
        _collider.SetPoints(_positionList);

        if (_awake)
        {
            _positionList = new List<Vector2>();
            for (int i = 0; i < _position.Length; i++)
            {
                _position[i] = _targetPosition[i];
                _positionList.Add(_targetPosition[i]);
            }
            _lineRenderer.SetPositions(_position);
            _collider.SetPoints(_positionList);
        }

        _moveIndex = 0;
        _moveTime = _loopTime / _position.Length;
        _sphere.transform.position = _position[_moveIndex];

        //apply material
        //_matInstance = new Material(_material);
        //_lineRenderer.material = _matInstance;


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("ici 2d");
    }

    public void AddGeneration()
    {
        StartGen[] newGens;
        if (_startGen.Length == 0)
        {
            newGens = new StartGen[1];
            newGens[0] = new StartGen
            {
                scale = 1,
                outwards = true
            };
        }
        else
        {
            newGens = new StartGen[_startGen.Length + 1];
            for (int i = 0; i < _startGen.Length; i++)
            {
                newGens[i] = _startGen[i];
            }
            newGens[_startGen.Length] = new StartGen
            {
                scale = newGens[_startGen.Length - 1].scale,
                outwards = newGens[_startGen.Length - 1].outwards
            };
        }
        
        _startGen = newGens;
    }

    public void RemoveGeneration()
    {
        StartGen[] newGens = new StartGen[_startGen.Length - 1];
        for (int i = 0; i < _startGen.Length - 1; i++)
        {
            newGens[i] = _startGen[i];
        }
        _startGen = newGens;
    }
    IEnumerator Wake()
    {
        while (_lerpAmount < 1)
        {
            _lerpAmount += _lerpIncr * Time.deltaTime;
            for (int i = 0; i < _position.Length; i++)
            {
                _lerpPosition[i] = Vector2.Lerp(_position[i], _targetPosition[i], _lerpAmount);
            }
            _lineRenderer.positionCount = _position.Length;
            _lineRenderer.SetPositions(_lerpPosition);
            SetPositionList();

            _positionList = new List<Vector2>();
            for (int i = 0; i < _lerpPosition.Length; i++)
            {
                _positionList.Add(_lerpPosition[i]);
            }
            _collider.SetPoints(_positionList);
            yield return new WaitForEndOfFrame();
        }

    }

    IEnumerator Move(Vector2 pPosition, float pTime)
    {
        //move in pTime second
        _isMoving = true;
        float chrono = 0;
        Vector2 startPosition = _sphere.transform.position;
        while(!_sphere.transform.position.Equals(pPosition))
        {
            if (_stopMovement)
            {
                _isMoving = false;
                _stopMovement = false;
                break;
            }
            chrono += Time.deltaTime / pTime;
            _sphere.transform.position = Vector2.Lerp(startPosition, pPosition, chrono);
            //_sample.setFrequency(_sample._startFreq + _sphere.transform.position.y * _sphere.transform.position.x);
            yield return new WaitForEndOfFrame();
        }
        _isMoving = false;
    }
    // Update is called once per frame
    void Update()
    {
        DrawGenerator();
        _position = _targetPosition;
        _lineRenderer.positionCount = _position.Length;
        _lineRenderer.SetPositions(_position);
        if (_startGen.Length != _savedGenerationCount)
        {
            _moveIndex = 0;
            _stopMovement = true;
            _listGen = new List<int>();
            for (int i = 0; i < _startGen.Length; i++)
            {
                _listGen.Add(i);
            }
            _genManager.AddItem(_listGen);
            _savedGenerationCount = _startGen.Length;
        }
        

        if (!_isMoving)
        {
            if (_moveIndex < _position.Length - 1)
            {
                _moveIndex++;
            } else if (_moveIndex == _position.Length - 1)
            {
                _moveIndex = 0;
            }
            _sample.readSound(_sample._startFreq + _position[_moveIndex].y * _position[_moveIndex].x);
            _moveTime = _loopTime / _position.Length;
            StartCoroutine(Move(_position[_moveIndex], _moveTime));
        }
        //_matInstance.SetColor("_EmissionColor",_color * AudioPeer._audioBandBuffer[_audioBandMaterial] * _emissionMultiplier);
        if (_generationCount != 0)
        {

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
