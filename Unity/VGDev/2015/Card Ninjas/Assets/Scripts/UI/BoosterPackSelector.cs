using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
using System;

namespace Assets.Scripts.UI
{
    public class BoosterPackSelector : MonoBehaviour
    {
        private static int playerIndex;
        private int thisPlayerIndex;
        private int numSelections = 0;
        private const int NUM_SELECTIONS = 7, MAX_SELECTIONS = 5;
        private const int NUM_BG_INDEX = 1, NUM_TEXT_INDEX = 2;

        private Player.Player player;

        private Text header;

        private Button[] packs;
        private List<int> selectedPacks;
        private int lastSelected = 0;

        private Button okay;

        private Enums.Element[] elementMap = {Enums.Element.Fire, Enums.Element.Water, Enums.Element.Thunder,
                                              Enums.Element.Earth, Enums.Element.Wood, Enums.Element.None, Enums.Element.None};
        void Start()
        {
            //should find players provided they are named in the fashion: "Player 1" or "Player 42"
            thisPlayerIndex = ++playerIndex;
            //player = GameObject.Find("Player " + playerIndex).GetComponent<Player.Player>();
            header = GameObject.Find("Header").GetComponent<Text>();

            packs = new Button[NUM_SELECTIONS];
            GameObject[] gos = GameObject.FindGameObjectsWithTag("Pack").OrderBy(go => go.name).ToArray();
            for (int i = 0; i < gos.Length; i++)
            {
                gos[i].tag = "Pack " + thisPlayerIndex.ToString();
                packs[i] = gos[i].GetComponent<Button>();
                packs[i].transform.GetChild(NUM_BG_INDEX).gameObject.SetActive(false);
                packs[i].transform.GetChild(NUM_TEXT_INDEX).gameObject.SetActive(false);
            }

            selectedPacks = new List<int>();
            try
            {
                okay = GameObject.Find("Okay").GetComponent<Button>();
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        void Update()
        {
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Up, thisPlayerIndex)) Navigator.Navigate(CustomInput.UserInput.Up, packs[0].gameObject);
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Down, thisPlayerIndex)) Navigator.Navigate(CustomInput.UserInput.Down, packs[0].gameObject);
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Right, thisPlayerIndex)) Navigator.Navigate(CustomInput.UserInput.Right, packs[0].gameObject);
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Left, thisPlayerIndex)) Navigator.Navigate(CustomInput.UserInput.Left, packs[0].gameObject);
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Accept, thisPlayerIndex)) Navigator.CallSubmit();

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Cancel, thisPlayerIndex)) RemovePack(lastSelected);
        }

        public void AddPack(int index)
        {
            if (numSelections >= MAX_SELECTIONS) return;
            numSelections++;
            selectedPacks.Add(index);

            packs[index].transform.GetChild(NUM_BG_INDEX).gameObject.SetActive(true);
            packs[index].transform.GetChild(NUM_TEXT_INDEX).gameObject.SetActive(true);
            packs[index].transform.GetChild(NUM_TEXT_INDEX).GetComponent<Text>().text = (int.Parse(packs[index].transform.GetChild(NUM_TEXT_INDEX).GetComponent<Text>().text)+1).ToString();

            header.text = "Select <color=#7A0000FF> " + (MAX_SELECTIONS - numSelections).ToString() + " </color> Pack(s)";
            if (numSelections >= MAX_SELECTIONS)
            {
                if (okay) okay.interactable = true;
            }
        }

        public void RemovePack(int index)
        {
            int numLeft = (int.Parse(packs[index].transform.GetChild(NUM_TEXT_INDEX).GetComponent<Text>().text) - 1);

            if (numSelections <= 0 || numLeft < 0) return;
            numSelections--;
            selectedPacks.Remove(index);

            if (numLeft <= 0)
            {

                packs[index].transform.GetChild(NUM_BG_INDEX).gameObject.SetActive(false);
                packs[index].transform.GetChild(NUM_TEXT_INDEX).gameObject.SetActive(false);
            }
            packs[index].transform.GetChild(NUM_TEXT_INDEX).GetComponent<Text>().text = numLeft.ToString();

            header.text = "Select <color=#7A0000FF>" + (MAX_SELECTIONS - numSelections).ToString() + " </color>Pack(s)";
            if (okay)
            {
                okay.interactable = false;
            }
        }

        public void Okay()
        {
            CardSystem.BoosterPackReader packReader = GameObject.FindObjectOfType<CardSystem.BoosterPackReader>();
            GameObject newDeck = new GameObject();
            newDeck.transform.name = "DeckTransfer " + thisPlayerIndex.ToString();
            newDeck.AddComponent<DeckTransfer>();

            List<CardSystem.Card> cards = new List<CardSystem.Card>();
            Dictionary<Enums.Element, int> map = new Dictionary<Enums.Element, int>();

            for (int i = 0; i < selectedPacks.Count; i++)
            {
                cards.AddRange(packReader.BoosterPacks[selectedPacks[i]].GetCards());

                Enums.Element key = elementMap[selectedPacks[i]];
                if (!map.ContainsKey(key)) map.Add(key, 1);
                else
                {
                    int value = 0;
                    map.TryGetValue(key, out value);
                    map[key] = value + 1;
                }
            }

            Enums.Element playerType = Enums.Element.None;
            int max = 0;
            foreach (KeyValuePair<Enums.Element, int> entry in map)
            {
                if(entry.Value > max)
                {
                    max = entry.Value;
                    playerType = entry.Key;
                }
            }
            CardSystem.Deck deck = new CardSystem.Deck(cards);

            DeckTransfer t = newDeck.GetComponent<DeckTransfer>();
            t.Deck = deck;
            t.Element = playerType;

            DontDestroyOnLoad(newDeck);

			// Load loading screen, correct level should already be stored from Main Menu
			LoadingScreen.instance.LoadLevel(LoadingScreen.LevelToLoad);
        }

        void OnLevelWasLoaded(int i)
        {
            playerIndex = 0;
        }

        public int LastSelected
        {
            set { lastSelected = value; }
        }
    }
}