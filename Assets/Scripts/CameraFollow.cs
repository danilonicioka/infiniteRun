using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // refer�ncia ao transform do player
    private Transform player;

    // vari�vel que guarda dist�ncia inicial da camera do player
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - player.position;
    }

    // chamado depois que o frame termina
    void LateUpdate()
    {
        // vari�vel que vai verificar nova posi��o da c�mera
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, player.position.z + offset.z);

        // atualiza posi��o
        transform.position = newPosition;
    }
}
