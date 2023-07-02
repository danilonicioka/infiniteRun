using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackScript : MonoBehaviour
{
    // referencia aos diferentes obst�culos 
    public GameObject[] obstacles;

    // armazena n�mero m�nimo e m�ximo de obst�culos instanciados, para variar e n�o ser sempre o mesmo n�mero
    public Vector2 numberOfObstacles;

    // lista que armazena os obst�culos instanciados
    public List<GameObject> newObstacles;

    // tamanho da pista (obtido manualmente ao duplicar)
    private float trackSize = 270f;

    // refer�ncia �s moedas
    public GameObject coin;

    // armazena n�mero m�nimo e m�ximo de moedas instanciadas
    public Vector2 numberOfCoins;

    // lista que armazena as moedas instanciadas
    public List<GameObject> newCoins;

    // Start is called before the first frame update
    void Start()
    {
        // varia n�mero de obst�culos como um n�mero aleat�rio entre a componente x e y de numberOfObstacles
        int newNumberOfObstacles = (int)Random.Range(numberOfObstacles.x, numberOfObstacles.y);

        // varia n�mero de moedas
        int newNumberOfCoins = (int)Random.Range(numberOfCoins.x, numberOfCoins.y);

        // loop para instanciar obst�culos
        for (int i = 0; i < newNumberOfCoins; i++)
        {
            // instancia um dos obst�culos armazenados em obstacles e armazena na lista de obst�culos instanciados
            newCoins.Add(Instantiate(coin, transform));

            // mant�m o obst�culo desativado inicialmente, primeiro deve-se posicion�-lo corretamente na pista
            newCoins[i].SetActive(false);
        }

        // loop para instanciar moedas
        for (int i = 0; i < newNumberOfObstacles; i++)
        {
            // instancia uma moeda e armazena na lista de moedas instanciadas
            newObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform));

            // mant�m a moeda desativada inicialmente
            newObstacles[i].SetActive(false);
        }

        // chama fun��o para posicionar obst�culos
        PositionateObstacles();

        // chama fun��o para posicionar moedas
        PositionateCoins();
    }

    // fun��o para posicionar obst�culos
    void PositionateObstacles()
    {
        // passa por cada obst�culo na lista de obst�culos instanciados
        for (int i = 0; i < newObstacles.Count; i++)
        {
            // posi��o m�nima em z em que o objeto ser� posicionado
            float posZMin = (trackSize / newObstacles.Count) + i * (trackSize / newObstacles.Count);

            // posi��o m�xima em z em que o objeto ser� posicionado
            float posZMax = (trackSize / newObstacles.Count) + i * (trackSize / newObstacles.Count) + 1;

            // altera posi��o do obst�culo entre zmin e zmax
            newObstacles[i].transform.localPosition = new Vector3(0,0, Random.Range(posZMin, posZMax));

            // ap�s colocar na posi��o certa, ativa
            newObstacles[i].SetActive(true);

            // apenas a lixeira ir� mudar de lane, ent�o deve-se verificar se o obst�culo tem essa componente, ou seja, se � a lixeira
            if (newObstacles[i].GetComponent<ChangeLane>() != null)
            {
                newObstacles[i].GetComponent<ChangeLane>().positionLane();
            }
        }
    }

    // fun��o para posicionar moedas
    void PositionateCoins()
    {
        // posi��o m�nima em z em que o objeto ser� posicionado
        float posZMin = 10f;

        // passa por cada moeda na lista de moedas instanciadas
        for (int i = 0; i < newCoins.Count; i++)
        {
            // posi��o m�xima em z em que o objeto ser� posicionado
            float posZMax = posZMin + 5;

            // pega um valor aleat�rio para z entre min e max
            float randomZPos = Random.Range(posZMin, posZMax);

            // altera posi��o do obst�culo entre zmin e zmax
            newCoins[i].transform.localPosition = new Vector3(transform.position.x, transform.position.y, randomZPos);

            // ap�s colocar na posi��o certa, ativa
            newCoins[i].SetActive(true);

            // pega script para trocar de lane
            newCoins[i].GetComponent<ChangeLane>().positionLane();

            posZMin = randomZPos + 1;
        }
    }

    // fun��o para reposicionar ao chegar no final da pista
    private void OnTriggerEnter(Collider other)
    {
        // verifica se o que colidiu foi o objeto com a tag player, ou seja, o personagem
        if (other.CompareTag("Player"))
        {
            // chama fun��o de aumentar velocidade
            other.GetComponent<PlayerScript>().IncreaseSpeed();

            // altera posi��o da track para o final da segunda
            transform.position = new Vector3(0, 0, transform.position.z + trackSize * 2);

            // chama fun��o para posicionar, pois a pista mudou de lugar
            // ap�s um tempo para n�o ser percept�vel na vis�o do jogador
            PositionateObstacles();
            PositionateCoins();
        }
    }
}
