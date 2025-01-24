using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandleAbilities: MonoBehaviour
{

    [Category("Abilities")]
    [SerializeField] float[] AbilityCooldowns = new float[5];
    [SerializeField] TextMeshProUGUI[] AbilityCooldownTexts = new TextMeshProUGUI[5];

    float[] currentCooldowns;

    void Start()
    {
        currentCooldowns = new float[AbilityCooldowns.Length];
    }

    void updateCooldowns()
    {
        for (int i = 0; i < currentCooldowns.Length; i++)
        {
            if (currentCooldowns[i] > 0.0f)
            {
                currentCooldowns[i] -= Time.deltaTime;
                AbilityCooldownTexts[i].text = currentCooldowns[i].ToString("F1");
            } else
            {
                currentCooldowns[i] = 0;
                AbilityCooldownTexts[i].text = "R";
            }
        }
    }

    public void useAbility(int abilityIndex)
    {
        if (currentCooldowns[abilityIndex] == 0)
        {
            switch (abilityIndex)
            {
                case 0:
                Debug.Log("Used Attack1");

                break;
                case 1:
                Debug.Log("Used Attack2");

                break;
                case 2:
                Debug.Log("Used Ability1");

                break;
                case 3:
                Debug.Log("Used Ability2");

                break;
                case 4:
                Debug.Log("Used Ultimate");

                break;
            }
            currentCooldowns[abilityIndex] = AbilityCooldowns[abilityIndex];
        }
    }


    void Update()
    {
        updateCooldowns();
    }



}
