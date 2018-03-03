using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

/*
    This script handles functionality for a UI element.
    This includes:
        - Retrieving information about the current characters/party
        - Displaying that information
        - Triggering appropriate animation
        - Switching between Exploration and Combat
    
    fromrustcg.com
    razburygames.com
*/

public class EPTracker : MonoBehaviour {

    //Components assigned in the Inspector
    public Animator epAnim;
    public List <GameObject> numberList = new List<GameObject>();
    public List <Image> spriteList = new List<Image>();

    //Used to differentiate between the main gear from the alternate
    //There is a sister gear that turns in sync with this one but has no other functionality
    public bool isMain;

    //Internal variables
    List <ICombatant> battleList = new List<ICombatant>();
    int pointsLeftToDisplay;
    int numberIndex;
    int battleIndex;
    bool firstSkip;

	void Start ()
    {
        SubscribeToEventBus();
        SetTextLayer();
	}

    void SubscribeToEventBus () //All game events that the script listens for and what function is run as a result
    {
        EventBus.ClientInstance.Subscribe(EventConstants.EPReduced, new EventHandler(MinusEP));
        EventBus.ClientInstance.Subscribe(EventConstants.DayEndedOnClient, new EventHandler(ClearGear));
        EventBus.ClientInstance.Subscribe(EventConstants.TurnStarted, new EventHandler(SetEP));

        EventBus.ClientInstance.Subscribe(EventConstants.CombatStarted,new EventHandler(StartCombat));
        EventBus.ClientInstance.Subscribe(EventConstants.CombatActionTaken, new EventHandler(AdvanceCombat));
        EventBus.ClientInstance.Subscribe(EventConstants.CombatEnded, new EventHandler(ClearGear));

        EventBus.ClientInstance.Subscribe(EventConstants.CharacterDeath, new EventHandler(ResetPortraits));
        EventBus.ClientInstance.Subscribe(EventConstants.MonsterDeath, new EventHandler(ResetPortraits));
    }
	
    void SetTextLayer () //Setting the UI element to the right spot in the layer
    {
        foreach (GameObject g in numberList)
        {
            g.GetComponent<MeshRenderer>().sortingLayerName = "EPTracker";
            g.GetComponent<MeshRenderer>().sortingOrder = 6;
        }
    }

    void ClearTextAndSprites () //Removes all items from the gear
    {
        foreach (GameObject g in numberList)
        {
            g.GetComponent<TextMesh>().text = null;
            g.GetComponent<TextMesh>().color = Color.clear;
        }

        foreach (Image im in spriteList)
        {
            im.sprite = null;
            im.enabled = false;
        }
    }

    void SetEP (object source, EventArgs args) //Spins/clears the gear and gets the new number to display
    {
        ResetGear();

        if (isMain)
        {
            numberIndex = 0;
            firstSkip = false;
            ClearTextAndSprites();
            pointsLeftToDisplay = FindObjectOfType<EPManager>().startingEP;
            EPNumbersToGear(pointsLeftToDisplay);
        }
    }

    void EPNumbersToGear (int ep) //Displays the number of moves left on the gear
    {
        if (ep < numberList.Count)
        {
            for (int i = 0; i < numberList.Count; i++)
            {
                if (i <= ep)
                {
                    numberList[i].GetComponent<TextMesh>().text = pointsLeftToDisplay.ToString();
                    pointsLeftToDisplay--;
                }
                else
                    numberList[i].GetComponent<TextMesh>().text = null;
            }
        }
        else
        {
            for (int i = 0; i < numberList.Count; i++)
            {
                numberList[i].GetComponent<TextMesh>().text = pointsLeftToDisplay.ToString();
                pointsLeftToDisplay--;
            }
        }
    }

    void MinusEP (object source, EventArgs args)
    {
        RotateGear();

        if (isMain)
            CheckEPGear();
    }

    void CheckEPGear () //Replaces numbers on the gear with next number or clears the spot
    {
        if (numberIndex > 9)
            numberIndex = 0;

        if (firstSkip)
        {
            if (pointsLeftToDisplay >= 0)
                numberList[numberIndex].GetComponent<TextMesh>().text = pointsLeftToDisplay.ToString();
            else
                numberList[numberIndex].GetComponent<TextMesh>().text = null;

            pointsLeftToDisplay--;
            numberIndex++;
        }
        else
            firstSkip = true;
    }

    void ClearGear (object source, EventArgs args) //Clears the gear
    {
        ResetGear();

        if (isMain)
            ClearTextAndSprites();
    }

    void StartCombat (object source, EventArgs args) //Resets the gear for combat
    {
        ResetGear();

        if (isMain)
        {
            numberIndex = 0;
            battleIndex = 0;
            firstSkip = false;
            ClearTextAndSprites();
            FillPortraits();
        }
    }

    void FillPortraits () //Get the list of combatants and displays them on the gear
    {
        battleList.Clear();

        for (int i = 0; i < CombatManager.Combatants.Count; i++)
        {
            battleList.Add(CombatManager.Combatants[i]);
        }
        
        foreach (Image im in spriteList)
        {
            im.enabled = true;

            if (battleIndex >= battleList.Count)
                battleIndex = 0;

            im.sprite = battleList[battleIndex].CombatPortrait;
            im.name = battleList[battleIndex].combatantName;

            battleIndex++;
        }
    }

    void AdvanceCombat (object source, EventArgs args) //Replaces portraits if needed
    {
        RotateGear();

        if (isMain)
        {
            if (firstSkip)
            {
                if (numberIndex >= spriteList.Count)
                    numberIndex = 0;

                if (battleIndex >= battleList.Count)
                    battleIndex = 0;

                spriteList[numberIndex].sprite = battleList[battleIndex].CombatPortrait;
                spriteList[numberIndex].name = battleList[battleIndex].combatantName;

                battleIndex++;
                numberIndex++;

            }
            else
            {
                firstSkip = true;
            }
        }
    }

    void ResetPortraits (object source, EventArgs args) //Redoes the portraits on the gear if a player is knocked out
    {
        if (isMain)
        {
            if (FindObjectOfType<CombatManager>().InCombat)
            {
                battleIndex = 0;
                numberIndex = 0;
                firstSkip = false;
                ResetGear();
                ClearTextAndSprites();
                FillPortraits();
            }
        }
    }

    void RotateGear () //Animation Trigger
    {
        epAnim.SetTrigger("SubtractEP");
    }

    void ResetGear () //Animation Trigger
    {
        epAnim.SetTrigger("ResetEP");
    }
}
