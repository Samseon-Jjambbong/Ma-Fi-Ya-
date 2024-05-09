using Cinemachine;
using System.Collections;
using UnityEngine;

public class VCamController : MonoBehaviour
{
    private static VCamController instance;
    public static VCamController Instance {  get { return instance; } }

    public enum VCam { Title, Login, Lobby }

    [SerializeField] CinemachineVirtualCamera vCamTitle;
    [SerializeField] CinemachineVirtualCamera vCamLogin;
    [SerializeField] CinemachineVirtualCamera vCamLobby;

    CinemachineVirtualCamera curCam;

    [SerializeField] float rotateTime;
    [SerializeField] float rotateAngle;
    private float elapsedTime;

    private Quaternion targetRotation;
    private Quaternion startRotation;

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if ( instance == null )
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVCam( VCam vCam )
    {
        switch ( vCam )
        {
            case VCam.Title:
                vCamTitle.Priority = 100;
                vCamLogin.Priority = 0;
                vCamLobby.Priority = 0;
                curCam = vCamTitle;
                break;
            case VCam.Login:
                vCamTitle.Priority = 0;
                vCamLobby.Priority = 100;
                vCamLobby.Priority = 0;
                curCam = vCamLogin;
                break;
            case VCam.Lobby:
                vCamTitle.Priority = 0;
                vCamLobby.Priority = 0;
                vCamLobby.Priority = 100;
                curCam = vCamLobby;
                break;
        }
    }

    public void RotateVCam( int dir = 1)
    {
        Rotate(curCam, dir);
    }

    private void Rotate( CinemachineVirtualCamera vCam, int dir )
    {
        float angle = rotateAngle * dir;

        startRotation = vCam.transform.rotation;
        targetRotation = Quaternion.Euler(vCam.transform.rotation.eulerAngles.x, vCam.transform.rotation.eulerAngles.y + angle , 0f);

        StartCoroutine(RotateRoutine(vCam));
    }

    IEnumerator RotateRoutine( CinemachineVirtualCamera vCam )
    {
        elapsedTime = 0f;
        while ( elapsedTime < rotateTime )
        {
            float t = Mathf.Clamp01(elapsedTime / rotateTime);
            vCam.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        vCam.transform.rotation = targetRotation;
        startRotation = targetRotation;
    }
}
