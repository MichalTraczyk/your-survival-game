using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotateMesh : MonoBehaviour
{
    [SerializeField] private Transform mesh;
    public int rotation;
    public float speed = 20;
    private void Awake()
    {
        rotation = 0;
    }
    void Update()
    {
        Quaternion meshRot = Quaternion.Lerp(mesh.localRotation, Quaternion.Euler(0, rotation, 0), Time.deltaTime * speed);
        mesh.localRotation = meshRot;
    }
}
