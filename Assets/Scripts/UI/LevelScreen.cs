using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelScreen : MonoBehaviour
{
    [SerializeField] UIDocument doc;
    [SerializeField] PlayerProgress playerProgress;
    [SerializeField] Cloud player;
    [SerializeField] GameUI gameUI;
    [SerializeField] BossAI boss;

    public enum BossHealthBarColor { Red, Blue, Gold}

    Button rainButton;
    Button lightningButton;
    Button tornadoButton;
    VisualElement lightningMask;
    VisualElement rainMask;
    VisualElement tornadoMask;
    VisualElement rainContent;
    VisualElement lightningContent;
    VisualElement tornadoContent;
    [SerializeField] int perkMaskMaxValue = 100;

    public event SimpleEvent OnRainButtonClick;
    public event SimpleEvent OnLightningButtonClick;
    public event SimpleEvent OnTornadoButtonClick;

    List<VisualElement> healths;
    [SerializeField] int activeHealth = 0;

    VisualElement bossHealthBar;
    VisualElement bossHealthBarMask;
    VisualElement bossBarContent;
    [SerializeField] int bossMaskMaxValue = 500;

    VisualElement nextHealthMask;
    [SerializeField] int nextHealthMaxValue = 400;
    VisualElement altBonus;
    VisualElement spdBonus;

    private void Start()
    {
        rainButton = (Button)doc.rootVisualElement.Query("RainButton");
        lightningButton = (Button)doc.rootVisualElement.Query("LightningButton");
        tornadoButton = (Button)doc.rootVisualElement.Query("TornadoButton");
        
        rainMask = doc.rootVisualElement.Query("RainMask");
        lightningMask = doc.rootVisualElement.Query("LightningMask");
        tornadoMask = doc.rootVisualElement.Query("TornadoMask");

        rainContent = doc.rootVisualElement.Query("RainContent");
        lightningContent = doc.rootVisualElement.Query("LightningContent");
        tornadoContent = doc.rootVisualElement.Query("TornadoContent");

        bossBarContent = doc.rootVisualElement.Query("BossHealthContent");
        bossHealthBar = doc.rootVisualElement.Query("BossHealthPanel");
        bossHealthBarMask = doc.rootVisualElement.Query("BossHealthMask");

        healths = doc.rootVisualElement.Query(className: "player-health").ToList();

        nextHealthMask = doc.rootVisualElement.Query("NextHealthMask");

        altBonus = doc.rootVisualElement.Query("AltitudeBonus");
        spdBonus = doc.rootVisualElement.Query("SpeedBonus");

        if (Application.platform == RuntimePlatform.Android)
            MovePerksToBottom(true);

        rainButton.clicked += RainButton_clicked;
        lightningButton.clicked += LightningButton_clicked;
        tornadoButton.clicked += TornadoButton_clicked;
    }

    private void TornadoButton_clicked()
    {
        player.TornadoActivate();
    }

    private void LightningButton_clicked()
    {
        player.LightningActivate();
    }

    private void RainButton_clicked()
    {
        player.RainActivate();
    }

    public int PlayerHealth
    {
        get
        {
            return activeHealth;
        }
        set
        {
            if (value > healths.Count)
                activeHealth = healths.Count;
            else
                activeHealth = value;

            for (int i = 0; i < healths.Count; i++)
                healths[i].visible = i <= activeHealth - 1;
        }
    }

    public bool RainButtonActive { get { return rainButton.visible; } set { rainButton.visible = value; } }

    public bool LightningButtonActive { get { return lightningButton.visible; } set { lightningButton.visible = value; } }

    public bool TornadoButtonActive { get { return tornadoButton.visible;  } set { tornadoButton.visible = value; } }

    public bool BossHealthBarVisible { get { return bossHealthBar.visible; } set { bossHealthBar.visible = value; } }

    public void SetBossHealthColor(BossHealthBarColor color)
    {
        bossBarContent.RemoveFromClassList("boss-content-gold");
        bossBarContent.RemoveFromClassList("boss-content-blue");
        bossBarContent.RemoveFromClassList("boss-content-red");

        if (color == BossHealthBarColor.Red)
        {
            bossBarContent.AddToClassList("boss-content-red");
            
        }

        if(color == BossHealthBarColor.Blue)
        {
            bossBarContent.AddToClassList("boss-content-blue");
        }

        if(color == BossHealthBarColor.Gold)
        {
            bossBarContent.AddToClassList("boss-content-gold");
        }
    }
    /// <summary>
    /// set bosss health level
    /// </summary>
    /// <param name="value">percent of all health 0..maxMaskValue</param>
    public void SetBossBarValue(int value)
    {
        if (value > bossMaskMaxValue)
            value = bossMaskMaxValue;
        if (value < 0)
            value = 0;

        StyleLength stLength = bossHealthBarMask.style.width;
        Length length = stLength.value;
        length.value = value;
        stLength.value = length;
        bossHealthBarMask.style.width = stLength;
    }
    public void SetRainCharge(int value)
    {
        if (value < 0)
            value = 0;
        if (value > perkMaskMaxValue)
            value = perkMaskMaxValue;

        StyleLength stLength = rainMask.style.height;
        Length length = stLength.value;
        length.value = value;
        stLength.value = length;
        rainMask.style.height = stLength;

        if (value == 100)
        {
            rainContent.AddToClassList("rain-content-full");
            rainContent.RemoveFromClassList("rain-content-filling");
        }
        else
        {
            rainContent.RemoveFromClassList("rain-content-full");
            rainContent.AddToClassList("rain-content-filling");
        }
    }
    public void SetLightningCharge(int value)
    {
        if (value < 0)
            value = 0;
        if (value > perkMaskMaxValue)
            value = perkMaskMaxValue;

        StyleLength stLength = lightningMask.style.height;
        Length length = stLength.value;
        length.value = value;
        stLength.value = length;
        lightningMask.style.height = stLength;

        if (value == 100)
        {
            lightningContent.AddToClassList("lightning-content-full");
            lightningContent.RemoveFromClassList("lightning-content-filling");
        }
        else
        {
            lightningContent.RemoveFromClassList("lightning-content-full");
            lightningContent.AddToClassList("lightning-content-filling");
        }
    }
    public void SetTornadogCharge(int value)
    {
        if (value < 0)
            value = 0;
        if (value > perkMaskMaxValue)
            value = perkMaskMaxValue;

        StyleLength stLength = tornadoMask.style.height;
        Length length = stLength.value;
        length.value = value;
        stLength.value = length;
        tornadoMask.style.height = stLength;

        if (value == 100)
        {
            tornadoContent.AddToClassList("tornado-content-full");
            tornadoContent.RemoveFromClassList("tornado-content-filling");
        }
        else
        {
            tornadoContent.RemoveFromClassList("tornado-content-full");
            tornadoContent.AddToClassList("tornado-content-filling");
        }
    }

    public void SetNextHealthValue(int value)
    {
        if (value < 0)
            value = 0;
        if (value > nextHealthMaxValue)
            value = nextHealthMaxValue;

        StyleLength stLength = nextHealthMask.style.width;
        Length length = stLength.value;
        length.value = value;
        stLength.value = length;
        nextHealthMask.style.width = stLength;
    }

    public void SetBonusesState()
    {
        altBonus.visible = playerProgress.CheckAltitudeBonus;
        spdBonus.visible = playerProgress.CheckSpeedBonus;
    }

    void MovePerksToBottom(bool toBottom)
    {
        VisualElement perksPanel = doc.rootVisualElement.Query("PerkPanel");
        VisualElement topPerksCorner = doc.rootVisualElement.Query("PerksColumn");
        VisualElement bottomPerksCorner = doc.rootVisualElement.Query("PerkColumn");

        if (toBottom)
            bottomPerksCorner.Insert(0, perksPanel);
    }

    private void Update()
    {
        PlayerHealth = playerProgress.Health;
        RainButtonActive = player.RainAvailableByProgress;
        LightningButtonActive = player.LightningAvailableByProgress;
        TornadoButtonActive = player.TornadoAvailableByProgress;

        SetRainCharge(Mathf.CeilToInt(gameUI.RainChargeLevel * perkMaskMaxValue));
        SetLightningCharge(Mathf.CeilToInt(gameUI.LightninhChargeLevel * perkMaskMaxValue));
        SetTornadogCharge(Mathf.CeilToInt(gameUI.TornadoChargeLevel * perkMaskMaxValue));

        BossHealthBarVisible = boss.gameObject.activeSelf;

        BossAI.BossStage stage = boss.GetCurrentStage;
        switch(stage)
        {
            case BossAI.BossStage.Simple:
                SetBossHealthColor(BossHealthBarColor.Red);
                break;
            case BossAI.BossStage.Normal:
                SetBossHealthColor(BossHealthBarColor.Blue);
                break;
            case BossAI.BossStage.Hard:
                SetBossHealthColor(BossHealthBarColor.Gold);
                break;
            default:
                SetBossHealthColor(BossHealthBarColor.Red);
                break;
        }

        SetBossBarValue(Mathf.CeilToInt(boss.HealthPercent * bossMaskMaxValue));

        SetNextHealthValue(Mathf.CeilToInt(gameUI.NextHealthProgress * nextHealthMaxValue));
        SetBonusesState();
    }
}
