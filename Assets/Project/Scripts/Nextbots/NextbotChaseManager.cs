using UnityEngine;
public class NextbotChaseManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private float _chaseStartDistance = 15f;
    private bool _isInitialized = false;
    private NextbotAI[] _ais;
    private Player _player;
    private float _nearestDistance = float.MaxValue;
    #endregion

    #region Code
    public void Initialize()
    {
        _ais = FindObjectsOfType<NextbotAI>();
        _player = FindObjectOfType<Player>();

        if (_ais.Length == 0)
        {
            Debug.LogError("Can not find NextbotAIs on Scene!");
            return;
        }

        if (_player == null)
        {
            Debug.LogError("Can not find Player on Scene!");
            return;
        }

        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        _nearestDistance = float.MaxValue;

        foreach (var bot in _ais)
        {
            float _distance = Vector3.Distance(bot.transform.position, _player.transform.position);
            if (_distance < _nearestDistance)
                _nearestDistance = _distance;
        }

        if (_nearestDistance <= _chaseStartDistance && ChaseManager.canChase && !ChaseManager.IsChasing)
        {
            ChaseManager.StartChase();
            return;
        }
        else if (_nearestDistance > _chaseStartDistance && ChaseManager.canChase && ChaseManager.IsChasing)
        {
            ChaseManager.EndChase();
            return;
        }
    }
    #endregion
}