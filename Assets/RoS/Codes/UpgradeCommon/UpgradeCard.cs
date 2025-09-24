using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    public UpgradeData upgradeData;
    public BasicAttackController basicAttackController;

    Image imageIcon;
    Text textLevel;
    Text textName;
    Text textDesc;
    public Image[] grids;
    GameObject gridPanel;

    private void Awake()
    {
        imageIcon = GetComponentsInChildren<Image>()[1];

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];

        Transform gridPanelTransform = transform.Find("Panel");

        if (gridPanelTransform != null)
        {
            gridPanel = gridPanelTransform.gameObject;
            // gridPanel 안의 Image들을 찾을 때도 비활성화를 고려해 true를 넣어주는 것이 안전합니다.
            grids = gridPanel.GetComponentsInChildren<Image>(true);
        }
        else
        {
            // 만약 못 찾았다면, 어떤 오브젝트에서 문제가 발생했는지 정확한 에러 메시지를 출력합니다.
            Debug.LogError("'" + gameObject.name + "' 오브젝트의 자식 중 'Panel'을 찾을 수 없습니다! 하이어라키 구조와 이름을 확인해주세요.", this.gameObject);
        }
    }

    public void SetUp(UpgradeData data)
    {
        gridPanel.SetActive(false);
        upgradeData = data;

        int curLevel = data.GetLevel(basicAttackController);

        imageIcon.sprite = data.icon;
        textLevel.text = "Lv." + (curLevel + 1);
        textName.text = data.title;
        textDesc.text = data.GetDesc(curLevel);

        if(data is ShapeData)
        {
            gridPanel.SetActive(true);

            // 초기화
            foreach (Image gridCell in grids)
            {
                gridCell.color = Color.white;
            }

            // 기존 공격 범위 검은색 칠
            foreach (Vector2Int existingTile in basicAttackController.shapeTiles)
            {
                GridVisual(existingTile, Color.black);
            }

            // 대상 타일 칠하기
            GridVisual(((ShapeData)data).tileToAdd, new Color(0.251f, 0.878f, 0.816f));
        }
    }

    public void OnClick()
    {
        upgradeData.Apply(basicAttackController);
    }

    void GridVisual(Vector2Int gridPos, Color color)
    {
        if (gridPos == new Vector2Int(0, 2))
        {
            grids[1].color = color;
        }
        else if (gridPos == new Vector2Int(1, 2))
        {
            grids[2].color = color;
        }
        else if (gridPos == new Vector2Int(2, 2))
        {
            grids[3].color = color;
        }
        else if (gridPos == new Vector2Int(3, 2))
        {
            grids[4].color = color;
        }
        else if (gridPos == new Vector2Int(4, 2))
        {
            grids[5].color = color;
        }
        else if (gridPos == new Vector2Int(0, 1))
        {
            grids[6].color = color;
        }
        else if (gridPos == new Vector2Int(1, 1))
        {
            grids[7].color = color;
        }
        else if (gridPos == new Vector2Int(2, 1))
        {
            grids[8].color = color;
        }
        else if (gridPos == new Vector2Int(3, 1))
        {
            grids[9].color = color;
        }
        else if (gridPos == new Vector2Int(4, 1))
        {
            grids[10].color = color;
        }
        else if (gridPos == new Vector2Int(0, 0))
        {
            grids[11].color = color;
        }
        else if (gridPos == new Vector2Int(1, 0))
        {
            grids[13].color = color;
        }
        else if (gridPos == new Vector2Int(2, 0))
        {
            grids[14].color = color;
        }
        else if (gridPos == new Vector2Int(3, 0))
        {
            grids[15].color = color;
        }
        else if (gridPos == new Vector2Int(4, 0))
        {
            grids[16].color = color;
        }
        else if (gridPos == new Vector2Int(0, -1))
        {
            grids[17].color = color;
        }
        else if (gridPos == new Vector2Int(1, -1))
        {
            grids[18].color = color;
        }
        else if (gridPos == new Vector2Int(2, -1))
        {
            grids[19].color = color;
        }
        else if (gridPos == new Vector2Int(3, -1))
        {
            grids[20].color = color;
        }
        else if (gridPos == new Vector2Int(4, -1))
        {
            grids[21].color = color;
        }
        else if (gridPos == new Vector2Int(0, -2))
        {
            grids[22].color = color;
        }
        else if (gridPos == new Vector2Int(1, -2))
        {
            grids[23].color = color;
        }
        else if (gridPos == new Vector2Int(2, -2))
        {
            grids[24].color = color;
        }
        else if (gridPos == new Vector2Int(3, -2))
        {
            grids[25].color = color;
        }
        else
        {
            grids[26].color = color;
        }
    }
}
