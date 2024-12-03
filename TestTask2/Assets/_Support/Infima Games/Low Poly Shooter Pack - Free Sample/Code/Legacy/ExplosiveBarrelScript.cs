using UnityEngine;
using System.Collections;

public class ExplosiveBarrelScript : MonoBehaviour, IHaveProjectileReaction { // Добавил интерфейс IHaveProjectileReaction
    /*
    Удалил переменные explod и routineStarted а также метод Update
	*/
	[Header("Prefabs")]
	public Transform explosionPrefab;
	public Transform destroyedBarrelPrefab;

	[Header("Customizable Options")]
	public float minTime = 0.05f;
	public float maxTime = 0.25f;

	[Header("Explosion Options")]
	public float explosionRadius = 12.5f;
	public float explosionForce = 4000.0f;

    private float randomTime; // Добавил private 

    public void React() =>
		StartCoroutine(Explode());

    private IEnumerator Explode () {
        randomTime = Random.Range(minTime, maxTime); // Перенес из Update

        yield return new WaitForSeconds(randomTime);

		Instantiate (destroyedBarrelPrefab, transform.position, 
		             transform.rotation); 

		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
		foreach (Collider hit in colliders) {
			Rigidbody rb = hit.GetComponent<Rigidbody> ();
			
			if (rb != null)
				rb.AddExplosionForce (explosionForce * 50, explosionPos, explosionRadius);

            // Изменил проверку по тэгам на проверку наличия компонента
            IHaveProjectileReaction hitTarget;
            hit.gameObject.TryGetComponent<IHaveProjectileReaction>(out hitTarget);

            if (hitTarget != null)
			{
                if (hitTarget.GetType() == typeof(GasTankScript))
                {
                    GasTankScript gasTank = (GasTankScript)hitTarget;
                    gasTank.SetExplosionTimer(0.05f);
                }

                hitTarget.React();
            }
		}

		RaycastHit checkGround;
		if (Physics.Raycast(transform.position, Vector3.down, out checkGround, 50))
		{
			Instantiate (explosionPrefab, checkGround.point, 
				Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}

		Destroy (gameObject);
	}
}