using UnityEngine;
using System.Collections;

public class DestroyAfterEffect : MonoBehaviour
{
    [Tooltip("Additional time to wait after particles are done before destroying the object")]
    public float extraDestroyDelay = 0.5f;
    
    [Tooltip("If true, will check all child particle systems too")]
    public bool checkChildParticles = true;
    
    private ParticleSystem[] allParticleSystems;
    
    void Start()
    {
        // Get all particle systems (including children if enabled)
        if (checkChildParticles)
        {
            allParticleSystems = GetComponentsInChildren<ParticleSystem>();
        }
        else
        {
            ParticleSystem mainPS = GetComponent<ParticleSystem>();
            if (mainPS != null)
            {
                allParticleSystems = new ParticleSystem[] { mainPS };
            }
        }
        
        // Make sure we have at least one particle system
        if (allParticleSystems == null || allParticleSystems.Length == 0)
        {
            Debug.LogWarning("DestroyAfterEffect: No particle systems found on " + gameObject.name);
            // Destroy after a safety delay if no particle systems found
            Destroy(gameObject, 2.0f);
            return;
        }
        
        // Set all particle systems to not loop
        foreach (ParticleSystem ps in allParticleSystems)
        {
            var main = ps.main;
            main.loop = false;
        }
        
        // Start the coroutine to check when particles are done
        StartCoroutine(CheckParticlesDone());
    }
    
    IEnumerator CheckParticlesDone()
    {
        // Wait for initial frame to make sure everything is initialized
        yield return null;
        
        bool allDone = false;
        
        // Keep checking until all particle systems are done
        while (!allDone)
        {
            allDone = true;
            
            // Check each particle system
            foreach (ParticleSystem ps in allParticleSystems)
            {
                if (ps.IsAlive(true)) // true means include inactive particles
                {
                    allDone = false;
                    break;
                }
            }
            
            // Wait before checking again
            yield return new WaitForSeconds(0.2f);
        }
        
        // All particle systems are done, wait for extra delay if specified
        if (extraDestroyDelay > 0)
        {
            yield return new WaitForSeconds(extraDestroyDelay);
        }
        
        // Destroy the game object
        Destroy(gameObject);
    }
}