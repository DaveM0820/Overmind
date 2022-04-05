using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleShot : MonoBehaviour
{
    bool alive = false;
    Vector3 velocity;
    public GameObject hitEffectGround;
    ParticleSystem hitEffectGroundParticle;
    public GameObject hitEffectUnit;
    ParticleSystem hitEffectUnitParticle;
    ParticleSystem barrelSplashParticle;
    UnitBehaviour targetUnitBehaviour;
    public GameObject bullet;
    public float shotVelocity;
    public int untrackedShotUpdateCounterMax;
    int untrackShotTimestepCounter = 0;
    GameObject target;
    public GameObject text;
    int destroyTimer;
    bool tracked;
    bool hit;
    int damagePerShot = 0;
    float traveltimemax;
    float traveltime;
    private void Start() {
        hitEffectGroundParticle = hitEffectGround.GetComponent<ParticleSystem>();
        hitEffectUnitParticle = hitEffectUnit.GetComponent<ParticleSystem>();
    }
    public void Initalize(Vector3 startPosition, Vector3 startDirection, int damage, bool tracked = false, GameObject target = null, bool hit = true, float distance = 0, UnitBehaviour targetUnitBehaviour = null) {
        damagePerShot = damage;
        bullet.SetActive(true);
        hitEffectGround.SetActive(false);

        hitEffectUnit.SetActive(false);

        alive = true;
        transform.position = startPosition;
        transform.forward = -startDirection;
        this.tracked = tracked;
        destroyTimer = 0;
        if (tracked)
        {
            gameObject.GetComponent<SphereCollider>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<SphereCollider>().enabled = false;
            this.target = target;
            this.hit = hit;
            traveltime = 0;
            this.targetUnitBehaviour = targetUnitBehaviour;
            if (!hit)
            {
                transform.forward += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            }
            else
            {
                if (targetUnitBehaviour.isBuilding)
                {
                    transform.forward += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

                }
            }
        }
        velocity = transform.forward * shotVelocity;
        traveltimemax = (distance * Time.deltaTime) / shotVelocity;

    }
    // Update is called once per frame
    void Update() {
        destroyTimer += 1;
        if (destroyTimer > 100)
        {
            gameObject.SetActive(false);
        }

        if (!tracked && alive)
        {

            untrackShotTimestepCounter++;
            if (untrackShotTimestepCounter > untrackedShotUpdateCounterMax) // is it time to update the shot
            {
            
                untrackShotTimestepCounter = 0;
                transform.position += velocity * untrackedShotUpdateCounterMax;

                if (!hit)
                {
            

                    if (transform.position.y <= 1)
                    {
                        hitEffectGround.SetActive(true);
                        hitEffectGround.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                        bullet.SetActive(false);

                        hitEffectGroundParticle.Play();
                        alive = false;

                    }
                }
                else
                {
                    if (target.activeSelf != true)
                    {
                        hit = false;
                    }
                    else
                    {
                   
                    traveltime += Time.deltaTime * untrackedShotUpdateCounterMax;

                    if (traveltime > traveltimemax)
                    {
                        alive = false;
                            hitEffectUnit.SetActive(true);

                            hitEffectUnitParticle.transform.position = new Vector3(target.transform.position.x, Random.Range(0, targetUnitBehaviour.headHeight), target.transform.position.z);
                        hitEffectUnitParticle.Play();
                        bullet.SetActive(false);
                        targetUnitBehaviour.changeHP(-damagePerShot);
                        }
                    }
                }
            }
        }
        if (tracked && alive)
        {
            transform.position += velocity;
            if (transform.position.y <= 1 && alive)
            {
                hitEffectGround.SetActive(true);

                hitEffectGround.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                hitEffectGroundParticle.Play();
                bullet.SetActive(false);

                alive = false;

            }


        }

    }

    private void OnTriggerEnter(Collider collider) {



        if (collider.gameObject.layer == 7 || collider.gameObject.layer == 9)
        {

            alive = false;
            hitEffectUnitParticle.transform.position = transform.position;
            //  hitEffectUnitParticle.transform.rotation = Quaternion.Euler(-90, 0, 0);
            hitEffectUnitParticle.Play();
            collider.gameObject.GetComponent<UnitBehaviour>().changeHP(-damagePerShot);
            GameObject newText = Instantiate(text);
            newText.GetComponent<TextMesh>().text = -damagePerShot + " HP";
            newText.GetComponent<TextMesh>().color = Color.red;
            newText.transform.position = transform.position;


        }
        else
        {
            alive = false;
            hitEffectGroundParticle.transform.position = transform.position;
            //  hitEffectUnitParticle.transform.rotation = Quaternion.Euler(-90, 0, 0);
            hitEffectGroundParticle.Play();

        }

    }

}

