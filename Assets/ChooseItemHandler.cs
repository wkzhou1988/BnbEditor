using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ChooseItemHandler : MonoBehaviour {

    public Manager manager;
    public int id;


    Button button;
    Image image;
    public static SpriteAtlas sprites;

    private void OnEnable()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        image = GetComponent<Image>();

        if (sprites == null)
        {
            sprites = Resources.Load<SpriteAtlas>("Icons");
        }

    }

    void OnButtonClick()
    {
        manager.SetCurrentSelectedId(id);   
    }

    // Use this for initialization
    void Start () {
        image.sprite = sprites.GetSprite(string.Format("game_img_block{0}_1", id));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
