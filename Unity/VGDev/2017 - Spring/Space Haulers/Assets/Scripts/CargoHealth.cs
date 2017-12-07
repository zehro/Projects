using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CargoHealth : MonoBehaviour {

    //Might move this
    public Text cargoText;

    int cargo = 3;
    bool diedOnce;

    private void Awake()
    {
        cargoText.text = string.Concat("x", cargo);
    }

    // Update is called once per frame
    void Update () {
        if (cargo <= 0 && !diedOnce)
        {
            LevelManager.instance.gameOver();
            diedOnce = true;
        }
    }

    public void loseCargo()
    {
        if (cargo > 0)
        {
            cargo--;
            cargoText.text = string.Concat("x", cargo);
        }
    }

    public void gainCargo()
    {
        if (cargo > 0)
        {
            cargo++;
            cargoText.text = string.Concat("x", cargo);
        }
    }

    public int getCargoAmt() {
        return cargo;
    }
}
