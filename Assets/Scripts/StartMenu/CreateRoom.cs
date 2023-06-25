using UnityEngine;

public class CreateRoom : MonoBehaviour
{
    public GameData GameData;
    private GameObject WebsocketManager;

    void Start()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
    }

    async public void createRoom()
    {
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string json = "{'type': 'create'}";
        await websocket.SendText(json);
    }

    async public void deleteCreatedRoom()
    {
        string pincode = GameData.joinedRoomCode;
        if (pincode != null)
        {
            GameData.isHost = false;
            GameData.playerID = "";
            GameData.playersList = null;
            GameData.joinedRoomCode = "";
            var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
            string json = "{'type': 'delete', 'params':{'code': '" + pincode + "'}}";
            await websocket.SendText(json);
        }
    }
}