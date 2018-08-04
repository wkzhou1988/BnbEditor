using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CellHandler : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler, IPointerUpHandler, IPointerDownHandler{

    public Manager manager;

    Image image;
    Text text;

    public int Row;
    public int Col;

    public static SpriteAtlas sprites;

	// Use this for initialization
	void OnEnable () {
        image = gameObject.transform.Find("Image").GetComponent<Image>();
        text = gameObject.transform.Find("Text").GetComponent<Text>();
        if (sprites == null)
        {
            sprites = Resources.Load<SpriteAtlas>("Icons");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick " + Row + "   " + Col);
        manager.ShowPanel(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit " + Row + "   " + Col);

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter " + Row + "   " + Col);
        manager.OnEnterCell(Row, Col);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown " + Row + "   " + Col);
        manager.TryStartDrag(Row, Col);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp " + Row + "   " + Col);
        manager.TryEndDrag();
    }

    public void SetValue(int value)
    {
        text.text = value.ToString();
    }

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void SetSprite(int value)
    {
        if (value == 0)
        {
            image.sprite = null;
            return;
        }
        
        var imageName = string.Format("game_img_block{0}_1", value);
        var sprite = sprites.GetSprite(imageName);
        Debug.Assert(sprite != null, "Sprite is null");
        image.sprite = sprite;
    }
}
