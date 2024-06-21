using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuns : MonoBehaviour
{
 [Header("GUN INFO")]
    [SerializeField]  private GameObject bulletPrefab;
    [SerializeField]  private Transform firePoint; // Место, откуда будет вылетать пуля
    [SerializeField]  private float bulletForce = 20f; // Сила выстрела
    private AnimatedTexture GunAnim;
    public float MagazineMax = 7;
    public float FireRate;
    public float MagazineReload = 2.0f;
    public bool isAutomat;
    [SerializeField] private bool isShotgun;
    [SerializeField] private int pelletCount = 5;
    [SerializeField] private float spreadAngle = 20f;
    private float lastShootTime;
    [HideInInspector]  public bool reloading;
    private Rigidbody rb;
    [HideInInspector]  public Transform enemy;   
    [HideInInspector]  public float currentMagazine;
    [HideInInspector]  public bool HaveToReload;
    [Header("Отдача")]
    [SerializeField] private float _moveAmount;
    [SerializeField] public float maxDeviationAngle;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GunAnim = GetComponentInChildren<AnimatedTexture>();
        lastShootTime = -FireRate;
        currentMagazine = MagazineMax;
        // InvokeRepeating("CheckBuff", 0f, 1f);
      //  if (BHD != null) StartCoroutine(AutoShoot());
    }

    void Update()
    {
        if (currentMagazine == 0) Reload();
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
    public void TryToShoot()
    {
        if ((Time.time - lastShootTime >= FireRate) && isActiveAndEnabled)
        {
            ShootBullet();
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
            if(!reloading)
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

}
