using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private CanvasGroup _hud;
    [SerializeField] private UIWindow _deathHUD;
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _player.OnDeathEvent += OnDeath;
    }

    private void OnDestroy()
    {
        _player.OnDeathEvent -= OnDeath;
    }

    public void OnDeath()
    {
        _hud.alpha = 0;
        _deathHUD.ShowWindow();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
