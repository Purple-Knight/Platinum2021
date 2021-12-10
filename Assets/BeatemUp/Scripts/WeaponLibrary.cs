using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLibrary : MonoBehaviour
{
    public static WeaponLibrary Instance { get { return _instance; } }
    private static WeaponLibrary _instance;

    private Dictionary<string, GameObject> WpLibrary;
    public List<GameObject> playersWeapons;

    [Header("   Fill Dictionnary")]
    public CharacterWeaponList[] characterWeaponLists;

    public Weapon GetFromLibrary(string key, Weapon instanceToReplace = null)
    {
        //Debug.Log("<color=yellow>" + key + "</color>");
        GameObject output;
        if(WpLibrary.TryGetValue(key, out output))
        {
            if(instanceToReplace != null)
            {
                GameObject replaceInList = playersWeapons.Find(x => x.GetComponent<Weapon>().CharacterID == instanceToReplace.CharacterID);
               // Debug.Log(replaceInList);
                if (replaceInList != null) playersWeapons.Remove(replaceInList);
                Destroy(replaceInList);
            }
            GameObject inst = Instantiate(output);
            playersWeapons.Add(inst);
            return inst.GetComponent<Weapon>();
        }
        else
        {
            Debug.Log("<color=red>No Weapon Value corresponding to key '</color>" + key + "<color=red>'</color>");
            return null;
        }
    }

    private void Awake()
    {
        if (Instance == null) _instance = this;

        // Fill Dictionnary
        WpLibrary = new Dictionary<string, GameObject>();
        for (int i = 0; i < characterWeaponLists.Length; i++)
        {
            CharacterWeaponList weaponList = characterWeaponLists[i];
            for (int j = 0; j < weaponList.weaponList.Count; j++)
            {
                string k = weaponList.characterKeyID.ToString() + weaponList.weaponList[j].GetComponent<Weapon>().weaponKey;
                WpLibrary.Add(k, weaponList.weaponList[j]);
            }
        }
    }

    [System.Serializable]
    public class CharacterWeaponList
    {
        public CharacterWeaponList(string name, int keyID)
        {
            characterName = name;
            characterKeyID = keyID;
        }

        [SerializeField] public string characterName;
        public int characterKeyID;
        public List<GameObject> weaponList = new List<GameObject>(4);
    }
}
