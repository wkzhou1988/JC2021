using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public MoveAction MoveAction;
    public Vector3 Begin;
    public Vector3 End;
    public float Time;
    public bool Pingpong;
    public EaseType Ease;

    public void TestMove() {
        MoveAction.Setup(Begin, End, Time, Pingpong, Ease);
    }

    public void TestNetwork() {
        RequestQueue.Send(new Request() { Url = "http://www.baidu.com", Method = "GET", callback = (resp) => Debug.Log(resp.data) });
        RequestQueue.Send(new Request() { Url = "http://www.bing.com", Method = "GET", callback = (resp) => Debug.Log(resp.data) });
        RequestQueue.Send(new Request() { Url = "http://www.taobao.com", Method = "GET", callback = (resp) => Debug.Log(resp.data) });
        RequestQueue.Send(new Request() { Url = "http://www.zhihu.com", Method = "GET", callback = (resp) => Debug.Log(resp.data) });
    }

    private void Update() {
        RequestQueue.Update();
    }

}
