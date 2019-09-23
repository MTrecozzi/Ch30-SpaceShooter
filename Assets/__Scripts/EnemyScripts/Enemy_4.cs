using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public class Part
{
    public string name;
    public float health;

    public string[] protectedBy;

    //[HideInInspector]
    public GameObject go;

    //[HideInInspector]
    public Material mat;


}

public class Enemy_4 : Enemy
{

    [Header("Set in INspector: Enemy_4")]
    
    public Part[] parts; // the array of ship parts

    private Vector3 p0, p1; // two points to interpolate
    private float timeStart;

    private float duration = 4;





    // Start is called before the first frame update
    void Start()
    {
        p0 = p1 = pos;

        InitMovement();

        Transform t;

        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if (t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }

    void InitMovement()
    {
        p0 = p1;

        // assign a new on-screen location to p1;

        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;

        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        timeStart = Time.time;

    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();

            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2);
        pos = (1 - u) * p0 + u * p1;
    }

    Part FindPart(string n)
    {
        foreach (Part prt in parts)
        {
            if (prt.name == n)
            {
                return prt;
            }
        }

        return (null);
    }

    Part FindPart (GameObject go)
    {
        foreach (Part prt in parts)
        {
            if (prt.go == go)
            {
                return (prt);
            }
        }
        return (null);
    }

    bool Destroyed (GameObject go)
    {
        return Destroyed(FindPart(go));
    }

    bool Destroyed (string n)
    {
        return Destroyed(FindPart(n));
    }

    bool Destroyed (Part prt)
    {
        if (prt == null)
        {
            return true;
        }

        return prt.health <= 0;
    }

    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;

        damageDoneTime = Time.time + showDamageDuration;

        showingDamage = true;
    }

    void OnCollisionEnter(Collision coll)
    {                                
        GameObject other = coll.gameObject;

        switch (other.tag)
        {
            case "ProjectileHero":

                Projectile p = other.GetComponent<Projectile>();
                // If this Enemy is off screen, don't damage it.

                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;

                }

                // Hurt this Enemy
                // Get the gameobject that was hit
                GameObject goHit = coll.contacts[0].thisCollider.gameObject; // f

                // use the gameobject that was hit to find out the part that was hit
                Part prtHit = FindPart(goHit);

                if (prtHit == null)
                { // If prtHit wasn't found…             // g

                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }

                // Check whether this part is still protected

                if (prtHit.protectedBy != null)
                {                            // h
                    foreach (string s in prtHit.protectedBy)
                    {


                        // If one of the protecting parts hasn't been destroyed...


                        if (!Destroyed(s))
                        {


                            // ...then don't damage this part yet


                            Destroy(other);  // Destroy the ProjectileHero


                            return;          // return before damaging Enemy_4


                        }


                    }


                }



                // It's not protected, so make it take damage


                // Get the damage amount from the Projectile.type and Main.W_DEFS


                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;


                // Show damage on the part


                ShowLocalizedDamage(prtHit.mat);


                if (prtHit.health <= 0)
                {                                    // i


                    // Instead of destroying this enemy, disable the damaged part


                    prtHit.go.SetActive(false);


                }


                // Check to see if the whole ship is destroyed


                bool allDestroyed = true; // Assume it is destroyed


                foreach (Part prt in parts)
                {


                    if (!Destroyed(prt))
                    {  // If a part still exists...


                        allDestroyed = false;  // ...change allDestroyed to false


                        break;                 // & break out of the foreach loop


                    }


                }


                if (allDestroyed)
                { // If it IS completely destroyed...      // j


                    // ...tell the Main singleton that this ship was destroyed


                    Main.S.ShipDestroyed(this);


                    // Destroy this Enemy


                    Destroy(this.gameObject);


                }


                Destroy(other);  // Destroy the ProjectileHero


                break;


        }


    }
}
