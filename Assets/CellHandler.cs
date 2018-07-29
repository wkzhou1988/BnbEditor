using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellHandler : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler, IPointerUpHandler, IPointerDownHandler{

    public Manager manager;

    Image image;
    Text text;

    public int Row;
    public int Col;

	// Use this for initialization
	void Start () {
        image = gameObject.transform.Find("Image").GetComponent<Image>();
        text = gameObject.transform.Find("Text").GetComponent<Text>();
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
        //Debug.Log("OnPointerEnter " + Row + "   " + Col);
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
        
    }
}
