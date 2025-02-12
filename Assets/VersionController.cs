using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionController : MonoBehaviour
{
    [SerializeField] bool _isAlpha = true;
    [SerializeField] bool _isBeta = false;

    private void Awake()
    {
        TMPro.TMP_Text text = GetComponentInChildren<TMPro.TMP_Text>();
        text.text = "Version " + (_isAlpha ? "Alpha " : "") + (_isBeta ? "Beta " : "") + Application.version + '.';

        DontDestroyOnLoad(gameObject);
        gameObject.isStatic = true;
    }
}
