using UnityEngine;


public class Laser : MonoBehaviour {

	public float laserAmt, warningAmt, cooldownAmt, initDelay;
    GameObject laser, laserWarning;
	protected string str = "Player";

	int laserState = 3;
    float laserTime;
    float warningTime;
    float cooldownTime;
	
	void Awake() {
        laserTime = laserAmt;
        warningTime = warningAmt;
        cooldownTime = cooldownAmt;

		GetComponent<Collider>().enabled = false;
		laser = transform.GetChild(0).gameObject;
		laserWarning = transform.GetChild(1).gameObject;
        SwitchOff();
    }

	void Update () {
		switch(laserState) {
			case 0:
                if (cooldownTime > 0)
                {
                    SwitchOff();
                    cooldownTime -= Time.deltaTime;
                }
                else
                {
                    cooldownTime = cooldownAmt;
                    laserState = 1;
                }
				break;
			case 1:
                if (warningTime >= 0)
                {
                    SwitchOnLaser(false);
                    warningTime -= Time.deltaTime;
                }
                else
                {
                    warningTime = warningAmt;
                    laserState = 2;
                }
				break;
			case 2:
                if (laserTime > 0)
                {
                    SwitchOnLaser(true);
                    laserTime -= Time.deltaTime;
                }
                else
                {
                    laserTime = laserAmt;
                    laserState = 0;
                }
				break;
			case 3:
                if (initDelay > 0)
                {
                    initDelay -= Time.deltaTime;
                }
                else
                {
                    laserState = 0;
                }
				break;
		}

		
	}

	private void SwitchOnLaser(bool on) {
		laser.SetActive(on);
		laserWarning.SetActive(!on);
		GetComponent<Collider>().enabled = on;
	}

    private void SwitchOff()
    {
        laser.SetActive(false);
        laserWarning.SetActive(false);
        GetComponent<Collider>().enabled = false;
    }

	private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(str))
        {
            other.GetComponent<CargoHealth>().loseCargo();
        }
    }
}
