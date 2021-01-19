using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0f, 2.5f, -8f);
    public float offsetSmooth = 10f;
    public float rotationSmooth = 10f;

    private void Start()
    {
        transform.position = player.position + (player.rotation * offset);
        transform.rotation = Quaternion.LookRotation(player.position - transform.position, player.up);
    }

    void FixedUpdate()
    {
        Vector3 toPos = player.position + (player.rotation * offset);
        transform.position = Vector3.Lerp(transform.position, toPos, offsetSmooth * Time.deltaTime);

        Quaternion toRot = Quaternion.LookRotation(player.position - transform.position, player.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRot, rotationSmooth * Time.deltaTime);
    }
}
