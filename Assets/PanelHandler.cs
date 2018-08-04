using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PanelHandler : MonoBehaviour {

    InputField input;
    public CellHandler clickedCell;
    public Manager manager;

	// Use this for initialization
	void Start () {
        input = transform.Find("InputField").GetComponent<InputField>();
        Hide();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (clickedCell != null)
            {
                int value = 0;
                int.TryParse(input.text, out value);
                manager.UpdateCellValue(clickedCell.Row, clickedCell.Col, value);

            }
            Hide();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }

	}

    public void Show(CellHandler clickedCell)
    {
        gameObject.SetActive(true);
        this.clickedCell = clickedCell;
        input.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
