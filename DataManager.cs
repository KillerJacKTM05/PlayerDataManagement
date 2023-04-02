using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SafeZone
{
    public class Data
    {
        public string name = "p";
        public List<string> chosenObjectives = new List<string>();
        public List<string> completionTimes = new List<string>();
        public List<bool> urgency = new List<bool>();
        public List<string> reachedNpc = new List<string>();

    }

    //used for storing the player robot's world position, rotation data in time axis.
    public class PlayerWorldData
    {
        public List<string> positionRotationVelocity = new List<string>();
        public string name = "p";

        public PlayerWorldData()
        {
            name = "Player";
        }

        public PlayerWorldData(List<string> keys)
        {
            this.positionRotationVelocity = keys;
        }
    }
    public class ContactInfo
    {
        public string contactName = "npc";
        public string contactId = "000";
        public float contactDistance = 0f;
        public float npcYRot = 0f;
        public float npcVel = 0f;
        public string contactBehavior = "null";
        public Vector3 contactNpcPosition = new Vector3(0, 0, 0);
        public string contactTime ="24:00";
        public string robotAbusement = "no";
        public string robotData = "null";
    }
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance { get; private set; }
        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }

        }

        private string contactCreationPathName = "";
        private string contactFilepath = "";
        private string taskCreationPathName = "";
        private string taskFilepath = "";
        private string positionCreationPathName = "";
        private string positionFilepath = "";
        Data playerData = new Data()
        {
            name = "Player",
        };
        PlayerWorldData pWorldData = new PlayerWorldData()
        {
            name = "Player",
        };
        void Start()
        {
            contactCreationPathName = "PlayerContacts.json";
            taskCreationPathName = "PlayerTasks.json";
            positionCreationPathName = "PlayerWorldPos.json";
            contactFilepath = Application.dataPath + "/";
            taskFilepath = Application.dataPath + "/";
            positionFilepath = Application.dataPath + "/";
            CreateDataSaveFile();
        }

        private void CreateDataSaveFile()
        {
            contactFilepath = contactFilepath + contactCreationPathName;
            taskFilepath = taskFilepath + taskCreationPathName;
            positionFilepath = positionFilepath + positionCreationPathName;
            if (System.IO.File.Exists(contactFilepath))
            {
                Debug.Log("PlayerContacts.json exists.");
            }
            else
            {
                Debug.Log("File created.");
                System.IO.File.Create(contactFilepath);
            }
            if (System.IO.File.Exists(taskFilepath))
            {
                Debug.Log("PlayerTasks.json exists.");
            }
            else
            {
                System.IO.File.Create(taskFilepath);
            }
            if (System.IO.File.Exists(positionFilepath))
            {
                Debug.Log("PlayerWorldPos.json exists");
            }
            else
            {
                System.IO.File.Create(positionFilepath);
            }
        }
        public void SetPlayer(string Name)
        {
            playerData.name = Name;
            pWorldData.name = Name;
        }
        public void UpdatePlayerData(string objective = null, string time = null, bool urgent = false)
        {
            if(objective != null)
            {
                playerData.chosenObjectives.Add(objective);
            }
            else
            {
                playerData.chosenObjectives.Add("No task.");
            }
            if(time != null)
            {
                playerData.completionTimes.Add(time);
            }
            else
            {
                playerData.completionTimes.Add("00:00");
            }
            playerData.urgency.Add(urgent);
        }
        public void UpdateCloseContact(ContactInfo cont)
        {
            string positionVector = cont.contactNpcPosition + ",";
            string message = cont.contactId+","+cont.contactName+","+positionVector+FloatFlooring(cont.npcYRot,2)+","+FloatFlooring(cont.npcVel,2)+","+cont.contactBehavior+","+cont.robotAbusement+","+cont.robotData+","+FloatFlooring(cont.contactDistance,2)+","+cont.contactTime;
            if (playerData != null)
            {
                playerData.reachedNpc.Add(message);
            }
        }
        public void UpdatePlayerWorldData(Vector3 position, Vector3 rotation, float velocity = 0)
        {
            string time = InterfaceManager.Instance.InGameUI.GetHourString();
            string message = position + "," + FloatFlooring(rotation.y,2) + "," + FloatFlooring(velocity, 2) + "," + time;
            //Debug.Log(pWorldData);
            //Debug.Log(message);
            if(pWorldData != null)
            {
                pWorldData.positionRotationVelocity.Add(message);
            }
        }
        public void WriteToFile()
        {
            try
            {
                //first, insert player task data to PlayerTasks json file.
                List<string> prepared = new List<string>();
                prepared.Add("User,"+ playerData.name);
                prepared.Add("Number," + "Quest," + "Urgency," + "Time");
                for (int i = 0; i < playerData.chosenObjectives.Count; i++)
                {
                    prepared.Add(i + "," + playerData.chosenObjectives[i] + "," + playerData.urgency[i] + "," + playerData.completionTimes[i]);
                }
                string[] preparedString = new string[prepared.Count];
                for (int i = 0; i < prepared.Count; i++)
                {
                    preparedString[i] = prepared[i];
                }
                File.WriteAllLines(taskFilepath, preparedString);

                //after that write the data to second json.
                prepared.Clear();
                prepared.Add("ID,"+"NPCName,"+"PosX,"+"PosY,"+"PosZ,"+"RotY,"+"Velocity,"+"Emotion,"+"Abusement,"+"RPosX,"+"RPosY,"+"RPosZ,"+"RRotY,"+"Distance,"+"Time");
                for (int j = 0; j < playerData.reachedNpc.Count; j++)
                {
                    prepared.Add(playerData.reachedNpc[j]);
                }
                preparedString = new string[prepared.Count];
                for(int i = 0; i < prepared.Count; i++)
                {
                    preparedString[i] = prepared[i];
                }
                File.WriteAllLines(contactFilepath, preparedString);

                //finally, we needed to write 3rd file with robot's world position-rotation-vel data.
                prepared.Clear();
                prepared.Add("RPosX," + "RPosY," + "RPosZ," + "RRotY," + "Velocity," + "Time");
                for(int i = 0; i < pWorldData.positionRotationVelocity.Count; i++)
                {
                    prepared.Add(pWorldData.positionRotationVelocity[i]);
                }
                preparedString = new string[prepared.Count];
                for (int k = 0; k < prepared.Count; k++)
                {
                    preparedString[k] = prepared[k];
                }
                File.WriteAllLines(positionFilepath, preparedString);
                //After the overwriting the files, we need to change their names with more suitable ones.
                string newContactPath = Application.dataPath + "/" + playerData.name + "_" + GameManager.Instance.GetActiveLevelName() + "_Contacts" + ".json";
                string newTaskPath = Application.dataPath + "/" + playerData.name + "_" + GameManager.Instance.GetActiveLevelName() + "_Tasks" + ".json";
                string newPosPath = Application.dataPath + "/" + playerData.name + "_" + GameManager.Instance.GetActiveLevelName() + "_PosRotTime" + ".json";
                File.Move(contactFilepath, newContactPath);
                File.Move(taskFilepath, newTaskPath);
                File.Move(positionFilepath, newPosPath);
                Debug.Log("File write success.");
            }
            catch
            {
                Debug.Log("File write failed.");
            }
        }
        public void DeletePlayerFile()
        {
            try
            {
                System.IO.File.Delete(contactFilepath);
                System.IO.File.Delete(taskFilepath);
                System.IO.File.Delete(positionFilepath);
                Debug.Log("Files deleted.");
            }
            catch
            {
                Debug.Log("Files couldn't deleted or file couldn't found.");
            }
        }

        [Tooltip("float parameter takes the float number that you wanted to fix it's notation. Digit parameter takes the number of digit after the decimal (.xx)")]
        public string FloatFlooring(float firstNumber, int digit = 1)
        {
            float floored = Mathf.Floor(firstNumber);
            float fracture = firstNumber - floored;
            for(int i = 0; i < digit; i++)
            {
                fracture *= 10;
            }
            return floored + "." + (int)fracture;
        }
    }
}
