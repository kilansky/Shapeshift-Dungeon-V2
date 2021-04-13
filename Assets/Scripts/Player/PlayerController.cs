using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public enum inputButtons { Dash, Attack, Charge, Special, Potion, Interact }

[System.Serializable]
public class ButtonInput
{
    public inputButtons inputButton;
    public float inputTime;
}

public class PlayerController : SingletonPattern<PlayerController>
{
    //================Publics================
    [Header("Movement Stats")]
    public PlayerStats baseMoveSpeed; //Variable for getting the move speed stat from ItemsEquipment
    public float rotateSpeed = 12f;
    public float dashRotateSpeed = 4f;

    [Header("Dash Stats")]
    public PlayerStats dashSpeed; //Dash Speed from ItemsEquipment for Dash Speed
    public PlayerStats dashTime; //Dash Time from ItemsEquipment for Dash Distance
    public PlayerStats dashCooldownTime; //Dash Cooldown Time used for DashRefreshSpeed for ItemsEquipment
    public PlayerStats dashInvincibilityTime; //Dash Invincibility Time

    [Header("Attack Stats")]
    public PlayerStats baseAttackDamage; //Attack Damage Variable used for AttackDam in ItemsEquipment
    public PlayerStats attackTime; //ItemsEquipment for Attack Speed
    public float attackSpeedMod = 0.25f;
    public float attack3DmgMod = 1.5f; //increases damage of third attack
    //public float targetMonsterDist = 4f;

    [Header("Dash Attack Stats")]
    public float dashAttackInputWindow = 0.6f;
    private float dashAttackWindow;

    [Header("Charge Attack Stats")]
    public GameObject chargeArrow; //GameObject to hold the arrow underneath the player during charge attacks
    public PlayerStats chargeRate; //ItemsEquipment for Charge Attack Time
    public float minChargeSpeed = 20f;
    public float maxChargeSpeed = 35f;
    public float chargeDeceleration = 55f;
    public float chargeCooldownTime = 0.2f;
    public float minChargeDmgModifier;
    public PlayerStats chargeDmgModifier; //ItemsEquipment for Dash Damage Modifier
    public float timeToFullCharge = 3f;

    [Header("Special Stats")]
    public float useSpecialTime = 0.5f;
    public PlayerStats specialCooldownTime; //ItemsEquipment for Special Item Recharge Time

    [Header("Potion Stats")]
    public float usePotionTime = 0.5f;
    public float potionCooldownTime = 0f;

    [Header("Input Timing")]
    public float earlyInputTimeAllowance = 0.25f;

    [Header("Mouse Aiming")]
    public Transform mouseTargetPoint;
    public LayerMask mouseAimMask;

    [Header("Items")]
    public ItemsEquipment SpecialSlot; //Special Item slot
    public ItemsEquipment HeadSlot; //Head Item slot
    public ItemsEquipment TorsoSlot; //Torso Item slot
    public ItemsEquipment FootSlot; //Foot Item slot
    public ItemsEquipment PocketSlot1; //Pocket 1 Item slot
    public ItemsEquipment PocketSlot2; //Pocket 2 Item slot
    public ItemsEquipment BagOfHoldingSlot; //Bag Of Holding Item Slot to swap between special items/ store additional special items
    private ItemsEquipment TemporarySlot; //This slot is used for item swapping and temporary holding of items or equipement
    public bool touchingItem = false; //Variable to track if the player is currently touching an item or not
    public bool pickupItem = false; //Variable to pick up the item
    public bool canAffordItem = false; //Variable to see if player can afford an item -Justin
    [HideInInspector] public bool hasRedHerb = false; //Variable to make sure that the player has the red herb (makes for less checking of both pocker slots) so they are able to regain health when they start a new level
    [HideInInspector] public bool hasBagOfHolding = false; //Variable to make sure that the player has the bag of holding item (makes for less checking of both pocker slots) so they are able to store/swap special items
    [HideInInspector] public bool isItemSwapping = false; //Variable to be used to check if the itms are currently being swapped or not

    //Settable properties
    //Keep track of the amount of times that a stat was upgraded
    public int StatMaxHealthCount {get; set;}
    public int StatAttackCount {get; set;}
    public int StatSpeedCount {get; set;}

    //Track the current charge of a special item
    public float SpecialCharge { get; set; }

