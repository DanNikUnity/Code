using System.Collections;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [Header("GUN INFO")]
    [SerializeField]  private GameObject bulletPrefab;
    [SerializeField]  private Transform firePoint; // Место, откуда будет вылетать пуля
    [SerializeField]  private float bulletForce = 20f; // Сила выстрела
    private AnimatedTexture GunAnim;
    public float MagazineMax = 7;
    public float FireRate;
    public float MagazineReload = 2.0f;
    public float HearRadius;
    
    public bool isAutomat;
    [SerializeField] public bool isShotgun;
    [SerializeField] private int pelletCount = 5;
    [SerializeField] public float spreadAngle = 20f;
    private float lastShootTime;
    [HideInInspector]  public bool reloading;
    private Rigidbody rb;
    [HideInInspector]  public Transform enemy;   
    [HideInInspector]  public float currentMagazine;
    [HideInInspector]  public int magazineCount = 3;
    [HideInInspector]  public bool HaveToReload;
    [Header("Отдача")]
    [SerializeField] private float _moveAmount;
    [SerializeField] public float maxDeviationAngle;

    [Header("CAMERA SHAKE")]
    public float shakeDuration = 0.1f; 
    public float shakeMagnitude = 0.1f;
    private Vector3 originalCameraPosition;
    private Transform camera;


    private GunRecoil gunRecoil;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        GunAnim = GetComponentInChildren<AnimatedTexture>();
        lastShootTime = -FireRate;
        currentMagazine = MagazineMax;
        // InvokeRepeating("CheckBuff", 0f, 1f);
        originalCameraPosition = camera.localPosition;
        gunRecoil = GetComponent<GunRecoil>();
    }
    public void OnEnable()
    {
        reloading = false; 
       // StartCoroutine(AutoShoot());
    }
    public void OnDisable()
    {
        reloading = false;
    }

    void Update()
    {
        if (enemy != null)
        {
            transform.LookAt(enemy);
        }
        if (currentMagazine == 0) Reload();
    }
    public void TryToShoot()
    {
        if ((Time.time - lastShootTime >= FireRate) && isActiveAndEnabled && currentMagazine > 0 && !isAutomat)
        {
            HearShoootings HS = FindObjectOfType<HearShoootings>();
            HS.SetSawPlayerForAll();
                    ShootBullet();
            StartCoroutine(ShakeCamera(false));
            gunRecoil.Shoot();
        }

    }

    void ShootBullet()
{
    if (reloading) return;
    MoveObject(_moveAmount);

    // Calculate random deviation angle
    float deviationAngle = Random.Range(-maxDeviationAngle, maxDeviationAngle);

    // Calculate rotation with deviation angle
    Quaternion deviationRotation = Quaternion.AngleAxis(deviationAngle, Vector3.up);
    Vector3 bulletDirection = deviationRotation * firePoint.forward;

    // Shoot multiple bullets for shotgun
    if (isShotgun)
    {

        for (int i = 0; i < pelletCount; i++)
        {
            Quaternion spreadRotation = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0);
            Vector3 spreadDirection = spreadRotation * bulletDirection;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(spreadDirection * bulletForce, ForceMode.Impulse);
            }
        }
    }
    else // Shoot single bullet for other weapons
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
                if(rb.GetComponent<GrenadeBullet>() != null) rb.GetComponent<GrenadeBullet>().Throw(10f, firePoint, bulletDirection );
                rb.AddForce(bulletDirection * bulletForce, ForceMode.Impulse);
        }
    }

    currentMagazine--;
    lastShootTime = Time.time;
    GunAnim.PlayShootAnim();
}

    public void Reload()
    {
        if (isActiveAndEnabled)
        {
            if(magazineCount >= 1 && !reloading)
            {
                StartCoroutine(ReloadMagazine());
            }
          
        }

        
    }
    IEnumerator ReloadMagazine()
    {
        reloading = true;
        yield return new WaitForSeconds(MagazineReload);
        reloading = false;
        currentMagazine = MagazineMax;
        magazineCount--;


    }
    void MoveObject(float impulseStrength)
    {
        if (rb != null)
        {
            // Получаем направление, противоположное направлению объекта
            Vector3 backwardDirection = -transform.forward;

            // Применяем импульс к Rigidbody
            rb.AddForce(backwardDirection * impulseStrength, ForceMode.Impulse);
        }
    }

    public void BuyMagazine()
    {
        if(isActiveAndEnabled) magazineCount++;
    }
    public void cameraBombShake()
    {
        StartCoroutine(ShakeCamera(true));
    }
    private IEnumerator ShakeCamera(bool explosion)
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            if (explosion && elapsedTime < shakeDuration*2)
            {
                camera.localPosition = originalCameraPosition + Random.insideUnitSphere * shakeMagnitude*2.2f;
            }
            else
            {
                camera.localPosition = originalCameraPosition + Random.insideUnitSphere * shakeMagnitude;
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        camera.localPosition = originalCameraPosition;
    }
    /* private void CheckBuff()
     {
         FireRate += FireRate * BuffController.FireRateBuff;
         lastShootTime = -FireRate;
     }*/
}
