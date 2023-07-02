using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    // referências aos textos das missões (recompensa, descrição e progresso)
    public Text[] missionDescription, missionProgress, missionReward;

    // referência dos botões para coletar missão caso esteja completada
    public GameObject[] rewardButton;

    // referência ao texto que exibe quantidade de moedas acumuladas
    public Text coinsText;

    // referência ao texto de custo de personagem
    public Text costText;

    // referência aos objetos dos personagens
    public GameObject[] characters;

    // variável para selecionar personagem
    private int charIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        // no começo já atualiza as missões
        SetMission();

        // atualiza quantidade de moedas
        UpdateCoins(GameManager.gm.coins);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // função para iniciar jogo ao apertar botão de start
    public void StartGame()
    {
        // verifica se tem moeda suficiente pra comprar o personagem selecionado
        if (GameManager.gm.charactersCost[charIndex] <= GameManager.gm.coins)
        {
            // consome as moedas, atualiza o custo pra zero pra não comprar dnv, salva o custo e começa a run
            GameManager.gm.coins -= GameManager.gm.charactersCost[charIndex];
            GameManager.gm.charactersCost[charIndex] = 0;
            GameManager.gm.Save();
            GameManager.gm.StartRun(charIndex);
        }
    }

    // função para atualizar textos das missões
    public void SetMission()
    {
        // passa por cada missão instanciada no game manager
        for (int i = 0; i < GameManager.gm.missions.Length; i++)
        {
            // pega missão i e atualiza texto por meio das funções definidas na classe da missão
            MissionBase mission = GameManager.gm.GetMission(i);
            missionDescription[i].text = mission.GetMissionDescription();
            missionReward[i].text = "Recompensa: " + mission.reward;
            missionProgress[i].text = mission.progress + mission.currentProgress + "/" + mission.objective;

            // se a missão estiver completada, deve-se exibir o botão
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

    // função para coletar recompensa das missões
    public void GetReward(int missionIndex)
    {
        // soma recompensa às moedas
        GameManager.gm.coins += GameManager.gm.GetMission(missionIndex).reward;

        // atualiza texto das moedas
        UpdateCoins(GameManager.gm.coins);

        // desativa botão de recompensa
        rewardButton[missionIndex].SetActive(false);

        // chama função que destroi missão e cria nova
        GameManager.gm.GenerateMission(missionIndex);
    }

    // função para alterar seleção de personagem
    public void ChangeChar(int index)
    {
        // atualiza qual personagem está sendo selecionado
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
