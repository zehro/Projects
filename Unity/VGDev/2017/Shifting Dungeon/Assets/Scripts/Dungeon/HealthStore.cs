﻿namespace ShiftingDungeon.Dungeon
{
    using UnityEngine;
    using UnityEngine.UI;
    using Character.Hero;

    public class HealthStore : MonoBehaviour
    {
        [SerializeField]
        private GameObject storeItem;
        [SerializeField]
        private int baseCost;
        [SerializeField]
        private Text healthLabel;
        [SerializeField]
        private Text shopkeeper;
        [SerializeField]
        private Util.SoundPlayer sfx;
        
        private void Start()
        {
            this.storeItem.SetActive(true);
            this.healthLabel.gameObject.SetActive(true);
            this.healthLabel.text = this.baseCost + "";
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == Util.Enums.Tags.Hero.ToString() && this.storeItem.activeSelf)
            {
                this.shopkeeper.text = "Press Attack to buy this health refill!";
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == Util.Enums.Tags.Hero.ToString() && this.storeItem.activeSelf)
            {
                if (Util.CustomInput.BoolFreshPress(Util.CustomInput.UserInput.Attack))
                {
                    this.sfx.PlaySong(0);
                    int cost = this.baseCost;
                    if (cost <= HeroData.Instance.money)
                    {
                        HeroData.Instance.money -= cost;
                        Managers.DungeonManager.GetHero().GetComponent<HeroBehavior>().AddHealth(2);
                        this.shopkeeper.text = "Thank you!";
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == Util.Enums.Tags.Hero.ToString() && this.storeItem.activeSelf)
            {
                this.shopkeeper.text = "Please buy something!";
            }
        }
    }
}
