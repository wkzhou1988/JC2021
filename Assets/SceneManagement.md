在创建场景的时候，或者场景初始化的时候，将场景划分成网格状，每个网格放一个GameObject作为网格的Root。网格的长宽根据场景的大小和摄像机的视角大小，
根据性能要求来确定。所有坐标落在一个网格中的渲染对象都挂在网格Root底下。镜头移动时，用射线或者自行计算坐标也可以，计算可视范围内的网格有哪些，把这些网格的
Root设置成Active，不可见的网格Root设置成非Active。通过把场景分成网格，减少了计算可视范围时的计算量。通过控制网格大小，可以调整SetActive调用的频率。

```
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

```