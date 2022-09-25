using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
public class IAMoveSimpleTest : Agent
{
    public GameManager gameManager;
    public AppleSpawner _spawner;
    public List<Apple> _apples = new List<Apple>();
    public float _rotationSpeed = 30;
    public float _moveSpeed = 2.5f;
    public float _lastEatenCount = 0f;
    public bool _hungry = false;
    float _hungryTimer;
    float _actionTimer;


    public override void OnEpisodeBegin()
    {
        transform.localPosition = DetermineSpawnPosition();
        foreach(Apple apple in _apples){
            apple.MoveApple();
        }
    }
    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition);
        foreach(Apple apple in _apples){
            sensor.AddObservation(apple.transform.localPosition);
            //sensor.AddObservation(Vector3.Distance(transform.localPosition, apple.transform.localPosition));
        }
        
    }


    public override void OnActionReceived(ActionBuffers actions){
        float x = actions.ContinuousActions[0];
        float y = actions.ContinuousActions[1];

        transform.position += new Vector3(x, 0, y) * _moveSpeed * Time.deltaTime;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        float horizontalAxis = Input.GetAxis("Horizontal");

        transform.Rotate(transform.up * _rotationSpeed * Time.deltaTime * horizontalAxis);
    }

    IEnumerator Whip(){
        _hungry = true;
        while(_hungry){
            //AddReward(-1);
            yield return new WaitForSeconds(1);
        }
        yield return null;
    }

    public Vector3 DetermineSpawnPosition()
    {
        Vector3 newPosition = new Vector3(Random.Range(-4,4), 0, Random.Range(-5.5f,-2));
        //newPosition = _mainCamera.ViewportToWorldPoint(newPosition);
        return newPosition;
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

        //transform.localPosition += transform.forward * _moveSpeed * Time.deltaTime;
    }

    void FixedUpdate() {

    }

    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent<Apple>(out Apple apple)){
            _hungryTimer = 0;
            _hungry = false;
            AddReward(10);
            apple.MoveApple();
        } else if(other.TryGetComponent<Wall>(out Wall wall)){
            SetReward(-1);
            EndEpisode();
        }
    }
}
