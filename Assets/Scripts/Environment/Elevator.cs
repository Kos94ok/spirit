using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    public float Speed;
    public float Height;

    float LowPosition;
    float HighPosition;
    float PositionTimer;
    bool MovingUp = true;
	void Start ()
    {
        LowPosition = transform.position.y;
        HighPosition = LowPosition + Height;
        PositionTimer = 3.00f;
	}
	
	void Update ()
    {
	    if (PositionTimer > 0.00f)
            PositionTimer -= Time.deltaTime;
        if (PositionTimer <= 0.00f)
        {
            if (MovingUp)
            {
                if (transform.position.y < HighPosition)
                    transform.Translate(0.00f, Speed * Time.fixedDeltaTime, 0.00f, Space.World);
                if (transform.position.y >= HighPosition)
                {
                    PositionTimer = 3.00f;
                    MovingUp = false;
                }
            }
            else
            {
                if (transform.position.y > LowPosition)
                    transform.Translate(0.00f, -Speed * Time.fixedDeltaTime, 0.00f, Space.World);
                if (transform.position.y <= LowPosition)
                {
                    PositionTimer = 3.00f;
                    MovingUp = true;
                }
            }
        }
	}
    void OnTriggerStay(Collider Passenger)
    {
        if (Passenger.tag == "Player")
        {
            Passenger.transform.parent = this.transform;
        }
    }
    void OnTriggerExit(Collider Passenger)
    {
        if (Passenger.tag == "Player")
        {
            if (Passenger.transform.parent == this.transform)
                Passenger.transform.parent = null;
        }
    }
}
