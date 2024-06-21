using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;
    public float shootingRadius = 10f; // Радиус для выстрела
    public Transform player; // Ссылка на игрока
    private EnemyGuns Gun;
    private FovSprite fov;

    private NavMeshAgent agent;
    private float nextWanderTime;
    public bool seePlayer = false;
    private bool hearedBullet;
    private Coroutine bulletCoroutine;
    public float rotationSpeed;
    public bool Patrolling;
    public bool Busy;
    public bool Checking;
    public bool flashed;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        nextWanderTime = Time.time + Random.Range(0, wanderInterval);
        Gun = GetComponentInChildren<EnemyGuns>();
        fov = GetComponent<FovSprite>();
        StartCoroutine(Patrol());
    }
    void FixedUpdate()
    {
        if ((seePlayer || fov.canSeePlayer) && !flashed)
        {
            StopAllCoroutines();
            StartCoroutine(AttackPlayer());
        }
        else if (!Busy && !Patrolling && !flashed)
        {
            StartCoroutine(Patrol());
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= shootingRadius && (fov.canSeePlayer || seePlayer) && !flashed)
        {
            Gun.TryToShoot();
        }

    }


    public void Flashed()
    {
        StopAllCoroutines();
        StartCoroutine(FlashTime());
    }
    IEnumerator FlashTime()
    {
        flashed = true;
        Gun.TryToShoot();
        agent.isStopped = true;

        float rotationSpeed = 60f;
        float rotationAngle = 30f;

        float initialRotationY = transform.eulerAngles.y;
        float rotationTime = 3f;

        float elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {
            // Вычисляем текущий угол поворота относительно начального угла
            float angle = Mathf.PingPong(elapsedTime * rotationSpeed, rotationAngle * 2) - rotationAngle;
            transform.rotation = Quaternion.Euler(0, initialRotationY + angle, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Устанавливаем финальный угол как текущее положение, без возврата к начальному углу

        flashed = false;
        agent.isStopped = false;
        CheckSound();
    }
    IEnumerator AttackPlayer()
    {
        while (!flashed)
        {
            Busy = true;

            // Перемещение к игроку
            agent.SetDestination(player.position);

            // Проверяем расстояние до игрока
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Если противник находится в пределах дистанции выстрела
            if (distanceToPlayer <= shootingRadius)
            {
                // Останавливаем движение противника
                agent.isStopped = true;

                // Плавный поворот к игроку
                Vector3 directionToPlayer = player.position - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                // Продолжаем движение к игроку, если он находится за пределами дистанции выстрела
                agent.isStopped = false;
                Busy = false;
                StopAllCoroutines();
                StartCoroutine(FollowPlayer());
            }

            yield return null;
        }

        // Если игрок больше не виден, вызываем корутину CheckSound
        if (!seePlayer && !fov.canSeePlayer)
        {
            
            StartCoroutine(CheckTheSound());
        }
    }
    IEnumerator FollowPlayer()
    {
        Busy = true;
        float followTime = 3f; // Время следования за игроком (в секундах)
        float elapsedTime = 0f; // Прошедшее время

        while (elapsedTime < followTime)
        {
            // Перемещаемся к игроку
            agent.SetDestination(player.position);

            // Увеличиваем прошедшее время
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        Busy = false;
    }
    IEnumerator Patrol()
    {
        while (true && !flashed)
        {
            Busy = true;
            if (!fov.canSeePlayer && !hearedBullet)
            {
                if (Time.time >= nextWanderTime)
                {
                    Vector3 newPosition = RandomNavSphere(transform.position, wanderRadius, -1);
                    agent.SetDestination(newPosition);
                    nextWanderTime = Time.time + wanderInterval;
                }
            }
            if (fov.canSeePlayer || seePlayer)
            {
                StartCoroutine(AttackPlayer());
                Busy = false;
                StopCoroutine(Patrol());
            }
            yield return null;
        }
    }

    private static Vector3 RandomNavSphere(Vector3 origin, float distance, int layerMask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layerMask);

        return navHit.position;
    }

    public void HearedBullet()
    {
        int i = Random.RandomRange(1, 5);
        if (!flashed && i == 1)
        {
            StopAllCoroutines();
            StartCoroutine(CheckTheSound());
        }
        else if(!flashed)
        {
            if (bulletCoroutine != null)
            {
                StopCoroutine(bulletCoroutine);
            }
            bulletCoroutine = StartCoroutine(OnBulletTrigger());
        }
       
    }
    public void HearedFlashBang(Transform flashBang)
    {
        if(!seePlayer && !flashed && !fov.canSeePlayer)
        {
            StopAllCoroutines();
            StartCoroutine(LookAtFlashBang(flashBang));
        }    

    }
    IEnumerator LookAtFlashBang(Transform flashBang)
    {
        Busy = true;

        // Получаем начальное и конечное направление к флэшбанку
        Vector3 initialDirection = flashBang.position - transform.position;
        initialDirection.y = 0f;
        Vector3 endDirection = initialDirection;

        // Вычисляем начальную и конечную ротации
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.LookRotation(endDirection);

        float rotationTime = 1f; // Время на поворот
        float elapsedTime = 0f;

        agent.isStopped = true;

        while (elapsedTime < rotationTime)
        {
            // Обновляем направление к флэшбанку, чтобы учитывать его текущее положение
            endDirection = flashBang.position - transform.position;
            endDirection.y = 0f;
            endRotation = Quaternion.LookRotation(endDirection);

            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Устанавливаем окончательную ротацию и продолжаем движение

        transform.rotation = endRotation;
        agent.isStopped = false;
        Busy = false;
        StartCoroutine(CheckTheSound());
    }
    public void CheckSound()
    {
        if(!fov.canSeePlayer && !seePlayer && !flashed)
        {
            StopAllCoroutines();
            StartCoroutine(CheckTheSound());
        }

    }
    IEnumerator CheckTheSound()
    {
        Busy = true;
        Vector3 initialPosition = transform.position;

        // Сохраняем позицию игрока
        Vector3 playerPosition = player.position;

        // Двигаем противника к позиции игрока
        agent.SetDestination(playerPosition);

        // Ожидаем, пока противник не достигнет позиции игрока
        yield return new WaitUntil(() => Vector3.Distance(transform.position, playerPosition) < 1f);

        // Останавливаем противника на 1 секунду
        agent.isStopped = true;
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 2; i++)
        {
            // Поворачиваем противника на рандомное количество градусов
            float randomAngle = Random.Range(-200f, 200f);
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = Quaternion.Euler(0, transform.eulerAngles.y + randomAngle, 0);

            float rotationTime = 1f; // Время на поворот
            float elapsedTime = 0f;

            while (elapsedTime < rotationTime)
            {
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / rotationTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.rotation = endRotation; // Убедитесь, что установка конечного угла

            // Останавливаем противника на 1 секунду
            yield return new WaitForSeconds(1f);
        }

        // Возвращаем противника на начальную позицию
        agent.SetDestination(initialPosition);
        agent.isStopped = false;
        Busy = false;
    }

    IEnumerator OnBulletTrigger()
    {
        Busy = true;
        hearedBullet = true;
        Vector3 directionToPlayer = player.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        float rotationSpeed = 1f; // Скорость поворота
        float elapsedTime = 0f;
        float duration = 1f; // Длительность поворота

        // Плавный поворот к игроку
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;

            if (fov.canSeePlayer || seePlayer)
            {
                yield break; // Прерываем корутину, если видим игрока
            }
        }

        // Остановить агента на 3 секунды
        agent.isStopped = true;
        yield return new WaitForSeconds(2.5f);

        // Возобновить движение
        agent.isStopped = false;
        hearedBullet = false;
        Busy = false;
    }

    public void Death()
    {
        GetComponentInChildren<DeathAlert>().Death();
        Destroy(gameObject, 0.01f); ;
    }
}
