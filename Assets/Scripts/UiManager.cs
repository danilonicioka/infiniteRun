using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    // referencia às imagens dos corações
    public Image[] lifes;

    // referencia ao texto das moedas
    public Text coinText;

    // referência ao painel do game over
    public GameObject gameOverPanel;

    // variável para armazenar texto do score
    public Text scoreText;

    // atualizar corações
    public void UpdateLives(int lives)
    {
        // passa por todas os corações
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

    // atualiza texto que indica número de moedas
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
