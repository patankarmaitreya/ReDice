using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpawnItemBehavior : MonoBehaviour
{
    public int Score;
    public bool causesDamage = false;
    public int damageCaused = 0;
    public GameObject destroyVisualEffect;
    public GameObject audioPlayer;
    private PlayerController playerController;

    public AudioClip SFX;
    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(this.name + " " + other.name);

        if (other.tag == "Player")
        {
            GameObject VFX=Instantiate(destroyVisualEffect);
            GameObject sound = Instantiate(audioPlayer);

            sound.transform.position = this.transform.position;
            VFX.transform.position=this.transform.position;

            sound.GetComponent<AudioSource>().clip = SFX;

            sound.GetComponent<AudioSource>().Play();

            destroyVisualEffect.GetComponent<ParticleSystem>().Play();

            playerController.collidedGOScore = Score;
            if (causesDamage)
            {
                ScoreSystem.GetInstance().notAFall = true;
                ScoreSystem.GetInstance().UpdateLife(damageCaused);
            }

            Destroy(this.gameObject);
        }
    }
    
}
