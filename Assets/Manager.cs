using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Config
{
    public class Layer
    {
        public string name;
        public List<List<int>> data = new List<List<int>>();
        public void AddRowData(string data)
        {
            var numbers = data.Substring(0, data.Length - 1).Split(',');
            var list = new List<int>();
            for (int i = 0; i < numbers.Length; i++)
            {
                list.Add(int.Parse(numbers[i]));
            }
            this.data.Add(list);
        }
    }

    public int RowCount = 0;
    public int ColCount = 0;
    public List<Layer> layers = new List<Layer>();
 
    public static Config Deserialize(string str)
    {
        var lines = Regex.Split(str, "\r\n|\r|\n");
        var config = new Config();
        Layer currentLayer = null;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line == "[layer]")
            {
                if (currentLayer != null)
                    config.layers.Add(currentLayer);
                
                currentLayer = new Layer();
            }
            else if (line.StartsWith("type=", StringComparison.CurrentCulture))
            {
                if (currentLayer != null)
                    currentLayer.name = line.Substring(5);
                else
                    Debug.LogError("111111");
            }
            else if (line == "data=")
            {
                
            }
            else 
            {
                if (currentLayer != null)
                {
                    currentLayer.AddRowData(line);
                }
            }
        }

        if (currentLayer != null)
            config.layers.Add(currentLayer);

        if (config.layers.Count == 0)
        {
            Debug.LogError("没有解析出任何layer");
        }

        if (config.layers[0].data.Count == 0)
        {
            Debug.LogError("没有解析出任何行数据");
        }

        if (config.layers[0].data[0].Count == 0)
        {
            Debug.LogError("没有解析出任何列数据");
        }

        config.RowCount = config.layers[0].data.Count;
        config.ColCount = config.layers[0].data[0].Count;

        return config;
    }

    public string Serialize()
    {
        string ret = "";
        string newline = "\r\n";

        for (int i = 0; i < layers.Count; i++)
        {
            ret += "[layer]" + newline;
            ret += "type=" + layers[i].name + newline;
            ret += "data=" + newline;
            for (int row = 0; row < layers[i].data.Count; row++)
            {
                string line = "";
                for (int col = 0; col < layers[i].data[row].Count; col++)
                {
                    if (row == layers[i].data.Count - 1 && col == layers[i].data[row].Count - 1)
                    {
                        line += layers[i].data[row][col].ToString() + "."; 
                    }
                    else
                    {
                        line += layers[i].data[row][col].ToString() + ","; 
                    }
                }
                line += newline;
                ret += line;
            }
        }

        return ret;
    }
}


public class Manager : MonoBehaviour {

    public InputField RowInput;
    public InputField ColInput;
    public Button ApplyButton;
    public Button ExportButton;

    GameObject cell;
    GameObject Root;
    public PanelHandler Panel;
    public InputField ValueInput;

    List<GameObject> todelete = new List<GameObject>();
    CellHandler[,] cellMap;

    Config config;


	// Use this for initialization
	void Start () {
        ApplyButton.onClick.AddListener(OnButtonClick);
        ExportButton.onClick.AddListener(Export);
        RowInput.text = "0";
        ColInput.text = "0";
        cell = Resources.Load("Cell") as GameObject;
        Root = GameObject.Find("Root");
        if (Root == null)
        {
            Root = new GameObject("Root");
        }
        Panel.manager = this;
	}
	
    void Export()
    {
        if (config != null)
        {
            Debug.Log(config.Serialize());
        }
    }

    void OnButtonClick()
    {
        var config = Config.Deserialize(@"[layer]
type=Tile Layer 1
data=
0,1,1,1,1,1,1,1,1,1,0,
0,1,0,0,0,0,0,0,0,1,0,
0,1,0,0,0,0,0,0,0,1,0,
0,1,1,1,1,1,1,1,1,1,0,
0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0.
[layer]
type=Tile Layer 2
data=
0,9,9,9,9,9,9,9,9,9,0,
0,11,0,0,0,0,0,0,0,11,0,
0,11,0,0,0,0,0,0,0,11,0,
0,9,9,9,9,9,9,9,9,9,0,
0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0.");

        DrawDrid(config);

        this.config = config;
    }

    void DrawCell(int row, int col, int layer1, int layer2)
    {
        var go = Instantiate<GameObject>(cell);
        go.transform.position = new Vector2(col, row * -1) * 100;
        go.transform.SetParent(Root.transform, false);
        go.name = string.Format("{0},{1}", row, col);
        go.transform.Find("Text").GetComponent<Text>().text = layer2.ToString();
        go.GetComponent<CellHandler>().manager = this;

        var cellHandler = go.GetComponent<CellHandler>();
        cellHandler.Row = row;
        cellHandler.Col = col;
        cellMap[row, col] = cellHandler;
    }


    void DrawDrid(Config config)
    {
        var RowCount = config.RowCount;
        var ColCount = config.ColCount;


        cellMap = new CellHandler[RowCount, ColCount];

        for (int row = 0; row < RowCount; row++)
        {
            for (int col = 0; col < ColCount; col++)
            {
                DrawCell(row, col, config.layers[0].data[row][col], config.layers[1].data[row][col]);
            }
        }

        float width = 1280;
        float height = 720;

        float totalWidth = ColCount * 100;
        float totalHeight = RowCount * 100;

        var scale = Mathf.Min(width/totalWidth, height/totalHeight);
        var x = (totalWidth * scale) / 2;
        var y = (totalHeight * scale) / 2 - 50;

        Root.transform.localScale = Vector3.one * scale;
        var trans = Root.GetComponent<RectTransform>();
        trans.localPosition = new Vector3(-x, y, 0);


    }


    public void ShowPanel(CellHandler clickedCell)
    {
        Panel.Show(clickedCell);
    }

    public void UpdateCellValue(int row, int col, int value)
    {
        var cellHandler = cellMap[row, col];
        cellHandler.SetValue(value);

        config.layers[1].data[row][col] = value;
    }

    int startDragRow = -1;
    int startDragCol = -1;

    public void TryStartDrag(int row, int col)
    {
        Debug.Log("TryStartDrag " + row + " " + col);
        startDragRow = row;
        startDragCol = col;
    }

    public void OnEnterCell(int row, int col)
    {
        if (row == startDragRow && col == startDragCol)
        {
            return;
        }

        if (startDragRow == -1 || startDragCol == -1)
        {
            return;
        }

        var source1 = config.layers[0].data[startDragRow][startDragCol];
        var source2 = config.layers[1].data[startDragRow][startDragCol];

        config.layers[0].data[row][col] = source1;
        config.layers[1].data[row][col] = source2;

        cellMap[row, col].SetSprite(source1);
        cellMap[row, col].SetValue(source2);
    }

    public void TryEndDrag()
    {
        startDragRow = -1;
        startDragCol = -1;
    }
}
