using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLane : MonoBehaviour
{
    // fun��o para colocar em uma lane aleat�ria
    public void positionLane()
    {
        // pega um n�mero aleat�rio entre -1 e 2, intervalo aberto em 2
        int randomLane = Random.Range(-1, 2);

        // define nova posi��o do objeto de acordo com randomLane
        transform.position = new Vector3(randomLane, transform.position.y, transform.position.z);
    }
}
