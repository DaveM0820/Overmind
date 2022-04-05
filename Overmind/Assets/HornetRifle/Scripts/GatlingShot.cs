using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingShot : MonoBehaviour
{
    // Start is called before the first frame update
    bool alive = false;
    Vector3 velocity;
    public GameObject hitEffectGround;
    ParticleSystem hitEffectGroundParticle;
    public GameObject hitEffectUnit;
    ParticleSystem hitEffectUnitParticle;
    ParticleSystem barrelSplashParticle;
    public GameObject bullet;
    GameObject barrelSplash;
    GameObject shooter;
    int destroyTimer;
    public int damage;
    public GameObject text;
    private void Start() {
        hitEffectGroundParticle = hitEffectGround.GetComponent<ParticleSystem>();
        hitEffectUnitParticle = hitEffectUnit.GetComponent<ParticleSystem>();



    }
    public void Initalize(Vector3 startVelocity, Vector3 startPosition,  Vector3 forward, GameObject shooter) {
        bullet.SetActive(true);
        alive = true;
        velocity = startVelocity;
        transform.forward = startVelocity.normalized;
        transform.position = startPosition;
        this.shooter = shooter;
        barrelSplash.transform.position = startPosition;
        barrelSplash.transform.forward = forward;
        destroyTimer = 0;
        transform.forward = velocity;
        transform.localScale = new Vector3(0, 1, 0); 
    }
    // Update is called once per frame
    void Update() {


        if (transform.localScale.z < 1)
        {
        transform.localScale += new Vector3(0.001f, 1, 0.001f);
        }
        transform.position += velocity;
            if (transform.position.y <= 1 && alive)
            {
                hitEffectGround.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                hitEffectGroundParticle.Play();
                alive = false;

            }
        
        destroyTimer += 1;
        if (destroyTimer > 100)
        {
            alive = false;

            gameObject.SetActive(false);
            destroyTimer = 0;
        }

      
    }
    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject != shooter)
        {
            if (collider.gameObject.layer == 7 || collider.gameObject.layer == 9)
            {
                alive = false;
                hitEffectUnitParticle.transform.position = new Vector3(hitEffectUnitParticle.transform.position.x, 0, hitEffectUnitParticle.transform.position.z);
                hitEffectUnitParticle.Play();
                collider.gameObject.GetComponent<UnitBehaviour>().changeHP(-damage);
                GameObject newText = Instantiate(text);

                newText.transform.position = collider.gameObject.transform.position;

                    newText.GetComponent<TextMesh>().text = -damage + " HP";
                    newText.GetComponent<TextMesh>().color = Color.red;
                    newText.transform.position = transform.position;


                }

            }


        }
      
    
}
