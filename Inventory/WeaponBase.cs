using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [SerializeField]
    private string weaponName;

    [SerializeField]
    private float damageValue;

    [SerializeField]
    private Sprite weaponIcon;

    [SerializeField]
    private Vector3 positionOffset;

    [SerializeField]
    private Vector3 rotationOffset;

    [SerializeField]
    private GameObject Tip;

    [SerializeField]
    private GameObject Base;

    [SerializeField]
    private GameObject trailMesh;

    [SerializeField]
    private int trailFrameLength;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private int frameCount;
    private Vector3 previousTipPosition;
    private Vector3 previousBasePosition;

    private const int NUM_VERTICES = 12;

    private void InitializeTrailParams()
    {
        mesh = new Mesh();
        trailMesh.GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[trailFrameLength * NUM_VERTICES];
        triangles = new int[vertices.Length];

        previousTipPosition = Tip.transform.position;
        previousBasePosition = Base.transform.position;
    }

    private void Start()
    {
        //InitializeTrailParams();
    }

    private void TrailDEPRECATED()
    {
        if (frameCount == (trailFrameLength * NUM_VERTICES))
        {
            frameCount = 0;
        }

        vertices[frameCount] = Base.transform.position;
        vertices[frameCount + 1] = Tip.transform.position;
        vertices[frameCount + 2] = previousTipPosition;

        vertices[frameCount + 3] = Base.transform.position;
        vertices[frameCount + 4] = previousTipPosition;
        vertices[frameCount + 5] = Tip.transform.position;

        vertices[frameCount + 6] = previousTipPosition;
        vertices[frameCount + 7] = Base.transform.position;
        vertices[frameCount + 8] = previousBasePosition;

        vertices[frameCount + 9] = previousTipPosition;
        vertices[frameCount + 10] = previousBasePosition;
        vertices[frameCount + 11] = Base.transform.position;

        triangles[frameCount] = frameCount;
        triangles[frameCount + 1] = frameCount + 1;
        triangles[frameCount + 2] = frameCount + 2;
        triangles[frameCount + 3] = frameCount + 3;
        triangles[frameCount + 4] = frameCount + 4;
        triangles[frameCount + 5] = frameCount + 5;
        triangles[frameCount + 6] = frameCount + 6;
        triangles[frameCount + 7] = frameCount + 7;
        triangles[frameCount + 8] = frameCount + 8;
        triangles[frameCount + 9] = frameCount + 9;
        triangles[frameCount + 10] = frameCount + 10;
        triangles[frameCount + 11] = frameCount + 11;

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        previousBasePosition = Tip.transform.position;
        previousBasePosition = Base.transform.position;

        frameCount += NUM_VERTICES;
    }

    private void LateUpdate()
    {
        
    }

    public string GetName()
    {
        return this.weaponName;
    }

    public float GetDamageValue()
    {
        return this.damageValue;
    }

    public Sprite GetIcon()
    {
        return this.weaponIcon;
    }

    public Vector3 GetPositionOffset()
    {
        return this.positionOffset;
    }

    public Vector3 GetRotationOffset()
    {
        return this.rotationOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleEnemyTriggerEnter(other);
    }

    private void HandleEnemyTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Monster")
        {
            var monster = other.gameObject.GetComponent<BaseEnemyController>();
            if (monster)
            {
                if (monster.IsDead())
                    return;

                var playerRef = FindObjectOfType<PlayerController>();
                if (playerRef)
                {
                    if (playerRef.isAttacking)
                    {
                        monster.DecrementHealth(this.damageValue);
                    }
                }
                if (monster.GetHealth() <= 0.0f)
                {
                    // Instantiate one of the collectable

                    GameObject prefab;

                    int ch = UnityEngine.Random.Range(0, 3);
                    if (ch == 0)
                    {
                        prefab = Resources.Load<GameObject>("coin");
                    }
                    else if (ch == 1)
                    {
                        prefab = Resources.Load<GameObject>("health");
                    }
                    else if (ch == 2)
                    {
                        prefab = Resources.Load<GameObject>("powerups");
                    }
                    else
                    {
                        prefab = Resources.Load<GameObject>("coin");
                    }

                    var collectable = Instantiate(prefab);

                    collectable.transform.position = new Vector3(monster.transform.position.x, collectable.transform.position.y, collectable.transform.position.z);

                    monster.GetComponent<BaseEnemyController>().Die();

                    var gameMan = FindObjectOfType<GameMan>();
                    if (gameMan)
                    {
                        gameMan.DecreaseEnemiesCount();
                    }

                    Destroy(monster.gameObject, 2);
                }
            }
        }
    }
}
