﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("Set in Inspector: Enemy")]

    public float speed = 10f;
    public float fireRate = 0.3f;  
    public float health = 10;
    public int score = 100;

    public float powerUpDropChance = 1f;

    public float showDamageDuration = 0.1f;

    public Vector3 pos
    {
        get { return this.transform.position; }

        set { this.transform.position = value; }

    }

    protected BoundsCheck bndCheck;

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;

    public Material[] materials;

    public bool showingDamage = false;

    public float damageDoneTime;

    public bool notifiedOfDestruction = false; // will be used later

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();

        materials = Utils.GetAllMaterials(gameObject);

        originalColors = new Color[materials.Length];

        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject otherGO = collision.gameObject;

        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                // Don't damage this if we're off screen
                if (!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                health -= Main.GetWeaponDefinition(p.type).damageOnHit;

                ShowDamage();

                if (health <= 0)
                {

                    if (!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }

                    notifiedOfDestruction = true;

                    Destroy(this.gameObject);


                }

                Destroy(otherGO);

                break;

            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name);
                break;
        }
    }

    void ShowDamage()
    {
        foreach (Material m in materials)
        {

            m.color = Color.red;

        }

        showingDamage = true;

        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage()
    {
        for (int i = 0; i < materials.Length; i++)
        {

            materials[i].color = originalColors[i];

        }

        showingDamage = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }


        if (bndCheck != null && bndCheck.offDown)
        {
                Destroy(gameObject);
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;

        tempPos.y -= speed * Time.deltaTime;

        pos = tempPos;
    }
}
