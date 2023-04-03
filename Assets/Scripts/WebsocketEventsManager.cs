using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;

public class WebsocketEventsManager : MonoBehaviour
{
    [SerializeField]
    public WebSocket websocket;

    public string joinedRoomCode;
    public string playerID;
    public GameObject WaitingInRoomCanvas;

    public class ParsedJSON
    {
        public string type { get; set; }
        public Params @params { get; set; }
    }

    public class Data
    {
        public List<PlayersList> playersList { get; set; }
        public string message { get; set; }
    }

    public class Params
    {
        public string action { get; set; }
        public Data data { get; set; }
    }

    public class PlayersList
    {
        public int id { get; set; }
        public string chosenCharacter { get; set; }
    }


    private ParsedJSON _ParsedJSON;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameObject.GetComponent<WebsocketManager>().websocket);

        websocket.OnMessage += (bytes) => {
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log(json);
         /*   _ParsedJSON = JsonConvert.DeserializeObject<ParsedJSON>(json);*/

            /*         switch (_ParsedJSON.type)
                     {
                         case "createdRoom":
                             joinedRoomCode = _ParsedJSON.@params.@data.message;
                             break;

                         case "joinedRoom":
                             GameObject.Find("JoinRoomCanvas").SetActive(false);
                             WaitingInRoomCanvas.SetActive(true);
                             joinedRoomCode = _ParsedJSON.@params.@data.message;
                             break;
                         case "  ":
                             joinedRoomCode = _ParsedJSON.@params.@data.message;
                             break;

                         case "receivedPlayersList":
                             Debug.Log(joinedRoomCode = _ParsedJSON.@params.@data.message);
                             break;
                         default:
                             // code block
                             break;
                     }*/

        };

    }

}
