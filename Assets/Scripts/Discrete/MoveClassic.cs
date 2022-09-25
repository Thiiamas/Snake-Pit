using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
public class MoveClassic : Agent
{
    public class Snake{
        public Vector3 localPosition;
        public Quaternion localRotation;

        public Snake(Vector3 pos, Quaternion rot){
            localPosition = pos;
            localRotation = rot;
        }
    }

    private BehaviorParameters _behavior;

    public AppleSpawner _spawner;
    public List<Apple> _apples = new List<Apple>();
    public GameObject _bodyPrefab;
    private List<Snake> snakeHistory = new List<Snake>();
    [SerializeField] private List<GameObject> bodyParts= new List<GameObject>();
    float _horizontalAxis;
    public float _rotationSpeed = 30;
    public float _moveSpeed = 0.5f;
    public int gap = 1;
    public float _tickLength = 0.3f;
        float _actionTimer;
    float _actionTimerHeuristic;

    public Vector3 _spawnPosition;


    public override void OnEpisodeBegin()
    {
        snakeHistory.Clear();
        snakeHistory.Insert(0,new Snake(new Vector3(0,150,0), Quaternion.identity));
        foreach(GameObject body in bodyParts){
            Destroy(body);
        }
        bodyParts.Clear();
        GrowPart();
        GrowPart();
        foreach(GameObject body in bodyParts){
            //body.GetComponent<BoxCollider>().enabled = false;
        }
        int bodyCount = Random.Range(0,3);
        for(int i = 0; i < bodyCount; i++){
            GrowPart();
        }
        transform.localPosition = _spawnPosition;
        foreach(Apple apple in _apples){
            apple.MoveApple();
        }
        
    }
    public override void CollectObservations(VectorSensor sensor){
        //sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.forward.x);
        sensor.AddObservation(transform.forward.z);
        foreach(Apple apple in _apples){
            //sensor.AddObservation(apple.transform.localPosition);
            Vector3 dir = apple.transform.localPosition - transform.localPosition;
            float angle = Vector3.SignedAngle(dir, transform.forward, Vector3.up)/180;
            sensor.AddObservation(angle);
            sensor.AddObservation(Vector3.Distance(transform.localPosition, apple.transform.localPosition));
            //sensor.AddObservation(Vector3.Distance(transform.localPosition, apple.transform.localPosition));
        }
        //sensor.AddObservation(bodyParts.Count);
        
    }


    public override void OnActionReceived(ActionBuffers actions){
        int direction = actions.DiscreteActions[0];
        if (direction == 0){
            direction = -90;
        }else if (direction == 1){
            direction = 0;
        }else if (direction == 2){
            direction = 90;
        }
        transform.Rotate(transform.up * direction);
        //Store position
        snakeHistory.Insert(0,new Snake(transform.position, transform.rotation));
        
        //move body parts
        int index = 0;
        foreach(var body in bodyParts){
            Snake vSnake = snakeHistory[Mathf.Min(index * gap, snakeHistory.Count - 1)];

            body.transform.position = vSnake.localPosition;
            body.transform.rotation = vSnake.localRotation;
            index++;
        }

        transform.localPosition += transform.forward * _moveSpeed;
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteAction = actionsOut.DiscreteActions; 
        
        if(_horizontalAxis == -1){
            discreteAction[0] = 0;
        } else if (_horizontalAxis == 0){
            discreteAction[0] = 1;
        } else if (_horizontalAxis == 1){
            discreteAction[0] = 2;
        }

    }
    // Start is called before the first frame update
    void Start()
    {
            _behavior = GetComponent<BehaviorParameters>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_behavior.IsInHeuristicMode()){
            _horizontalAxis = Input.GetAxisRaw("Horizontal");
        }
        if(_apples.Count == 0){
            _apples = _spawner.Foods;
        }
        _actionTimer += Time.deltaTime;
        if(_actionTimer > _tickLength){
            _actionTimer = 0;
            RequestDecision();
        }

        foreach(Apple apple in _apples){
            Vector3 dir = apple.transform.localPosition - transform.localPosition;
            float angle = Vector3.SignedAngle(dir, transform.forward, Vector3.up);
            //print("angle =" + angle);
        }

    }

    void FixedUpdate() {

    }

        public void GrowPart(){
        GameObject body = Instantiate(_bodyPrefab);
        this.bodyParts.Add(body);
        Snake vSnake = snakeHistory[Mathf.Min(bodyParts.Count, snakeHistory.Count - 1)];
        body.transform.position = vSnake.localPosition;
        body.transform.rotation = vSnake.localRotation;
        
    }

    private void OnTriggerStay(Collider other) {
        if(other.TryGetComponent<Apple>(out Apple apple)){
            AddReward(1);
            //EndEpisode();
            GrowPart();
            apple.MoveApple();
        } else if(other.TryGetComponent<Wall>(out Wall wall)){
            AddReward(-5);
            EndEpisode();
        } else if(other.TryGetComponent<SnakeBody>(out SnakeBody body)){
            AddReward(-5);
            EndEpisode();
        }else{
            print(other.gameObject);
        }
    }
}
