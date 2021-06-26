using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;

public class Request {
    public string Url;
    public Dictionary<string, string> Headers;
    public string Method;
    public Action<Response> callback;
}

public class Response {
    public string data;
    public Action<Response> callback;
}


/// <summary>
///  多线程的基本思想是类似于producer/consumer的模型，用一个队列缓存请求，用一个线程池来处理
///  缓存的请求。请求处理完成后，需要返回主线程来处理response，所以response也需要一个队列，通常可以
///  在Update里面处理。线程的同步机制可以使用信号量，这里只使用比较简单的加锁方式实现。处理Http请求的地方
///  可以继续扩展，使之支持使用小缓存下载较大文件。
/// </summary>
public class RequestQueue
{
    static object _lockObj = new object();
    static Queue<Request> _requests = new Queue<Request>();
    static Queue<Response> _responses = new Queue<Response>();
    static List<Thread> _jobs = new List<Thread>();
    const int MAX_CONCURRENCY = 4;

    static RequestQueue() {
        for (int i = 0; i < MAX_CONCURRENCY; i++) {
            var job = new Job();
            var thread = new Thread(job.Run);
            _jobs.Add(thread);
            thread.Start();
        }
    }

    public static void Send(Request req) {
        lock (_lockObj) {
            _requests.Enqueue(req);
        }
    }

    public static bool TryGet(out Request req) {
        lock (_lockObj) {
            if (_requests.Count > 0) {
                req = _requests.Dequeue();
                return true;
            }
            req = null;
            return false;
        }
    }

    public static void PushResponse(Response resp) {
        lock (_lockObj) {
            _responses.Enqueue(resp);
        }
    }

    public static void Update() {
        lock (_lockObj) {
            while (_responses.Count > 0) {
                HandleResponse(_responses.Dequeue());
            }
        }
    }

    static void HandleResponse(Response resp) {
        resp.callback(resp);
    }
}

public class Job {
    Request _req;
    public void Run() {
        while (true) {
            while (!RequestQueue.TryGet(out _req)) {
                Thread.Sleep(100);
            }
            var httpRequest = (HttpWebRequest)HttpWebRequest.Create(_req.Url);
            httpRequest.Method = _req.Method;
            using (var response = httpRequest.GetResponse()) {
                using (var stream = response.GetResponseStream()) {
                    using (var reader = new StreamReader(stream)) {
                        var data = reader.ReadToEnd();
                        RequestQueue.PushResponse(new Response() { data = data, callback = _req.callback });
                    }
                }
            }
        }
    }
}