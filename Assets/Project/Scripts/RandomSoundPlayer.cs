using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomSoundPlayer : MonoBehaviour
{
    #region Variables
    [Header("Time Set-Up")]
    [SerializeField] private float _minTimeToPlaySound = 20;
    [SerializeField] private float _maxTimeToPlaySound = 40;
    [SerializeField] private float _probalityToPlaySound = 0.4f;
    [Header("Sounds Set-Up")]
    [SerializeField] private AudioSource _audioSource;
    [Space(5)]
    [SerializeField] private int _maxAttemptsToPlaySounds = 4;
    [SerializeField] private SoundWithProbality[] _sounds = new SoundWithProbality[0];

    private float _time = 0f;
    #endregion

    #region Custom classes
    [System.Serializable]
    public class SoundWithProbality
    {
        #region Variables
        [SerializeField] private AudioClip _sound;
        [SerializeField] private float _probality = 0.5f;
        #endregion

        #region Variables
        public AudioClip Sound => _sound;
        public float Probality => _probality;
        #endregion
    }
    #endregion

    void Start()
    {
        _time = Random.Range(_minTimeToPlaySound, _maxTimeToPlaySound);

        StartCoroutine(PlayRandomSound());
    }

    private IEnumerator PlayRandomSound()
    {
        yield return new WaitForSeconds(_time);

        _time = Random.Range(_minTimeToPlaySound, _maxTimeToPlaySound);

        if (CheckProbality(_probalityToPlaySound) && _sounds.Length > 0)
        {
            for (int i = 0; i < _maxAttemptsToPlaySounds; i++)
            {
                int index = Random.Range(0, _sounds.Length);

                if (CheckProbality(_sounds[index].Probality))
                {
                    _audioSource.PlayOneShot(_sounds[index].Sound);
                    break;
                }
            }
        }

        StartCoroutine(PlayRandomSound());
    }

    private bool CheckProbality(float probality)
    {
        if (probality > 1 || probality <= 0)
            return false;

        return Random.Range(0f, 1f) <= probality;
            }
}