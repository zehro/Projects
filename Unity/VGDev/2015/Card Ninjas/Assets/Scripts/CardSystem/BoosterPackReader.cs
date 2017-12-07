using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace Assets.Scripts.CardSystem
{
    class BoosterPackReader : MonoBehaviour
    {
        [SerializeField]
        private Sprite ErrorImage;
        [SerializeField]
        private Weapons.Hitbox hitbox;
        [SerializeField]
        private TextAsset[] BoosterPackXMLFiles;
        [SerializeField]
        private int[] BoosterPackSizes;
        [SerializeField]
        private Sprite[] BoosterPackImages;

        private BoosterPack[] boosterPacks;

        public BoosterPack[] BoosterPacks
        {
            get
            {
                if (boosterPacks == null)
                    boosterPacks = Packs();
                return boosterPacks;
            }
        }

        void Start()
        {
            if (boosterPacks == null)
                boosterPacks = Packs();
        } 


        private BoosterPack[] Packs()
        {
            BoosterPack[] packs = new BoosterPack[BoosterPackXMLFiles.Length];
            for (int i = 0; i < packs.Length; i++)
                packs[i] = ReadPack(BoosterPackXMLFiles[i], BoosterPackSizes[i], BoosterPackImages[i]);
            return packs;
        }

        /* XML Expected format
        <library>
            <card name= "Sword"> // The card name.
                <element>None</element>
                <image>Images/a</image>
                <type>Sword</type> // The card type, has to one of the Enums.CardTypes.
                <action range= "1" damage= "3" prefab= "Prefabs/HitBox">Sword</action> // Defines the action class for this card, must implement action and be in the namespace Assets.Scripts.CardSystem.Actions.
                <description>A basic sword.</description> // The string description of the card.
            </card>
        </library>
        */

        private BoosterPack ReadPack(TextAsset pack, int packSize, Sprite packImage)
        {
            //List<Card> tempList = new List<Card>();
            List<Card> cards = new List<Card>();
            GameObject prefab;
            Sprite image;
            string name, element, type, actionType, description;
            int range, damage;
            using (XmlReader reader = XmlReader.Create(new StringReader(pack.text)))
            {
                while (reader.ReadToFollowing("card"))
                {
                    //get name attribute
                    reader.MoveToAttribute(0);
                    name = reader.Value;
                    //get element
                    reader.ReadToFollowing("element");
                    element = reader.ReadElementContentAsString();
                    //load image
                    reader.ReadToFollowing("image");
                    image = (Resources.Load(reader.ReadElementContentAsString(), typeof(Sprite)) as Sprite);
                    if (image == null)
                        image = ErrorImage;
                    //get type
                    reader.ReadToFollowing("type");
                    type = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("action");
                    //get range
                    reader.MoveToFirstAttribute();
                    range = int.Parse(reader.Value);
                    //get damage
                    reader.MoveToNextAttribute();
                    damage = int.Parse(reader.Value);
                    //load prefab
                    reader.MoveToNextAttribute();
                    if (reader.Value != "")
                        prefab = (Resources.Load(reader.Value, typeof(GameObject)) as GameObject);
                    else
                        prefab = null;
                    reader.MoveToContent();
                    //get class
                    actionType = reader.ReadElementContentAsString();
                    //get description
                    reader.ReadToFollowing("description");
                    description = reader.ReadElementContentAsString();

                    cards.Add(new Card(name, hitbox, element, type, range, damage, actionType, prefab, description, image));
                }
                reader.Close();
                return new BoosterPack(cards, packSize, packImage);
            }
        }
    }
}
