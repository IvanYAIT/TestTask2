using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
    /*
		Заменил все GetComponent<Rigidbody>() на rb
		Удалил if (hit.CompareTag("Player")) continue;
		Удалил переменную explodeSelf
		Убрал лишнию проверку поподания снаряда в цель 
	*/
	[Tooltip("Enable to use constant force, instead of force at launch only")]
	public bool useConstantForce;
	[Tooltip("How fast the projectile moves")]
	public float constantForceSpeed;
	[Tooltip("How long after spawning that the projectile self destructs")]
	public float explodeAfter;
	private bool hasStartedExplode;

	private bool hasCollided;

	[Header("Explosion Prefabs")]
	public Transform explosionPrefab;

	[Header("Customizable Options")]
	[Tooltip("Initial launch force")]
	public float force = 5000f;
	[Tooltip("How long after spawning should the projectile object destroy")]
	public float despawnTime = 30f;

	[Header("Explosion Options")]
	[Tooltip("Explosion radius")]
	public float radius = 50.0F;
	[Tooltip("Explosion intensity")]
	public float power = 250.0F;

	[Header("Rocket Launcher Projectile")]
	[Tooltip("Enabled if the projectile has particle effects")]
	public bool usesParticles;
	public ParticleSystem smokeParticles;
	public ParticleSystem flameParticles;
	[Tooltip("Added delay to let particle effects finish playing, " +
		"before destroying object")]
	public float destroyDelay;

	[Header("Other")]
	[SerializeField] private Rigidbody rb;
	[SerializeField] private MeshRenderer meshRenderer;
	[SerializeField] private BoxCollider boxCollider;

	private void Start () 
	{
		if (!useConstantForce) 
		{
            rb.AddForce(gameObject.transform.forward * force);
		}

		StartCoroutine (DestroyTimer ());
	}
		
	private void FixedUpdate()
	{
		if(rb.velocity != Vector3.zero)
            rb.rotation = Quaternion.LookRotation(rb.velocity);  

		if (useConstantForce == true && !hasStartedExplode) {
            rb.AddForce(gameObject.transform.forward * constantForceSpeed);

			StartCoroutine (ExplodeSelf());

			hasStartedExplode = true;
		}
	}

	private IEnumerator ExplodeSelf () 
	{
		yield return new WaitForSeconds (explodeAfter);

		if (!hasCollided) {
			Instantiate (explosionPrefab, transform.position, transform.rotation);
		}

		meshRenderer.enabled = false; // Заменил gameObject.GetComponent<MeshRenderer> () на meshRenderer
        rb.isKinematic = true;
		boxCollider.isTrigger = true; // Заменил gameObject.GetComponent<BoxCollider>() на boxCollider

        if (usesParticles == true) {
			flameParticles.Stop(); // убрал .GetComponent <ParticleSystem> ()
            smokeParticles.Stop(); // убрал .GetComponent <ParticleSystem> ()
        }

		yield return new WaitForSeconds (destroyDelay);

		Destroy (gameObject);
	}

	private IEnumerator DestroyTimer () 
	{
		yield return new WaitForSeconds (despawnTime);
		Destroy (gameObject);
	}

	private IEnumerator DestroyTimerAfterCollision () 
	{
		yield return new WaitForSeconds (destroyDelay);
		Destroy (gameObject);
	}

	private void OnCollisionEnter (Collision collision)
	{
		//if (collision.transform.CompareTag("Player"))  Возможно лишняя проверка
		//	return;
		
		hasCollided = true;

		meshRenderer.enabled = false;  // Заменил gameObject.GetComponent<MeshRenderer> () на meshRenderer
        rb.isKinematic = true;
		boxCollider.isTrigger = true;  // Заменил gameObject.GetComponent<BoxCollider>() на boxCollider

        if (usesParticles) {
			flameParticles.Stop (); // убрал .GetComponent <ParticleSystem> ()
            smokeParticles.Stop (); // убрал .GetComponent <ParticleSystem> ()
        }

		StartCoroutine (DestroyTimerAfterCollision ());

		Instantiate(explosionPrefab,collision.contacts[0].point,
			Quaternion.LookRotation(collision.contacts[0].normal));

		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders)
		{	
			Rigidbody hitRb = hit.GetComponent<Rigidbody> ();

			if (hitRb != null)
				hitRb.AddExplosionForce (power * 50, explosionPos, radius, 3.0F);

			// Изменил проверку по тэгам на проверку наличия компонента
			IHaveProjectileReaction hitTarget;
			hit.gameObject.TryGetComponent<IHaveProjectileReaction>(out hitTarget);

			if(hitTarget != null)
			{
                if (hitTarget.GetType() == typeof(GasTankScript))
                {
                    GasTankScript gasTank = (GasTankScript)hitTarget;
					gasTank.SetExplosionTimer(0.05f);
                }

                hitTarget.React();
			}
		}
	}
}