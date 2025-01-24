using UnityEngine;

public class WinningAreas : MonoBehaviour
{
    
    [SerializeField] public WinningArea[] winningArea;

    private void Start()
    {
        int _randomIndex = Random.Range(0, winningArea.Length);

        for (int _i = 0; _i < winningArea.Length; _i++)
        {
            winningArea[_i].canObtainPoints = false;
            winningArea[_i].canObtainPoints = _i == _randomIndex;
        }
    }
}
