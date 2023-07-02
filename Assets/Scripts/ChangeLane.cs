using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLane : MonoBehaviour
{
    // função para colocar em uma lane aleatória
    public void positionLane()
    {
        // pega um número aleatório entre -1 e 2, intervalo aberto em 2
        int randomLane = Random.Range(-1, 2);

        // define nova posição do objeto de acordo com randomLane
        transform.position = new Vector3(randomLane, transform.position.y, transform.position.z);
    }
}
