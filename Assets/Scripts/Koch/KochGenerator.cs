using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KochGenerator : MonoBehaviour
{
    public bool _refresh;
    protected enum _axis
    {
        XAxis,
        YAxis,
        ZAxis
    }
    [SerializeField]
    protected _axis axis = new _axis();

    protected enum _initiator
    {
        Triangle,
        Square,
        Pentagon,
        Hexagon,
        Heptagon,
        Octagon
    }
    [SerializeField]
    protected _initiator initiator = new _initiator();

    public struct LineSegment
    {
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
        public Vector3 Direction { get; set; }
        public float Length { get; set; }
    }

    [SerializeField]
    protected AnimationCurve _generator;
    protected Keyframe[] _keys;

    [System.Serializable]
    public struct StartGen
    {
        public bool outwards;
        public float scale;
    }

    public StartGen[] _startGen;

    [SerializeField]
    protected bool _useBezierCurves;
    [SerializeField]
    [Range(8,24)]
    protected int _bezierVertexCount;

    protected int _generationCount;
    protected int _initiatorPointAmount;
    private Vector3[] _initiatorPoints;
    private Vector3 _rotateVector;
    private Vector3 _rotateAxis;
    protected float _initialRotation;
    [SerializeField]
    protected float _initiatorSize;

    protected Vector3[] _position;
    protected List<Vector2> _positionList;
    protected Vector3[] _targetPosition;
    protected Vector3[] _bezierPosition;
    private List<LineSegment> _lineSegments;

    protected Vector3[] BezierCurve(Vector3[] points, int vertexCount)
    {
        var pointList = new List<Vector3>();
        for(int i = 0; i < points.Length; i += 2)
        {
            if (i + 2 <= points.Length - 1)
            {
                for (float ratio = 0f; ratio <= 1f; ratio += 1.0f / vertexCount)
                {
                    var tangentLineVertex1 = Vector3.Lerp(points[i], points[i + 1], ratio);
                    var tangentLineVertex2 = Vector3.Lerp(points[i + 1], points[i + 2], ratio);
                    var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
                    pointList.Add(bezierPoint);
                }
            }
        }
        return pointList.ToArray();
    }

    
    private void Awake()
    {
        DrawGenerator();
    }

    protected void KochGenerate(Vector3[] positions, bool outwards,float generatorMultiplier)
    {
        _lineSegments.Clear();
        //Creating line segment
        for (int i = 0; i < positions.Length - 1; i++)
        {
            LineSegment line = new LineSegment();
            line.StartPosition = positions[i];
            if (i == positions.Length - 1)
            {
                line.EndPosition = positions[0];
            } else
            {
                line.EndPosition = positions[i + 1];
            }
            line.Direction = (line.EndPosition - line.StartPosition).normalized;
            line.Length = Vector3.Distance(line.EndPosition, line.StartPosition);
            _lineSegments.Add(line);
        }

        // add the lien segment points to a point array
        List<Vector3> newPos = new List<Vector3>();
        List<Vector3> targetPos = new List<Vector3>();

        for (int i = 0; i < _lineSegments.Count; i++)
        {
            newPos.Add(_lineSegments[i].StartPosition);
            targetPos.Add(_lineSegments[i].StartPosition);

            for (int j = 1; j < _keys.Length - 1; j++)
            {
                float moveAmount = _lineSegments[i].Length * _keys[j].time;
                float heightAmount = (_lineSegments[i].Length * _keys[j].value) * generatorMultiplier;
                Vector3 movePos = _lineSegments[i].StartPosition + (_lineSegments[i].Direction * moveAmount);
                Vector3 Dir;
                if (outwards)
                {
                    Dir = Quaternion.AngleAxis(-90, _rotateAxis) * _lineSegments[i].Direction;
                }
                else
                {
                    Dir = Quaternion.AngleAxis(90, _rotateAxis) * _lineSegments[i].Direction;
                }
                newPos.Add(movePos);
                targetPos.Add(movePos + (Dir * heightAmount));
            }
        }
        newPos.Add(_lineSegments[0].StartPosition);
        targetPos.Add(_lineSegments[0].StartPosition);
        _position = new Vector3[newPos.Count];
        _targetPosition = new Vector3[targetPos.Count];
        _position = newPos.ToArray();
        _targetPosition = targetPos.ToArray();
        _bezierPosition = BezierCurve(_targetPosition, _bezierVertexCount);

        _generationCount++;
    }
    private void OnDrawGizmos()
    {
        getInitiatorPoints();
        _initiatorPoints = new Vector3[_initiatorPointAmount];
        _rotateVector = Quaternion.AngleAxis(_initialRotation, _rotateAxis) * _rotateVector;

        for (int i = 0; i < _initiatorPointAmount; i++)
        {
            _initiatorPoints[i] = _rotateVector * _initiatorSize;
            _rotateVector = Quaternion.AngleAxis(360 / _initiatorPointAmount, _rotateAxis) * _rotateVector;
        }

        for (int i = 0; i < _initiatorPointAmount; i++)
        {
            Gizmos.color = Color.white;
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;
            if(i < _initiatorPointAmount - 1)
            {
                Gizmos.DrawLine(_initiatorPoints[i], _initiatorPoints[i+1]);
            } else
            {
                Gizmos.DrawLine(_initiatorPoints[i], _initiatorPoints[0]);
            }
        }
    }
    private void getInitiatorPoints()
    {
        switch (initiator)
        {
            case _initiator.Triangle:
                _initiatorPointAmount = 3;
                _initialRotation = 0;
                break;
            case _initiator.Square:
                _initiatorPointAmount = 4;
                _initialRotation = 45;
                break;
            case _initiator.Pentagon:
                _initiatorPointAmount = 5;
                _initialRotation = 36;
                break;
            case _initiator.Hexagon:
                _initiatorPointAmount = 6;
                _initialRotation = 30;
                break;
            case _initiator.Heptagon:
                _initiatorPointAmount = 7;
                _initialRotation = 25.71428f;
                break;
            case _initiator.Octagon:
                _initiatorPointAmount = 8;
                _initialRotation = 22.5f;
                break;

            default:
                _initiatorPointAmount = 3;
                _initialRotation = 0;
                break;
        };

        switch (axis)
        {
            case _axis.XAxis:
                _rotateVector = new Vector3(1, 0, 0);
                _rotateAxis = new Vector3(0, 0, 1);
                break;
            case _axis.YAxis:
                _rotateVector = new Vector3(0, 1, 0);
                _rotateAxis = new Vector3(1, 0, 0);
                break;
            case _axis.ZAxis:
                _rotateVector = new Vector3(0, 0, 1);
                _rotateAxis = new Vector3(0, 1, 0);
                break;
            default:
                _rotateVector = new Vector3(0, 0, 1);
                _rotateAxis = new Vector3(0, 1, 0);
                break;
        }
    }
    // Start is called before the first frame update
    protected void DrawGenerator()
    {
        getInitiatorPoints();
        //assign lists and arrays
        _position = new Vector3[_initiatorPointAmount + 1];
        _targetPosition = new Vector3[_initiatorPointAmount + 1];
        _keys = _generator.keys;
        _lineSegments = new List<LineSegment>();

        _rotateVector = Quaternion.AngleAxis(_initialRotation, _rotateAxis) * _rotateVector;

        for (int i = 0; i < _initiatorPointAmount; i++)
        {
            _position[i] = _rotateVector * _initiatorSize;
            _rotateVector = Quaternion.AngleAxis(360 / _initiatorPointAmount, _rotateAxis) * _rotateVector;
        }
        _position[_initiatorPointAmount] = _position[0];
        _targetPosition = _position;

        for (int i = 0; i < _startGen.Length; i++)
        {
            KochGenerate(_targetPosition, _startGen[i].outwards, _startGen[i].scale);
        }
        SetPositionList();
    }

    protected void SetPositionList()
    {
        _positionList = new List<Vector2>();
        for (int i = 0; i < _position.Length; i++)
        {
            _positionList.Add(_position[i]);
        }
    }
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("avant if");
        if (_refresh)
        {
            Debug.Log(" if");

            Awake();
            _refresh = false;
        }

    }
}
