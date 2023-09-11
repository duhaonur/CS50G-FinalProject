using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image healthBarImg;
    [SerializeField] private Gradient healthBarColor;
    [SerializeField] private TextMeshProUGUI bulletText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private float maxHealth;
    [SerializeField] private float healthDecreaseAmount;
    [SerializeField] private float healthIncreaseAmount;
    [SerializeField] private float scoreIncreaseAmount;

    private float scoreHolder;
    private float health;

    private void Start()
    {
        scoreHolder = 0;

        health = maxHealth;

        slider.maxValue = maxHealth;
        slider.value = health;

        healthBarImg.color = healthBarColor.Evaluate(Mathf.InverseLerp(0, maxHealth, health));

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        MyEvents.OnDecreaseHealth += DecreaseHealth;
        MyEvents.OnGetBulletAmount += GetBulletAmount;
        MyEvents.OnIncreaseScore += IncreaseScoreHealth;
    }
    private void OnDisable()
    {
        MyEvents.OnDecreaseHealth -= DecreaseHealth;
        MyEvents.OnGetBulletAmount -= GetBulletAmount;
        MyEvents.OnIncreaseScore -= IncreaseScoreHealth;
    }

    private void IncreaseScoreHealth()
    {
        scoreHolder += scoreIncreaseAmount;
        scoreText.text = "Score: " + scoreHolder;

        if (health < 100)
        {
            health += healthIncreaseAmount;
            ChangeHealthBar();
        }
    }

    private void GetBulletAmount(int bullet)
    {
        bulletText.text = bullet + " / \u221E";
    }

    public void DecreaseHealth()
    {
        health -= healthDecreaseAmount;

        if (health <= 0)
        {
            health = 0;

            if (PlayerPrefs.GetFloat("HighestScore") < scoreHolder)
                PlayerPrefs.SetFloat("HighestScore", scoreHolder);

            MyEvents.CallGetScore(scoreHolder);
            MyEvents.CallPlayerDied();
        }
        ChangeHealthBar();
    }

    private void ChangeHealthBar()
    {
        slider.value = health;
        healthBarImg.color = healthBarColor.Evaluate(Mathf.InverseLerp(0, maxHealth, health));
    }
}
