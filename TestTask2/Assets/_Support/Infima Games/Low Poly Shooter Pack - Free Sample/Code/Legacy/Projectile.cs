using System;
using UnityEngine;
using System.Collections;
using InfimaGames.LowPolyShooterPack;
using Random = UnityEngine.Random;

public class Projectile : MonoBehaviour {

	[Range(5, 100)]
	[Tooltip("After how long time should the bullet prefab be destroyed?")]
	public float destroyAfter;
	[Tooltip("If enabled the bullet destroys on impact")]
	public bool destroyOnImpact = false;
	[Tooltip("Minimum time after impact that the bullet is destroyed")]
	public float minDestroyTime;
	[Tooltip("Maximum time after impact that the bullet is destroyed")]
	public float maxDestroyTime;

	[Header("Impact Effect Prefabs")]
	public Transform [] bloodImpactPrefabs;
	public Transform [] metalImpactPrefabs;
	public Transform [] dirtImpactPrefabs;
	public Transform []	concreteImpactPrefabs;

	private void Start ()
	{
		var gameModeService = ServiceLocator.Current.Get<IGameModeService>();
		Physics.IgnoreCollision(gameModeService.GetPlayerCharacter().GetComponent<Collider>(), GetComponent<Collider>());
		
		StartCoroutine (DestroyAfter ());
	}

	private void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.GetComponent<Projectile>() != null)
			return;
		
		// //Ignore collision if bullet collides with "Player" tag
		// if (collision.gameObject.CompareTag("Player")) 
		// {
		// 	//Physics.IgnoreCollision (collision.collider);
		// 	Debug.LogWarning("Collides with player");
		// 	//Physics.IgnoreCollision(GetComponent<Collider>(), GetComponent<Collider>());
		//
		// 	//Ignore player character collision, otherwise this moves it, which is quite odd, and other weird stuff happens!
		// 	Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
		//
		// 	//Return, otherwise we will destroy with this hit, which we don't want!
		// 	return;
		// }
		//
		//If destroy on impact is false, start 
		//coroutine with random destroy timer
		if (!destroyOnImpact) 
		{
			StartCoroutine (DestroyTimer ());
		}
		else 
		{
			Destroy (gameObject);
		}


		int randomNum = Random.Range(0, bloodImpactPrefabs.Length);
		Quaternion rot = Quaternion.LookRotation(collision.contacts[0].normal);
        // Поменял 4 if на switch а также вывел в переменные повторяющиеся элементы
        switch (collision.transform.tag)
		{
			case "Blood":
                Instantiate(bloodImpactPrefabs[randomNum], 
							transform.position,
							rot);
                Destroy(gameObject);
				break;
            case "Metal":
                Instantiate(metalImpactPrefabs[randomNum], 
							transform.position, 
							rot);
                Destroy(gameObject);
                break;
            case "Dirt":
                Instantiate(dirtImpactPrefabs[randomNum], 
							transform.position, 
							rot);
                Destroy(gameObject);
                break;
            case "Concrete":
                Instantiate(concreteImpactPrefabs[randomNum], 
							transform.position, 
							rot);
                Destroy(gameObject);
                break;
        }

        // Изменил проверку по тэгам на проверку наличия компонента
        IHaveProjectileReaction hit;
        collision.gameObject.TryGetComponent<IHaveProjectileReaction>(out hit);

        if (hit != null)
		{
			hit.React();

            Destroy(gameObject);
        }
	}

	private IEnumerator DestroyTimer () 
	{
		yield return new WaitForSeconds
			(Random.Range(minDestroyTime, maxDestroyTime));
		Destroy(gameObject);
	}

	private IEnumerator DestroyAfter () 
	{
		yield return new WaitForSeconds (destroyAfter);
		Destroy (gameObject);
	}
}