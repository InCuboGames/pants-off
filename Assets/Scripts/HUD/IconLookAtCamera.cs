using UnityEngine;
using System.Collections;

public class IconLookAtCamera : MonoBehaviour 
{
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

}