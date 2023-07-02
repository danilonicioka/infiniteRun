using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // referência ao transform do player
    private Transform player;

    // variável que guarda distância inicial da camera do player
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
        // variável que vai verificar nova posição da câmera
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, player.position.z + offset.z);

        // atualiza posição
        transform.position = newPosition;
    }
}
