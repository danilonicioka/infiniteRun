using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// para armazenar os tipos de missão
public enum MissionType
{
    SingleRun, TotalMeters, SingleRunCollect
}

// abstrata pois será utilizada apenas para criar as missões em si
public abstract class MissionBase : MonoBehaviour
{
    // objetivo da missão (valor a ser atingido)
    public int objective;

    // progresso da missão
    public int progress;

    // recompensa por completar missão
    public int reward;

    // referência ao player
    public PlayerScript player;

    // armazena progresso de uma missão que pode continuar em outra run
    public int currentProgress;

    // armazenar tipo da missão
    public MissionType missionType;
    
    // função chamada quando a missão é criada
    public abstract void Created();

    // para retorna descrição da missão
    public abstract string GetMissionDescription();

    // quando o game iniciar
    public abstract void StartRun();

    // para atualizar status das missões
    public abstract void Update();

    // verifica se missão foi concluída
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

// missão para correr em uma única run
public class SingleRun : MissionBase
{
    public override void Created()
    {
        // guarda tipo da missão
        missionType = MissionType.SingleRun;

        // valores máximos como objetivos das missões
        int[] maxValues = { 1000, 2000, 3000, 4000 };

        // pega um dos valores aleatoriamente
        int randomMaxValue = Random.Range(0, maxValues.Length);

        // recompensas
        int[] rewards = { 100, 200, 300, 400 };

        // define recompensa e objetivo a partir do valor máximo obtido
        reward = rewards[randomMaxValue];

        objective = maxValues[randomMaxValue];
    }

    public override string GetMissionDescription()
    {
        return "Corra " + objective + "m em uma única corrida";
    }

    public override void StartRun()
    {
        // ao iniciar uma corrida, deve-se resetar, pois a missão deve ser feita em uma run
        progress = 0;

        // pega referencia do player
        player = FindAnyObjectByType<PlayerScript>();
    }

    public override void Update()
    {
        // se estiver no menu, não existe player e sai da função
        if (player == null)
        {
            return;
        }

        progress = (int)player.score;
    }
}

// missão para correr um total em várias runs
public class TotalMeters : MissionBase
{
    public override void Created()
    {
        // guarda tipo da missão
        missionType = MissionType.TotalMeters;

        // valores máximos como objetivos das missões
        int[] maxValues = { 10000, 20000, 30000, 40000 };

        // pega um dos valores aleatoriamente
        int randomMaxValue = Random.Range(0, maxValues.Length);

        // recompensas
        int[] rewards = { 1000, 2000, 3000, 4000 };

        // define recompensa e objetivo a partir do valor máximo obtido
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
        // se estiver no menu, não existe player e sai da função
        if (player == null)
        {
            return;
        }

        currentProgress = (int)player.score;
    }
}

// missão para coletar moedas em uma única corrida
public class SingleRunCollect : MissionBase
{
    public override void Created()
    {
        // guarda tipo da missão
        missionType = MissionType.SingleRunCollect;

        // valores máximos como objetivos das missões
        int[] maxValues = { 100, 200, 300, 400, 500 };

        // pega um dos valores aleatoriamente
        int randomMaxValue = Random.Range(0, maxValues.Length);

        // recompensas
        int[] rewards = { 100, 200, 300, 400, 500 };

        // define recompensa e objetivo a partir do valor máximo obtido
        reward = rewards[randomMaxValue];

        objective = maxValues[randomMaxValue];
    }

    public override string GetMissionDescription()
    {
        return "Colete " + objective + " peixes em uma única corrida";
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
        // se estiver no menu, não existe player e sai da função
        if (player == null)
        {
            return;
        }

        progress = (int)player.coins;
    }
}