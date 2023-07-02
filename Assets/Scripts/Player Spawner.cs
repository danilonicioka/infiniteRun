using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    // referência aos players disponíveis nos prefabs
    public GameObject[] players;

    void Awake()
    {
        // instancia o player indicado pelo game manager na posição do objeto mesmo e sem rotação
        Instantiate(players[GameManager.gm.charIndex].gameObject, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
