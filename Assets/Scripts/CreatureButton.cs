using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureButton : MonoBehaviour
{
    public CreatureData data;
	public GameObject creaturesMainList;
	public GameObject fullSizeView;
	public TextMeshProUGUI creatureNameText;
	public Image creatureImage;
	[HideInInspector]
	public string creatureName;
	[HideInInspector]
	public Sprite image;

	private void Awake()
	{
		creatureName = data.creatureName;
		image = data.image;
		GetComponent<Button>().onClick.AddListener(() => Clicked());
	}

	void Clicked()
	{
		creaturesMainList.SetActive(false);
		creatureNameText.text = creatureName;
		creatureImage.sprite = image;
		fullSizeView.SetActive(true);
	}
}