    //The amount of speed reduction while in sand
    //Default is 1 until entering sand tile
    public float SandSpeedMod { get; set; }

    //================Privates================
    private float gravity = 9.81f / 3;
    private float vSpeed = 0;
    private Animator animator;
    private CharacterController controller;
    private Vector3 movementVector;
    private Quaternion lastTargetRotation;
    private Queue<ButtonInput> inputQueue = new Queue<ButtonInput>();
    private ButtonInput newInput = new ButtonInput();
    private Vector2 mouseAimPosition;
    private float currMoveSpeed; //the current move speed of the player
    private float moveVelocity; //based on controller movement input, used for walk/run blending
    private int attackComboState = 0; //0 = not attacking, 1 = attack1, 2 = attack2, 3 = attack3
    private float currAttackDamage;
    private int priceOfLastTouchedItem = 0; //I need this to store prices -Justin
    private float specialCharge2 = 0; //Varaible to hold the special charge of the item in the bag of holding
    private bool specialIsCharging = false;

    //Allow/prevent input actions
    private bool canMove = true;
    private bool canDash = true;
    private bool canAttack = true;
    //private bool canDashAttack = false;
    private bool canChargeAttack = true;
    private bool canUseSpecial = true;
    private bool canUsePotion = true;

    //Current action/state of the player
    private bool isDashing = false;
    private bool isAttacking = false;
    //private bool isDashAttacking = false;
    private bool isCharging = false;
    private bool isChargeAttacking = false;
    private bool isUsingPotion = false;
    private bool isUsingSpecial = false;
    private bool isPaused = false;
    private bool isUsingMouse = false;

    //================Properties================
    //read only - can be read from other functions
    public float MoveSpeed { get { return currMoveSpeed; } }        //Set to the current move speed of the player
    public float CurrAttackDamage { get { return currAttackDamage; } } //Set to the current attack speed of the player
    public bool IsDashing { get { return isDashing; } }             //True during entire dash
    public bool IsAttacking { get { return isAttacking; } }         //True during combo attacks until hitbox is deactiviated
    //public bool IsDashAttacking { get { return isDashAttacking; } } //True during dash attack
    public bool IsCharging { get { return isCharging; } }           //True while charge attack button is held
    public bool IsChargeAttacking { get { return isChargeAttacking; } } //True when charge attack button released
    public bool IsUsingPotion { get { return isUsingPotion; } }     //True while drinking a potion
    public bool IsUsingSpecial { get { return isUsingSpecial; } }   //True while using a Special Item
    public bool IsPaused { get { return isPaused; } }               //True while game is paused
    public bool IsUsingMouse { get { return isUsingMouse; } }      //True while player is using mouse and keyboard controls

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        currMoveSpeed = baseMoveSpeed.Value;
        lastTargetRotation = Quaternion.identity;
        dashAttackWindow = dashAttackInputWindow;
        chargeArrow.SetActive(false);

        StatMaxHealthCount = 0;
        StatAttackCount = 0;
        StatSpeedCount = 0;
        SandSpeedMod = 1;

