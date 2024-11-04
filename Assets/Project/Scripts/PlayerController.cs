using System;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    [SerializeField] private JoyStickController joyStick;
    [SerializeField] private float speed = 5f;


    private void Update()
    {

        Vector2 input = joyStick.GetInputDirection();

        Vector2 direction = new Vector2(input.x, input.y).normalized;

        // Move the airplance based on joystick input

        //

        if (direction.magnitude > 0.1f)
        {
            //calculate the angle in radians and convert to  degrees 
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply rotation to the airplace to point in the direction of movement
            transform.rotation = Quaternion.Euler(0, 0, angle - 180);

        }
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Hit");
    }

}


