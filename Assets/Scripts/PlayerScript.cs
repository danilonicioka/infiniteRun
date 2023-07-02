using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // refer�ncia ao componente rigidbody
    private Rigidbody rb;

    // referencia ao box collider -> ao dar slide, diminui a hitbox
    private BoxCollider bc;

    // vari�vel para controlar velocidade que o personagem vai pra frente
    public float speed;

    // velocidade de mudan�a de pista fixa em td run
    public float laneSpeed = 10;

    // dist�ncia total do pulo na horizontal
    public float jumpLenght;

    // altura total do pulo
    public float jumpHeight;

    // referencia ao animator
    private Animator animator;

    // vari�vel que indica em qual pista o personagem est�
    private int currentLane = 1;

    // vari�vel que indica posi��o do personagem
    private Vector3 verticalTargetPosition;

    // verifica se j� est� pulando para n�o ficar pulando infinitamente
    private bool jumping = false;

    // verifica se j� est� sliding para n�o ficar pulando infinitamente
    private bool sliding = false;

    // posi��o inicial antes do pulo
    private float jumpStart;

    // posi��o inicial antes do slide
    private float slideStart;

    // dist�ncia total do slide
    public float slideLenght;

    // armazena tamanho original do box collider pra voltar depois do slide
    private Vector3 bcOriginalSize;

    /*
    // vari�veis para controles de celular
    // verifica se t� deslizando o dedo na tela
    private bool isSwipping = false;

    // armazena posi��o inicial do toque para verificar para qual lado deslizou a partir dele
    // 2D, pois � na tela
    private Vector2 initialTouch;
    */

    // vari�vel para armazenar vida total
    public int maxLife = 3;

    // vari�vel para armazenar vida atual
    private int currentLife;

    // velocidade inicial do jogo
    public float initialSpeed = 10f;

    // armazena velocidade atual para, ao colidir, reduzir e voltar a anterior
    public float currentSpeed;

    // vari�vel para verificar se t� invulner�vel
    private bool isInvulnerable = false;

    // tempo que vai ficar invulner�vel
    public float invulnerableTime = 2f;

    // refer�ncia ao pr�prio gameObject (character)
    public GameObject character;

    // para mexer com o UiManager definido no script
    public UiManager uiManager;

    // vari�vel para verificar se personagem pode ou n�o se mover (levar dano, morrer, etc)
    public bool canMove;

    // vari�vel para contar moedas
    // oculta para n�o ser alterada diretamente no Unity
    [HideInInspector]
    public int coins;

    // vari�vel para armazenar score
    [HideInInspector]
    public float score;

    // Start is called before the first frame update
    void Start()
    {
        // inicia parado
        speed = 0;

        // inicialmente n�o pode se mexer, s� quando come�ar a correr
        canMove = false;

        // indica que a rigidbody em rb � a componente do pr�prio objeto
        rb = GetComponent<Rigidbody>();

        // indica que o animator � componente do filho(charactor)
        animator = GetComponentInChildren<Animator>();

        bc = GetComponent<BoxCollider>();

        character = GameObject.FindGameObjectWithTag("Character");

        // armazena tamanho original do box collider
        bcOriginalSize = bc.size;


        // inicia jogo com vida total
        currentLife = maxLife;

        // pega o ui manager definido
        uiManager = FindAnyObjectByType<UiManager>();

        // inicia miss�es
        GameManager.gm.StartMissions();

        // chama fun��o para come�ar a correr ap�s certo tempo
        Invoke("StartRun", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        // se n�o puder se mover, sa� do update
        if (!canMove)
        {
            return;
        }

        // soma score a medida que avan�a
        score += Time.deltaTime * speed;

        // atualiza texto a partir da contagem do score
        uiManager.UpdateScore((int)score);

        // controles para pc -> setas do teclado
        if (Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            ChangeLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
        }
        else if ( Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide();
        }

        /*
        // controles para celular
        
        // verifica se h� apenas um toque na tela
        if (Input.touchCount == 1)
        {
            // e se estiver deslizando
            if (isSwipping)
            {
                // calcula onde est� tocando a partir do toque inicial
                Vector2 swipeDir = Input.GetTouch(0).position - initialTouch;

                // atualiza esse valor a partir do tamanho da tela do celular
                swipeDir = new Vector2(swipeDir.x / Screen.width, swipeDir.y / Screen.height);

                // se a magnitude alterou, significa que houve swipe para alguma dire��o
                if (swipeDir.magnitude > 0.01f)
                {
                    // verifica em qual dire��o houve maior swipe para determinar se � pulo, slide ou trocar a pista
                    if(Mathf.Abs(swipeDir.y) > Mathf.Abs(swipeDir.x))
                    {
                        // se o valor for menor, quer dizer que foi pra baixo -> slide
                        if(swipeDir.y < 0)
                        {
                            Slide();
                        } else
                        {
                            Jump();
                        }
                    }
                    // se x for maior, troca a pista
                    else
                    {
                        // se o valor for menor, foi pra esquerda
                        if (swipeDir.x < 0)
                        {
                            ChangeLane(-1);
                        }
                        else
                        {
                            ChangeLane(1);
                        }
                    }

                    // ap�s uma a��o, termina o swipe
                    isSwipping = false;
                }
            }

            // verifica se ainda t� tocando na tela
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                initialTouch = Input.GetTouch(0).position;
                isSwipping = true;
            }
            // verifica se n�o t� mais tocando
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                isSwipping = false;
            }
        }
        */

        // atualizar posi��o de acordo com o pulo
        if (jumping)
        {
            // vari�vel para verificar se pulo j� acabou
            float ratio = (transform.position.z - jumpStart) / jumpLenght;

            // se for verdadeiro, o pulo j� terminou
            if (ratio >= 1f)
            {
                jumping = false;
                animator.SetBool("Jumping", false);
            }
            // se n�o for, atualiza a posi��o do personagem
            else
            {
                // obtido no projeto da Unity
                verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
            }
        }
        // Como o boneco n�o possui gravidade, deve ter um m�todo para puxar ele pra baixo
        else
        {
            // move da posi��o atual y para 0 em uma velocidade 4 independente dos frames
            verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, 5 * Time.deltaTime);
        }

        

        if (sliding)
        {
            // vari�vel para verificar se slide j� acabou
            float ratio = (transform.position.z - slideStart) / slideLenght;

            if(ratio >= 1f)
            {
                sliding = false;
                animator.SetBool("Sliding", false);
                bc.size = bcOriginalSize;
            }
        }

        // vetor para indicar nova posi��o
        Vector3 targetPosition = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);

        // fun��o que indica posi��o original, aonde vai e em qual velocidade
        // multiplica por time.deltatime para ser independente dos frames
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);

        // armazena velocidade atual quando n�o tiver reduzida
        if (!isInvulnerable)
        {
            currentSpeed = speed;
        }
    }

    // chamada em um tempo fixo (default = 0.02 s), o update � a cada frame
    private void FixedUpdate()
    {
        // atualiza velocidade a partir do rb com um vetor (0,0,1)*speed
        rb.velocity = Vector3.forward * speed;
    }

    // fun��o para come�ar a correr
    void StartRun()
    {
        // inicia anima��o de correr, a que est� conectada �s outras anima��es
        animator.Play("runStart");

        // inicia jogo com velocidade definida em initialSpeed
        speed = initialSpeed;

        // 
        canMove = true;
    }

    // fun��o para habilitar movimento
    void CanMove()
    {
        canMove = true;
    }

    // fun��o para alterar pista do personagem
    void ChangeLane(int direction) 
    {
        // verifica nova lane a partir da dire��o que andou
        int targetLane = currentLane + direction;

        // para n�o mudar al�m das 3 lanes 0, 1 e 2
        if (targetLane < 0 || targetLane > 2)
        {
            return;
        }

        // altera pista
        currentLane = targetLane;

        // atualiza valor para 0 (currentLane come�a em 1
        verticalTargetPosition = new Vector3((currentLane - 1),0,0);
    }

    // Fun��o para atualizar valores da anima��o de pular
    void Jump()
    {
        if (!jumping)
        {
            // altura inicial do pulo � a que o personagem est� 
            jumpStart = transform.position.z;

            // define a Jump Speed, vari�vel do animator
            animator.SetFloat("JumpSpeed", speed / jumpLenght);

            // altera a vari�vel Jumping do animator para true
            animator.SetBool("Jumping", true);

            // vari�vel usada para permitir pulo
            jumping = true;
        }
    }

    // Fun��o para atualizar valores da anima��o de slide e a hitbox do personagem
    void Slide()
    {
        if (!sliding && !jumping) 
        {
            slideStart = transform.position.z;
            animator.SetFloat("JumpSpeed", speed / slideLenght);
            animator.SetBool("Sliding", true);
            Vector3 slideSize = bc.size;
            slideSize.y /= 2;
            bc.size = slideSize;
            sliding = true;
        }
    }

    // fun��o para atualizar vida ao colidir com um obst�culo e verificar se deu game over
    private void OnTriggerEnter(Collider other)
    {
        // verifica se o objeto foi uma moeda
        if (other.CompareTag("Coin"))
        {
            coins++;
            uiManager.UpdateCoins(coins);
            other.transform.parent.gameObject.SetActive(false);
        }

        // se estiver invulner�vel, sai da fun��o -> n�o toma outro dano
        if (isInvulnerable)
        {
            return;
        }

        // verifica se a colis�o foi com um obst�culo (objeto com tag obstacle)
        if(other.CompareTag("Obstacle"))
        {
            // logo ao tomar dano, n�o poder� se mover
            canMove = false;

            // diminui vida atual
            currentLife--;

            // atualiza ui com os cora��es
            uiManager.UpdateLives(currentLife);

            // envia trigger para anima��o de levar dano
            animator.SetTrigger("Hit");

            // reduz a velocidade atual
            speed = currentSpeed / 3;

            // verifica se j� n�o possui mais vidas
            if (currentLife<=0) 
            {
                // game over
                speed = 0;

                // anima��o de morte
                animator.SetBool("Dead", true);

                // exibe painel de game over
                uiManager.gameOverPanel.SetActive(true);

                // chama fun��o que exibe menu ap�s um tempo
                Invoke("CallMenu", 2f);
            }
            // se n�o morreu, fica invulner�vel e reduz a velocidade por um tempo
            else
            {
                CanMove();
                StartCoroutine(Blinking(invulnerableTime));
            }
        }
    }

    // co-rotina para ficar invulner�vel quando tomar dano
    IEnumerator Blinking(float invul_time)
    {
        isInvulnerable = true;

        // vari�vel para servir como timer para invulnerabilidade
        float timer = 0;

        // tempo do blink atual
        float currentBlink = 1f;

        // tempo do �ltimo blink
        float lastBlink = 0;

        // frequ�ncia do blink
        float blinkPeriod = 0.1f;

        // vari�vel para ativar e desativar personagem (efeito de blink)
        bool enabled = false;

        // tempo que vai durar a invulnerabilidade
        yield return new WaitForSeconds(1f);

        // velocidade volta ao normal ap�s o tempo
        speed = currentSpeed;

        // loop para contar tempo
        while(timer < invul_time && isInvulnerable)
        {
            // alterna personagem para habilitado e desabilitado
            character.SetActive(enabled);

            // ao retorna null, ele espera por 1 frame apenas
            yield return null;

            // incrementa timer
            timer += Time.deltaTime;

            // atualiza �ltimo blink
            lastBlink += Time.deltaTime;

            //
            if(blinkPeriod < lastBlink)
            {
                lastBlink = 0;
                currentBlink = 1f - currentBlink;
                enabled = !enabled;
            }
        }

        character.SetActive(true);
        isInvulnerable = false;

    }

    // fun��o para chamar menu
    void CallMenu()
    {
        // antes de chamar menu, atualiza quantidade de moedas
        GameManager.gm.coins += coins;

        GameManager.gm.EndRun();
    }

    // fun��o para aumentar a velocidade a medida que decorre o jogo
    public void IncreaseSpeed()
    {
        // aumenta em 15%
        speed *= 1.10f;
    }
}
