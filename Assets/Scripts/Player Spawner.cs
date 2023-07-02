using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    // refer�ncia aos players dispon�veis nos prefabs
    public GameObject[] players;

    void Awake()
    {
        // instancia o player indicado pelo game manager na posi��o do objeto mesmo e sem rota��o
        Instantiate(players[GameManager.gm.charIndex].gameObject, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
