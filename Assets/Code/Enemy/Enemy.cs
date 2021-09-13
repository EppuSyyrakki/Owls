using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Enemy
{
    public class Enemy : MonoBehaviour
    {
	    [SerializeField]
	    private ParticleSystem[] deathFx;

	    private bool _isAlive = true;

	    private void Awake()
	    {
		    foreach (var fx in deathFx)
		    {
			    fx.gameObject.SetActive(false);
		    }
	    }   

        private void Update()
        {
	        if (Input.GetKeyDown(KeyCode.Space) && _isAlive)
	        {
		        Kill();
	        }
        }

        private void Kill()
        {
	        foreach (Transform child in transform)
	        { 
		        child.gameObject.SetActive(false);   
	        }

	        foreach (var fx in deathFx)
	        {
		        var ps = Instantiate(fx, transform);
				ps.gameObject.SetActive(true);
	        }

	        _isAlive = false;
        }
    }
}
