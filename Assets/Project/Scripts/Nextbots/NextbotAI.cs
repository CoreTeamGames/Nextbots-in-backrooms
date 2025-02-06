using UnityEngine;
using UnityEngine.AI;

public class NextbotAI : MonoBehaviour
{
    [Header("Настройки AI")]
    private Transform target;

    public bool enableChasing = true;

    [Tooltip("Минимальное расстояние до цели, при котором Nextbot остановится")]
    public float stoppingDistance = 1f;

    private NavMeshAgent agent;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (target == null)
            gameObject.SetActive(false);

        // Получаем компонент NavMeshAgent
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent не найден на объекте " + gameObject.name);
        }

        target.GetComponent<Player>().OnDeathEvent += () => { agent.isStopped = true; };
    }

    private void OnDestroy()
    {
        if (target == null)
            return;

        target.GetComponent<Player>().OnDeathEvent -= () => { agent.isStopped = true; };
    }

    void Update()
    {
        if (target == null || agent == null || !enableChasing)
            return;

        // Вычисляем расстояние до цели
        float distance = Vector3.Distance(transform.position, target.position);

        agent.SetDestination(target.position);

        // Если Nextbot достаточно близко к цели, можно остановиться или выполнить какую-то логику
        if (distance <= stoppingDistance)
        {
            // Например, можно остановить агент или запустить атаку
            agent.isStopped = true;
            target.GetComponent<Player>().Death();
            // Здесь можно добавить код для атаки или другой логики
        }
        else
        {
            agent.isStopped = false;
        }
    }
}

