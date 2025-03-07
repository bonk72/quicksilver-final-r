using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;

public class dontDestroyData : MonoBehaviour
{
    public GameObject[] shops;

    private int[] shopXValues;
    private float[] shopTimeRatios;
    
    // Awake is called before Start
    private void Awake()
    {
        // Don't destroy this object when loading a new scene
        DontDestroyOnLoad(gameObject);
        
        // Register for scene loaded events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize arrays to store shop data
        shopXValues = new int[shops.Length];
        shopTimeRatios = new float[shops.Length];
        
        // Store initial values from each shop
        for (int i = 0; i < shops.Length; i++)
        {
            shop shopScript = shops[i].GetComponent<shop>();
            if (shopScript != null)
            {
                // Use reflection to access private fields
                FieldInfo xField = typeof(shop).GetField("x", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo timeRatioField = typeof(shop).GetField("timeRatio", BindingFlags.NonPublic | BindingFlags.Static);
                
                if (xField != null && timeRatioField != null)
                {
                    shopXValues[i] = (int)xField.GetValue(shopScript);
                    shopTimeRatios[i] = (float)timeRatioField.GetValue(null);
                }
            }
        }
    }
    
    // Called when a scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Wait a frame to ensure all objects are initialized
        StartCoroutine(UpdateShopsNextFrame());
    }
    
    // Coroutine to update shops after a frame
    IEnumerator UpdateShopsNextFrame()
    {
        // Wait for the end of the frame
        yield return new WaitForEndOfFrame();
        
        // Update each shop with its stored values
        for (int i = 0; i < shops.Length; i++)
        {
            shop shopScript = shops[i].GetComponent<shop>();
            if (shopScript != null)
            {
                // Use reflection to set private fields
                FieldInfo xField = typeof(shop).GetField("x", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo timeRatioField = typeof(shop).GetField("timeRatio", BindingFlags.NonPublic | BindingFlags.Static);
                
                if (xField != null && timeRatioField != null)
                {
                    xField.SetValue(shopScript, shopXValues[i]);
                    timeRatioField.SetValue(null, shopTimeRatios[i]);
                }
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // Store current values from each shop
        for (int i = 0; i < shops.Length; i++)
        {
            shop shopScript = shops[i].GetComponent<shop>();
            if (shopScript != null)
            {
                // Use reflection to access private fields
                FieldInfo xField = typeof(shop).GetField("x", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo timeRatioField = typeof(shop).GetField("timeRatio", BindingFlags.NonPublic | BindingFlags.Static);
                
                if (xField != null && timeRatioField != null)
                {
                    shopXValues[i] = (int)xField.GetValue(shopScript);
                    shopTimeRatios[i] = (float)timeRatioField.GetValue(null);
                }
            }
        }
    }
    
    // Called when the object is destroyed
    private void OnDestroy()
    {
        // Unregister from scene loaded events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
