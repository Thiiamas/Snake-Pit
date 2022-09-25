using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class SnakeController : MonoBehaviour
{

    public class Snake{
        public Vector3 position;
        public Quaternion rotation;

        public Snake(Vector3 pos, Quaternion rot){
            position = pos;
            rotation = rot;
        }
    }

    [SerializeField]
    private float speed = 10f;

    public MMFeedbacks EatFeedback;
    public MMFeedbacks BodyEatFeedback;
    public MMF_Player DeathFeedback;
    public GameObject _deathParticles;

    private List<Snake> snakeHistory = new List<Snake>();
    private List<GameObject> bodyParts= new List<GameObject>();
    public GameObject bodyPrefab;
    public int gap = 350;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        //transform.Translate()

        //Store position
        snakeHistory.Insert(0,new Snake(transform.position, transform.rotation));
        
        //move body parts
        int index = 0;
        foreach(var body in bodyParts){
            Snake vSnake = snakeHistory[Mathf.Min(index * gap, snakeHistory.Count - 1)];

            body.transform.position = vSnake.position;
            body.transform.rotation = vSnake.rotation;
            index++;
        }


    }
    private void OnCollisionEnter(Collision other) {
        print(other.gameObject.layer);
        if (other.gameObject.layer.Equals("Walls")){
            print("ok");
        }
    }

    public void eatApple(){
        GrowPart();
        EatFeedback?.StopFeedbacks();
        EatFeedback?.PlayFeedbacks();
        StopAllCoroutines();
        StartCoroutine(EatBodyFB());

    }
    public void TriggerDeath(){
        
        print("dead");
        GameObject deathParticles = Instantiate(_deathParticles,transform);
        Destroy(deathParticles, deathParticles.GetComponent<ParticleSystem>().startLifetime);
        DeathFeedback?.PlayFeedbacks();
        Destroy(this.gameObject,0.2f);
    }

    IEnumerator EatBodyFB(){
        MMFeedbackScale FBScale = BodyEatFeedback.GetComponent<MMFeedbackScale>();
        int index = 0;
        float delay = 1f/bodyParts.Count;
        if(delay > 0.1f){
            FBScale.AnimateScaleDuration = delay;
        }
        foreach(GameObject body in bodyParts){
            if(FBScale){
                FBScale.AnimateScaleTarget = body.transform;
            }
            BodyEatFeedback.StopFeedbacks();
            BodyEatFeedback.PlayFeedbacks();
            index++;
            yield return new WaitForSeconds(delay);
        }
        yield return null;
    }

    public void GrowPart(){
        GameObject body = Instantiate(bodyPrefab,transform);
        this.bodyParts.Add(body);
    }
}
