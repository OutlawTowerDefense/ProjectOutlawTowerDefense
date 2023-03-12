using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GatlingGun : MonoBehaviour
{
    // target the gun will aim at
    Transform go_target;

    // Gameobjects need to control rotation and aiming
    public Transform go_baseRotation;
    public Transform go_GunBody;
    public Transform go_barrel;
    public AudioClip startShootAudio, shootingAudio, endShootAudio, rotationAudio;
    public AudioSource src;
    public AudioSource src2;
    // Gun barrel rotation
    public float barrelRotationSpeed;

    public Transform gunHeadTransform;
    public float gunHeadRestingHeight = 0.1f;
    public float restingLerpSpeed = 10.0f;
    // Distance the turret can aim and fire from
    public float firingRange;

    // Particle system for the muzzel flash
    public ParticleSystem muzzelFlash;

    private Vector3 position;

    float currentRotationSpeed;  

    // Used to start and stop the turret firing
    bool canFire = false;
    bool hasPlayedStart = false;

    public Vector3 Position { get; set; }

    void Start()
    {
        // Set the firing range distance
        GetComponent<SphereCollider>().radius = firingRange;
    }

    void Update()
    {
        AimAndFire();
    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to show the firing range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, firingRange);
    }

    // Detect an Enemy, aim and fire
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            go_target = other.transform;
            canFire = true;
        }

    }
    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            go_target = other.transform;
            canFire = true;
        }
    }

    // Stop firing
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            canFire = false;
        }
    }



    void AimAndFire()
    {
        // Gun barrel rotation
        go_barrel.transform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);

        // if can fire turret activates
        if (canFire)
        {
            // aim at enemy
            Vector3 baseTargetPosition = new Vector3(go_target.position.x, transform.position.y, go_target.position.z);
            Vector3 gunBodyTargetPosition = new Vector3(go_target.position.x, go_target.position.y, go_target.position.z);

            if (!src2.isPlaying)
                src2.PlayOneShot(rotationAudio);

            Quaternion baseTargetRotation = Quaternion.LookRotation(baseTargetPosition - transform.position);
            Quaternion gunBodyTargetRotation = Quaternion.LookRotation(gunBodyTargetPosition - go_GunBody.transform.position);

            float baseRotationSpeed = 5f; // Change this value to adjust rotation speed
            float gunBodyRotationSpeed = 10f; // Change this value to adjust rotation speed

            go_baseRotation.transform.rotation = Quaternion.Slerp(go_baseRotation.transform.rotation, baseTargetRotation, Time.deltaTime * baseRotationSpeed);
            go_GunBody.transform.rotation = Quaternion.Slerp(go_GunBody.transform.rotation, gunBodyTargetRotation, Time.deltaTime * gunBodyRotationSpeed);


            // start particle system 
            if (!muzzelFlash.isPlaying)
            {
                if (!src.isPlaying && !hasPlayedStart)
                {                 
                    src.PlayOneShot(startShootAudio);
                    hasPlayedStart = true;
                }
                if (src.isPlaying)
                    // start rotation
                    currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, barrelRotationSpeed, Time.deltaTime * 0.2f);

                if (!src.isPlaying && hasPlayedStart)
                {                    
                    muzzelFlash.Play();
                    src.clip = shootingAudio;
                    src.Play();
                }
            }
        }

        else
        {
            // slow down barrel rotation and stop
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0, 2f * Time.deltaTime);

            // stop the particle system
            if (muzzelFlash.isPlaying || !src.isPlaying && hasPlayedStart)
            {
                muzzelFlash.Stop();
                src.Stop();
                src.PlayOneShot(endShootAudio);
                hasPlayedStart = false;
            }

        }
    }
}