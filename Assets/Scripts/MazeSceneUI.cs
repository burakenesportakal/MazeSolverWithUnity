using UnityEngine;
using TMPro;

public class MazeSceneUI : MonoBehaviour
{
    public TMP_Text algorithmText;
    public TMP_Text sizeText;

    void Start()
    {
        
        var solver = GameManager.Instance.selectedSolver;
        int w = GameManager.Instance.mazeWidth;
        int h = GameManager.Instance.mazeHeight;

        algorithmText.text = "Algorithm: " + solver.ToString();
        sizeText.text = "Size: " + w + "x" + h;
    }
}
