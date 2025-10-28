using UnityEngine;
using Unity.Cinemachine;
public class CameraManager : MonoBehaviour
{
    public CinemachineCamera[] cameras;

    public CinemachineCamera startCam;
    private CinemachineCamera currentCam;



    void Start()
    {
        currentCam = startCam;

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] == currentCam)
            {
                cameras[i].Priority = 20;
            }
            else
            {
                cameras[i].Priority = 10;
            }
        }
    }

    public void SwitchCamera(CinemachineCamera newCam)
    {
        currentCam = newCam;

        currentCam.Priority = 20;

        for (int i = 0; i < cameras.Length; i++)
            if (cameras[i] != currentCam) cameras[i].Priority = 10;
    }
   
    void Update()
    {
        
    }
}
