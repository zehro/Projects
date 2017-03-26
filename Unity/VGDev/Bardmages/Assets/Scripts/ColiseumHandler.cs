using UnityEngine;

class ColiseumHandler : MonoBehaviour
{
    public ColiseumSpectacter[] spectators;

    private static bool reset;

    private void Start()
    {
        Assets.Scripts.Data.RoundHandler.Instance.RegisterReset(ResetStuff);
        reset = true;
    }

    private void Update()
    {
        if(reset)
        {
            foreach(ColiseumSpectacter c in spectators)
                c.Reset(0);
            reset = false;
        }
    }

    private static void ResetStuff(int stuff)
    {
        reset = true;
    }
}
