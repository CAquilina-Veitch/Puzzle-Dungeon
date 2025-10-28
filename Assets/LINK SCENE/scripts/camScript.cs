using UnityEngine;

public class camScript : MonoBehaviour
{
    public Vector3 minCamPos;
    public Vector3 maxCamPos;
    void Start()
    {
        
    }


    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minCamPos.x, maxCamPos.x), transform.position.y, Mathf.Clamp(transform.position.z, minCamPos.z, maxCamPos.z));
    }
}
