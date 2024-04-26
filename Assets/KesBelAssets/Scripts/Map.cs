//MIT License
//Copyright (c) 2023 DA LAB (https://www.youtube.com/@DA-LAB)
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;         //Unity Arayüzüne Erişim
using UnityEngine.Networking; //Internet Erişimi
using System;

public class Map : MonoBehaviour
{
    private string apiKey = ""; //API Key yazılacak.
    public float lat = 40.20097f;                                      //Latitude - Enlem
    public float lon = 29.21309f;                                      //Longtitude - Boylam
    public int zoom = 16;                                              //Harita Yakınlaştırması
    public enum resolution { low = 1, high = 2 };                      //Çözünürlük Unity Arayüzü üzerinden değiştirilmesi için.
    public resolution mapResolution = resolution.high;                 //Yüksek Doğruluk ve Netlik
    public enum type { roadmap, satellite, gybrid, terrain };          //Google Maps Üzerinde Bulunan Harita Çeşitleri Arazi, Trafik, Uydu  Görüntüsü gibi
    public type mapType = type.terrain;                                //Yükseltilerin Görünebilmesi için Arazi Seçildi.
    private string url = "";                                           //Kullanıcının konumu ve gidilecek konum alınarak url oluşturulacak
    private int mapWidth = 800;                                        //Haritanın ekran çözünürlüğü - Genişlik
    private int mapHeight = 520;                                       //Haritanın ekran çözünürlüğü - Yükseklik
    private bool mapIsLoading = true;                                  //Harita yükleniyor mu kontrol için bool değeri
    private Rect rect;                                                 //Google Maps verisinin Unity'e 2D aktarılması için X ve Y 

    private string apiKeyLast;
    private float latLast = 40.20097f;
    private float lonLast = 29.21309f;
    private int zoomLast = 16;
    private resolution mapResolutionLast = resolution.high;
    private type mapTypeLast = type.terrain;
    private bool updateMap = true;                                      //Haritayı güncellemek için bool


    // Start is called before the first frame update 
    void Start()
    {
        StartCoroutine(GetGoogleMap());
        rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
        mapWidth = (int)Math.Round(rect.width);
        mapHeight = (int)Math.Round(rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateMap && (apiKeyLast != apiKey || !Mathf.Approximately(latLast, lat) || !Mathf.Approximately(lonLast, lon) || zoomLast != zoom || mapResolutionLast != mapResolution || mapTypeLast != mapType))
        {
            rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
            mapWidth = (int)Math.Round(rect.width);
            mapHeight = (int)Math.Round(rect.height);
            StartCoroutine(GetGoogleMap());
            updateMap = false;
        }
    }


    IEnumerator GetGoogleMap()
    {
        url = "https://maps.googleapis.com/maps/api/staticmap?center=" + lat + "," + lon + "&zoom=" + zoom + "&size=" + mapWidth + "x" + mapHeight + "&scale=" + mapResolution + "&maptype=" + mapType + "&key=" + apiKey;
        mapIsLoading = true;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error);
        }
        else
        {
            mapIsLoading = false;
            gameObject.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            apiKeyLast = apiKey;
            latLast = lat;
            lonLast = lon;
            zoomLast = zoom;
            mapResolutionLast = mapResolution;
            mapTypeLast = mapType;
            updateMap = true;
        }
    }

}