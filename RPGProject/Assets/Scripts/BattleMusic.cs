using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMusic : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] AudioClip overworld;
    [SerializeField] AudioClip battleTheme;
    [SerializeField] AudioClip battleIntro;
    [SerializeField] AudioClip victory;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayBattleTheme()
    {
        StartCoroutine(LoopBattleTheme());
    }

    IEnumerator LoopBattleTheme()
    {
        audioSource.clip = battleIntro;
        audioSource.Play();

        yield return new WaitForSeconds(battleIntro.length);

        audioSource.clip = battleTheme;
        audioSource.Play();
    }

    public void PlayOverworld()
    {
        audioSource.loop = true;
        audioSource.clip = overworld;
        audioSource.Play();
    }

    public void PlayVictory()
    {
        audioSource.loop = false;
        audioSource.clip = victory;
        audioSource.Play();
    }
}
