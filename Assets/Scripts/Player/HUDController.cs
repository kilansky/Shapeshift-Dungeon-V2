using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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
    public GameObject controllerRecText;
    public Color grayedTextColor;

    [Header("Special Item Panel")]
    public UIPanel specialItemPanel;
    public Image speicalItemIcon;
    public Image chargeBarFill;

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

    public bool ShowLevelReview { get; set; }

    private PlayerController player;
    private PlayerInput playerInput;
    private string currentControlScheme;
    private bool itemCollected = false;
    private bool pocketSlot1Used = false;
    private bool pocketSlot2Used = false;

    void Start()
    {
        ShowLevelReview = true;
        player = PlayerController.Instance;
        playerInput = player.gameObject.GetComponent<PlayerInput>();
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
        }
    }

    //Change the UI based on the new control type
    public void ControlSchemeChanged()
    {
        currentControlScheme = playerInput.currentControlScheme;

        //Using Keyboard
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            potionsPanel.image.sprite = potionsPanel.keyboardButton;
            quickHintPanel.image.sprite = quickHintPanel.keyboardButton;

            movePanel.image.sprite = movePanel.keyboardButton;
            dashPanel.image.sprite = dashPanel.keyboardButton;
            attackPanel.image.sprite = attackPanel.keyboardButton;
            chargePanel.image.sprite = chargePanel.keyboardButton;
            specialItemPanel.image.sprite = specialItemPanel.keyboardButton;
        }
        //Using Controller
        else
        {
            //Using PS4 Controller
            if (Gamepad.current.device.ToString().Contains("DualShock"))
            {
                potionsPanel.image.sprite = potionsPanel.psButton;
                quickHintPanel.image.sprite = quickHintPanel.psButton;

                movePanel.image.sprite = movePanel.psButton;
                dashPanel.image.sprite = dashPanel.psButton;
                attackPanel.image.sprite = attackPanel.psButton;
                chargePanel.image.sprite = chargePanel.psButton;
                specialItemPanel.image.sprite = specialItemPanel.psButton;
            }
            //Using Xbox Controller
            else
            {
                potionsPanel.image.sprite = potionsPanel.xboxButton;
                quickHintPanel.image.sprite = quickHintPanel.xboxButton;

                movePanel.image.sprite = movePanel.xboxButton;
                dashPanel.image.sprite = dashPanel.xboxButton;
                attackPanel.image.sprite = attackPanel.xboxButton;
                chargePanel.image.sprite = chargePanel.xboxButton;
                specialItemPanel.image.sprite = specialItemPanel.xboxButton;
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

        chargeBarFill.fillAmount = value / maxValue;
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
        PlayerController.Instance.SpecialCharge = PlayerController.Instance.specialCooldownTime.Value;
        UpdateSpecialCharge();
    }

    public void HideSpecialItemPanel()
    {
        specialItemPanel.panel.SetActive(false);
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
        if (PlayerHealth.Instance.Health < PlayerHealth.Instance.maxHealth / 3)
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
        if (PlayerHealth.Instance.Health < PlayerHealth.Instance.maxHealth / 3)
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
