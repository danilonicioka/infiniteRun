using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// bibliotecas para salvar dados do jogo em binário
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// Random existe na unity e na system, então deve-se definir qual vai usar
using Random = UnityEngine.Random;

// classe para comunicar dados no jogo e dados salvos
[Serializable]
public class PlayerData
{
    public int coins;

    // não é possível salvar a classe da missão, então deve-se extrair todos os dados da missão, pois, ao carregar os dados, é como instanciar a missão novamente com os dados fornecidos
    public int[] objective;
    public int[] progress;
    public int[] reward;
    public int[] currentProgress;
    public string[] missionType;
    public int[] charactersCost;
}

public class GameManager : MonoBehaviour
{
    // para poder acessar as coins com a classe game manager
    public int coins;

    // caminho para salvar arquivo
    public string path;

    // guardar o gm para manter o mesmo entre as cenas
    public static GameManager gm;

    // referência às missões criadas
    [HideInInspector]
    public MissionBase[] missions;

    // custo dos personagens
    public int[] charactersCost;

    // index do personagem
    public int charIndex;

    // função executada antes do start
    private void Awake()
    {
        // verifica se já há um gm, se não, se torna
        if(gm == null)
        {
            gm = this;
        }
        // se não for ele, destroi para garantir que há só um gm
        else if (gm != this)
        {
            Destroy(gameObject);
        }

        // gm não pode ser destruído ao carregar uma cena
        DontDestroyOnLoad(gameObject);

        // caminho padrão utilizado pela Unity para salvar dados de jogo
        path = Application.persistentDataPath + "/playerinfo.dat";

        // instanciar missões
        // define número de missões (2)
        missions = new MissionBase[2];

        // verifica se há dados salvos
        if(File.Exists(path))
        {
            Load();
        }
        else
        {
            // cria uma missão para cada espaço em missions
            for (int i = 0; i < missions.Length; i++)
            {
                // cria um novo objeto para cada missão
                GameObject newMission = new GameObject("Mission" + i);

                // atribui gm como parent de cada mission
                newMission.transform.SetParent(transform);

                // define variável com tipos de missões para sortear
                MissionType[] missionType = { MissionType.SingleRun, MissionType.TotalMeters, MissionType.SingleRunCollect };

                // variável para escolher tipo de missão
                int randomType = Random.Range(0, missionType.Length);

                // verifica qual tipo de missão foi sorteada e adiciona componente de acordo
                if (randomType == (int)MissionType.SingleRun)
                {
                    // adiciona componente singlerun ao game object, new Mission
                    missions[i] = newMission.AddComponent<SingleRun>();
                }
                else if (randomType == (int)MissionType.TotalMeters)
                {
                    // adiciona componente TotalMeters ao game object, new Mission
                    missions[i] = newMission.AddComponent<TotalMeters>();
                }
                else if (randomType == (int)MissionType.SingleRunCollect)
                {
                    // adiciona componente SingleRunCollect ao game object, new Mission
                    missions[i] = newMission.AddComponent<SingleRunCollect>();
                }

                // chama função para indicar criação e definir valores
                missions[i].Created();
            }
        }
    }

    // função para salvar dados
    public void Save()
    {
        // instancia um formatador binário para binarizar os dados
        BinaryFormatter bf = new BinaryFormatter();

        // cria arquivo para salvar dados no caminho indicado em path
        FileStream file = File.Create(path);

        // cria variáveis para dados a serem salvos na classe criada
        PlayerData data = new PlayerData();
        data.coins = coins;
        data.objective = new int[missions.Length];
        data.progress = new int[missions.Length];
        data.currentProgress = new int[missions.Length];
        data.reward = new int[missions.Length];
        data.missionType = new string[missions.Length];
        data.charactersCost = new int[charactersCost.Length];

        // loop para salvar os dados nas variáveis
        for (int i = 0; i < missions.Length; i++)
        {
            data.objective[i] = missions[i].objective;
            data.progress[i] = missions[i].progress;
            data.currentProgress[i] = missions[i].currentProgress;
            data.reward[i] = missions[i].reward;
            data.missionType[i] = missions[i].missionType.ToString();
        }

        // armazenar custos dos personagens
        for (int i = 0; i < charactersCost.Length; i++)
        {
            data.charactersCost[i] = charactersCost[i];
        }

        // serializa os dados e guarda no arquivo criado
        bf.Serialize(file, data);

        // fecha o arquivo
        file.Close();
    }

