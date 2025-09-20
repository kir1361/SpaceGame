using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] private GameObject[] flames;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Sprite normalSprite;

    [SerializeField] Transform ShootPos;
    [SerializeField] Transform ShootPos1;
    public float targetAspect = 3.85f / 5f;
    public static bool IsPlayerDead = false;
    float ShootTimer;
    [SerializeField] float TimeBetweenShoot;
    [SerializeField] float TimeBetweenLaserShoot;
    GameObject laserBeam;


    //public int Health;
    // public HealthUI healthUI;
    public int maxHealth = 30;
    public int maxShield = 30;
    public int currentHealth;
    public int currentShield;

    public float shieldRechargeDelay = 20f;
    public float shieldRechargeRate = 10f;
    private bool isRechargingShield = false;
    private bool isShieldRecharged = false;

    public static Player instance;
    Animator Anim;
    public TextMeshProUGUI CoinsText;
    public int currentCoins;
    public Rigidbody2D rb;
    public Collider2D cl;
    Vector2 moveVelocity;

    public bool hasShield = false;
    public GameObject Shield;
    //private bool hasRocketLauncher = false;
    public SpriteRenderer spriteRenderer;

    public bool canMove = false;
    public bool canShoot = false;
    public bool isActiveCoin2xBuff = false;
    public bool isActiveDashBuff = false;

    public Image BorderPortrait;
    public CanvasGroup Portrait;
    [SerializeField] private GameObject laserBeamPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float laserDuration;
    public bool hasLaser = false;
    private bool isFiring = false;
    private bool isCanBeDamaged = false;
    private bool canDash = false;
    public bool isDashing = false;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2f;


    [Header("Visual Effects")]
    [SerializeField] private TrailRenderer dashTrail;
    [SerializeField] private ParticleSystem dashParticles;

    private void Start()
    {
        CoinsText.text = currentCoins.ToString();
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cl = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Shield.SetActive(false);
        currentHealth = maxHealth;
        ShootTimer = TimeBetweenShoot;
        IsPlayerDead = false;
    }
    public void RestoreShield()
    {
        if (hasShield && currentShield > 0)
        {
            Shield.transform.localScale = Vector3.zero;
            Shield.SetActive(true);
            Shield.transform.DOScale(1.4f, 0.5f)
             .SetEase(Ease.OutBack).SetLink(Shield, LinkBehaviour.KillOnDestroy)
             .OnComplete(() =>
             {
                 Shield.transform.DOScale(1.2f, 0.5f)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutSine)
                     .From(1.4f).SetLink(Shield, LinkBehaviour.KillOnDestroy);
             });
            // Обновляем UI
            // if (HealthUI.instance != null)
            // {
            //     HealthUI.instance.ShieldCheck(hasShield);
            //     HealthUI.instance.SetupUIShield();
            //     HealthUI.instance.UpdateShieldUI(currentShield);
            // }
        }
    }
    void OnEnable()
    {
        Anim.Rebind();
        Anim.Update(0);

        spriteRenderer.sprite = normalSprite;
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        IsPlayerDead = false;
    }

    public void SetPlayerControl(bool canMove, bool canShoot, bool isCanBeDamaged, bool gravityOn, Vector2 velocity, bool canDash = false)
    {
        instance.canMove = canMove;
        instance.canShoot = canShoot;
        instance.isCanBeDamaged = isCanBeDamaged;
        instance.rb.gravityScale = gravityOn ? 4f : 0f;
        instance.rb.velocity = velocity;

    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("Is dashing: " + isDashing);
        Debug.Log("CanDash : " + canDash);
        Debug.Log("isActiveDashBuff state: " + isActiveDashBuff);

        ShootTimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && isActiveDashBuff && !isDashing)
        {
            Dash();
        }
        if (Input.GetKeyDown(KeyCode.Space) && ShootTimer >= TimeBetweenShoot && !hasLaser)
        {
            Shoot();
            ShootTimer = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && ShootTimer >= TimeBetweenLaserShoot && hasLaser && !isFiring)
        {
            StartCoroutine(FireLaser());
            ShootTimer = 0;
        }
        CoinsText.text = currentCoins.ToString();
    }
    void FixedUpdate()
    {
        if (IsPlayerDead) return;
        Move();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor") && isCanBeDamaged)
        {
            DamagePlayer(10);
            StartCoroutine(PlayerBlink());
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor") && isCanBeDamaged)
        {
            DamagePlayer(10);
            StartCoroutine(PlayerBlink());
        }
    }

    IEnumerator PlayerBlink()
    {
        SetPlayerControl(true, true, false, true, Vector2.zero);
        float blinkDuration = 0.1f;
        int blinkCount = 10;
        for (int t = 0; t < blinkCount; t++)
        {
            instance.spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
            yield return new WaitForSeconds(blinkDuration);
            instance.spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(blinkDuration);
        }
        SetPlayerControl(true, true, true, true, Vector2.zero);
    }
    void Shoot()
    {
        if (!canShoot) return;
        Instantiate(bullet, ShootPos.position, ShootPos.rotation);
        Instantiate(bullet, ShootPos1.position, ShootPos.rotation);
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.laserSound, volume: 0.5f);
    }
    IEnumerator FireLaser()
    {
        if (!canShoot) yield break;
        if (IsPlayerDead) yield break;
        isFiring = true;
        StartCoroutine(ShakePlayerBeforeLaser());
        yield return new WaitForSeconds(1f);
        laserBeam = Instantiate(laserBeamPrefab, transform);
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.laserSoundEffectClip, volume: 0.15f);
        if (IsPlayerDead)
        {
            DestroyLaserIfExists();
        }
        laserBeam.transform.localPosition = new Vector3(0f, 0.1f, 0f); // чуть выше игрока
        laserBeam.transform.localScale = new Vector3(0f, 1f, 0f);
        yield return new WaitForSeconds(laserDuration);
        if (laserBeam != null)
        {
            Destroy(laserBeam);
        }
        isFiring = false;
    }
    public void DestroyLaserIfExists()
    {
        if (laserBeam != null)
        {
            Destroy(laserBeam);
            laserBeam = null;
        }
        isFiring = false;
    }
    public void EnableLaser()
    {
        hasLaser = true;
    }
    IEnumerator ShakePlayerBeforeLaser()
    {
        float shakeDuration = 1f;
        float shakeMagnitude = 0.1f;
        Vector3 originalPosition = transform.localPosition;

        for (float t = 0; t < shakeDuration; t += Time.deltaTime)
        {
            float xOffset = Random.Range(-shakeMagnitude, shakeMagnitude);
            float yOffset = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0);
            yield return null;
        }
        transform.localPosition = originalPosition;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin") && !isActiveCoin2xBuff)
        {
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.coinBuffSoundEffectClip, volume: 0.05f);
            AddCoins(other.GetComponent<Coins>().coinValue);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Coin") && isActiveCoin2xBuff)
        {
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.coinBuffSoundEffectClip, volume: 0.05f);
            AddCoins(other.GetComponent<Coins>().coinValue * 2);
            Destroy(other.gameObject);
        }
    }
    public void SetFlamesActivePlayer(bool state)
    {
        foreach (GameObject flame in flames)
        {
            flame.SetActive(state);
        }
    }
    void Move()
    {
        if (!canMove) return;
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * speed;
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);

        bool isMoving = moveInput != Vector2.zero;
        if (IsPlayerDead) return;

        SetFlamesActivePlayer(isMoving);
    }
    public void DamagePlayer(int Damage)
    {
        if (!isCanBeDamaged) return;
        if (IsPlayerDead) return;
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.heartSound, volume: 0.2f);

        StartCoroutine(BorderPortraitSwitch());
        if (currentShield > 0)
        {
            currentShield -= Damage;

            HealthUI.instance.UpdateShieldUI(currentShield);
            StartCoroutine(RechargeShield());


            if (currentShield <= 0)
            {
                Shield.transform.DOScale(0f, 0.5f).SetLink(Shield, LinkBehaviour.KillOnDestroy);
                Shield.SetActive(false);
                currentHealth += currentShield;
                currentShield = 0;
                HealthUI.instance.UpdateShieldUI(currentShield);
                HealthUI.instance.UpdateHealthUI(currentHealth, maxHealth);
                isShieldRecharged = false;
                StartCoroutine(RechargeShield());

            }
        }
        else
        {
            currentHealth -= Damage;
            HealthUI.instance.UpdateHealthUI(currentHealth, maxHealth);
        }
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            IsPlayerDead = true;
            SetFlamesActivePlayer(false);
            PauseManager.Instance.canBePaused = false;
            Anim.SetTrigger("isDeath");
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.deathSound, volume: 0.25f, p1: 0.5f, p2: 0.9f);
            SetPlayerControl(false, false, false, false, Vector2.zero);
            HealthUI.instance.UpdateHealthUI(currentHealth, maxHealth);
            DestroyLaserIfExists();
        }
    }
    IEnumerator BorderPortraitSwitch()
    {
        BorderPortrait.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        BorderPortrait.gameObject.SetActive(false);
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(Anim.GetCurrentAnimatorStateInfo(0).length);
        Anim.Rebind();

        gameObject.SetActive(false);
        SceneManager.LoadScene("GameOver");
    }

    IEnumerator RechargeShield()
    {
        if (isRechargingShield) yield break;
        isRechargingShield = true;

        yield return new WaitForSeconds(shieldRechargeDelay);

        while (currentShield < maxShield)
        {
            yield return new WaitForSeconds(shieldRechargeRate);
            currentShield += 2;
            HealthUI.instance.UpdateShieldUI(currentShield);

            if (currentShield > 0 && !isShieldRecharged)
            {
                Shield.transform.DOScale(1.4f, 0.5f)
                .SetEase(Ease.OutBack).SetLink(Shield, LinkBehaviour.KillOnDestroy)
                .OnComplete(() =>
                {
                    Shield.transform.DOScale(1.2f, 0.5f)
                             .SetLoops(-1, LoopType.Yoyo)
                             .SetEase(Ease.InOutSine)
                             .From(1.4f).SetLink(Shield, LinkBehaviour.KillOnDestroy);
                }).SetLink(Shield, LinkBehaviour.KillOnDestroy);
                Shield.SetActive(true);

                Sounds.Instance.PlaySoundEffect(Sounds.Instance.shieldSoundEffectClip, volume: 0.05f);

                Shield.transform.localScale = Vector3.zero;

                isShieldRecharged = true;
            }

        }
        isRechargingShield = false;
    }
    public void AddCoins(int value)
    {
        currentCoins += value;
        CoinsText.text = currentCoins.ToString();
    }

    public void AddCoins2X()
    {
        if (isActiveCoin2xBuff) return;
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.coinBuffSoundEffectClip, volume: 0.05f);
        isActiveCoin2xBuff = true;
    }
    public void AddDash()
    {
        //Sounds.Instance.PlaySoundEffect(Sounds.Instance.dashSoundEffectClip, volume: 0.05f);
        if (isActiveDashBuff) return;
        //Sounds.Instance.PlaySoundEffect(Sounds.Instance, volume: 0.05f);
        isActiveDashBuff = true;
        canDash = true;
    }
    public void SpendCoins(int amount)
    {
        currentCoins -= amount;
    }
    public void AddShield()
    {

        if (hasShield == false)
        {
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.shieldSoundEffectClip, volume: 0.05f);
            Shield.transform.localScale = Vector3.zero;
            Shield.SetActive(true);
            Shield.transform.DOScale(1.4f, 0.5f)
             .SetEase(Ease.OutBack).SetLink(Shield, LinkBehaviour.KillOnDestroy)
             .OnComplete(() =>
             {
                 Shield.transform.DOScale(1.2f, 0.5f)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutSine)
                     .From(1.4f).SetLink(Shield, LinkBehaviour.KillOnDestroy);
             });
            hasShield = true;
            currentShield = maxShield;
            HealthUI.instance.ShieldCheck(hasShield);
            HealthUI.instance.SetupUIShield();
        }
        else return;
    }

    public void AddHp()
    {
        if (currentHealth >= maxHealth) return;
        currentHealth += 5;
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.buffAddHp, volume: 0.05f);
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        HealthUI.instance.UpdateHealthUI(currentHealth, maxHealth);
    }
    private void Dash()
    {
        if (!canDash || IsPlayerDead || !isActiveDashBuff) return;

        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (direction == Vector2.zero)
        {
            direction = Vector2.up; // Default dash direction if no input
        }
        // rb.AddForce(direction * dashForce, ForceMode2D.Impulse);

        if (dashTrail != null)
        {
            dashTrail.Clear();
            dashTrail.emitting = true;
        }

        isDashing = true;
        canDash = false;
        canMove = false;

        //if (Sounds.Instance != null) Sounds.Instance.PlaySoundEffect(Sounds.Instance.dashSoundEffectClip, volume: 0.1f);
        Vector2 targetPos = rb.position + direction * (dashForce * 0.1f);
    
         DOTween.To(() => rb.position, x => rb.MovePosition(x), targetPos, dashDuration)
        .SetEase(Ease.OutCubic)
        .OnComplete(StopDash).SetLink(gameObject,LinkBehaviour.KillOnDestroy);
    
        // Invoke(nameof(StopDash), dashDuration);
        Invoke(nameof(ResetDash), dashCooldown);
    }
    private void StopDash()
    {
        rb.velocity *= 0.7f;
        isDashing = false;
        canMove = true;
        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }
    }
    private void ResetDash()
    {
        canDash = true;
    }

    // public void EnableRocketLauncher()
    // {
    //    hasRocketLauncher = true;
    // }
}
