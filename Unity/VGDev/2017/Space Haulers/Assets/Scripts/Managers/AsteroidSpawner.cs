using UnityEngine;
using System.Collections;
using YeggQuest.NS_Spline;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class AsteroidSpawner : MonoBehaviour {

    [Header("Warp Tunnel Dimensions")]
    [Range(0f, 100f)]
    public float warpTunnelRadius = 50f;
    [Range(0f, 1f)]
    public float minimumAsteroidProbability;
    public int asteroidsPerSector = 10;
    public int sectors = 8;
    public int jitterRange = 10;
    public int lengthBetweenDisks;

    [Header("Asteroid Options")]
    public GameObject[] asteroids;
    public AnimationCurve distribution;
    public Material asteroidMaterial;
    public Mesh asteroidMesh;
    public SplineWrapper wrapper;

    private ArrayList asteroidObjects;
    private GameObject asteroidParent;
    float length;

    void OnEnable()
    {
        if (wrapper == null)
            wrapper = GameObject.FindObjectOfType<SplineMeshWrapper>();
    }

    public void spawnAsteroids() {
        if (Application.isPlaying)
            return;

        if (asteroidParent == null)
            asteroidParent = new GameObject("asteroidParent" + Time.time);
        //asteroidParent.transform.SetParent(transform.parent);
        //asteroidObjects = new ArrayList();
        // Hand like a clock, it rotates
        Vector3 hand = new Vector3(0, 1, 0);
        // Refernce to next vertex on the spline
        SplineLerpResult vertex;
        // Used for moving the asteroids around to remove uniformity
        Vector3 jitter;
        GameObject asteroidTemp, childTemp;
        length = wrapper.spline.totalLength / wrapper.spline.vertices.Length;
        // First loop: Iterate through the length of the tube
        for (int j = 1; j < length; j++)
        {
            // Second loop: Iterate through the sectors of a disk
            for (int k = 1; k < sectors; k++)
            {
                // Third loop: Iterate through individual asteroids in a sector
                for (int i = 1; i < asteroidsPerSector; ++i)
                {
                    // Calculate random jitter variation
                    jitter = Vector3.up * Random.Range(-jitterRange, jitterRange)
                        + Vector3.right * Random.Range(-jitterRange, jitterRange)
                        + Vector3.forward * Random.Range(-jitterRange, jitterRange);

                    // Evaluate the curve to get a length along the sector
                    float coef = distribution.Evaluate(Random.value);

                    if (coef > 0) {
                        asteroidTemp = new GameObject("asteroid " + i);
                        
                        asteroidTemp.transform.position = hand + coef * warpTunnelRadius * jitter;
                        asteroidTemp.transform.SetParent(asteroidParent.transform);

                        // Spawn asteroid at random location
                        childTemp = (GameObject)GameObject.Instantiate(
                            asteroids[Random.Range(0, asteroids.Length)],
                            asteroidTemp.transform.position,
                            Random.rotation);

                        childTemp.transform.SetParent(asteroidTemp.transform);
                        //asteroidObjects.Add(asteroidTemp);
                    }
                }
                // Rotate the hand
                //hand = Quaternion.AngleAxis(360 / sectors, Vector3.forward) * hand;
            }
            // Move hand to the next location

            vertex = wrapper.Lerp(new SplineLerpQuery(j / length));
            hand += (vertex.worldPosition - hand);
        }
        
    }

    public void destroyAsteroids()
    {
        if (Application.isPlaying)
            return;
        if (asteroidParent == null)
            return;

        int temp = asteroidParent.transform.childCount;
        while (temp != 0)
        {
            temp--;
            DestroyImmediate(asteroidParent.transform.GetChild(0).gameObject);
        }
        DestroyImmediate(asteroidParent);
        asteroidParent = null;
    }

}