    // função para carregar dados do arquivo
    void Load()
    {
        // instancia um formatador binário para trabalhar com dados binarizos
        BinaryFormatter bf = new BinaryFormatter();

        // abre arquivo
        FileStream file = File.Open(path, FileMode.Open);

        // deserializa os dados do arquivo
        PlayerData data = (PlayerData)bf.Deserialize(file);

        // fecha arquivo, pois já extraiu os dados
        file.Close();

        // extrai os dados
        coins = data.coins;

        // deve-se criar novas missões a partir dos dados, não basta pegar os dados e atribuir
        for (int i = 0; i < missions.Length; i++)
        {
            // instancia nova missão
            GameObject newMission = new GameObject("Mission " + i);

            // define game manager como pai
            newMission.transform.SetParent(transform);

            // verifica qual tipo de missão e adiciona componente
            if (data.missionType[i] == MissionType.SingleRun.ToString())
            {
                // adiciona componente singlerun ao game object, new Mission
                missions[i] = newMission.AddComponent<SingleRun>();
                missions[i].missionType = MissionType.SingleRun;
            }
            else if (data.missionType[i] == MissionType.TotalMeters.ToString())
            {
                // adiciona componente TotalMeters ao game object, new Mission
                missions[i] = newMission.AddComponent<TotalMeters>();
                missions[i].missionType = MissionType.TotalMeters;
            }
            else if (data.missionType[i] == MissionType.SingleRunCollect.ToString())
            {
                // adiciona componente SingleRunCollect ao game object, new Mission
                missions[i] = newMission.AddComponent<SingleRunCollect>();
                missions[i].missionType = MissionType.SingleRunCollect;
            }

            missions[i].objective = data.objective[i];
            missions[i].progress = data.progress[i];
            missions[i].currentProgress = data.currentProgress[i];
            missions[i].reward = data.reward[i];
        }

        // carregar custos dos personagens
        for (int i = 0; i < data.charactersCost.Length - 1; i++)
        {
            charactersCost[i] = data.charactersCost[i];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // função para chamar cena do jogo em si
    public void StartRun(int cIndex)
    {
        // pega qual index do caractere selecionado
        charIndex = cIndex;
        SceneManager.LoadScene("Game");
    }

    // função para chamar menu
    public void EndRun()
    {
        SceneManager.LoadScene("Menu");
    }

    // função que retorna qual a missão de acordo com o índice
    public MissionBase GetMission(int index)
    {
        return missions[index];
    }

    // função para indicar que uma nova run foi iniciada
    public void StartMissions()
    {
        // passa por todas as missões para chamar a função que indica inicio de uma nova run
        for (int i = 0; i < GameManager.gm.missions.Length; i++)
        {
            missions[i].StartRun();
        }
    }

    // função para substituir missão ao concluir outra
    public void GenerateMission(int i)
    {
        // destroi missão concluida
        Destroy(missions[i].gameObject);

        // cria missão para substituir a destruida
        // cria um novo objeto para cada missão
        GameObject newMission = new GameObject("Mission" + i);

        // atribui gm como parent de cada mission
        newMission.transform.SetParent(transform);

        // define variável com tipos de missões para sortear
        MissionType[] missionType = { MissionType.SingleRun, MissionType.TotalMeters, MissionType.SingleRunCollect };

        // variável para escolher tipo de missão
        int randomType = Random.Range(0, missionType.Length);

        // verifica qual tipo de missão foi sorteada e adiciona componente de acordo
        if (randomType == (int)MissionType.SingleRun)
        {
            // adiciona componente singlerun ao game object, new Mission
            missions[i] = newMission.AddComponent<SingleRun>();
        }
        else if (randomType == (int)MissionType.TotalMeters)
        {
            // adiciona componente TotalMeters ao game object, new Mission
            missions[i] = newMission.AddComponent<TotalMeters>();
        }
        else if (randomType == (int)MissionType.SingleRunCollect)
        {
            // adiciona componente SingleRunCollect ao game object, new Mission
            missions[i] = newMission.AddComponent<SingleRunCollect>();
        }

        // chama função para indicar criação e definir valores
        missions[i].Created();

        // atualiza missões no menu
        FindAnyObjectByType<Menu>().SetMission();
    }
}
