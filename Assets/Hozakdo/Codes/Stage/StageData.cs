using UnityEngine;

public class StageData : MonoBehaviour
{
    [Header("Index")]
    public Vector2Int index;

    [Header("Datas")]
    public Vector2Int centerCoord;
    public Vector2Int minCoord;
    public Vector2Int maxCoord;

    [Header("Settings")]
    public bool IsVisited = false;

    [Header("References")]
    public GameObject fogBlack;
    public GameObject fogWhite;

    public void SetFog(bool isCurStage)
    {
        if (isCurStage)
        {
            fogBlack.SetActive(false);
            fogWhite.SetActive(false);
        }
        else
        {
            if (!IsVisited)
            {
                fogBlack.SetActive(true);
                fogWhite.SetActive(false);
            }
            else
            {
                fogBlack.SetActive(false);
                fogWhite.SetActive(true);
            }
        }
    }
}
