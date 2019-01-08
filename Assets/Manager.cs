using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

public class Config
{
    public class Layer
    {
        public string name;
        public List<List<int>> data = new List<List<int>>();
        public void AddRowData(string data)
        {
            if (data.Length == 0)
                return;
            
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

    public Config()
    {
        
    }

    public Config(int row, int col)
    {
        RowCount = row;
        ColCount = col;
        var layer1 = new Layer();
        var layer2 = new Layer();
        for (int r = 0; r < row; r++)
        {
            var rowData1 = new List<int>();
            var rowData2 = new List<int>();
            for (int c = 0; c < col; c++)
            {
                rowData1.Add(0);
                rowData2.Add(0);
            }
            layer1.data.Add(rowData1);
            layer2.data.Add(rowData2);
        }
        layers.Add(layer1);
        layers.Add(layer2);
    }
 
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
    public Button LoadButton;

    GameObject cell;
    GameObject text;
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
        LoadButton.onClick.AddListener(Load);
        RowInput.text = "0";
        ColInput.text = "0";
        cell = Resources.Load("Cell") as GameObject;
        text = Resources.Load("Text") as GameObject;
        Root = GameObject.Find("Root");
        if (Root == null)
        {
            Root = new GameObject("Root");
        }
        Panel.manager = this;
        mode = copySpriteMode;
	}

    void Load()
    {
        //var path = UnityEditor.EditorUtility.OpenFilePanel("", "D:/BnbLevels", "txt");
        //Debug.Log(path);
        //if (path.Length == 0)
        //{
        //    Debug.Log("nothing seletect");
        //    return;
        //}
        var path = "D:/BnBlevel/duqu.txt";
        var contents = File.ReadAllText(path);
        config = Config.Deserialize(contents);
		
		ClearAll();
        DrawDrid(config);
        InitSelectItems();
    }
	
    void Export()
    {
        if (config != null)
        {
            //var path = UnityEditor.EditorUtility.SaveFilePanel("Test.txt", "D:/BnbLevels", "Level", "txt");
            var path = "D:/BnBlevel/Daochu.txt";
            File.WriteAllText(path, config.Serialize());
        }
    }

    void OnButtonClick()
    {
        //        var config = Config.Deserialize(@"[layer]
        //type=Tile Layer 1
        //data=
        //0,1,1,1,1,1,1,1,1,1,0,
        //0,1,0,0,0,0,0,0,0,1,0,
        //0,1,0,0,0,0,0,0,0,1,0,
        //0,1,1,1,1,1,1,1,1,1,0,
        //0,0,0,0,0,0,0,0,0,0,0,
        //0,0,0,0,0,0,0,0,0,0,0,
        //0,0,0,0,0,0,0,0,0,0,0,
        //0,0,0,0,0,0,0,0,0,0,0,
        //0,0,0,0,0,0,0,0,0,0,0,
        //0,0,0,0,0,0,0,0,0,0,0,
        //0,0,0,0,0,0,0,0,0,0,0.
        //[layer]
        //type=Tile Layer 2
        //data=
        //0,9,9,9,9,9,9,9,9,9,0,
        //0,11,0,0,0,0,0,0,0,11,0,
        //0,11,0,0,0,0,0,0,0,11,0,
        //0,9,9,9,9,9,9,9,9,9,0,
        //0,0,0,0,0,0,0,0,0,0,0,
        //0,0,0,0,0,0,0,0,0,0,0,
        //0,0,0,0,0,0,0,0,0,0,0,
        //0,0,0,0,0,0,0,0,0,0,0,
        //0,0,0,0,0,0,0,0,0,0,0,
        //0,0,0,0,0,0,0,0,0,0,0,
        //0,0,0,0,0,0,0,0,0,0,0.");
        if (this.config == null)
        {
        	int row = 0;
	        int col = 0;
	        int.TryParse(RowInput.text, out row);
	        int.TryParse(ColInput.text, out col);
	        var _config = new Config(row, col);
	        this.config = _config;
	        DrawDrid(this.config);

	        InitSelectItems();
        }
        else //已有config,在上面加一行/列。
        {
        	int row = 0;
	        int.TryParse(RowInput.text, out row);
	        
	        //加新的
	        for (int r = this.config.RowCount; r < row; r++)
	        {
	            var rowData1 = new List<int>();
	            var rowData2 = new List<int>();
	            for (int c = 0; c < this.config.ColCount; c++)
	            {
	                rowData1.Add(0);
	                rowData2.Add(0);
	            }
	            this.config.layers[0].data.Add(rowData1);
	            this.config.layers[1].data.Add(rowData2);
	        }

	        //移动一下
	        int delta = row - this.config.RowCount;
	        for (int r = row-1; r >=0 ; r--)
	        {
	        	if(r<delta){
	        		Debug.Log("empty");
	        		var rowData1 = new List<int>();
	            	var rowData2 = new List<int>();
		           	for (int c = 0; c < this.config.ColCount; c++)
		            {
		                rowData1.Add(0);
		                rowData2.Add(0);
		            }
	            	this.config.layers[0].data[r] = rowData1;
            		this.config.layers[1].data[r] = rowData2;
	        	}
	        	else
	        	{
	        		Debug.Log("copy from " + (r-delta) + " to " + r);
	        		this.config.layers[0].data[r] = this.config.layers[0].data[r-delta]; 
	        		this.config.layers[1].data[r] = this.config.layers[1].data[r-delta];
	        	}
	        }

	        this.config.RowCount = row;
        	ClearAll();

	        DrawDrid(this.config);

	        InitSelectItems();
        }

    }

