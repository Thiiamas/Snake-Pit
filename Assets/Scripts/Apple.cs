using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

public class Apple : MonoBehaviour
{
    public GameObject Model;
    public MMFeedbacks eatenFeedback;
    public MMFeedbackScale AppearFeedback;
    public AppleSpawner Spawner;
    public BoxCollider _collider;


    /// a duration (in seconds) during which the food is inactive before moving it to another position
    public float OffDelay = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collider)
    {
        SnakeController SK = collider.GetComponent<SnakeController>();
        if (SK)
        {
            //Grow + play animation explosion
            //SK.eatApple();
            //eatenFeedback.PlayFeedbacks();
            //StartCoroutine(MoveFood());

        }
        else
        {
            //Debug
        }

    }

    /// <summary>
    /// Moves the food to another spot
    /// </summary>
    /// <returns></returns>
    public void MoveApple()
    {
        StartCoroutine(MoveFoodInstant());
    }
    protected virtual IEnumerator MoveFood()
    {
        Model.SetActive(false);
        _collider.enabled = false;
        yield return MMCoroutine.WaitFor(OffDelay);
        Model.SetActive(true);
        _collider.enabled = true;
        this.transform.localPosition = Spawner.DetermineSpawnPositionHalf();
        //AppearFeedback?.PlayFeedbacks();
    }

    protected virtual IEnumerator MoveFoodInstant()
    {
        _collider.enabled = false;
        this.transform.localPosition = Spawner.RandomPositionRange();
        _collider.enabled = true;
        yield return null;
        //AppearFeedback?.PlayFeedbacks();
    }
}
