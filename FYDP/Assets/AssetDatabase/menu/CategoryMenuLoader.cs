﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryMenuLoader : MonoBehaviour
{

    public GameObject spawner;

    public List<GameObject> furniturePage;

    public GameObject forward;
    public GameObject backward;
    public int pageSize;

    public GameObject titleLabel;

    private List<GameObject> furnitures;

    private Dictionary<string, int> pageHistory = new Dictionary<string, int>();
    private string currentCategory;

    public void LoadList(string category)
    {
        SpawnList spawnList = spawner.GetComponent<SpawnList>();

        currentCategory = category;
        // set title
        titleLabel.GetComponent<Text>().text = spawnList.getCategoryTitle(currentCategory);

        furnitures = spawnList.findBundle(category);

        int currentIndex = 0;
        if (pageHistory.ContainsKey(category))
        {
            currentIndex = pageHistory[category];
        }
        else
        {
            pageHistory[category] = 0;
        }

        int i = currentIndex * pageSize;

        foreach (GameObject furnitureBtn in furniturePage)
        {
            if (i < Math.Min(furnitures.Count, (currentIndex + 1) * pageSize))
            {
                furnitureBtn.SetActive(true);
                GameObject f = furnitures[i];
                FurnitureProperty properties = spawnList.findAssetProperties(category, f.name);
                furnitureBtn.GetComponent<FurnitureMenuItemProperty>().furnitureProperty = properties;
                furnitureBtn.GetComponent<FurnitureMenuItemProperty>().index = i;
                if (!category.Contains("paint"))
                {
                    furnitureBtn.GetComponent<Image>().sprite = spawnList.findAssetIcon(category, i);
                    furnitureBtn.GetComponent<Image>().material = null;
                }
                else
                {
                    furnitureBtn.GetComponent<Image>().sprite = null;
                    furnitureBtn.GetComponent<Image>().material = spawnList.findPaintIcon(i);
                }
                i++;
            }
            else
            {
                furnitureBtn.SetActive(false);
            }
        }

        if (furnitures.Count <= pageSize)
        {
            forward.SetActive(false);
            backward.SetActive(false);
        }
        else if (currentIndex == 0)
        {
            forward.SetActive(true);
            backward.SetActive(false);
        }
        else if ((furnitures.Count - currentIndex * pageSize) <= pageSize)
        {
            forward.SetActive(false);
            backward.SetActive(true);
        }
        else
        {
            backward.SetActive(true);
            forward.SetActive(true);
        }
    }

    public void nextPage()
    {
        pageHistory[currentCategory] = Math.Min(pageHistory[currentCategory] + 1, furnitures.Count / pageSize);
        LoadList(currentCategory);
    }

    public void prevPage()
    {
        pageHistory[currentCategory] = Math.Max(pageHistory[currentCategory] - 1, 0);
        LoadList(currentCategory);
    }
}
