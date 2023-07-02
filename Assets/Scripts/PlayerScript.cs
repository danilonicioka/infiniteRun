using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // referência ao componente rigidbody
    private Rigidbody rb;

    // referencia ao box collider -> ao dar slide, diminui a hitbox
    private BoxCollider bc;

    // variável para controlar velocidade que o personagem vai pra frente
    public float speed;

    // velocidade de mudança de pista fixa em td run
    public float laneSpeed = 10;

    // distância total do pulo na horizontal
    public float jumpLenght;

    // altura total do pulo
    public float jumpHeight;

    // referencia ao animator
    private Animator animator;

    // variável que indica em qual pista o personagem está
    private int currentLane = 1;

    // variável que indica posição do personagem
    private Vector3 verticalTargetPosition;

    // verifica se já está pulando para não ficar pulando infinitamente
    private bool jumping = false;

    // verifica se já está sliding para não ficar pulando infinitamente
    private bool sliding = false;

    // posição inicial antes do pulo
    private float jumpStart;

    // posição inicial antes do slide
    private float slideStart;

    // distância total do slide
    public float slideLenght;

    // armazena tamanho original do box collider pra voltar depois do slide
    private Vector3 bcOriginalSize;

    /*
    // variáveis para controles de celular
    // verifica se tá deslizando o dedo na tela
    private bool isSwipping = false;

    // armazena posição inicial do toque para verificar para qual lado deslizou a partir dele
    // 2D, pois é na tela
    private Vector2 initialTouch;
    */

    // variável para armazenar vida total
    public int maxLife = 3;

    // variável para armazenar vida atual
    private int currentLife;

    // velocidade inicial do jogo
    public float initialSpeed = 10f;

    // armazena velocidade atual para, ao colidir, reduzir e voltar a anterior
    public float currentSpeed;

    // variável para verificar se tá invulnerável
    private bool isInvulnerable = false;

    // tempo que vai ficar invulnerável
    public float invulnerableTime = 2f;

    // referência ao próprio gameObject (character)
    public GameObject character;

    // para mexer com o UiManager definido no script
    public UiManager uiManager;

    // variável para verificar se personagem pode ou não se mover (levar dano, morrer, etc)
    public bool canMove;

    // variável para contar moedas
    // oculta para não ser alterada diretamente no Unity
    [HideInInspector]
    public int coins;

    // variável para armazenar score
    [HideInInspector]
    public float score;

    // Start is called before the first frame update
    void Start()
    {
        // inicia parado
        speed = 0;

        // inicialmente não pode se mexer, só quando começar a correr
        canMove = false;

        // indica que a rigidbody em rb é a componente do próprio objeto
        rb = GetComponent<Rigidbody>();

        // indica que o animator é componente do filho(charactor)
        animator = GetComponentInChildren<Animator>();

        bc = GetComponent<BoxCollider>();

        character = GameObject.FindGameObjectWithTag("Character");

        // armazena tamanho original do box collider
        bcOriginalSize = bc.size;


        // inicia jogo com vida total
        currentLife = maxLife;

        // pega o ui manager definido
        uiManager = FindAnyObjectByType<UiManager>();

        // inicia missões
        GameManager.gm.StartMissions();

        // chama função para começar a correr após certo tempo
        Invoke("StartRun", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        // se não puder se mover, saí do update
        if (!canMove)
        {
            return;
        }

        // soma score a medida que avança
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
        
        // verifica se há apenas um toque na tela
        if (Input.touchCount == 1)
        {
            // e se estiver deslizando
            if (isSwipping)
            {
                // calcula onde está tocando a partir do toque inicial
                Vector2 swipeDir = Input.GetTouch(0).position - initialTouch;

                // atualiza esse valor a partir do tamanho da tela do celular
                swipeDir = new Vector2(swipeDir.x / Screen.width, swipeDir.y / Screen.height);

                // se a magnitude alterou, significa que houve swipe para alguma direção
                if (swipeDir.magnitude > 0.01f)
                {
                    // verifica em qual direção houve maior swipe para determinar se é pulo, slide ou trocar a pista
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

                    // após uma ação, termina o swipe
                    isSwipping = false;
                }
            }

            // verifica se ainda tá tocando na tela
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                initialTouch = Input.GetTouch(0).position;
                isSwipping = true;
            }
            // verifica se não tá mais tocando
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                isSwipping = false;
            }
        }
        */

        // atualizar posição de acordo com o pulo
        if (jumping)
        {
            // variável para verificar se pulo já acabou
            float ratio = (transform.position.z - jumpStart) / jumpLenght;

            // se for verdadeiro, o pulo já terminou
            if (ratio >= 1f)
            {
                jumping = false;
                animator.SetBool("Jumping", false);
            }
            // se não for, atualiza a posição do personagem
            else
            {
                // obtido no projeto da Unity
                verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
            }
        }
        // Como o boneco não possui gravidade, deve ter um método para puxar ele pra baixo
        else
        {
            // move da posição atual y para 0 em uma velocidade 4 independente dos frames
            verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, 5 * Time.deltaTime);
        }

        

        if (sliding)
        {
            // variável para verificar se slide já acabou
            float ratio = (transform.position.z - slideStart) / slideLenght;

            if(ratio >= 1f)
            {
                sliding = false;
                animator.SetBool("Sliding", false);
                bc.size = bcOriginalSize;
            }
        }

        // vetor para indicar nova posição
        Vector3 targetPosition = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);

        // função que indica posição original, aonde vai e em qual velocidade
        // multiplica por time.deltatime para ser independente dos frames
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);

        // armazena velocidade atual quando não tiver reduzida
        if (!isInvulnerable)
        {
            currentSpeed = speed;
        }
    }

    // chamada em um tempo fixo (default = 0.02 s), o update é a cada frame
    private void FixedUpdate()
    {
        // atualiza velocidade a partir do rb com um vetor (0,0,1)*speed
        rb.velocity = Vector3.forward * speed;
    }

    // função para começar a correr
    void StartRun()
    {
        // inicia animação de correr, a que está conectada às outras animações
        animator.Play("runStart");

        // inicia jogo com velocidade definida em initialSpeed
        speed = initialSpeed;

        // 
        canMove = true;
    }

    // função para habilitar movimento
    void CanMove()
    {
        canMove = true;
    }

    // função para alterar pista do personagem
    void ChangeLane(int direction) 
    {
        // verifica nova lane a partir da direção que andou
        int targetLane = currentLane + direction;

        // para não mudar além das 3 lanes 0, 1 e 2
        if (targetLane < 0 || targetLane > 2)
        {
            return;
        }

        // altera pista
        currentLane = targetLane;

        // atualiza valor para 0 (currentLane começa em 1
        verticalTargetPosition = new Vector3((currentLane - 1),0,0);
    }

    // Função para atualizar valores da animação de pular
    void Jump()
    {
        if (!jumping)
        {
            // altura inicial do pulo é a que o personagem está 
            jumpStart = transform.position.z;

            // define a Jump Speed, variável do animator
            animator.SetFloat("JumpSpeed", speed / jumpLenght);

            // altera a variável Jumping do animator para true
            animator.SetBool("Jumping", true);

            // variável usada para permitir pulo
            jumping = true;
        }
    }

    // Função para atualizar valores da animação de slide e a hitbox do personagem
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

    // função para atualizar vida ao colidir com um obstáculo e verificar se deu game over
    private void OnTriggerEnter(Collider other)
    {
        // verifica se o objeto foi uma moeda
        if (other.CompareTag("Coin"))
        {
            coins++;
            uiManager.UpdateCoins(coins);
            other.transform.parent.gameObject.SetActive(false);
        }

        // se estiver invulnerável, sai da função -> não toma outro dano
        if (isInvulnerable)
        {
            return;
        }

        // verifica se a colisão foi com um obstáculo (objeto com tag obstacle)
        if(other.CompareTag("Obstacle"))
        {
            // logo ao tomar dano, não poderá se mover
            canMove = false;

            // diminui vida atual
            currentLife--;

            // atualiza ui com os corações
            uiManager.UpdateLives(currentLife);

            // envia trigger para animação de levar dano
            animator.SetTrigger("Hit");

            // reduz a velocidade atual
            speed = currentSpeed / 3;

            // verifica se já não possui mais vidas
            if (currentLife<=0) 
            {
                // game over
                speed = 0;

                // animação de morte
                animator.SetBool("Dead", true);

                // exibe painel de game over
                uiManager.gameOverPanel.SetActive(true);

                // chama função que exibe menu após um tempo
                Invoke("CallMenu", 2f);
            }
            // se não morreu, fica invulnerável e reduz a velocidade por um tempo
            else
            {
                CanMove();
                StartCoroutine(Blinking(invulnerableTime));
            }
        }
    }

    // co-rotina para ficar invulnerável quando tomar dano
    IEnumerator Blinking(float invul_time)
    {
        isInvulnerable = true;

        // variável para servir como timer para invulnerabilidade
        float timer = 0;

        // tempo do blink atual
        float currentBlink = 1f;

        // tempo do último blink
        float lastBlink = 0;

        // frequência do blink
        float blinkPeriod = 0.1f;

        // variável para ativar e desativar personagem (efeito de blink)
        bool enabled = false;

        // tempo que vai durar a invulnerabilidade
        yield return new WaitForSeconds(1f);

        // velocidade volta ao normal após o tempo
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

            // atualiza último blink
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

    // função para chamar menu
    void CallMenu()
    {
        // antes de chamar menu, atualiza quantidade de moedas
        GameManager.gm.coins += coins;

        GameManager.gm.EndRun();
    }

    // função para aumentar a velocidade a medida que decorre o jogo
    public void IncreaseSpeed()
    {
        // aumenta em 15%
        speed *= 1.10f;
    }
}
