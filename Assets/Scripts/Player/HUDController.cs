using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class UIPanel
{
    public GameObject panel;
    public Image image;
    public TextMeshProUGUI textBox;
    public Sprite keyboardButton;
    public Sprite psButton;
    public Sprite xboxButton;
}

public class HUDController : SingletonPattern<HUDController>
{
    [Header("Health UI")]
    public GameObject healthBar;
    public Image healthBarFill;
    public Sprite[] healthBarBackgrounds = new Sprite[5];
    public float healthLerpSpeed;
    public TextMeshProUGUI maxHealthText;
    public TextMeshProUGUI currentHealthText;
    public UIPanel potionsPanel;

    [Header("Gem Counter")]
    public GameObject gemCounter;
    public TextMeshProUGUI gemCountText;

    [Header("Quick Hint Panel")]  
    public UIPanel quickHintPanel;

    [Header("Controls Panel")]
    public GameObject controlsPanel;
    public UIPanel movePanel;
    public UIPanel dashPanel;
    public UIPanel attackPanel;
    public UIPanel chargePanel;
    public UIPanel zoomPanel;
    public GameObject controllerRecText;
    public Color grayedTextColor;

    [Header("Special Item Panel")]
    public UIPanel specialItemPanel;
    public GameObject specialItem2Panel;
    public Image speicalItemIcon;
    public Image specialItemIcon2;
    public GameObject speicalItemEmptyIcon;
    public Image chargeBarFill;
    public Image chargeBar2Fill;
    public UIPanel swapItemPanel;

    [Header("Equipment Panel")]
    public GameObject equipmentPanel;
    public Image[] equipmentSlots = new Image[5];

    [Header("Stat Potion Panel")]
    public GameObject statPotionPanel;

    [Header("Review Panel")]
    public GameObject levelReviewPanel;

    [Header("PauseScreen")]
    public GameObject pauseScreen;

    [Header("Game Over")]
    public GameObject gameOverScreen;

    [Header("Win Screen")]
    public GameObject winScreen;

    [Header("Player Damaged Overlay")]
    public GameObject playerDamagedOverlay;

    [Header("Player Damaged Overlay")]
    public GameObject runTimer;

    private PlayerController player;
    private PlayerInput playerInput;
    private string currentControlScheme;
    private bool itemCollected = false;
    private bool pocketSlot1Used = false;
    private bool pocketSlot2Used = false;

    public bool ShowLevelReview { get; set; }
    public string CurrentControlScheme { get { return currentControlScheme; } }

    void Start()
    {
        ShowLevelReview = true;
        player = PlayerController.Instance;
        playerInput = player.gameObject.GetComponent<PlayerInput>();

        HidePotionsPanel();
        HideGemCounter();
        HideEquipmentPanel();
        HideSpecialItemPanel();
        HideQuickHint();

        ControlSchemeChanged();
    }

    //Check for when the player changes controllers
    void Update()
    {
        if (playerInput.currentControlScheme != currentControlScheme)
        {
            ControlSchemeChanged();

            if (controlsPanel.activeSelf)
            {
                movePanel.textBox.color = Color.white;
                dashPanel.textBox.color = Color.white;
                attackPanel.textBox.color = Color.white;
                chargePanel.textBox.color = Color.white;
                zoomPanel.textBox.color = Color.white;

                if (playerInput.currentControlScheme == "Keyboard&Mouse")
                    controllerRecText.SetActive(true);
                else
                    controllerRecText.SetActive(false);
            }
        }


        //While the control panel is active, check for player inputs
        if(controlsPanel.activeSelf)
        {
            if(player.MoveSpeed > 0)
                movePanel.textBox.color = grayedTextColor;
            if (player.IsDashing)
                dashPanel.textBox.color = grayedTextColor;
            if (player.IsAttacking)
                attackPanel.textBox.color = grayedTextColor;
            if (player.IsCharging)
                chargePanel.textBox.color = grayedTextColor;
            if (player.IsZooming)
                zoomPanel.textBox.color = grayedTextColor;
        }
    }

