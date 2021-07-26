using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Transform greenBar;
    private Transform blackBar;

    private GameObject unit;
    float hp;
    float hpMax;
    float hpPercent;
    float elapsed;
    // Start is called before the first frame update
    void Start()
    {
        greenBar = transform.Find("Green");
        blackBar = transform.Find("Black");

        unit = transform.parent.parent.gameObject;
        hp = unit.GetComponent<UnitBehaviour>().hp;
        hpMax = unit.GetComponent<UnitBehaviour>().hpMax;
        UpdateHP();
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
     if (elapsed >= 0.5f) {
         elapsed = elapsed % 0.5f;
            UpdateHP();
        }
        }
    void UpdateHP()
    {
        unit = transform.parent.parent.gameObject;
        hp = unit.GetComponent<UnitBehaviour>().hp;
        hpMax = unit.GetComponent<UnitBehaviour>().hpMax;
        hpPercent = hp / hpMax;
             greenBar.localScale = new Vector3(hpPercent, greenBar.localScale.y, greenBar.localScale.z);
        blackBar.localScale = new Vector3(1- hpPercent, blackBar.localScale.y, blackBar.localScale.z);


    }
}
