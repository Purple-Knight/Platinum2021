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
    public List<GameObject> valueList;

    public Weapon GetFromLibrary(string key, Weapon instanceToReplace = null)
    {
        GameObject output;
        if(WpLibrary.TryGetValue(key, out output))
        {
            if(instanceToReplace != null)
            {
                GameObject replaceInList = playersWeapons.Find(x => x.GetComponent<Weapon>().PlayerID == instanceToReplace.PlayerID);
                Debug.Log(replaceInList);
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

        WpLibrary = new Dictionary<string, GameObject>();
        for (int i = 0; i < valueList.Count; i++)
        {
            string k = valueList[i].GetComponent<Weapon>().weaponKey.ToString();
            WpLibrary.Add(k, valueList[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
