using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
public class MoveDiscrete : Agent
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
    public float _rotationSpeed = 30;
    public float _moveSpeed = 2.5f;
    public int gap;
    public float _lastEatenCount = 0f;
    public bool _hungry = false;
    float _hungryTimer;
    float _actionTimer;
    float _actionTimerHeuristic;


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
        int bodyCount = Random.Range(5,15);
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
        _actionTimer += Time.fixedDeltaTime;
        if(_actionTimer > 0.1f){
            _actionTimer = 0f;
            int direction = actions.DiscreteActions[0];
            if (direction == 0){
                direction = -90;
            }else if (direction == 1){
                direction = 0;
            }else if (direction == 2){
                direction = 90;
            }
            transform.Rotate(transform.up * direction);
        }
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

        transform.localPosition += transform.forward * _moveSpeed * Time.deltaTime;
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        _actionTimerHeuristic += Time.fixedDeltaTime;
        if(true){
            _actionTimerHeuristic = 0;
            float horizontalAxis = Input.GetAxisRaw("Horizontal");
            
            print(horizontalAxis);
            transform.Rotate(transform.up * 90 * horizontalAxis);

        }
        transform.localPosition += transform.forward * _moveSpeed * Time.deltaTime;

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(_apples.Count == 0){
            _apples = _spawner.Foods;
        }
        _hungryTimer += Time.deltaTime;
        if(_hungryTimer > 3f && !_hungry){
            StartCoroutine(Whip());
        }

        foreach(Apple apple in _apples){
            Vector3 dir = apple.transform.localPosition - transform.localPosition;
            float angle = Vector3.SignedAngle(dir, transform.forward, Vector3.up);
            //print("angle =" + angle);
        }

    }

    void FixedUpdate() {

    }

    IEnumerator Whip(){
        _hungry = true;
        while(_hungry){
            //AddReward(-1);
            yield return new WaitForSeconds(1);
        }
        yield return null;
    }

        public void GrowPart(){
        GameObject body = Instantiate(_bodyPrefab);
        body.transform.position += -transform.forward * gap;
        this.bodyParts.Add(body);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent<Apple>(out Apple apple)){
            _hungryTimer = 0;
            _hungry = false;
            AddReward(1);
            //EndEpisode();
            GrowPart();
            apple.MoveApple();
        } else if(other.TryGetComponent<Wall>(out Wall wall)){
            AddReward(-1);
            EndEpisode();
        } else if(other.TryGetComponent<SnakeBody>(out SnakeBody body)){
            AddReward(-1);
            EndEpisode();
        }else{
            print(other.gameObject);
        }
    }
}
