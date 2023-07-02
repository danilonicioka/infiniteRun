using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// para armazenar os tipos de miss�o
public enum MissionType
{
    SingleRun, TotalMeters, SingleRunCollect
}

// abstrata pois ser� utilizada apenas para criar as miss�es em si
public abstract class MissionBase : MonoBehaviour
{
    // objetivo da miss�o (valor a ser atingido)
    public int objective;

    // progresso da miss�o
    public int progress;

    // recompensa por completar miss�o
    public int reward;

    // refer�ncia ao player
    public PlayerScript player;

    // armazena progresso de uma miss�o que pode continuar em outra run
    public int currentProgress;

    // armazenar tipo da miss�o
    public MissionType missionType;
    
    // fun��o chamada quando a miss�o � criada
    public abstract void Created();

    // para retorna descri��o da miss�o
    public abstract string GetMissionDescription();

    // quando o game iniciar
    public abstract void StartRun();

    // para atualizar status das miss�es
    public abstract void Update();

    // verifica se miss�o foi conclu�da
    public bool GetMissionStatus()
    {
        if ((progress + currentProgress) >= objective)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

// miss�o para correr em uma �nica run
public class SingleRun : MissionBase
{
    public override void Created()
    {
        // guarda tipo da miss�o
        missionType = MissionType.SingleRun;

        // valores m�ximos como objetivos das miss�es
        int[] maxValues = { 1000, 2000, 3000, 4000 };

        // pega um dos valores aleatoriamente
        int randomMaxValue = Random.Range(0, maxValues.Length);

        // recompensas
        int[] rewards = { 100, 200, 300, 400 };

        // define recompensa e objetivo a partir do valor m�ximo obtido
        reward = rewards[randomMaxValue];

        objective = maxValues[randomMaxValue];
    }

    public override string GetMissionDescription()
    {
        return "Corra " + objective + "m em uma �nica corrida";
    }

    public override void StartRun()
    {
        // ao iniciar uma corrida, deve-se resetar, pois a miss�o deve ser feita em uma run
        progress = 0;

        // pega referencia do player
        player = FindAnyObjectByType<PlayerScript>();
    }

    public override void Update()
    {
        // se estiver no menu, n�o existe player e sai da fun��o
        if (player == null)
        {
            return;
        }

        progress = (int)player.score;
    }
}

// miss�o para correr um total em v�rias runs
public class TotalMeters : MissionBase
{
    public override void Created()
    {
        // guarda tipo da miss�o
        missionType = MissionType.TotalMeters;

        // valores m�ximos como objetivos das miss�es
        int[] maxValues = { 10000, 20000, 30000, 40000 };

        // pega um dos valores aleatoriamente
        int randomMaxValue = Random.Range(0, maxValues.Length);

        // recompensas
        int[] rewards = { 1000, 2000, 3000, 4000 };

        // define recompensa e objetivo a partir do valor m�ximo obtido
        reward = rewards[randomMaxValue];

        objective = maxValues[randomMaxValue];
    }

    public override string GetMissionDescription()
    {
        return "Corra um total de " + objective + "m";
    }

    public override void StartRun()
    {
        // ao iniciar uma corrida, guarda o progresso atual
        progress += currentProgress;

        // pega referencia do player
        player = FindAnyObjectByType<PlayerScript>();
    }

    public override void Update()
    {
        // se estiver no menu, n�o existe player e sai da fun��o
        if (player == null)
        {
            return;
        }

        currentProgress = (int)player.score;
    }
}

// miss�o para coletar moedas em uma �nica corrida
public class SingleRunCollect : MissionBase
{
    public override void Created()
    {
        // guarda tipo da miss�o
        missionType = MissionType.SingleRunCollect;

        // valores m�ximos como objetivos das miss�es
        int[] maxValues = { 100, 200, 300, 400, 500 };

        // pega um dos valores aleatoriamente
        int randomMaxValue = Random.Range(0, maxValues.Length);

        // recompensas
        int[] rewards = { 100, 200, 300, 400, 500 };

        // define recompensa e objetivo a partir do valor m�ximo obtido
        reward = rewards[randomMaxValue];

        objective = maxValues[randomMaxValue];
    }

    public override string GetMissionDescription()
    {
        return "Colete " + objective + " peixes em uma �nica corrida";
    }

    public override void StartRun()
    {
        // ao iniciar uma corrida, guarda o progresso atual
        progress = 0;

        // pega referencia do player
        player = FindAnyObjectByType<PlayerScript>();
    }

    public override void Update()
    {
        // se estiver no menu, n�o existe player e sai da fun��o
        if (player == null)
        {
            return;
        }

        progress = (int)player.coins;
    }
}