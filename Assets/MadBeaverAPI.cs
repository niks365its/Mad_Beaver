using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class MadBeaverAPI : MonoBehaviour
{
    string url = "https://scservice.com.ua/api/api.php";

    public void SaveGameProgress(string playerId, string jsonData)
    {
        StartCoroutine(PostProgress(playerId, jsonData));
    }

    IEnumerator PostProgress(string playerId, string jsonData)
    {
        string jsonToSend = "{\"player_id\":\"" + playerId + "\",\"data\":" + jsonData + "}";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonToSend);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.Log("Response: " + request.downloadHandler.text);
    }

    public void LoadGameProgress(string playerId)
    {
        StartCoroutine(GetProgress(playerId));
    }

    IEnumerator GetProgress(string playerId)
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "?player_id=" + playerId);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("Game progress: " + json);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}
