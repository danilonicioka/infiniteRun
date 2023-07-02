using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    // referencia �s imagens dos cora��es
    public Image[] lifes;

    // referencia ao texto das moedas
    public Text coinText;

    // refer�ncia ao painel do game over
    public GameObject gameOverPanel;

    // vari�vel para armazenar texto do score
    public Text scoreText;

    // atualizar cora��es
    public void UpdateLives(int lives)
    {
        // passa por todas os cora��es
        for (int i = 0; i < lifes.Length; i++)
        {
            if (lives > i) 
            {
                lifes[i].color = Color.white;
            }
            else
            {
                lifes[i].color = Color.black;
            }
        }
    }

    // atualiza texto que indica n�mero de moedas
    public void UpdateCoins(int coins)
    {
        coinText.text = coins.ToString();
    }

    // atualiza score
    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score + "m";
    }
}
