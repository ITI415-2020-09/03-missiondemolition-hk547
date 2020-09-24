using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    static private Slingshot S;

    public GameObject launchPoint;

    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    public GameObject prefabProjectile;

    public float velocityMult = 8f;
    private Rigidbody projectileRigidbody;

    static public Vector3 LAUNCH_POS
    { 
        get
        {
            if (S == null) return Vector3.zero;
            return S.launchPos;
        }
    }

    private void Awake()
    {
        S = this;

        //transform.Find("") searches for a child of slingshot named launchpoint and returns its transform
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        //setactive() launchpoint tells the game whether or not to ignore them

        launchPos = launchPointTrans.position;
        // stores the 3d world position of launchpoint
    }

    void OnMouseEnter()
    {
        //print("Slingshot: OnMouseEnter()"); 마우스 들어가면 Halo 켜짐
        launchPoint.SetActive(true);
    }

    void OnMouseExit()
    {
        //print("Slingshot: OnMouseExit()");
        launchPoint.SetActive(false);
    }

    private void OnMouseDown()
    {
        //The player has pressed the mouse button while over slingshot
        aimingMode = true; //normally false
        // instantiate a projectile
        projectile = Instantiate(prefabProjectile) as GameObject;
        //start it at the launchpoint
        projectile.transform.position = launchPos;
        // set it to iskinematic for now
        projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.isKinematic = true;
        //projectile.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update()
    {
        //if slingshot is not in aimingmode, dont run this code
        if (!aimingMode) return;

        //Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        //find the delta from the launchPos to the moousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;
        //limit mouseDelta to the radius of the Slingshot SphereCollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if(mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        //move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        if (Input.GetMouseButtonUp(0))
        {
            //the mouse has been released
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile;
            projectile = null;

            MissionDemolition.ShotFired();
            ProjectileLine.S.poi = projectile;
        }
    }
}
