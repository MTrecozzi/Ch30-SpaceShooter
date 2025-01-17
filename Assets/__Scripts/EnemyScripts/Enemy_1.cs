﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    public float waveFrequency = 2;
    public float waveWidth = 3;
    public float waveRoty = 45;

    private float x0;
    private float birthTime;

    private void Start()
    {
        x0 = pos.x;
        birthTime = Time.time;
    }

    public override void Move()
    {
        Vector3 tempPos = pos;

        float age = Time.time - birthTime;

        float theta = Mathf.PI * 2 * age / waveFrequency;

        float sine = Mathf.Sin(theta);

        tempPos.x = waveWidth * sine;

        pos = tempPos;

        Vector3 rot = new Vector3(0, sine * waveRoty, 0);

        this.transform.rotation = Quaternion.Euler(rot);

        // translate down on the y axis
        base.Move();
    }
}
