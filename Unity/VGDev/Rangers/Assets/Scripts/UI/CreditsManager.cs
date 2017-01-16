using UnityEngine;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
    public class CreditsManager : MonoBehaviour
    {
        private string creditsDataPath = "Files/credits";
        private TextAsset xmlFile;
        private List<string> displayText;
        private int counter = 0;

        private float timer, loadnewTime = 0.75f, destroyTime = 5f;

        [SerializeField]
        private GameObject arrow, spawnPoint, textObject;

        void Start()
        {
            displayText = new List<string>();

            xmlFile = (TextAsset)Resources.Load(creditsDataPath);
            if (xmlFile != null)
            {
                MemoryStream assetStream = new MemoryStream(xmlFile.bytes);
                XmlReader reader = XmlReader.Create(assetStream);

                bool endofFile = false;
                while (reader.Read() && !endofFile)
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.LocalName)
                        {
                            case "Finish":
                                endofFile = true;
                                break;
                            case "Space":
                                displayText.Add("");
                                break;
                            case "Credits":
                                displayText.Add("<size=30>Credits</size>");
                                break;
                            default:
                                displayText.Add(reader.ReadString());
                                break;
                        }
                    }
                }
                reader.Close();
            }
        }

        void Update()
        {
            timer += Time.deltaTime;
            if(timer > loadnewTime)
            {
                timer = 0;
                GameObject newText = Instantiate(textObject);
                TextMesh t = newText.GetComponentInChildren<TextMesh>();
                t.text = displayText[counter++];
                t.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                if (counter >= displayText.Count) counter = 0;
                newText.transform.parent = arrow.transform;
                newText.transform.position = spawnPoint.transform.position;
                newText.transform.localScale = new Vector3(1, 1, 1);
                Destroy(newText, destroyTime);
            }

            if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.A, PlayerID.One)) Navigator.CallSubmit();
        }

        public void GoToMain()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    } 
}
