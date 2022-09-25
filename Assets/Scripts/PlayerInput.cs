using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private float horizontalAxis;
    [SerializeField]
    private float rotationSpeed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalAxis = Input.GetAxis("Horizontal");

        transform.Rotate(transform.up * rotationSpeed * horizontalAxis);

        if(Input.GetKeyDown("a")){
            print("a");
            SnakeController snakeController = GameObject.FindGameObjectWithTag("Snake").GetComponent<SnakeController>();
            snakeController.GrowPart();
        }
        
    }
}
