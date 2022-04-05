using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackedArty : MonoBehaviour
{
    // Start is called before the first frame update
    bool alive = false;
    Vector3 velocity;
    public GameObject explosion;
    ParticleSystem explosionParticle;
    ParticleSystem barrelSplashParticle;
    public GameObject bullet;
    public GameObject barrelSplash;
    GameObject shooter;
    float destroyTimer;
    public int damage;
    public GameObject text;
    private void Start() {
        explosionParticle = explosion.GetComponent<ParticleSystem>();
            
            barrelSplashParticle = barrelSplash.GetComponent<ParticleSystem>();

    }
    public void initalize(Vector3 startVelocity, Vector3 startPosition, GameObject shooter) {
        bullet.SetActive(true);
        alive = true;
        velocity = startVelocity;
        transform.forward = startVelocity.normalized;
        transform.position = startPosition;
        this.shooter = shooter;
        barrelSplash.transform.position = startPosition;
        barrelSplash.transform.forward = startVelocity;
        destroyTimer = 0;

        barrelSplashParticle.Play();

    }
    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            transform.position += velocity;
            velocity -= new Vector3(0, 0.015f, 0);
            transform.forward = velocity;
        }
        else
        {
            destroyTimer += Time.deltaTime;
            if (destroyTimer > 5)
            {
                explosionParticle.Stop();
                gameObject.SetActive(false);
                destroyTimer = 0;
            }
        }
        if (transform.position.y <= 1 && alive)
        {
            alive = false;
            bullet.SetActive(false);
            explosionParticle.Play();
            Collider[] hitColliders = Physics.OverlapBox(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(6, 6, 6), transform.rotation,LayerMask.GetMask("Buildings", "Units"), QueryTriggerInteraction.Collide);//get a list of objects within the selection box
           
            
            foreach (Collider col in hitColliders)
            {
         
                    col.gameObject.GetComponent<UnitBehaviour>().changeHP(-damage);
                    GameObject newText = Instantiate(text);
                    newText.transform.position = col.gameObject.transform.position;
                    newText.GetComponent<TextMesh>().text = -damage + " HP";
                    newText.GetComponent<TextMesh>().color = Color.red;


                

            }
            Debug.Log("colliders " + hitColliders.Length);
        }
     
    }
    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject != shooter)
        {
            alive = false;
            bullet.SetActive(false);
            explosionParticle.Play();
            Collider[] hitColliders = Physics.OverlapBox(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(6, 6, 6), transform.rotation, LayerMask.GetMask("Buildings", "Units"), QueryTriggerInteraction.Collide);//get a list of objects within the selection box
            
            foreach (Collider col in hitColliders)
            {
             
                    col.gameObject.GetComponent<UnitBehaviour>().changeHP(-damage);
                    GameObject newText = Instantiate(text);
                    newText.transform.position = col.gameObject.transform.position;

                    newText.GetComponent<TextMesh>().text = -damage + " HP";
                    newText.GetComponent<TextMesh>().color = Color.red;



            }

        }
    }

}