    void DrawCell(int row, int col, int layer1, int layer2)
    {
        var go = Instantiate<GameObject>(cell);
        go.transform.position = new Vector2(col, row * -1) * 100;
        go.transform.SetParent(Root.transform, false);
        go.name = string.Format("cell{0},{1}", row, col);
        go.transform.Find("Text").GetComponent<Text>().text = layer2.ToString();
        go.GetComponent<CellHandler>().manager = this;

        var cellHandler = go.GetComponent<CellHandler>();
        cellHandler.Row = row;
        cellHandler.Col = col;
        cellMap[row, col] = cellHandler;

        UpdateCell(row, col);

    }

    void ClearAll()
    {
    	//Debug.Log(Root);
    	foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "number"))
    	{
    		Destroy(go);
    	}
    	foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.Contains("cell"))){
    		Destroy(go);
    	}
    }

    void DrawNumber(int row, int col, int number)
    {
        var go = Instantiate<GameObject>(text);
        go.transform.position = new Vector2(col, row * -1) * 100;
        go.transform.SetParent(Root.transform, false);
        go.name = "number";
        go.GetComponent<Text>().text = number.ToString();
        go.GetComponent<Text>().color = Color.black;
    }

    void DrawDrid(Config config)
    {
        var RowCount = config.RowCount;
        var ColCount = config.ColCount;


        cellMap = new CellHandler[RowCount, ColCount];

        for (int row = 0; row < RowCount; row++)
        {
            DrawNumber(row, -1, RowCount - row);

            for (int col = 0; col < ColCount; col++)
            {
                DrawCell(row, col, config.layers[0].data[row][col], config.layers[1].data[row][col]);
            }
        }

        for (int col = 0; col < ColCount; col++)
        {
            DrawNumber(RowCount, col, col + 1);
        }

        float width = 1100;
        float height = 600;

        float totalWidth = ColCount * 100;
        float totalHeight = RowCount * 100;

        var scale = Mathf.Min(width / totalWidth, height / totalHeight);
        scale = Mathf.Min(1.0f, scale);
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
        //Debug.Log("TryStartDrag " + row + " " + col);
        startDragRow = row;
        startDragCol = col;
    }

    void CopyValueModeEnterCell(int row, int col)
    {
        Debug.Log("CopyModeEnterCell");
        //var source1 = config.layers[0].data[startDragRow][startDragCol];
        var source2 = config.layers[1].data[startDragRow][startDragCol];

        //config.layers[0].data[row][col] = source1;
        config.layers[1].data[row][col] = source2;

        UpdateCell(row, col);
    }

    void CopySpriteModeEnterCell(int row, int col)
    {
        Debug.Log("BrushModeEnterCell");
        config.layers[0].data[row][col] = seletectedId;
        UpdateCell(row, col);
    }

    public void OnEnterCell(int row, int col)
    {

        if (startDragRow == -1 || startDragCol == -1)
        {
            return;
        }

        if (IsCopyValueMode())
        {
            CopyValueModeEnterCell(row, col);
        }
        else
        {
            CopySpriteModeEnterCell(row, col);
        }
    }

    void UpdateCell(int row, int col)
    {
        var source1 = config.layers[0].data[row][col];
        var source2 = config.layers[1].data[row][col];

        cellMap[row, col].SetSprite(source1);
        cellMap[row, col].SetValue(source2);
    }

    public void TryEndDrag()
    {
        startDragRow = -1;
        startDragCol = -1;
    }

    int seletectedId = 0;
    public void SetCurrentSelectedId(int id)
    {
        seletectedId = id;   
    }

    void InitSelectItems()
    {
        var prefab = Resources.Load<GameObject>("ChooseItem");
        var content = GameObject.Find("Canvas/Scroll View/Viewport/Content");

        const int MAX_ITEMS = 61;

        for (int i = 0; i <= MAX_ITEMS; i++)
        {
            var go = Instantiate(prefab);
            var handler = go.GetComponent<ChooseItemHandler>();
            handler.id = i;
            handler.manager = this;

            go.transform.SetParent(content.transform, false);
        }
    }

    public int mode = 1;
    const int copySpriteMode = 1;
    const int copyValueMode = 0;

    public bool IsCopySpriteMode()
    {
        return mode == copySpriteMode;
    }

    public bool IsCopyValueMode()
    {
        return mode == copyValueMode;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            mode = copyValueMode;
            Debug.Log("111");
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Debug.Log("222");
            mode = copySpriteMode;
        }
    }
}