    //Change the UI based on the new control type
    public void ControlSchemeChanged()
    {
        currentControlScheme = playerInput.currentControlScheme;

        //Using Keyboard
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            //Set Cursor to be visible
            Cursor.visible = true;

            //Clear selected buttons
            GetComponent<EventSystem>().SetSelectedGameObject(null);

            //Update up button icons
            potionsPanel.image.sprite = potionsPanel.keyboardButton;
            quickHintPanel.image.sprite = quickHintPanel.keyboardButton;

            movePanel.image.sprite = movePanel.keyboardButton;
            dashPanel.image.sprite = dashPanel.keyboardButton;
            attackPanel.image.sprite = attackPanel.keyboardButton;
            chargePanel.image.sprite = chargePanel.keyboardButton;
            zoomPanel.image.sprite = zoomPanel.keyboardButton;
            specialItemPanel.image.sprite = specialItemPanel.keyboardButton;
            swapItemPanel.image.sprite = swapItemPanel.keyboardButton;
        }
        //Using Controller
        else
        {
            //Set Cursor to not be visible
            Cursor.visible = false;

            //Using PS4 Controller
            if (Gamepad.current.device.ToString().Contains("DualShock"))
            {
                //Update up button icons
                potionsPanel.image.sprite = potionsPanel.psButton;
                quickHintPanel.image.sprite = quickHintPanel.psButton;

                movePanel.image.sprite = movePanel.psButton;
                dashPanel.image.sprite = dashPanel.psButton;
                attackPanel.image.sprite = attackPanel.psButton;
                chargePanel.image.sprite = chargePanel.psButton;
                zoomPanel.image.sprite = zoomPanel.psButton;
                specialItemPanel.image.sprite = specialItemPanel.psButton;
                swapItemPanel.image.sprite = swapItemPanel.psButton;
            }
            //Using Xbox Controller
            else
            {
                //Update up button icons
                potionsPanel.image.sprite = potionsPanel.xboxButton;
                quickHintPanel.image.sprite = quickHintPanel.xboxButton;

                movePanel.image.sprite = movePanel.xboxButton;
                dashPanel.image.sprite = dashPanel.xboxButton;
                attackPanel.image.sprite = attackPanel.xboxButton;
                chargePanel.image.sprite = chargePanel.xboxButton;
                zoomPanel.image.sprite = zoomPanel.xboxButton;
                specialItemPanel.image.sprite = specialItemPanel.xboxButton;
                swapItemPanel.image.sprite = swapItemPanel.xboxButton;
            }
        }
    }

    public IEnumerator UpdateHealthBar(float currHealth, float maxHealth)
    {
        //Set text and damage state
        maxHealthText.text = maxHealth.ToString("0");
        currentHealthText.text = currHealth.ToString("0");
        SetHealthBarBackground(currHealth, maxHealth);

        //lerp health to new value
        if (currHealth == 0)
            healthBarFill.fillAmount = 0;
        else
        { 
            float fillFromValue = healthBarFill.fillAmount;
            float fillToValue = currHealth / maxHealth;
            float t = 0;
            while (t <= 1)
            {
                healthBarFill.fillAmount = Mathf.Lerp(fillFromValue, fillToValue, t);
                t += Time.deltaTime * healthLerpSpeed;
                yield return new WaitForEndOfFrame();
            }
            healthBarFill.fillAmount = fillToValue;
        }
    }

    //Sets the damaged condition of the health bar based on the current health value
    public void SetHealthBarBackground(float currHealth, float maxHealth)
    {
        int i = healthBarBackgrounds.Length - 1;
        foreach (Sprite healthBarSprite in healthBarBackgrounds)
        {
            //With 5 health bar backgrounds & 30 max health at start:
            //index i starts at 4, loop runs 5 times
            //1: >= (30/4)*4=30     - undamaged
            //2: >= (30/4)*3=22.5   - damageBar1
            //3: >= (30/4)*2=15     - damageBar2
            //4: >= (30/4)*1=7.5    - damageBar3
            //5: >= (30/4)*0=0      - damageBar4
            if (currHealth >= (maxHealth / (healthBarBackgrounds.Length-1)) * i)
            {
                healthBar.GetComponent<Image>().sprite = healthBarSprite;
                return;
            }

            i--;
        }
    }

    public void ShowGemCounter()
    {
        gemCounter.SetActive(true);
    }

    public void HideGemCounter()
    {
        gemCounter.SetActive(false);
    }

    public void UpdateGemCount(int gemCount)
    {
        gemCountText.text = gemCount.ToString();
    }

    public void UpdateSpecialCharge()
    {
        float maxValue = PlayerController.Instance.specialCooldownTime.Value;
        float value = PlayerController.Instance.SpecialCharge;

        if(PlayerController.Instance.isItemSwapping == false)
        {
            chargeBarFill.fillAmount = value / maxValue;

            //If statement to make sure that the second item only gets filled in if there is an item and we can get a value
            if (PlayerController.Instance.BagOfHoldingSlot != null)
            {
                //If there is a value in the second max value charge variable then we set it like the normal charge bar
                if (PlayerController.Instance.specialCharge2MaxValue > 0)
                    chargeBar2Fill.fillAmount = PlayerController.Instance.specialCharge2 / PlayerController.Instance.specialCharge2MaxValue;

                //else if the value is less than 0 (meaning it wasn't set yet) then we set it to be divided by itself (This is to make sure the Kapala doesn't start with a value of 1
                else
                {
                    if (PlayerController.Instance.BagOfHoldingSlot.ItemName != "Kapala")
                        chargeBar2Fill.fillAmount = 1;

                    else
                        chargeBar2Fill.fillAmount = 0;
                }
                    
            }
        }
    }

    public void ShowHealthBar()
    {
        healthBar.SetActive(true);
    }

    public void HideHealthBar()
    {
        healthBar.SetActive(false);
    }

    public void ShowPotionsPanel()
    {
        potionsPanel.panel.SetActive(true);
    }

    public void HidePotionsPanel()
    {
        potionsPanel.panel.SetActive(false);
    }

    public void ShowQuickHint(string quickHintText)
    {
        quickHintPanel.panel.SetActive(true);
        quickHintPanel.textBox.text = quickHintText;
    }

    public void HideQuickHint()
    {
        quickHintPanel.panel.SetActive(false);
    }

    public void ShowControlsPanel()
    {
        controlsPanel.SetActive(true);
    }

    public void HideControlsPanel()
    {
        controlsPanel.SetActive(false);
    }

    public void ShowEquipmentPanel()
    {
        equipmentPanel.SetActive(true);
    }

    public void HideEquipmentPanel()
    {
        equipmentPanel.SetActive(false);
    }

    //Sets the item icon of equipped items in the equipment panel
    public void SetEquipmentPanelItem(int itemType, Sprite itemIcon)
    {
        if(!itemCollected && (itemType != 0 & itemType <= 4))
        {
            ShowEquipmentPanel();
            itemCollected = true;
        }

        switch(itemType)
        {
            case 1:
                equipmentSlots[0].sprite = itemIcon;
                break;
            case 2:
                equipmentSlots[1].sprite = itemIcon;
                break;
            case 3:
                equipmentSlots[2].sprite = itemIcon;
                break;
            case 4:
                {
                    if(!pocketSlot1Used)//first pocket item taken
                    {
                        equipmentSlots[3].sprite = itemIcon;
                        pocketSlot1Used = true;
                    }
                    else if(!pocketSlot2Used)//second pocket item taken
                    {
                        equipmentSlots[4].sprite = itemIcon;
                        pocketSlot2Used = true;
                    }
                    else//third+ pocket item taken
                    {
                        equipmentSlots[3].sprite = equipmentSlots[4].sprite;
                        equipmentSlots[4].sprite = itemIcon;
                    }
                    break;
                }
            default:
                break;
        }
    }

    public void ShowStatPotionPanel()
    {
        player.gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        Time.timeScale = 0;
        statPotionPanel.SetActive(true);

        //Set initial selected button if using controller
        if (playerInput.currentControlScheme != "Keyboard&Mouse")
            statPotionPanel.GetComponent<Buttons>().SetSelectedButton();
        else
            statPotionPanel.GetComponent<Buttons>().ClearSelectedButtons();
    }

    public void HideStatPotionPanel()
    {
        player.gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        Time.timeScale = 1;
        statPotionPanel.SetActive(false);
    }

    public void ShowSpecialItemPanel()
    {
        specialItemPanel.panel.SetActive(true);
        speicalItemIcon.sprite = PlayerController.Instance.SpecialSlot.sprite;

        //If the picked up item wasn't due to item swapping and it isn't the Kapala then we set its special charge to full
        if (!PlayerController.Instance.isItemSwapping && PlayerController.Instance.SpecialSlot.ItemName != "Kapala")
            PlayerController.Instance.SpecialCharge = PlayerController.Instance.specialCooldownTime.Value;

        //Where if the item picked up wasn't due to item swapping and it is the Kapala then we set it initially to 0
        else if (!PlayerController.Instance.isItemSwapping && PlayerController.Instance.SpecialSlot.ItemName == "Kapala")
            PlayerController.Instance.SpecialCharge = 0;


        if(PlayerController.Instance.hasBagOfHolding && PlayerController.Instance.BagOfHoldingSlot)
        {
            if(speicalItemEmptyIcon)
            {
                //Deactivate the SpecialItemEmptyIcon
                speicalItemEmptyIcon.SetActive(false);

                //Activate the second charge wheel and adjust the values
                specialItem2Panel.SetActive(true);
                SetNewSpecialItemIcons();
            }

            //If the picked up item wasn't due to item swapping and it isn't the Kapala then we set its special charge to full (Which is just a very high number as it is hard to get the actual value
            if (!PlayerController.Instance.isItemSwapping && PlayerController.Instance.BagOfHoldingSlot.ItemName != "Kapala")
                PlayerController.Instance.specialCharge2 = 15;
        }
            

        UpdateSpecialCharge();
    }

    public void HideSpecialItemPanel()
    {
        specialItemPanel.panel.SetActive(false);
    }

    public void ShowBagOfHoldingSlot()
    {
        speicalItemEmptyIcon.SetActive(true);
    }

    public void HideBagOfHoldingSlot()
    {
        speicalItemEmptyIcon.SetActive(false);
    }

    public void SetNewSpecialItemIcon()
    {
        speicalItemIcon.sprite = PlayerController.Instance.SpecialSlot.sprite;
    }

    public void SetNewSpecialItemIcons()
    {
        speicalItemIcon.sprite = PlayerController.Instance.SpecialSlot.sprite;
        specialItemIcon2.sprite = PlayerController.Instance.BagOfHoldingSlot.sprite;
    }

    public void ShowSpecialSwapPanel()
    {
        swapItemPanel.panel.SetActive(true);
    }

    public void HideSpecialSwapPanel()
    {
        swapItemPanel.panel.SetActive(false);
    }

    public void ShowLevelReviewPanel()
    {
        player.gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        Time.timeScale = 0;
        levelReviewPanel.SetActive(true);
    }

    public void HideLevelReviewPanel()
    {
        if (PlayerHealth.Instance.Health > 0)
        {
            player.gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
            Time.timeScale = 1;
        }
        levelReviewPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        player.gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        gameOverScreen.SetActive(true);
        playerDamagedOverlay.SetActive(false);
        GameOverStats.Instance.SetGameOverStats();

        //Set initial selected button if using controller
        if (playerInput.currentControlScheme != "Keyboard&Mouse")
            gameOverScreen.GetComponent<Buttons>().SetSelectedButton();
        else
            gameOverScreen.GetComponent<Buttons>().ClearSelectedButtons();

        StartCoroutine(gameOverScreen.GetComponent<Buttons>().WaitToDisplayReview());
    }

    public void HideGameOver()
    {
        player.gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        gameOverScreen.SetActive(false);
    }

    public void ShowWinScreen()
    {
        player.gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        winScreen.SetActive(true);
        Time.timeScale = 0;
        playerDamagedOverlay.SetActive(false);

        //Set initial selected button if using controller
        if (playerInput.currentControlScheme != "Keyboard&Mouse")
            winScreen.GetComponent<Buttons>().SetSelectedButton();
        else
            winScreen.GetComponent<Buttons>().ClearSelectedButtons();
    }

    public void HideWinScreen()
    {
        player.gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        winScreen.SetActive(false);       
    }

    public void ShowPauseScreen()
    {
        player.gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        pauseScreen.SetActive(true);

        //Set initial selected button if using controller
        if (playerInput.currentControlScheme != "Keyboard&Mouse")
            pauseScreen.GetComponent<Buttons>().SetSelectedButton();
        else
            pauseScreen.GetComponent<Buttons>().ClearSelectedButtons();
    }

    public void HidePauseScreen()
    {
        player.gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        pauseScreen.SetActive(false);
    }

    public void ShowRunTimer()
    {
        runTimer.GetComponent<TextMeshProUGUI>().enabled = true;
    }

    public void HideRunTimer()
    {
        runTimer.GetComponent<TextMeshProUGUI>().enabled = false;
    }

    public IEnumerator ShowPlayerDamagedOverlay()
    {
        playerDamagedOverlay.SetActive(true);
        Color noAlpha = new Color(1, 1, 1, 0);
        Color lowHealth = new Color(1, 1, 1, 0.05f);
        Color hurtAlpha = new Color(1, 1, 1, 0.25f);
        Color lowHealthHurt = new Color(1, 1, 1, 0.35f);
        Color lerpToColor;
        Color lerpFromColor;
        float lerpSpeed = 3f;

        //Set color to lerp to based on if player is at low health
        if (PlayerHealth.Instance.Health < PlayerHealth.Instance.maxHealth / 4)
        {
            lerpFromColor = lowHealth;
            lerpToColor = lowHealthHurt;
        }
        else
        {
            lerpFromColor = noAlpha;
            lerpToColor = hurtAlpha;
        }


        //Lerp from no alpha to full alpha on the overlay
        float timer = 0;
        while(timer < 1/lerpSpeed)
        {
            playerDamagedOverlay.GetComponent<Image>().color = Color.Lerp(noAlpha, lerpToColor, timer);
            timer += Time.deltaTime * lerpSpeed;
            yield return new WaitForEndOfFrame();
        }

        //Wait briefly before lerping back
        yield return new WaitForSeconds(0.2f);
        lerpFromColor = lerpToColor;

        //Set color to lerp to based on if player is at low health
        if (PlayerHealth.Instance.Health < PlayerHealth.Instance.maxHealth / 4)
            lerpToColor = lowHealth;
        else
            lerpToColor = noAlpha;

        //Lerp from full alpha to no alpha on the overlay
        while (timer > 0)
        {
            playerDamagedOverlay.GetComponent<Image>().color = Color.Lerp(lerpToColor, lerpFromColor, timer);
            timer -= Time.deltaTime * lerpSpeed/1.5f;
            yield return new WaitForEndOfFrame();
        }

        if(lerpToColor == noAlpha)
            playerDamagedOverlay.SetActive(false);
    }

    public IEnumerator HidePlayerDamagedOverlay()
    {
        Color noAlpha = new Color(1, 1, 1, 0);
        Color lowHealth = new Color(1, 1, 1, 0.1f);
        float lerpSpeed = 3f;

        float timer = 1 / lerpSpeed;
        //Lerp from full alpha to no alpha on the overlay
        while (timer > 0)
        {
            playerDamagedOverlay.GetComponent<Image>().color = Color.Lerp(noAlpha, lowHealth, timer);
            timer -= Time.deltaTime * lerpSpeed / 1.5f;
            yield return new WaitForEndOfFrame();
        }
        playerDamagedOverlay.SetActive(false);
    }
}
