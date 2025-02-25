using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using Unity.VisualScripting;
public class P_MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public Transform cameraBasePosition;

    public Renderer parentRenderer;

    public Alteruna.Avatar avatar;
    void Start()
    {
        
    }

    void Update()
    {
        // Update camera position to match the target position
        if (cameraPosition != null)
        {
            transform.position = cameraPosition.position;
        }

        if (avatar.IsMe)
        {
            parentRenderer.enabled = false;
        }
        else if (!avatar.IsMe) 
        {
            parentRenderer.enabled = true;

            gameObject.SetActive(false);

            List<GameObject> _players = FindObjectsOnLayer(9);

            foreach (var obj in _players)
            {
                if (obj == gameObject)
                    return;

                Transform parentTransform = obj.transform;

                // Find the child GameObjects by name
                Transform firstChild = parentTransform.Find("PreyComponent");
                Transform orientation = firstChild.Find("Orientation");
                Transform camerahold = firstChild.Find("CameraHolder");
                Transform cam = camerahold.Find("PlayerCam"); 
                Transform arm = cam.Find("PreyFPSArms");
                Transform rend = arm.Find("PreyV2Rabbit_3JointRig.009");


                rend.gameObject.GetComponent<Renderer>().enabled = false;

                //Transform secondChild = parentTransform.Find("HunterComponent");
                //secondChild.gameObject.GetComponent<Renderer>().enabled = false;

            }
        }


        
    }

    List<GameObject> FindObjectsOnLayer(int layer)
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> objectsInLayer = new List<GameObject>();

        foreach (var obj in allPlayers)
        {
            if (obj.layer == layer)
            {
                objectsInLayer.Add(obj);
            }
        }

        return objectsInLayer;
    }
}
