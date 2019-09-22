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

    public Weapon[] weapons;

    [Header("Set Dynamically")]

    [SerializeField]
    private float _shieldLevel = 1;

    private GameObject lastTriggerGO = null;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    private void Start()
    {
        if (s == null)
        {
            s = this;
        } else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero Singleton!");
        }
        //fireDelegate += TempFire;

        ClearWeapons();

        weapons[0].SetType(WeaponType.blaster);
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
        } else if (go.CompareTag("PowerUp")) {
            AbsorbPowerUp(go);
        }
        else print("Triggered by non-Enemy: " + go.name);
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;

            default:
                if (pu.type == weapons[0].type) // same type, so add another level of it
                {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null) // if remaining slots != null
                    {
                        w.SetType(pu.type);
                    }
                } else
                {
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }

        pu.AbsorbedBy(this.gameObject);
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

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons()
    {
        foreach(Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }
}