        if (PlayerPrefs.GetInt("UserID") == 0)
        {
            int randUserID = UnityEngine.Random.Range(10000, 100000);
            PlayerPrefs.SetInt("UserID", randUserID);
        }
        //Debug.Log("User ID is: " + PlayerPrefs.GetInt("UserID"));
    }

    void Update()
    {
        //Move, Rotate, & Animate Player
        MovePlayer();
        RotatePlayer();
        AnimatePlayer();

        //Set mouse target pos if using M&K
        if (GetComponent<PlayerInput>().currentControlScheme == "Keyboard&Mouse")
        {
            isUsingMouse = true;
            SetMouseTargetPosition();
        }
        else
            isUsingMouse = false;

        /*
        //Set whether the player can dash attack after a dash
        if(canDashAttack)
        {
            dashAttackWindow -= Time.deltaTime;
            if(dashAttackWindow <= 0)
            {
                canDashAttack = false;
                dashAttackWindow = dashAttackInputWindow;
            }
        }
        */

        //Check if there are any new button inputs and attempt to activate them
        if (inputQueue.Count > 0)
            ActivateQueuedInputs();

        //If the input queue has two or more inputs, remove the first button press w/o performing it
        if (inputQueue.Count >= 2)
            inputQueue.Dequeue();
    }

    //Checks the input queue for player input and responds according to input type & timing
    private void ActivateQueuedInputs()
    {
        //Debug.Log("The next queued input is : " + inputQueue.Peek().inputButton);

        //if the player inputs an attack during a dash (Dash Attack!)
        /*
        if (canDashAttack && inputQueue.Peek().inputButton == inputButtons.Attack)
        {
            //Debug.Log("Start Dash Attack");
            //ActivateDashAttack();
            return;
        }
        */

        //Check if player is currently performing an action
        //If not, attempt to activate the next input in the queue
        if (!IsAttacking && !IsCharging && !IsChargeAttacking && !IsUsingPotion && !IsUsingSpecial || IsAttacking && !IsCharging)
        {
            float timePassedSinceInput = Time.time - inputQueue.Peek().inputTime;
            //Debug.Log(timePassedSinceInput + " seconds passed since button was input");

            //Drop inputs that were pressed too early
            if (timePassedSinceInput > earlyInputTimeAllowance)
            {
                //Debug.Log("Input was dropped since it was pressed too early");
                inputQueue.Dequeue();
                return;
            }

            //Attempt to perform input
            inputButtons nextInput = inputQueue.Peek().inputButton;
            PerformInput(nextInput);
        }
    }

    //Activate the nextInput from the inputQueue (if the input is not on cooldown)
    private void PerformInput(inputButtons nextInput)
    {
        switch(nextInput)
        {
            case inputButtons.Dash:
                if (canDash)
                    StartCoroutine(ActivateDash());
                break;
            case inputButtons.Attack:
                if (canAttack)
                    ActivateAttack();
                break;
            case inputButtons.Charge:
                if (canChargeAttack)
                    StartCoroutine(ActivateChargeAttack());
                break;
            case inputButtons.Special:
                if (canUseSpecial)
                    StartCoroutine(ActivateSpecial());
                break;
            case inputButtons.Interact:
                Interact();
                break;
            case inputButtons.Potion:
                if (canUsePotion)
                    StartCoroutine(ActivatePotion());
                break;
            default:
                break;
        }
    }

    private void MovePlayer()
    {
        //Set vertical speed to zero if grouned or dashing
        if (controller.isGrounded || isDashing)
            vSpeed = 0;
        else //otherwise, calculate gravity
            vSpeed -= gravity * Time.deltaTime;

        movementVector.y = vSpeed;

        if(canMove)
        {
            //Set velocity based on highest value directional input
            moveVelocity = Mathf.Abs(movementVector.x);
            if (moveVelocity < Mathf.Abs(movementVector.z))
                moveVelocity = Mathf.Abs(movementVector.z);
        }
        else
            moveVelocity = 0;

        Vector3 attackVector = new Vector3(transform.forward.x, vSpeed, transform.forward.z);

        if (IsDashing)
            controller.Move(transform.forward * currMoveSpeed * SandSpeedMod * Time.deltaTime);
        else if (IsAttacking || IsChargeAttacking)
            controller.Move(attackVector * moveVelocity * currMoveSpeed * SandSpeedMod * Time.deltaTime);
        else
            controller.Move(movementVector * currMoveSpeed * SandSpeedMod * Time.deltaTime);
    }

    private void SetMouseTargetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseAimPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
        {
            mouseTargetPoint.position = hit.point;
        }
    }

    private void RotatePlayer()
    {
        if (!canMove)
            return;

        //Smoothly Rotate Character in movement direction (if moving)
        if (Mathf.Abs(movementVector.x) > 0 || Mathf.Abs(movementVector.z) > 0)
        {
            Vector3 rotationVector = new Vector3(movementVector.x, 0, movementVector.z);
            Quaternion targetRotation = Quaternion.LookRotation(rotationVector);

            //Change rotation speed based on player state
            float rotSpeed = rotateSpeed;

            if (IsDashing)
                rotSpeed = dashRotateSpeed;

            //Smoothly Rotate Player
            lastTargetRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * SandSpeedMod * Time.deltaTime);
            transform.rotation = lastTargetRotation;
        }

        //Rotate player towards mouse position to attack if using Mouse & Keyboard
        if ((IsAttacking || IsCharging) && GetComponent<PlayerInput>().currentControlScheme == "Keyboard&Mouse")
        {
            //Rotate Character in direction of mouse position
            Vector3 targetPoint = new Vector3(mouseTargetPoint.position.x, 0, mouseTargetPoint.position.z) - new Vector3(transform.position.x, 0, transform.position.z);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint);
            transform.rotation = targetRotation;
        }
        //Rotate player towards joystick directions to attack if using a Controller
        else if ((IsAttacking || IsCharging) && GetComponent<PlayerInput>().currentControlScheme == "Keyboard&Mouse")
        {
            //Rotate Character in direction of joystick direction
            Vector3 rotationVector = new Vector3(movementVector.x, 0, movementVector.z);
            Quaternion targetRotation = Quaternion.LookRotation(rotationVector);
            transform.rotation = targetRotation;
        }
    }

    //Returns the transform of the closest monster to the player
    private Transform GetClosestMonster()
    {
        //Get all monsters in the scene, store in 'monsters' array
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");

        //Get the distance to the first monster in the array, set as closest monster
        float distToClosestMonster = Vector3.Distance(transform.position, monsters[0].transform.position);
        Transform closestMonster = monsters[0].transform;

        //Determine which monster is closest to the player
        foreach (GameObject monster in monsters)
        {
            float monsterDist = Vector3.Distance(transform.position, monster.transform.position);
            if (monsterDist <= distToClosestMonster)
            {
                closestMonster = monster.transform;
                distToClosestMonster = monsterDist;
            }
        }
        return closestMonster;
    }

    //Sets animation state & rotation based on movementVector
    private void AnimatePlayer()
    {
        //Set velocity based on highest value directional input
        moveVelocity = Mathf.Abs(movementVector.x);
        if (moveVelocity < Mathf.Abs(movementVector.z))
            moveVelocity = Mathf.Abs(movementVector.z);

        //Use blend tree for walk/run anims based on velocity
        animator.SetFloat("velocity", moveVelocity);
    }

    private bool IsGrounded()
    {
        float distToGround = GetComponent<CharacterController>().bounds.extents.y;
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    //---------------------------------------------------------------------------
    //-------------------------RECIEVE ACTION INPUTS-----------------------------
    //---------------------------------------------------------------------------

    //Set movementVector based on movement input
    public void Move(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        movementVector = new Vector3(moveInput.x, 0, moveInput.y); //Set input y value to z axis
    }

    //Dash Button Pressed
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            newInput.inputButton = inputButtons.Dash;
            newInput.inputTime = Time.time;
            inputQueue.Enqueue(newInput);
        }
    }

    //Attack Button Pressed
    public void Attack(InputAction.CallbackContext context)
    {
        if(context.canceled && !IsCharging)
        {
            newInput.inputButton = inputButtons.Attack;
            newInput.inputTime = Time.time;
            inputQueue.Enqueue(newInput);
        }
    }

    //Charge Input Pressed
    public void Charge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            newInput.inputButton = inputButtons.Charge;
            newInput.inputTime = Time.time;
            inputQueue.Enqueue(newInput);
        }

        //Release held charge
        if(context.canceled && isCharging)
        {
            isCharging = false;
            isChargeAttacking = true;
        }
    }

    //Speical Button Pressed
    public void Special(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            newInput.inputButton = inputButtons.Special;
            newInput.inputTime = Time.time;
            inputQueue.Enqueue(newInput);
        }
    }

    //Interact Button Pressed
    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed) //Checks if the interact button was pressed and then puts that on the queue for putting on the item
        {
          newInput.inputButton = inputButtons.Interact;
          newInput.inputTime = Time.time;
          inputQueue.Enqueue(newInput);
        }
    }

    //Potion Button Pressed
    public void Potion(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            newInput.inputButton = inputButtons.Potion;
            newInput.inputTime = Time.time;
            inputQueue.Enqueue(newInput);
        }
    }

    //Bag Of Holding Item Swap Button Pressed - AHL (4/8/21)
    public void BagOfHoldingItemSwap(InputAction.CallbackContext context)
    {
        if (context.performed && hasBagOfHolding && BagOfHoldingSlot) //This action is only performed when the Bag of Holding Item is equipped and there is an item in the slot
        {
            //Adjusts bool to make sure things work as intended during this process
            isItemSwapping = true;

            //Puts the Special Item into the Temporary slot to begin the item transfer
            TemporarySlot = SpecialSlot;

            //Assigns the secondary special charge to here
            float tempSpecial = SpecialCharge;

            //Equips the Bag Of Holding Item
            BagOfHoldingSlot.prefab.GetComponent<Item>().Equip(this, PlayerHealth.Instance);

            //Swaps the rest of the items and makes temporary null again (Just in case o.o)
            BagOfHoldingSlot = TemporarySlot;
            TemporarySlot = null;

            //Swaps the charge values
            if(specialCharge2 != 0)
                SpecialCharge = specialCharge2;
            
            specialCharge2 = tempSpecial;

            //Adjusts the bool to make sure things work as inteded after this process
            isItemSwapping = false;

            //Starts the Corutine if the special charge is not equal to the value
            StartCoroutine(RechargeSpecial());

            //Sets the new current item in the active special slot
            HUDController.Instance.SetNewSpecialItemIcons();

            HUDController.Instance.UpdateSpecialCharge();
        }
    }

    /// Pause function to pause the game based on the isPause variable and will stop the game time while displaying the pause screen
    public void Pause(InputAction.CallbackContext context)
    {
        if(context.performed && PlayerHealth.Instance.Health > 0 && !HUDController.Instance.levelReviewPanel.activeSelf) //prevent pausing when dead
        {
            if (!IsPaused) //If the game is not paused then pause the game
            {
                isPaused = true;
                Time.timeScale = 0;
                HUDController.Instance.ShowPauseScreen();
                HUDController.Instance.ShowControlsPanel();
            }
            else //If the game is paused then unpause the game
            {
                isPaused = false;
                Time.timeScale = 1;
                HUDController.Instance.HidePauseScreen();
                HUDController.Instance.HideControlsPanel();
            }
        }
    }

    //Mouse Aiming Screen Position
    public void SetMousePosition(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mouseAimPosition = context.ReadValue<Vector2>();
        }
    }

    //---------------------------------------------------------------------------
    //----------------------------RECIEVE UI INPUTS------------------------------
    //---------------------------------------------------------------------------

    //Set movementVector based on movement input
    public void Navigate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 navigationInput = context.ReadValue<Vector2>();

            Buttons buttons = FindObjectOfType<Buttons>();

            if (navigationInput.y > 0.5f || navigationInput.x < -0.5f)
                buttons.PreviousButton();

            if (navigationInput.y < -0.5f || navigationInput.x > 0.5f)
                buttons.NextButton();
        }
    }

    //Dash Button Pressed
    public void Submit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            FindObjectOfType<Buttons>().SubmitButton();
        }
    }

    //Attack Button Pressed
    public void Cancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log("CANCEL PRESSED");
        }
    }

    //---------------------------------------------------------------------------
    //-----------------------------INPUT ACTIVATION------------------------------
    //---------------------------------------------------------------------------

    //Starts and Ends a Dash
    IEnumerator ActivateDash()
    {
        inputQueue.Dequeue();
        ResetAttackCombo();

        if(moveVelocity > 0.5f) //Must be running to start a dash
        {
            canDash = false;
            isDashing = true;
            //canDashAttack = true;
            animator.SetBool("isDashing", true);
            currMoveSpeed = dashSpeed.Value; //set dash speed
            PlayerHealth.Instance.isInvincible = true;

            yield return new WaitForSeconds(dashInvincibilityTime.Value); //wait to disable invincibility
            PlayerHealth.Instance.isInvincible = false;

            yield return new WaitForSeconds(dashTime.Value - dashInvincibilityTime.Value); //wait for end of dash & restore base speed
            currMoveSpeed = baseMoveSpeed.Value;
            isDashing = false;
            animator.SetBool("isDashing", false);

            yield return new WaitForSeconds(dashCooldownTime.Value); //wait to refresh dash
            canDash = true;
        }
    }

    /*
    private void ActivateDashAttack()
    {
        inputQueue.Dequeue();

        isDashAttacking = true;
        canDashAttack = false;
        animator.SetBool("isDashAttacking", true);
    }

    public void EndDashAttack()
    {
        isDashAttacking = false;
        animator.SetBool("isDashAttacking", false);
    }
    */

    //Starts an Attack - called on button input
    private void ActivateAttack()
    {
        inputQueue.Dequeue();
        attackComboState++;
        //Debug.Log("attackComboState is: " + attackComboState);

        if (attackComboState > 3)
            attackComboState = 1;

        isAttacking = true;

        switch (attackComboState)
        {
            case 1:
                animator.SetBool("attack1", true);
                break;
            case 2:
                animator.SetBool("attack2", true);
                break;
            case 3:
                animator.SetBool("attack3", true);
                canAttack = false;
                break;
            default:
                break;
        }

        currMoveSpeed = baseMoveSpeed.Value * attackSpeedMod; //slows movment while attacking
    }

    //Ends an Attack - called from attack animation event
    public void EndAttack(float attackNum)
    {
        switch (attackNum)
        {
            case 1:
                animator.SetBool("attack1", false);
                break;
            case 2:
                animator.SetBool("attack2", false);
                break;
            case 3:
                animator.SetBool("attack3", false);
                ResetAttackCombo();
                break;
            default:
                Debug.LogError("Invalid attack number called to end");
                break;
        }
    }

    //Resets the attack combo
    public void ResetAttackCombo()
    {
        attackComboState = 0;
        isAttacking = false;
        //isDashAttacking = false;
        canAttack = true;
        canMove = true;
        animator.SetBool("attack1", false);
        animator.SetBool("attack2", false);
        animator.SetBool("attack3", false);
        //animator.SetBool("isDashAttacking", false);
        PlayerAttackController.Instance.DeactivateAllHitboxes();
        currAttackDamage = baseAttackDamage.Value;

        if(!IsDashing)
            currMoveSpeed = baseMoveSpeed.Value;
    }

    //Prevents/Re-enables the player from moving during Attack3 animation
    public void DisableMovement() { canMove = false; }
    public void EnableMovement() { canMove = true; }

    //Starts and Ends a Charge Attack
    IEnumerator ActivateChargeAttack()
    {
        inputQueue.Dequeue();
        ResetAttackCombo();
        isCharging = true;
        canChargeAttack = false;
        animator.SetBool("isCharging", true);
        currMoveSpeed = 0; //prevent movement while charging

        //Charge input is held down
        chargeArrow.SetActive(true);
        float arrowLength = chargeArrow.transform.localScale.y;
        float arrowWidth = chargeArrow.transform.localScale.x;
        float chargeSpeed = minChargeSpeed;
        currAttackDamage = baseAttackDamage.Value * minChargeDmgModifier;

        float timeElapsed = 0;
        while (isCharging) //Increase charge speed & arrow UI until button is released
        {
            currMoveSpeed = 0; //prevent movement while charging

            chargeSpeed = Mathf.Lerp(minChargeSpeed, maxChargeSpeed, timeElapsed / timeToFullCharge);
            currAttackDamage = Mathf.Lerp(baseAttackDamage.Value * minChargeDmgModifier, baseAttackDamage.Value * chargeDmgModifier.Value, timeElapsed / timeToFullCharge);

            arrowLength = Mathf.Lerp(0.5f, 3.5f, timeElapsed / timeToFullCharge);
            arrowWidth = Mathf.Lerp(0.8f, 1.2f, timeElapsed / timeToFullCharge);
            chargeArrow.transform.localScale = new Vector3(arrowWidth, arrowLength, arrowWidth);

            timeElapsed += Time.deltaTime;
            if (timeElapsed > timeToFullCharge)
            {
                timeElapsed = timeToFullCharge;
                chargeSpeed = maxChargeSpeed;
                currAttackDamage = baseAttackDamage.Value * chargeDmgModifier.Value;
            }

            yield return new WaitForEndOfFrame();
        }

        //Charge input is released
        chargeArrow.SetActive(false);
        chargeArrow.transform.localScale = new Vector3(1, 1, 1);

        Vector3 chargeVector = transform.forward;

        animator.SetBool("isCharging", false);

        //Charge forward & apply deceleration until speed is nearly zero
        while (chargeSpeed > 3f)
        {
            controller.Move(chargeVector * chargeSpeed * Time.deltaTime);
            chargeSpeed -= chargeDeceleration * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        vSpeed = 0;

        //Charge attack ended
        isChargeAttacking = false;
        yield return new WaitForSeconds(chargeCooldownTime); //wait for charge anim to play out before going back to normal
        canChargeAttack = true;
        currMoveSpeed = baseMoveSpeed.Value;
        currAttackDamage = baseAttackDamage.Value; //reset attack damage
    }

    //Starts and Ends using Special item
    IEnumerator ActivateSpecial()
    {
        inputQueue.Dequeue();
        ResetAttackCombo();
        canUseSpecial = false;
        isUsingSpecial = true;

        //If there is an item in the special slot then we trigger the specific program assigned to the item
        if (SpecialSlot != null)
        {
            //IF statement chain to see what special item is currently being used

            //Bowling Ball Item
            if (SpecialSlot.ItemName == "Bowling Ball")
            {
                Vector3 spawnDirection = transform.forward;
                Quaternion spawnRotation = lastTargetRotation;

                if (IsUsingMouse)//Spawn in direction of mouse pointer if using a mouse
                {
                    spawnDirection = new Vector3(mouseTargetPoint.position.x, 0, mouseTargetPoint.position.z) - new Vector3(transform.position.x, 0, transform.position.z);
                    spawnDirection = spawnDirection.normalized /2;

                    spawnRotation = Quaternion.LookRotation(spawnDirection);
                }

                SpecialSlot.prefab.GetComponent<BowlingBall>().spawnBowlingBall(transform.position + new Vector3(0, 0.35f, 0), spawnDirection, spawnRotation);
            }

            //Bomb Item
            else if (SpecialSlot.ItemName == "Bomb Bag")
                SpecialSlot.prefab.GetComponent<BombBag>().spawnBomb(transform.position, transform.rotation);

            //Fire Wand Item
            else if (SpecialSlot.ItemName == "Fire Wand")
            {
                Vector3 spawnDirection = transform.forward;
                Quaternion spawnRotation = lastTargetRotation;

                if (IsUsingMouse)//Spawn in direction of mouse pointer if using a mouse
                {
                    spawnDirection = new Vector3(mouseTargetPoint.position.x, 0, mouseTargetPoint.position.z) - new Vector3(transform.position.x, 0, transform.position.z);
                    spawnDirection = spawnDirection.normalized / 2;

                    spawnRotation = Quaternion.LookRotation(spawnDirection);
                }

                SpecialSlot.prefab.GetComponent<FireWand>().spawnFireBall(transform.position, spawnDirection, spawnRotation);
            }

            //Lazer Wand Item
            else if (SpecialSlot.ItemName == "Lazer Wand")
            {
                Vector3 spawnDirection = transform.forward;
                Quaternion spawnRotation = lastTargetRotation;

                if (IsUsingMouse)//Spawn in direction of mouse pointer if using a mouse
                {
                    spawnDirection = new Vector3(mouseTargetPoint.position.x, 0, mouseTargetPoint.position.z) - new Vector3(transform.position.x, 0, transform.position.z);
                    spawnDirection = spawnDirection.normalized / 2;

                    spawnRotation = Quaternion.LookRotation(spawnDirection);
                }

                SpecialSlot.prefab.GetComponent<LazerWand>().spawnLazer(transform.position, spawnDirection, spawnRotation);
            }
        }

        SpecialCharge = 0;

        yield return new WaitForSeconds(useSpecialTime);
        isUsingSpecial = false;

        StartCoroutine(RechargeSpecial());
    }

    IEnumerator RechargeSpecial()
    {
        if(!specialIsCharging)
        {
            while (SpecialCharge < specialCooldownTime.Value && !isItemSwapping)
            {
                specialIsCharging = true;

                SpecialCharge += Time.deltaTime;
                HUDController.Instance.UpdateSpecialCharge();

                if (isItemSwapping)
                    break;

                yield return new WaitForEndOfFrame();
            }
            specialIsCharging = false;

            if (!isItemSwapping && SpecialCharge >= specialCooldownTime.Value)
            {
                canUseSpecial = true;
                HUDController.Instance.UpdateSpecialCharge();
            }
        }           
    }

    //Starts and Ends using a Potion
    IEnumerator ActivatePotion()
    {
        inputQueue.Dequeue();
        ResetAttackCombo();

        //Only use a potion if player is less than max health
        if (PlayerHealth.Instance.Health < PlayerHealth.Instance.maxHealth)
        {
            canUsePotion = false;
            isUsingPotion = true;

            yield return new WaitForSeconds(usePotionTime); //wait for player to drink potion

            PlayerHealth.Instance.UsePotion(); //heal
            isUsingPotion = false;

            yield return new WaitForSeconds(potionCooldownTime); //wait to refresh potion use
            canUsePotion = true;
        }
    }

    public void Interact()
    {
        inputQueue.Dequeue();
        ResetAttackCombo();

        //If on center tile
        if (CenterTile.Instance.onTile)
        {
            if (LevelManager.Instance.currFloor == 0)//floor 0 stuff
            {
                RunTimer.Instance.IncreaseTimer = true;
                HUDController.Instance.ShowRunTimer();

                AnalyticsEvents.Instance.PlayerControls(); //Sends an analytics event describing the players current controls

                LevelManager.Instance.TransitionLevel();
                CenterTile.Instance.onTile = false;

                HUDController.Instance.controlsPanel.SetActive(false);
                HUDController.Instance.HideQuickHint();
            }
            else if (LevelManager.Instance.currFloor % 5 == 0)//shop stuff
            {
                LevelManager.Instance.TransitionLevel();
                CenterTile.Instance.onTile = false;

                HUDController.Instance.controlsPanel.SetActive(false);
                HUDController.Instance.HideQuickHint();
            }
            else if (LevelManager.Instance.currFloor == 19)//End game stuff
            {
                RunTimer.Instance.IncreaseTimer = false;
                Time.timeScale = 0;
                HUDController.Instance.ShowWinScreen();
                HUDController.Instance.ShowLevelReviewPanel();
            }
            else
            {
                //LevelManager.Instance.TransitionLevel();
                if(HUDController.Instance.ShowLevelReview)
                    HUDController.Instance.ShowLevelReviewPanel(); //Show the level review panel if enabled
                else
                    LevelManager.Instance.TransitionLevel(); //if disabled, instantly transition the level

                CenterTile.Instance.onTile = false;

                HUDController.Instance.controlsPanel.SetActive(false);
                HUDController.Instance.HideQuickHint();
            }
        }

        //If there is currently an item being touched then set pickup Item to true
        if (touchingItem == true && canAffordItem)
        {
            pickupItem = true;
            touchingItem = false;
            canAffordItem = false;
            PlayerGems.Instance.SubtractGems(priceOfLastTouchedItem);
            HUDController.Instance.HideQuickHint();
        }
    }

    //---------------------------------------------------------------------------
    //-----------------------------ITEM INTERACTIONS-----------------------------
    //---------------------------------------------------------------------------

    /// <summary>
    /// AHL - 2/22/21
    /// While the player is inside the Trigger of an object it will check if they are still touching the object so it can be picked up with the associated action button
    /// </summary>

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gem")
        {
            if(PlayerGems.Instance.GemCount == 0)
                HUDController.Instance.ShowGemCounter();

            PlayerGems.Instance.AddGems(1);
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Item") //If the other gameobject is an item then check if the pickup button was pressed
        {
            touchingItem = true;
            if (PlayerGems.Instance.GemCount >= other.GetComponentInParent<Item>().price)
            {
                canAffordItem = true;
                priceOfLastTouchedItem = other.GetComponentInParent<Item>().price;
                HUDController.Instance.ShowQuickHint("Pick Up");
            }
            else
                canAffordItem = false;

            if (pickupItem == true) //If the item can be picked up
            {
                other.GetComponentInParent<Item>().Equip(this, GetComponent<PlayerHealth>()); //Equip the item to the player

                if(LevelManager.Instance.currFloor %5 != 0)
                    AnalyticsEvents.Instance.ItemTaken(other.GetComponentInParent<Item>().item.ItemName); //Send Item Taken analytics event
                else if(LevelManager.Instance.currFloor != 0)
                    AnalyticsEvents.Instance.ItemPurchased(other.GetComponentInParent<Item>().item.ItemName); //Send Item Purchased analytics event

                if (other.GetComponentInParent<Item>().IsSecondItem())
                    PedestalManager.Instance.secondItemPrice += 2;

                Destroy(other.gameObject); //Destroy the instance of the item in the gamescene
                pickupItem = false; //Set pickup to false

                PedestalManager.Instance.DeactivatePedestals();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Item")
        {
            touchingItem = false;
            HUDController.Instance.HideQuickHint();
        }
    }
}
