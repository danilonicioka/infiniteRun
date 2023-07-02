using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    // refer�ncias aos textos das miss�es (recompensa, descri��o e progresso)
    public Text[] missionDescription, missionProgress, missionReward;

    // refer�ncia dos bot�es para coletar miss�o caso esteja completada
    public GameObject[] rewardButton;

    // refer�ncia ao texto que exibe quantidade de moedas acumuladas
    public Text coinsText;

    // refer�ncia ao texto de custo de personagem
    public Text costText;

    // refer�ncia aos objetos dos personagens
    public GameObject[] characters;

    // vari�vel para selecionar personagem
    private int charIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        // no come�o j� atualiza as miss�es
        SetMission();

        // atualiza quantidade de moedas
        UpdateCoins(GameManager.gm.coins);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // fun��o para iniciar jogo ao apertar bot�o de start
    public void StartGame()
    {
        // verifica se tem moeda suficiente pra comprar o personagem selecionado
        if (GameManager.gm.charactersCost[charIndex] <= GameManager.gm.coins)
        {
            // consome as moedas, atualiza o custo pra zero pra n�o comprar dnv, salva o custo e come�a a run
            GameManager.gm.coins -= GameManager.gm.charactersCost[charIndex];
            GameManager.gm.charactersCost[charIndex] = 0;
            GameManager.gm.Save();
            GameManager.gm.StartRun(charIndex);
        }
    }

    // fun��o para atualizar textos das miss�es
    public void SetMission()
    {
        // passa por cada miss�o instanciada no game manager
        for (int i = 0; i < GameManager.gm.missions.Length; i++)
        {
            // pega miss�o i e atualiza texto por meio das fun��es definidas na classe da miss�o
            MissionBase mission = GameManager.gm.GetMission(i);
            missionDescription[i].text = mission.GetMissionDescription();
            missionReward[i].text = "Recompensa: " + mission.reward;
            missionProgress[i].text = mission.progress + mission.currentProgress + "/" + mission.objective;

            // se a miss�o estiver completada, deve-se exibir o bot�o
            if (mission.GetMissionStatus())
            {
                rewardButton[i].SetActive(true);
            }
        }

        // salva dados no arquivo
        GameManager.gm.Save();
    }

    // atualizar quantidade de moedas acumuladas
    public void UpdateCoins(int coins)
    {
        coinsText.text = coins.ToString();
    }

    // fun��o para coletar recompensa das miss�es
    public void GetReward(int missionIndex)
    {
        // soma recompensa �s moedas
        GameManager.gm.coins += GameManager.gm.GetMission(missionIndex).reward;

        // atualiza texto das moedas
        UpdateCoins(GameManager.gm.coins);

        // desativa bot�o de recompensa
        rewardButton[missionIndex].SetActive(false);

        // chama fun��o que destroi miss�o e cria nova
        GameManager.gm.GenerateMission(missionIndex);
    }

    // fun��o para alterar sele��o de personagem
    public void ChangeChar(int index)
    {
        // atualiza qual personagem est� sendo selecionado
        charIndex += index;

        // se passar dos personagens existentes, volta pro primeiro personagem
        if(charIndex >= characters.Length)
        {
            charIndex = 0;
        } else if(charIndex < 0)
        {
            charIndex = characters.Length - 1;
        }

        // exibe personagem selecionado
        for (int i = 0; i < characters.Length; i++)
        {
            if(i == charIndex)
            {
                characters[i].SetActive(true);
            }
            else
            {
                characters[i].SetActive(false);
            }
        }

        // armazena texto do custo
        string cost = "";

        // se custo for zero deixa sem texto, se n, pega custo
        if (GameManager.gm.charactersCost[charIndex] != 0)
        {
            cost = GameManager.gm.charactersCost[charIndex].ToString();
        }

        // atualiza texto de custo exibido
        costText.text = cost;
    }
}
