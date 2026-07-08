using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Inscribed")] 
    public GameObject projectilePrefab;
    public GameObject heavyProjectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    
    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    private int shotCountThisLevel = 0;

    void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }

    public void ResetShotCounter()
    {
        shotCountThisLevel = 0;
    }

    private void OnMouseEnter()
    {
        //print("Slingshot: OnMouseEnter");
        launchPoint.SetActive(true);
    }

    private void OnMouseExit()
    {
        //print("Slingshot: OnMouseExit");
        launchPoint.SetActive(false);
    }

    private void OnMouseDown()
    {
        aimingMode = true;
        shotCountThisLevel++;
        bool isHeavyShot = (shotCountThisLevel % 3 == 0);
        GameObject prefabToFire = (isHeavyShot && heavyProjectilePrefab != null) ? heavyProjectilePrefab : projectilePrefab;
        projectile = Instantiate(prefabToFire) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;
    }
    void Update()
    {
        // If Slingshot is not in aimingMode, don't run this code
        if (!aimingMode) return;
        
        // Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);
        
        // Find the delta from the launchPos to the mousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;
        
        // Limit mouseDelta to the radius of the Slingshot SphereCollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        
        // Move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;
        
        // Catch the frame that player releases mouse button
        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;
            // Switch to slingshot view immediately before setting POI
            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
            // Set the MainCamera POI to follow the instantiated projectile
            FollowCam.POI = projectile;
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();
        }
    }
}
