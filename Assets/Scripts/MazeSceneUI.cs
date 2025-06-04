using UnityEngine;
using TMPro;

public class MazeSceneUI : MonoBehaviour
{
    public TMP_Text algorithmText;
    public TMP_Text sizeText;

    void Start()
    {
        // GameManager'dan bilgileri al
        var solver = GameManager.Instance.selectedSolver;
        int w = GameManager.Instance.mazeWidth;
        int h = GameManager.Instance.mazeHeight;

        // Yazıya aktar
        algorithmText.text = "Algorithm: " + solver.ToString();
        sizeText.text = "Size: " + w + "x" + h;
    }
}
