using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;

namespace Assets.Scripts.UI
{
    public class BoosterPackSelectorMultiplayer : MonoBehaviour
    {

        public delegate void SelectionPackAction();
        public static event SelectionPackAction PacksSelected;
        public static event SelectionPackAction PacksDeselected;

        void OnEnable()
        {
            SelectionTimer.TimerFinish += Okay;
        }
        void OnDisable()
        {
            SelectionTimer.TimerFinish -= Okay;
        }

        private static int playerIndex;
        private int thisPlayerIndex;
        private int numSelections = 0;
        private static int totalSelections = 0;
        private const int NUM_SELECTIONS = 7, MAX_SELECTIONS = 5;

        [SerializeField]
        private List<Sprite> xboxButtonSprites;

        private Player.Player player;

        private static Image[] xboxButtons;
        private static Text[] keyboardButtons;
        private Image[] selectedIcons;
        private List<int> selectedPacks;

        private static Enums.Element[] elementMap = {Enums.Element.Fire, Enums.Element.Water, Enums.Element.Thunder,
                                              Enums.Element.Earth, Enums.Element.Wood, Enums.Element.None, Enums.Element.None};

        void OnLevelWasLoaded(int i)
        {
            playerIndex = 0;
        }

        void Start()
        {
            thisPlayerIndex = ++playerIndex;

            if (xboxButtons == null)
            {
                xboxButtons = new Image[xboxButtonSprites.Count];
                GameObject[] gos = GameObject.FindGameObjectsWithTag("Xbox Button").OrderBy(go => go.name).ToArray();
                for (int i = 0; i < gos.Length; i++)
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
            }

            if (keyboardButtons == null)
            {
                keyboardButtons = new Text[7];
                GameObject[] gos = GameObject.FindGameObjectsWithTag("Keyboard Button").OrderBy(go => go.name).ToArray();
                for (int i = 0; i < gos.Length; i++)
                {
                    keyboardButtons[i] = gos[i].GetComponent<Text>();
                }

                keyboardButtons[0].text = CustomInput.keyboardKey(CustomInput.UserInput.PickCard0, thisPlayerIndex).ToString();
                keyboardButtons[1].text = CustomInput.keyboardKey(CustomInput.UserInput.PickCard1, thisPlayerIndex).ToString();
                keyboardButtons[2].text = CustomInput.keyboardKey(CustomInput.UserInput.PickCard2, thisPlayerIndex).ToString();
                keyboardButtons[3].text = CustomInput.keyboardKey(CustomInput.UserInput.PickCard3, thisPlayerIndex).ToString();
                keyboardButtons[4].text = CustomInput.keyboardKey(CustomInput.UserInput.PickCard4, thisPlayerIndex).ToString();
                keyboardButtons[5].text = CustomInput.keyboardKey(CustomInput.UserInput.PickCard5, thisPlayerIndex).ToString();
                keyboardButtons[6].text = CustomInput.keyboardKey(CustomInput.UserInput.PickCard6, thisPlayerIndex).ToString();
            }

            selectedIcons = new Image[MAX_SELECTIONS];
            GameObject[] icons = GameObject.FindGameObjectsWithTag("Multiplayer Selected").OrderBy(go => go.name).ToArray();
            for (int i = 0; i < MAX_SELECTIONS; i++)
            {
                selectedIcons[i] = icons[i].GetComponent<Image>();
                selectedIcons[i].transform.tag = "Multiplayer Selected " + thisPlayerIndex.ToString();
            }

            selectedPacks = new List<int>();
        }

        void Update()
        {
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard0, thisPlayerIndex)) AddPack(0);
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard1, thisPlayerIndex)) AddPack(1);
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard2, thisPlayerIndex)) AddPack(2);
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard3, thisPlayerIndex)) AddPack(3);
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard4, thisPlayerIndex)) AddPack(4);
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard5, thisPlayerIndex)) AddPack(5);
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.PickCard6, thisPlayerIndex)) AddPack(6);

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Cancel, thisPlayerIndex)) RemovePack();
        }

        public void AddPack(int index)
        {
            if (numSelections >= MAX_SELECTIONS) return;
            selectedPacks.Add(index);

            selectedIcons[numSelections++].color = CustomColor.Convert255(0, 92, 122);
            totalSelections++;

            if(totalSelections >= MAX_SELECTIONS * 2)
            {
                if (PacksSelected != null) PacksSelected();
            }
        }

        public void RemovePack()
        {
            if (numSelections <= 0) return;
            selectedPacks.RemoveAt(--numSelections);
            totalSelections--;
            selectedIcons[numSelections].color = CustomColor.Convert255(255, 255, 255);
            if (PacksDeselected != null) PacksDeselected();
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
                if (entry.Value > max)
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
			LoadingScreen.instance.LoadLevel("MultiplayerBattle");
        }
    }
}