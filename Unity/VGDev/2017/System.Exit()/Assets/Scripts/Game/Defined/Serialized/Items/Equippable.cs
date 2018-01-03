﻿using Scripts.Game.Defined.Serialized.Buffs;
using Scripts.Game.Defined.Serialized.Spells;
using Scripts.Model.Buffs;
using Scripts.Model.Items;
using Scripts.Model.Stats;

namespace Scripts.Game.Defined.Serialized.Items {

    public class PoisonArmor : EquippableItem {

        public PoisonArmor() : base(EquipType.ARMOR.Sprite, EquipType.ARMOR, 10, "Poisoned Armor", "This doesn't look safe.") {
            AddFlatStatBonus(StatType.VITALITY, 3);
            AddFlatStatBonus(StatType.AGILITY, -1);
        }

        public override PermanentBuff CreateBuff() {
            return new Poison();
        }
    }

    public class Shield : EquippableItem {

        public Shield() : base(Util.GetSprite("round-shield"), EquipType.OFFHAND, 1, "Basic Shield ", "A basic wooden shield.") {
            AddFlatStatBonus(StatType.AGILITY, -10);
            AddFlatStatBonus(StatType.VITALITY, 10);
        }

        public override PermanentBuff CreateBuff() {
            return new DamageResist();
        }
    }
}

namespace Scripts.Game.Defined.Unserialized.Items {

    public class SingleStatTrinket : EquippableItem {

        public SingleStatTrinket(
            StatType stat,
            int price,
            int statIncreaseAmount,
            string name)
            : base(
                  Util.GetSprite("gem-pendant"),
                  EquipType.TRINKET,
                  price,
                  string.Format("Pendant of {0}", name),
                  string.Format("A magical pendant that boosts {0}.", stat)) {
            AddFlatStatBonus(stat, statIncreaseAmount);
        }
    }
}