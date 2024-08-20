using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject _left;
    [SerializeField] GameObject _right;
    [SerializeField] private float barSpeed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A)) 
        {
            MoveBar(_left, barSpeed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            MoveBar(_right, barSpeed);
        }
        
    }

    void MoveBar(GameObject gameObject, float speed) 
    {
        var rotation = gameObject.transform.rotation;
        gameObject.transform.rotation = Quaternion.Lerp(rotation, Quaternion.Euler(0f, 0f, speed), speed * Time.deltaTime);
    }
}
