using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackScript : MonoBehaviour
{
    // referencia aos diferentes obstáculos 
    public GameObject[] obstacles;

    // armazena número mínimo e máximo de obstáculos instanciados, para variar e não ser sempre o mesmo número
    public Vector2 numberOfObstacles;

    // lista que armazena os obstáculos instanciados
    public List<GameObject> newObstacles;

    // tamanho da pista (obtido manualmente ao duplicar)
    private float trackSize = 270f;

    // referência às moedas
    public GameObject coin;

    // armazena número mínimo e máximo de moedas instanciadas
    public Vector2 numberOfCoins;

    // lista que armazena as moedas instanciadas
    public List<GameObject> newCoins;

    // Start is called before the first frame update
    void Start()
    {
        // varia número de obstáculos como um número aleatório entre a componente x e y de numberOfObstacles
        int newNumberOfObstacles = (int)Random.Range(numberOfObstacles.x, numberOfObstacles.y);

        // varia número de moedas
        int newNumberOfCoins = (int)Random.Range(numberOfCoins.x, numberOfCoins.y);

        // loop para instanciar obstáculos
        for (int i = 0; i < newNumberOfCoins; i++)
        {
            // instancia um dos obstáculos armazenados em obstacles e armazena na lista de obstáculos instanciados
            newCoins.Add(Instantiate(coin, transform));

            // mantém o obstáculo desativado inicialmente, primeiro deve-se posicioná-lo corretamente na pista
            newCoins[i].SetActive(false);
        }

        // loop para instanciar moedas
        for (int i = 0; i < newNumberOfObstacles; i++)
        {
            // instancia uma moeda e armazena na lista de moedas instanciadas
            newObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform));

            // mantém a moeda desativada inicialmente
            newObstacles[i].SetActive(false);
        }

        // chama função para posicionar obstáculos
        PositionateObstacles();

        // chama função para posicionar moedas
        PositionateCoins();
    }

    // função para posicionar obstáculos
    void PositionateObstacles()
    {
        // passa por cada obstáculo na lista de obstáculos instanciados
        for (int i = 0; i < newObstacles.Count; i++)
        {
            // posição mínima em z em que o objeto será posicionado
            float posZMin = (trackSize / newObstacles.Count) + i * (trackSize / newObstacles.Count);

            // posição máxima em z em que o objeto será posicionado
            float posZMax = (trackSize / newObstacles.Count) + i * (trackSize / newObstacles.Count) + 1;

            // altera posição do obstáculo entre zmin e zmax
            newObstacles[i].transform.localPosition = new Vector3(0,0, Random.Range(posZMin, posZMax));

            // após colocar na posição certa, ativa
            newObstacles[i].SetActive(true);

            // apenas a lixeira irá mudar de lane, então deve-se verificar se o obstáculo tem essa componente, ou seja, se é a lixeira
            if (newObstacles[i].GetComponent<ChangeLane>() != null)
            {
                newObstacles[i].GetComponent<ChangeLane>().positionLane();
            }
        }
    }

    // função para posicionar moedas
    void PositionateCoins()
    {
        // posição mínima em z em que o objeto será posicionado
        float posZMin = 10f;

        // passa por cada moeda na lista de moedas instanciadas
        for (int i = 0; i < newCoins.Count; i++)
        {
            // posição máxima em z em que o objeto será posicionado
            float posZMax = posZMin + 5;

            // pega um valor aleatório para z entre min e max
            float randomZPos = Random.Range(posZMin, posZMax);

            // altera posição do obstáculo entre zmin e zmax
            newCoins[i].transform.localPosition = new Vector3(transform.position.x, transform.position.y, randomZPos);

            // após colocar na posição certa, ativa
            newCoins[i].SetActive(true);

            // pega script para trocar de lane
            newCoins[i].GetComponent<ChangeLane>().positionLane();

            posZMin = randomZPos + 1;
        }
    }

    // função para reposicionar ao chegar no final da pista
    private void OnTriggerEnter(Collider other)
    {
        // verifica se o que colidiu foi o objeto com a tag player, ou seja, o personagem
        if (other.CompareTag("Player"))
        {
            // chama função de aumentar velocidade
            other.GetComponent<PlayerScript>().IncreaseSpeed();

            // altera posição da track para o final da segunda
            transform.position = new Vector3(0, 0, transform.position.z + trackSize * 2);

            // chama função para posicionar, pois a pista mudou de lugar
            // após um tempo para não ser perceptível na visão do jogador
            PositionateObstacles();
            PositionateCoins();
        }
    }
}
