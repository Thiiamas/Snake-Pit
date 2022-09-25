using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.SceneManagement;

public class AppleSpawner : MonoBehaviour
{

    Camera _mainCamera;
    public List<Apple> Foods;
    public GameObject ApplePrefab;
    public GameManager gameManager;

    public bool _isTraining;

    public int AmountOfFood = 1;
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
            Foods = new List<Apple>();
            for (int i = 0; i < AmountOfFood; i++)
            {
                Apple food = Instantiate(ApplePrefab,this.transform).GetComponent<Apple>();
                //SceneManager.MoveGameObjectToScene(food.gameObject, this.gameObject.scene);
                if(_isTraining){
                    food.transform.localPosition = DetermineSpawnPositionHalf();
                }else{
                    food.transform.localPosition = RandomPositionRange();
                }
                food.Spawner = this;
                Foods.Add(food);
                gameManager.CurrentApple = food;
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
        /// Determines a valid position at which to spawn food
        /// </summary>
        /// <returns></returns>
        public virtual Vector3 DetermineSpawnPosition()
        {
            Vector3 newPosition = new Vector3(Random.Range(-4,4), -0.5f, Random.Range(-3,3));
            //newPosition = _mainCamera.ViewportToWorldPoint(newPosition);
            return newPosition;
        }
        public virtual Vector3 DetermineSpawnPositionHalf()
        {
            Vector3 newPosition = new Vector3(Random.Range(-3f,2f),0, Random.Range(0.05f,1.2f));
            Vector3 randCircle = Random.onUnitSphere;
            float range = Random.Range(0.1f,2.3f);
            randCircle *= 2.5f;
            randCircle.y = -0.5f;
            return randCircle;
        }

        public virtual Vector3 RandomPositionRange(){
            Vector3 newPosition = new Vector3(Random.Range(gameManager._horizontalPoint.x, gameManager._horizontalPoint.y),0, Random.Range(gameManager._verticalPoint.x,gameManager._verticalPoint.y));
            return newPosition;
        }
}
