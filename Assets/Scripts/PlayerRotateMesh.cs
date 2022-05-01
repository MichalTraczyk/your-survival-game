using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotateMesh : MonoBehaviour
{
    public Transform mesh;
    public int rotation =0;
    public float speed = 20;
    // Update is called once per frame
    void Update()
    {
        Quaternion meshRot = Quaternion.Lerp(mesh.localRotation, Quaternion.Euler(0, rotation, 0), Time.deltaTime * speed);
        mesh.localRotation = meshRot;
    }
}
