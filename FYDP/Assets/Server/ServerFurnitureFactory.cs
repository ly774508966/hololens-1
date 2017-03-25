using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// using MySql.Data.MySqlClient;
// using System.Data.Common;

public class ServerFurnitureFactory : MonoBehaviour
{

    public static ServerFurnitureFactory Instance { get; private set; }

    public GameObject furniture;
    Dictionary<string, GameObject> avaliableObjects = new Dictionary<string, GameObject>();
    List<Object> furnitures = new List<Object>();

    public string basicTable = "BasicTable", woodenChair = "WoodenChair", modernSofa = "ModernSofa", woodenChair2 = "WoodenChair2",
        deskLamp = "DeskLamp", roundLamp = "RoundLamp", woodenTable = "WoodenTable", r2d2 = "R2D2", basicWoodTable = "BasicWoodTable",
        wallLight = "light_wall", ceilingLight = "light_ceiling";
    List<string> furnitureNames;
    AssetManager assetManager;

    public void CreateFurniture(string furniture_name, Vector3 position = default(Vector3), Quaternion angle = default(Quaternion))
    {
        Debug.Log("BARFOO HELP");
        dbread();
        Debug.Log("BARFOO DONE " + dbread().Current);

        if (FurnitureConstants.FETCH_MODELS && FurnitureConstants.FURNITURE_MAP.ContainsKey(furniture_name))
        {
            if (assetManager == null)
                assetManager = gameObject.AddComponent<AssetManager>() as AssetManager;
            FurnitureMetadata metadata = FurnitureConstants.FURNITURE_MAP[furniture_name];
            assetManager.InstantiateGameObject(metadata.bundle, metadata.name);
        }
        else
        {
            Vector3 infront;
            if (position == default(Vector3))
            {
                infront = new Vector3(0.5f, -0.5f, 2.0f);
                position = Camera.main.ViewportToWorldPoint(infront);
            }
            if (angle == default(Quaternion))
            {
                angle = Quaternion.identity;
            }
            furniture = avaliableObjects[furniture_name];
            GameObject temp = (GameObject)Instantiate(furniture, position, angle);
            temp.SetActive(true);
            furnitures.Add(temp);
        }
        
    }

    public IEnumerator<string> dbread()
    {
        Debug.Log("EMPTY");
        string[] items;
        WWW itemsData = new WWW("http://monisk.heliohost.org/dbread.php");
        // yield return itemsData;
        string itemsDataString = itemsData.text;
        Debug.Log("FULL " + itemsDataString);
        yield return itemsDataString;
        print(itemsDataString);
        items = itemsDataString.Split(';');
        Debug.Log("FOOBAR " + itemsDataString);
    }

    public void furnitureCreation(string name)
    {
        CreateFurniture(name);
    }

    public void ResetAllFocus()
    {
        foreach (GameObject f in furnitures)
        {
            f.SendMessageUpwards("OnRotateStop");
        }
    }

    void ClearAll()
    {
        if (assetManager && FurnitureConstants.FETCH_MODELS)
        {
            assetManager.clearFurnitureAssets();
        }

        foreach (GameObject f in furnitures)
        {
            f.SetActive(false);
            Destroy(f);
        }
        furnitures.Clear();
    }

    public void DeleteFurniture(GameObject obj)
    {
        furnitures.Remove(obj);
        Destroy(obj);
    }

    void Start()
    {
        furnitureNames = new List<string>() { basicTable, woodenChair, modernSofa, woodenChair2, deskLamp, roundLamp, woodenTable, r2d2, basicWoodTable, wallLight, ceilingLight };
        foreach(var name in furnitureNames)
        {
            furniture = GameObject.Find(name);
            avaliableObjects[name] = furniture;
            furniture.SetActive(false);
        }
        Instance = this;
        WWW itemsData = new WWW("http://monisk.heliohost.org/dbread.php");
        StartCoroutine(WaitForRequest(itemsData));
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
