using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemyLaser : MonoBehaviour
{
    public Transform start, end;
    LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startColor = new Color(1, 1, 1, 0);
        line.endColor = new Color(1, 1, 1, 0);
        line.startWidth = 1f;
        line.endWidth = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, start.position);
        line.SetPosition(1, end.position);
    }
}
