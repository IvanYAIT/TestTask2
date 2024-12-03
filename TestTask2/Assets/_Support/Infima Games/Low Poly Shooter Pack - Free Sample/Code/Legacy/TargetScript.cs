using UnityEngine;
using System.Collections;

public class TargetScript : MonoBehaviour, IHaveProjectileReaction { // Добавил интерфейс IHaveProjectileReaction
    /*
        Удалил переменные routineStarted а также метод Update
        Изменил переменную isHit на свойста IsHit
    */
	[Header("Customizable Options")]
	public float minTime;
	public float maxTime;

	[Header("Audio")]
    public AudioSource audioSource;
    public AudioClip upSound;
	public AudioClip downSound;

	[Header("Animations")]
	public Animation animation; //Добавил переменную
    public AnimationClip targetUp;
	public AnimationClip targetDown;

    private float randomTime;// Добавил private 

    public bool IsHit { get; private set; } 

    public void React()
	{
        // Перенес логику из Update
        if (!IsHit)
        {
            IsHit = true; // Добавил

            animation.clip = targetDown; // Поменял gameObject.GetComponent<Animation>() на animation
            animation.Play(); // Поменял gameObject.GetComponent<Animation>() на animation

            audioSource.clip = downSound; // Убрал .GetComponent<AudioSource>()
            audioSource.Play();

            StartCoroutine(DelayTimer());
        }
    }

    private IEnumerator DelayTimer () {
        randomTime = Random.Range(minTime, maxTime); // Перенес из Update

        yield return new WaitForSeconds(randomTime);
        animation.clip = targetUp; // Поменял gameObject.GetComponent<Animation>() на animation
        animation.Play(); // Поменял gameObject.GetComponent<Animation>() на animation

        audioSource.clip = upSound; // Убрал .GetComponent<AudioSource>()
        audioSource.Play();

        IsHit = false; // Добавил
    }

}