using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Joystick playerJoystick;

    [SerializeField]
    private Animator playerAnimator;

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private CharacterController characterController;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private Dictionary<string, AudioClip> sfxClips;

    [SerializeField]
    private Slider powerProgressBar;

    [SerializeField]
    private AvatarObject avatarObject;

    [SerializeField]
    private WeaponBase currentWeapon;

    [SerializeField]
    private GameObject rightHandNode;

    [SerializeField]
    private GameObject groundSlashPrefab;

    [SerializeField]
    private TMP_Text coinsTxt;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float jumpForce;

    private bool isGrounded = false;

    public bool isAttacking = false;

    private bool isClimbing = false;

    private bool inPosition;
    private bool isLerping;
    float climbT;
    Vector3 startPos;
    Vector3 targetPos;
    Quaternion startRot;
    Quaternion targetRot;
    Transform climbHelper;

    [SerializeField]
    private float positionOffset;

    [SerializeField]
    private float offsetFromWall = 0.3f;

    [SerializeField]
    private float speedMutlipier = 0.2f;

    [SerializeField]
    private float health;
    [SerializeField]
    private float power;
    [SerializeField]
    private int money;
    [SerializeField]
    private int score;

    [SerializeField]
    public float minXMovement;

    [SerializeField]
    public float maxXMovement;

    [SerializeField]
    private float maxJump = 2.0f;

    private GameObject currentWeaponObject;
    private GroundSlash groundSlash;
    private Vector3 slashDestination;

    #region ClimbingFunctionality
    private void InitForClimb()
    {
        climbHelper = new GameObject().transform;
        climbHelper.name = "ClimbHelper";

        CheckForClimb();
    }

    private void CheckForClimb()
    {
        Vector3 origin = this.transform.position;
        origin.y += 1.4f;
        Vector3 dir = this.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, 2))
        {
            // Check walls here 
            if (hit.transform.gameObject.tag == "Wall")
            {
                InitForClimb(hit);
            }
            else
            {
                isClimbing = false;
                if (playerAnimator)
                {
                    Debug.Log("Player is not climbing anymore ... ");
                    playerAnimator.SetBool("isClimbing", isClimbing);
                }
            }
        }
    }

    private void InitForClimb(RaycastHit hit)
    {
        isClimbing = true;
        climbHelper.transform.rotation = Quaternion.LookRotation(-hit.normal);
        startPos = this.transform.position;
        targetPos = hit.point + (hit.normal * offsetFromWall);
        climbT = 0;
        inPosition = false;
        //characterController.enabled = false;
        if (playerAnimator)
        {
            Debug.Log("Player is climbing ... ");
            playerAnimator.SetBool("isClimbing", isClimbing);
        }
    }

    private void GetInClimbPosition()
    {
        climbT += Time.deltaTime;

        if (climbT > 1)
        {
            climbT = 1;
            inPosition = true;

            // Enable the IK
        }

        Vector3 tp = Vector3.Lerp(startPos, targetPos, climbT);
        this.transform.position = tp;
    }

    private void ClimbTick()
    {
        if (!isClimbing)
            return;

        if (!inPosition)
        {
            GetInClimbPosition();
            return;
        }
    }

    private Vector3 PosWithOffset(Vector3 origin, Vector3 target)
    {
        Vector3 direction = origin - target;
        direction.Normalize();
        Vector3 offset = direction * offsetFromWall;
        return target + offset;
    }
    #endregion

    #region Weapons
    public WeaponBase GetCurrentWeapon()
    {
        return this.currentWeapon;
    }

    public void InjectWeapon(WeaponBase weapon)
    {
        this.currentWeapon = weapon;

        PlaceWeapon(weapon);

        PlayerPrefs.SetString("current_player_weapon", weapon.GetName());
    }

    private void PlaceWeapon(WeaponBase weapon)
    {
        StartCoroutine(placeWeapon(weapon));
    }

    IEnumerator placeWeapon(WeaponBase weapon)
    {
        if (avatarObject)
        {
            if (currentWeaponObject != null)
            {
                Destroy(currentWeaponObject.gameObject);
            }

            weapon.gameObject.layer = this.rightHandNode.layer;

            yield return new WaitForSeconds(0.5f);

            currentWeaponObject = Instantiate(weapon.gameObject, rightHandNode.transform);

            // TODO: apply weapon placement parameters here
            currentWeaponObject.transform.localPosition = Vector3.zero + weapon.GetPositionOffset();
            currentWeaponObject.transform.localEulerAngles = weapon.GetRotationOffset();

            avatarObject.PlaceWeapon(weapon);
        }
    }
    #endregion

    #region UIAndInventory
    private void UpdateCoinsTxt()
    {
        if (coinsTxt)
        {
            coinsTxt.text = $"{money} $";
        }
    }

    private void UpdatePowerSlider()
    {
        if (powerProgressBar)
        {
            powerProgressBar.value = power;
        }
    }

    private void LoadPlayerData()
    {
        power = PlayerPrefs.GetFloat("player_power", 10.0f);
        health = PlayerPrefs.GetFloat("player_health", 1.0f);
        money = PlayerPrefs.GetInt("player_money", 0);
        score = PlayerPrefs.GetInt("player_score", 0);
    }

    private void SavePlayerData()
    {
        PlayerPrefs.SetFloat("player_health", health);
        PlayerPrefs.SetFloat("player_power", power);
        PlayerPrefs.SetInt("player_money", money);
        PlayerPrefs.SetInt("player_score", score);
    }

    private void DecrementPower(float value)
    {
        if (power > 0.0f)
        {
            if ((power - value) <= 0.0f)
            {
                power = 0.0f;
            }
            else
            {
                power -= value;
            }
        }
        else
        {
            power = 0.0f;
        }

        SavePlayerData();
    }

    public void IncrementPower(float value)
    {
        power += value;
        if (power >= 15.0f)
        {
            power = 15.0f;
        }
        SavePlayerData();

        UpdatePowerSlider();
    }

    public void DecrementHealth(float value)
    {
        if (health <= 0.0f)
        {
            // TODO: restart game here
            health = 1.0f;
            power = 10.0f;
            SavePlayerData();
        }
        else
        {
            health -= value;
            SavePlayerData();
        }

        var gameMan = FindObjectOfType<GameMan>();
        if (gameMan)
        {
            gameMan.UpdateHealthProgressBar();
        }
    }

    private void IncrementHealth(float value)
    {
        health += value;

        if (health >= 1.0f)
        {
            health = 1.0f;
        }

        SavePlayerData();
    }

    private void IncrementScore(int value)
    {
        score += value;

        SavePlayerData();
    }

    private void IncrementMoney(int value)
    {
        money += value;

        SavePlayerData();

        UpdateCoinsTxt();
    }
    #endregion

    #region Animations
    private void Move()
    {
        if (playerJoystick)
        {
            var xVal = playerJoystick.Horizontal;
            var yVal = playerJoystick.Vertical;

            if (xVal > 0)
            {
                this.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else if (xVal < 0)
            {
                this.transform.rotation = Quaternion.Euler(0, 270, 0);
            }

            if (xVal != 0)
            {
                if (this.transform.position.x <= minXMovement)
                {
                    if (xVal > 0)
                    {
                        //rb.velocity = new Vector3(xVal * speed, -9.8f, 0) * Time.deltaTime;
                        //rb.AddForce(new Vector3(xVal * speed, 0, 0) * Time.deltaTime);
                        characterController.Move(new Vector3(xVal * speed, 0, 0) * Time.deltaTime);
                        this.PlaySound("walk");
                    }
                }
                else if (this.transform.position.x >= maxXMovement)
                {
                    if (xVal < 0)
                    {
                        //rb.velocity = new Vector3(xVal * speed, -9.8f, 0) * Time.deltaTime;
                        //rb.AddForce(new Vector3(xVal * speed, 0, 0) * Time.deltaTime);
                        characterController.Move(new Vector3(xVal * speed, 0, 0) * Time.deltaTime);
                        this.PlaySound("walk");
                    }
                }
                else
                {
                    //rb.velocity = new Vector3(xVal * speed, -9.8f, 0) * Time.deltaTime;
                    //rb.AddForce(new Vector3(xVal * speed, 0, 0) * Time.deltaTime);
                    characterController.Move(new Vector3(xVal * speed, 0, 0) * Time.deltaTime);
                    this.PlaySound("walk");
                }
            }
            //else
            //{
            //    this.StopSound();
            //}

            this.transform.position = new Vector3(this.transform.position.x, 0, 0);
        }
    }

    public void Attack()
    {
        if (playerAnimator)
        {
            if (power <= 0.0f)
            {
                return;
            }

            int rand = UnityEngine.Random.Range(0, 7);

            playerAnimator.SetFloat("AttackRandomizerFloat", rand);
            playerAnimator.SetBool("isAttacking", true);

            isAttacking = true;

            // Decrement power with each attack
            DecrementPower(0.1f);

            UpdatePowerSlider();

            //if (rand == 2)
            //{
            //    MakeGroundSlash();
            //}

            this.PlaySound("attack");
        }
    }

    private void MakeGroundSlash()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        slashDestination = ray.GetPoint(1000);
        InstantiateSlash();
    }

    private void InstantiateSlash()
    {
        var slashObj = Instantiate(groundSlashPrefab, rightHandNode.transform.position, Quaternion.identity) as GameObject;

        groundSlash = slashObj.GetComponent<GroundSlash>();
        RotateToDestination(slashObj, slashDestination, true);
        slashObj.GetComponent<Rigidbody>().velocity = this.transform.forward * groundSlash.speed;
    }

    private void RotateToDestination(GameObject obj, Vector3 destination, bool onlyY)
    {
        var direction = destination - obj.transform.position;
        var rotation = Quaternion.LookRotation(direction);

        rotation = this.transform.rotation;

        if (onlyY)
        {
            rotation.x = 0;
            rotation.z = 0;
        }

        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
    }

    public void StopAttacking()
    {
        if (playerAnimator)
        {
            playerAnimator.SetBool("isAttacking", false);
            isAttacking = false;
        }
    }

    public void ToggleJump()
    {
        if (isGrounded)
        {
            if (rb)
            {
                rb.AddForce(Vector3.up * jumpForce * Time.deltaTime);
                if (playerAnimator)
                {
                    playerAnimator.SetBool("isJumping", true);
                }

                this.PlaySound("jump");
            }
        }
    }

    public void Dodge()
    {
        if (power <= 0.0f)
            return;

        if (playerAnimator)
        {
            int dodgeIndex = UnityEngine.Random.Range(0, 4);
            playerAnimator.SetBool("isDodging", true);
            playerAnimator.SetFloat("DodgeRandomizer", dodgeIndex);
            this.PlaySound("jump");

            if (dodgeIndex == 0)
            {
                var pos = this.transform.position - this.transform.forward;
                if (pos.x >= minXMovement && pos.x <= maxXMovement)
                {
                    this.transform.position = pos;
                }
            }
            else if (dodgeIndex == 1)
            {
                var pos = this.transform.position - this.transform.forward;
                if (pos.x >= minXMovement && pos.x <= maxXMovement)
                {
                    this.transform.position = pos;
                }
            }
            else if (dodgeIndex == 2)
            {
                var pos = this.transform.position + this.transform.forward;
                if (pos.x >= minXMovement && pos.x <= maxXMovement)
                {
                    this.transform.position = pos;
                }
            }
            else if (dodgeIndex == 3)
            {
                var pos = this.transform.position + this.transform.forward;
                if (pos.x >= minXMovement && pos.x <= maxXMovement)
                {
                    this.transform.position = pos;
                }
            }

            // Decrement power with each attack
            DecrementPower(0.2f);

            UpdatePowerSlider();
        }
    }

    public void StopDodging()
    {
        if (playerAnimator)
        {
            playerAnimator.SetBool("isDodging", false);
        }
    }

    private void PlayAnimations()
    {
        if (playerAnimator)
        {
            var xVal = playerJoystick.Horizontal;

            playerAnimator.SetFloat("Speed", Mathf.Abs(xVal));
        }
    }
    #endregion

    #region Sounds
    private void PlaySound(string key)
    {
        if (audioSource)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = this.sfxClips[key];
                audioSource.Play();
            }
        }
    }

    private void StopSound()
    {
        if (audioSource)
        {
            audioSource.Stop();
        }
    }

    private void LoadSounds()
    {
        this.sfxClips = new Dictionary<string, AudioClip>();
        var audioClips = Resources.LoadAll("SFX", typeof(AudioClip));
        foreach (var audioClip in audioClips)
        {
            var clip = (AudioClip)audioClip;
            //Debug.Log("Loaded clip: " + clip.name);
            this.sfxClips.Add(clip.name, clip);
        }
    }
    #endregion

    private void OnEnable()
    {
        LoadPlayerData();

        UpdatePowerSlider();

        UpdateCoinsTxt();

        InitForClimb();

        LoadSounds();
    }

    private void FixedUpdate()
    {
        Move();

        PlayAnimations();

        CheckMaxJump();

        CheckForClimb();

        ClimbTick();
    }

    private void CheckMaxJump()
    {
        if (this.transform.position.y > maxJump)
        {
            this.transform.position = new Vector3(this.transform.position.x, 0, this.transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Collectable")
        {
            CoinCollectable coinCollectable;
            HealthCollectable healthCollectable;
            PowerUpCollectable powerUpCollectable;

            if (other.gameObject.TryGetComponent(out coinCollectable))
            {
                IncrementMoney(coinCollectable.value);

                Destroy(coinCollectable.gameObject);
            }
            else if (other.gameObject.TryGetComponent(out healthCollectable))
            {
                IncrementHealth(healthCollectable.value);

                Destroy(healthCollectable.gameObject);
            }
            else if (other.gameObject.TryGetComponent(out powerUpCollectable))
            {
                //IncrementPower(powerUpCollectable.value);
                Debug.Log("Collected power up .. ");

                var invCtrl = FindObjectOfType<InventoryController>();
                if (invCtrl)
                {
                    invCtrl.AddItem(powerUpCollectable);
                }

                Destroy(powerUpCollectable.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;

            if (playerAnimator)
            {
                playerAnimator.SetBool("isJumping", false);
            }

            Debug.Log("Stopped jumping ... ");
        }    
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}
