using UnityEngine;

public class SceneManagement : MonoBehaviour
{
    static Vector2 GridSize; //网格大小
    Vector2 _origin; // 地图原点
    GameObject[][] _rootGameObjects; // 网格Root节点
    Vector3 _lastCameraPos; 
    Transform _camera;
    int _row;
    int _col;
    int _offset; // offset越大，屏幕外渲染的就越多


    public Vector2 GetGridPosition(int r, int c) {
        return _origin + new Vector2(r * GridSize.y, c * GridSize.x);
    }

    /// <summary>
    /// 根据Camera的投射和大小、角度、坐标等来计算
    /// BottomLeft, TopLeft, TopRight, BottomRight
    /// </summary>
    /// <returns></returns>
    public Vector2[] GetCameraVisibleRange() {
        return new Vector2[4];
    }

    public void Update() {
        if (_camera.position == _lastCameraPos) return;
        _lastCameraPos = _camera.position;
        var visibleRange = GetCameraVisibleRange();
        for (int r = 0; r < _row; r++) {
            for (int c = 0; c < _col; c++) {
                var gridPos = GetGridPosition(r, c);
                if (gridPos.x + _offset >= visibleRange[0].x && gridPos.x - _offset <= visibleRange[2].x
                    && gridPos.y + _offset >= visibleRange[0].y && gridPos.y - _offset <= visibleRange[2].y) {
                    // 在可视范围内，如果不是active，就设置成active
                    if (!_rootGameObjects[r][c].activeSelf) _rootGameObjects[r][c].SetActive(true);
                } else {
                    // 不在可视范围内
                    if (_rootGameObjects[r][c].activeSelf) _rootGameObjects[r][c].SetActive(false);
                }
            }
        }
    }
}
