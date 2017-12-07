using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
using Assets.Scripts.CardSystem;

namespace Assets.Scripts.UI
{
    public class CardSelectorMultiplayer : MonoBehaviour
    {
        public delegate void CardSelectorAction();
        public static event CardSelectorAction CardSelectorEnabled, CardSelectorDisabled, CardsSelected, CardsDeselected;

        private static int playerIndex;
        private int thisPlayerIndex;
        private int numSelections = 0;
        private static int totalSelections = 0;
        private const int NUM_SELECTIONS = 8, MAX_SELECTIONS = 4;
        private const int ELEMENT_INDEX = 0, CHILD_IMAGE_INDEX = 1;

        private Player.Player player;
        private Deck deck;
        private List<Card> selectedCards, selectionOptions;
        private List<int> finalMap;
        private GameObject[] selectionButtons;
        private Image[] finalButtons;

        private static Image[] xboxButtons;

        [SerializeField]
        private List<Sprite> xboxButtonSprites;

        void OnEnable()
        {
            SelectionTimer.TimerFinish += EnableCanvas;
            CardTimer.TimerFinish += Okay;
        }
        void OnDisable()
        {
            SelectionTimer.TimerFinish -= EnableCanvas;
            CardTimer.TimerFinish -= Okay;
        }

        void Start()
        {
            //should find players provided they are named in the fashion: "Player 1" or "Player 42"
            thisPlayerIndex = ++playerIndex;
            player = GameObject.Find("Player " + playerIndex).GetComponent<Player.Player>();
            deck = player.Deck;
            selectionOptions = new List<Card>();
            selectedCards = new List<Card>();
            finalMap = new List<int>();

            selectionButtons = new GameObject[NUM_SELECTIONS];
            finalButtons = new Image[MAX_SELECTIONS];
            GameObject[] gos = GameObject.FindGameObjectsWithTag("Selection").OrderBy(go => go.name).ToArray();
            for (int i = 0; i < NUM_SELECTIONS; i++)
            {
                gos[i].tag = "Selection " + thisPlayerIndex.ToString();
                selectionButtons[i] = gos[i];
            }
            
            gos = GameObject.FindGameObjectsWithTag("Final").OrderBy(go => go.name).ToArray();
            for (int i = 0; i < MAX_SELECTIONS; i++)
            {
                gos[i].tag = "Final " + thisPlayerIndex.ToString();
                finalButtons[i] = gos[i].GetComponent<Image>();
            }

            if (xboxButtons == null)
            {
                xboxButtons = new Image[xboxButtonSprites.Count];
                gos = GameObject.FindGameObjectsWithTag("Xbox Button").OrderBy(go => go.name).ToArray();
                for (int i = 0; i < NUM_SELECTIONS * 2; i++)
                {
                    xboxButtons[i] = gos[i].GetComponent<Image>();
                }

                xboxButtons[0].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard0, thisPlayerIndex)));
                xboxButtons[1].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard1, thisPlayerIndex)));
                xboxButtons[2].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard2, thisPlayerIndex)));
                xboxButtons[3].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard3, thisPlayerIndex)));
                xboxButtons[4].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard4, thisPlayerIndex)));
                xboxButtons[5].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard5, thisPlayerIndex)));
                xboxButtons[6].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard6, thisPlayerIndex)));
                xboxButtons[7].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard7, thisPlayerIndex)));
                
                xboxButtons[8].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard0, thisPlayerIndex)));
                xboxButtons[9].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard1, thisPlayerIndex)));
                xboxButtons[10].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard2, thisPlayerIndex)));
                xboxButtons[11].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard3, thisPlayerIndex)));
                xboxButtons[12].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard4, thisPlayerIndex)));
                xboxButtons[13].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard5, thisPlayerIndex)));
                xboxButtons[14].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard6, thisPlayerIndex)));
                xboxButtons[15].sprite = xboxButtonSprites.Find(x => x.name.Contains(CustomInput.gamepadButton(CustomInput.UserInput.PickCard7, thisPlayerIndex)));
            }

            DrawPossibleSelections();
            InitializeDisplayData();
            UpdateFinalDisplayData();
        }

        void Update()
        {
            if (Managers.GameManager.State == Enums.GameStates.CardSelection)
            {
                if (CustomInput.BoolFreshPress(CustomInput.UserInput.Attack))
                {
                    DrawPossibleSelections();
                }

                if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard0, thisPlayerIndex)) SelectCard(0);
                if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard1, thisPlayerIndex)) SelectCard(1);
                if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard2, thisPlayerIndex)) SelectCard(2);
                if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard3, thisPlayerIndex)) SelectCard(3);
                if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard4, thisPlayerIndex)) SelectCard(4);
                if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard5, thisPlayerIndex)) SelectCard(5);
                if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard6, thisPlayerIndex)) SelectCard(6);
                if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard7, thisPlayerIndex)) SelectCard(7);
            }
        }

        private void DrawPossibleSelections()
        {
            selectionOptions = deck.DrawHand();
            for (int i = 0; i < NUM_SELECTIONS; i++)
            {
                if (selectionOptions != null && i < selectionOptions.Count && selectionOptions[i] != null)
                {
                    selectionButtons[i].transform.GetChild(CHILD_IMAGE_INDEX).GetComponent<Image>().sprite = selectionOptions[i].Image;
                }
                else
                {
                    selectionButtons[i].transform.GetChild(CHILD_IMAGE_INDEX).GetComponent<Image>().sprite = null;
                }
            }
        }

        public void SelectCard(int index)
        {
            if (selectionOptions == null || index >= selectionOptions.Count) return;
            if (finalMap.Contains(index))
            {
                selectedCards.Remove(selectionOptions[index]);
                finalMap.Remove(index);
                UpdateFinalDisplayData();
                if (CardsDeselected != null) CardsDeselected();
                if (--numSelections < 0) numSelections = 0;
                if (--totalSelections < 0) totalSelections = 0;
            }
            else
            {
                if (numSelections >= MAX_SELECTIONS) return;
                numSelections++;
                totalSelections++;
                selectedCards.Add(selectionOptions[index]);
                finalMap.Add(index);
                UpdateFinalDisplayData();
            }
        }

        public void EnableCanvas()
        {
            selectedCards.Clear();
            finalMap.Clear();
            numSelections = 0;
            totalSelections = 0;
            transform.GetComponent<Canvas>().enabled = true;
            DrawPossibleSelections();
            UpdateDisplayData();
            UpdateFinalDisplayData();
            if (CardSelectorEnabled != null) CardSelectorEnabled();
        }

        public void Okay()
        {
            if (selectionOptions != null)
            {
                for (int i = 0; i < selectedCards.Count; i++)
                {
                    selectionOptions.Remove(selectedCards[i]);
                }
                deck.ReturnUsedCards(selectionOptions);
                player.AddCardsToHand(selectedCards);
            }
            transform.GetComponent<Canvas>().enabled = false;
            if (CardSelectorDisabled != null) CardSelectorDisabled();
        }

        void OnLevelWasLoaded(int i)
        {
            playerIndex = 0;
        }

        #region DISPLAY_DATA
        public void UpdateFinalDisplayData()
        {
            for (int i = 0; i < MAX_SELECTIONS; i++)
            {
                if (i < finalMap.Count)
                {
                    finalButtons[i].color = CustomColor.Convert255(0, 92, 122);
                }
                else
                {
                    finalButtons[i].color = CustomColor.Convert255(255, 255, 255);
                }
            }
            if (totalSelections >= MAX_SELECTIONS * 2 || selectionOptions == null || totalSelections >= selectionOptions.Count)
            {
                if (CardsSelected != null) CardsSelected();
            }
        }

        public void InitializeDisplayData()
        {
            UpdateDisplayData();
        }

        public void UpdateDisplayData()
        {
            for (int i = 0; i < NUM_SELECTIONS; i++)
            {
                if (selectionOptions != null && i < selectionOptions.Count)
                {
                    selectionButtons[i].transform.GetChild(ELEMENT_INDEX).GetComponent<Image>().color = CustomColor.ColorFromElement(selectionOptions[i].Element);
                    selectionButtons[i].transform.GetChild(CHILD_IMAGE_INDEX).GetComponent<Image>().color = Color.white;
                    selectionButtons[i].transform.GetChild(CHILD_IMAGE_INDEX).GetComponent<Image>().sprite = selectionOptions[i].Image;
                }
                else
                {
                    selectionButtons[i].transform.GetChild(ELEMENT_INDEX).GetComponent<Image>().color = CustomColor.ColorFromElement(Enums.Element.None);
                    selectionButtons[i].transform.GetChild(CHILD_IMAGE_INDEX).GetComponent<Image>().color = Color.black;
                    selectionButtons[i].transform.GetChild(CHILD_IMAGE_INDEX).GetComponent<Image>().sprite = null;
                }
            }
        }
        #endregion
    }
}