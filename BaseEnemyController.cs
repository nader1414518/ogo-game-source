using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseEnemyController : MonoBehaviour
{
    [SerializeField]
    protected string enemyName;
    [SerializeField]
    protected float damageValue;
    [SerializeField]
    protected float speed;

    [SerializeField]
    protected Slider healthSlider;

    protected Animator animator;
    protected Rigidbody rb;
    protected PlayerController playerRef;

    private float attackCooldown = 0.0f;
    private float health = 1.0f;

    private bool isAttacking = false;

    protected void InitializeFields()
    {
        this.animator = this.GetComponent<Animator>();
        this.rb = this.GetComponent<Rigidbody>();
        this.playerRef = FindObjectOfType<PlayerController>();

        this.health = 1.0f;

        this.healthSlider.value = 1.0f;
    }

    private void UpdateHealthSlider()
    {
        this.healthSlider.value = this.health;
    }

    public void DecrementHealth(float value)
    {
        this.health -= value;
        UpdateHealthSlider();
    }

    public float GetHealth()
    {
        return this.health;
    }

    private void OnEnable()
    {
        InitializeFields();
    }

    private void Update()
    {
        Move();
    }

    protected void Move()
    {
        if (playerRef)
        {
            if (!isAttacking)
            {
                this.transform.position = Vector3.MoveTowards(
                    this.transform.position,
                    new Vector3(playerRef.transform.position.x,
                        this.transform.position.y,
                        playerRef.transform.position.z),
                    speed * Time.deltaTime);
                //this.transform.LookAt(playerRef.transform.position, Vector3.up);
                Vector3 relativePos = playerRef.transform.position - this.transform.position;

                // the second argument, upwards, defaults to Vector3.up
                Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, rotation.eulerAngles.y, this.transform.eulerAngles.z);

                this.animator.SetFloat("Speed", 0.0f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isAttacking = true;
            this.animator.SetFloat("Speed", 0.5f);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isAttacking = false;
            this.animator.SetFloat("Speed", 0.0f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player)
            {
                if (attackCooldown <= 0.0f)
                {
                    player.DecrementHealth(this.damageValue);
                    var gameMan = FindObjectOfType<GameMan>();
                    if (gameMan)
                    {
                        gameMan.UpdateHealthProgressBar();
                    }
                    attackCooldown = 1.0f;
                }
                else
                {
                    attackCooldown -= Time.deltaTime;
                }
            }
        }
    }

    public void Die()
    {
        this.animator.SetFloat("Speed", 1.0f);
    }
}
