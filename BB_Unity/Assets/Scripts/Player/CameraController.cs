using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("오브젝트 연결")]
    [SerializeField]
    private Transform player;

    [Header("설정")]
    [SerializeField, Range(0f, 10f)]
    private float xPos = 0f;
    [SerializeField, Range(0f, 10f)]
    private float yPos = 2.5f;

    void Update()
    {
        transform.position = new Vector3(player.position.x + xPos, player.position.y + yPos, transform.position.z);
    }
}
