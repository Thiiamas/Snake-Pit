using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ContinuousMove : Agent
{

    public class Snake{
            public Vector3 localPosition;
            public Quaternion localRotation;

            public Snake(Vector3 pos, Quaternion rot){
                localPosition = pos;
                localRotation = rot;
            }
        }
    public AppleSpawner _spawner;
    public List<Apple> _apples = new List<Apple>();
    public GameObject _bodyPrefab;
    private List<Snake> snakeHistory = new List<Snake>();
    [SerializeField] private List<GameObject> bodyParts= new List<GameObject>();
    float horizontalAxis;
    public float _rotationSpeed = 30;
    public float _moveSpeed = 2.5f;
    public float _lastEatenCount = 0f;
    public int gap;



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
            body.GetComponent<BoxCollider>().enabled = false;
        }
        int bodyCount = Random.Range(0,5);
        for(int i = 0; i < bodyCount; i++){
            GrowPart();
        }

        transform.localPosition = Vector3.zero;
    }
    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition);
        foreach(Apple apple in _apples){
            //sensor.AddObservation(apple.transform.localPosition);
            Vector3 dir = apple.transform.localPosition - transform.localPosition;
            float angle = Vector3.SignedAngle(dir, transform.forward, Vector3.up)/180;
            sensor.AddObservation(angle);
            sensor.AddObservation(Vector3.Distance(transform.localPosition, apple.transform.localPosition));
            //sensor.AddObservation(Vector3.Distance(transform.localPosition, apple.transform.localPosition));
        }
        sensor.AddObservation(bodyParts.Count);
        
    }


    public override void OnActionReceived(ActionBuffers actions){
        horizontalAxis = actions.ContinuousActions[0];

        //transform.Rotate(transform.up * _rotationSpeed * rotation * Time.deltaTime);

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
        transform.Rotate(transform.up * _rotationSpeed * Time.fixedDeltaTime * horizontalAxis);
        transform.localPosition += transform.forward * _moveSpeed * Time.deltaTime;

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //make sure apple are loaded
        if(_apples.Count == 0){
            _apples = _spawner.Foods;
        }
    }

    void FixedUpdate() {

    }

    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent<Apple>(out Apple apple)){
            AddReward(1);
            GrowPart();
            apple.MoveApple();
        } else if(other.TryGetComponent<Wall>(out Wall wall)){
            AddReward(-1);
            EndEpisode();
        } else if(other.TryGetComponent<SnakeBody>(out SnakeBody body)){
            AddReward(-1);
            EndEpisode();
        }
    }

    public void GrowPart(){
        GameObject body = Instantiate(_bodyPrefab);
        body.transform.position += -transform.forward * gap;
        this.bodyParts.Add(body);
    }
}
