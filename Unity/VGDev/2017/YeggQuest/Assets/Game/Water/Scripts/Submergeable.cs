using UnityEngine;

namespace YeggQuest.NS_Water
{
    // An object that can be submerged in water.

    public interface Submergeable
    {
        // Whether the object is in water.

        bool inWater
        {
            get;
            set;
        }
    }
}