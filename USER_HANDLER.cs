using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class USER_HANDLER : MonoBehaviour
{
    private string url = "https://my-json-server.typicode.com/Sportsy1/Fake_JSON/users/";
    private string mortyAPI = "https://rickandmortyapi.com/api/character/";
    public RawImage[] image;

    public Text[] text;

    public int contador = 0;

    public Text usuario_text;


    public void SendRequest(int user_id)
    {
        StartCoroutine("GetUser", user_id);
    }


    IEnumerator GetUser(int user_id)
    {
        UnityWebRequest request = UnityWebRequest.Get(url + user_id);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                User user = JsonUtility.FromJson<User>(request.downloadHandler.text);

                Debug.Log(user.username + " tiene el id " + user.id + " con las cartas " + string.Join(", ", user.cards));

                usuario_text.text = user.username;

                foreach (int card in user.cards)
                {
                    StartCoroutine("getCharacter", card);
                }
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    IEnumerator getCharacter(int id_character)
    {
        UnityWebRequest request = UnityWebRequest.Get(mortyAPI + id_character);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                Character character = JsonUtility.FromJson<Character>(request.downloadHandler.text);
                Debug.Log(character.name + " is a " + character.species);
                StartCoroutine("DownloadImage", character);
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }


    IEnumerator DownloadImage(Character character)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(character.image);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            image[contador].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            text[contador].text = character.name;
        }
        contador++;

        if (contador == 5)
            contador = 0;
    }


}

[System.Serializable]
public class User
{
    public string username;
    public int id;
    public int[] cards;
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string species;
    public string image;
}
