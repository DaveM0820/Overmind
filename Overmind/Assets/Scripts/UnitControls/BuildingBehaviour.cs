using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public List<string> buildQueue;
    private Transform scaffold;
    public float maxScaffoldHeight;
    private float scaffoldPosition;
    public float scaffoldHeight;
    public bool placed;
    public bool built;
    private float hp;
    private float hpMax;
    private float hpPercent;
    private float elapsed;

    void Update()
    {
        if (built == false)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= 0.25f)
            {
                elapsed = elapsed % 0.25f;
                UpdateScaffold();
            }
        } else
        {
            scaffold.gameObject.SetActive(false);


        }
    }

    // Update is called once per frame
    public void UpdateScaffold()
    {
        scaffold = transform.Find("Scaffold");
        hp = gameObject.GetComponent<UnitBehaviour>().hp;
        hpMax = gameObject.GetComponent<UnitBehaviour>().hpMax;

        hpPercent = 100 * (hp / hpMax);
        scaffoldHeight = maxScaffoldHeight - ((maxScaffoldHeight) * (hpPercent / 100));

        scaffold.position = new Vector3(transform.position.x, scaffoldHeight, transform.position.z);
        if (hp >= hpMax)
        {
           built = true;
           gameObject.GetComponent<UnitBehaviour>().hp = hpMax;

        }
      
    }

}
