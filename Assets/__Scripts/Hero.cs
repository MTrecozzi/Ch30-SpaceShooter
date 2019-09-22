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

    [Header("Set Dynamically")]
    public float shieldLevel = 1;

    private void Awake()
    {
        if (s == null)
        {
            s = this;
        } else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero Singleton!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
    }
}
