using UnityEngine;

public class liftScript : MonoBehaviour
{
    public float maxHeight;
    public float minHeight;
    public float moveSpeed;
    public bool liftMove;
    public void CallLift()
    {       
        if (transform.position.y <= minHeight) moveSpeed = Mathf.Abs(moveSpeed);
        else moveSpeed = -Mathf.Abs(moveSpeed);
        Invoke("StartMove", 1f);
    }
    public void StartMove()
    {
        liftMove = true;
    }
    public void FixedUpdate()
    {
        if (liftMove == true)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + moveSpeed, transform.position.z);
            if (transform.position.y >= maxHeight && moveSpeed > 0) liftMove = false;
            if (transform.position.y <= minHeight && moveSpeed < 0) liftMove = false;
        }
    }
}
