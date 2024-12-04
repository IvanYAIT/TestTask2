using UnityEngine;
using System.Collections;

public class GasTankScript : MonoBehaviour, IHaveProjectileReaction { // Добавил интерфейс IHaveProjectileReaction
    /*
	Удалил переменные isHit и routineStarted а также метод Update
	*/
	[Header("Prefabs")]
	public Transform explosionPrefab;
	public Transform destroyedGasTankPrefab;

	[Header("Customizable Options")]
	public float explosionTimer;
	public float rotationSpeed;
	public float maxRotationSpeed;
	public float moveSpeed;
	public float audioPitchIncrease = 0.5f;

	[Header("Explosion Options")]
	public float explosionRadius = 12.5f;
	public float explosionForce = 4000.0f;

	[Header("Light")]
	public Light lightObject;

	[Header("Particle Systems")]
	public ParticleSystem flameParticles;
	public ParticleSystem smokeParticles;

	[Header("Audio")]
	public AudioSource flameSound;
	public AudioSource impactSound;
	bool audioHasPlayed = false;

	[Header("Other")]
	public Rigidbody rb; // Добавил переменную

    private float randomRotationValue; // Добавил private 
    private float randomValue; // Добавил private 

    private void Start () {
		lightObject.intensity = 0;
		randomValue = Random.Range (-50, 50);
	}

	private void OnCollisionEnter (Collision collision) {
		impactSound.Play ();
	}

    public void React()
    {
        randomRotationValue += 1.0f * Time.deltaTime;

        if (randomRotationValue > maxRotationSpeed)
        {
            randomRotationValue = maxRotationSpeed;
        }

        rb.AddRelativeForce(Vector3.down * moveSpeed * 50 * Time.deltaTime); // Заменил gameObject.GetComponent<Rigidbody>() на rb

        transform.Rotate(randomRotationValue, 0, randomValue *
                          rotationSpeed * Time.deltaTime);

        flameParticles.Play();
        smokeParticles.Play();

        flameSound.pitch += audioPitchIncrease * Time.deltaTime;

        if (!audioHasPlayed)
        {
            flameSound.Play();
            audioHasPlayed = true;
        }

        StartCoroutine(Explode());
        lightObject.intensity = 3;
    }

    private IEnumerator Explode ()
	{
		yield return new WaitForSeconds(explosionTimer);
		
		Instantiate (destroyedGasTankPrefab, transform.position, 
		             transform.rotation); 
		
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
		foreach (Collider hit in colliders) 
		{
			Rigidbody hitRb = hit.GetComponent<Rigidbody> ();

            if (hitRb != null)
				hitRb.AddExplosionForce (explosionForce * 50, explosionPos, explosionRadius);

            // Изменил проверку по тэгам на проверку наличия компонента
            IHaveProjectileReaction hitTarget;
            hit.gameObject.TryGetComponent<IHaveProjectileReaction>(out hitTarget);

            if (hitTarget != null)
                hitTarget.React();
		}
		
		Instantiate (explosionPrefab, transform.position, 
		             transform.rotation); 

		Destroy (gameObject);
	}

	public void SetExplosionTimer(float value) // Добавил метод
		=> explosionTimer = value;
}