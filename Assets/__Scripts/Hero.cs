using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{

    static public Hero s;

    [Header("Set In Inspector")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;

    public GameObject projectilePrefab;

    public float projectileSpeed = 40;

    [Header("Set Dynamically")]

    [SerializeField]
    private float _shieldLevel = 1;

    private GameObject lastTriggerGO = null;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    private void Awake()
    {
        if (s == null)
        {
            s = this;
        } else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero Singleton!");
        }

        //fireDelegate += TempFire;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        print("Triggered: " + go.name);

        if (go == lastTriggerGO)
        {
            return;
        }

        if (go.CompareTag("Enemy"))
        {
            shieldLevel -= 1;

            Destroy(go);
        }
        else print("Triggered by non-Enemy: " + go.name);
    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 newPos = transform.position;

        newPos.x += xAxis * speed * Time.deltaTime;
        newPos.y += yAxis * speed * Time.deltaTime;

        transform.position = newPos;

        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        //if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        //{
        //    TempFire();
        //}

        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
    }


    public float shieldLevel
    {
        get { return _shieldLevel; }
        set {
            _shieldLevel = Mathf.Min(value, 4);
                if (value < 0)
                {
                    Destroy(this.gameObject);
                    Main.S.DelayedRestart(gameRestartDelay);
                }
            }
    }
}
