using System.Collections;
using System.Collections.Generic;
using Ubiq.Networking;
using UnityEngine;
using Ubiq.Dictionaries;
using Ubiq.Messaging;
using Ubiq.Logging.Utf8Json;
using Ubiq.Rooms;
using System;
using System.Text;

public class TranscriptionCollector : MonoBehaviour
{
    public NetworkId networkId = new NetworkId(98);
    private NetworkContext context;

    [Serializable]
    private struct Message
    {
        public string type;
        public string peer;
        public string data;
    }

    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this,networkId);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage data)
    {
        Message message = data.FromJson<Message>();
        Debug.Log(message.peer + " " + message.data);
    }
}
