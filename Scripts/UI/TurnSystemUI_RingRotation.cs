using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystemUI_RingRotation : MonoBehaviour
{
    [SerializeField] private Transform ring1_Transform;
    [SerializeField] private Transform ring2_Transform;

    private float ring1_SpeedRotation;
    private float ring2_SpeedRotation;

    private void Awake()
    {
        ring1_SpeedRotation = 15f * Time.deltaTime;
        ring2_SpeedRotation = 10f * Time.deltaTime;
    }

    private void Update()
    {
        ring1_Transform.eulerAngles += new Vector3(0, 0, ring1_SpeedRotation);
        ring2_Transform.eulerAngles -= new Vector3(0, 0, ring2_SpeedRotation);
    }
}
