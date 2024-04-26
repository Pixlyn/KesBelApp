using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading;
using TMPro;

public class SceneManager : MonoBehaviour
{
    [SerializeField] public GameObject SplashScene,MainSC,BackButton,puanObje;
    [SerializeField] public GameObject[] objGroup = {};
    [SerializeField] public List<GameObject> backList = new List<GameObject>();
    private TextMeshProUGUI puanText;

    public GameObject KodOkunduText,PuanVerildi,KodOkunamadiText,KonumOkunamadiText;
    private string playerPrefsKey = "Puan";
    private int previousPuanValue;

    private void Awake() 
    {
        puanText = puanObje.GetComponent<TextMeshProUGUI>();
    }

    /*------------------------------------------------------------------------------------------------------------*/

    void Start()
    {
        Input.location.Start();
        StartCoroutine(StartLocationService());
        SplashOff();

        // Başlangıçta Playerprefs değerini al ve sakla
        previousPuanValue = PlayerPrefs.GetInt(playerPrefsKey);

        // Puanı ilk kez ayarla
        UpdatePuanText();

        //puanText.text = "Puan : " + PlayerPrefs.GetInt("Puan");       
    }

    /*------------------------------------------------------------------------------------------------------------*/

    void Update()
    {
        // Playerprefs değerini kontrol et ve güncelleme durumunu kontrol et
        int currentPuanValue = PlayerPrefs.GetInt(playerPrefsKey);
        if (currentPuanValue != previousPuanValue)
        {
            // Değer değiştiğinde yapılması gereken işlemleri buraya yaz
            UpdatePuanText();
            // Önceki değeri güncelle
            previousPuanValue = currentPuanValue;
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if(backList == null)
            {
                //Debug.Log("Quit");
                Application.Quit();
            }
            //Debug.Log("Back");
            BackMenu();
        }
    }

    /*------------------------------------------------------------------------------------------------------------*/

    public void HediyePuan()
    {
        if(PlayerPrefs.HasKey("Puan"))
        {
            int puan = PlayerPrefs.GetInt("Puan");
        }
        else
        {
            Debug.Log("Puan diye bir veri yok.");
            PlayerPrefs.SetInt("Puan",0);
        }
    }

    private void OnPlayerPrefsChanged()
    {
        // Playerprefs değerini kontrol et ve güncelleme durumunu kontrol et
        int currentPuanValue = PlayerPrefs.GetInt(playerPrefsKey);
        if (currentPuanValue != previousPuanValue)
        {
            // Değer değiştiğinde yapılması gereken işlemleri buraya yaz
            UpdatePuanText();
            // Önceki değeri güncelle
            previousPuanValue = currentPuanValue;
        }
    }

    private void UpdatePuanText()
    {
        // Puanı güncelle
        puanText.text = "Puan: " + PlayerPrefs.GetInt(playerPrefsKey);
    }

    /*------------------------------------------------------------------------------------------------------------*/

    public void SplashOff()
    {
        Thread.Sleep(5000);
        FadeFalse(SplashScene);  
        /*----------------------------------------------------------*/
        Thread.Sleep(2000);
        FadeTrue(MainSC);
    }

    /*------------------------------------------------------------------------------------------------------------*/

    public void MenuChange(string target)
    {
        string[] targets = target.Split('-');
        string mevcut = targets[0];
        string hedef = targets[1];

        Debug.Log(mevcut+" to "+hedef);

        GameObject mevcutObj = GameObject.Find(mevcut);
        GameObject hedefObj = GameObject.Find(hedef);

        backList.Add(mevcutObj);
        backList.Add(hedefObj);

        FadeTrue(BackButton);
        /*----------------------------------------------------------*/
        FadeFalse(mevcutObj);
        /*----------------------------------------------------------*/
        FadeTrue(hedefObj);
    }

    /*------------------------------------------------------------------------------------------------------------*/

    public void BackMenu()
    {
        int lastIndex = backList.Count-1;
        GameObject passiveLast = backList[lastIndex];
        FadeFalse(passiveLast);
        backList.RemoveAt(lastIndex);
        /*----------------------------------------------------------*/        
        lastIndex = backList.Count-1;
        GameObject activeLast = backList[lastIndex];
        if(activeLast.name == "MainMenu")
        {
            FadeFalse(BackButton);
        }
        FadeTrue(activeLast);
        backList.RemoveAt(lastIndex);
        Debug.Log(lastIndex);
    }

    /*------------------------------------------------------------------------------------------------------------*/

    public void OpenMap(string destination)
    {
        
        if (Input.location.status == LocationServiceStatus.Running)
        {
            string origin = Input.location.lastData.latitude.ToString().Replace(',','.') + "," + Input.location.lastData.longitude.ToString().Replace(',','.');
            string mapurl = "https://www.google.com/maps/dir/?api=1&origin=" + origin + "&destination=" + destination;
            Debug.Log(mapurl); // URL'yi konsola yazdırır
            Application.OpenURL(mapurl);
            
        }
    }

    /*------------------------------------------------------------------------------------------------------------*/

    public void ToMenu()
    {
        foreach (GameObject obj in objGroup)
        {
            FadeFalse(obj);
        }
        FadeFalse(BackButton);
        FadeTrue(MainSC);
        Debug.Log("To MainMenu");
        backList.Clear();
    }

    /*------------------------------------------------------------------------------------------------------------*/

    public void FadeTrue(GameObject Obj)
    {
        Obj.GetComponent<CanvasGroup>().interactable = true;
        Obj.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Obj.GetComponent<CanvasGroup>().DOFade(1,1f);
    }

    /*------------------------------------------------------------------------------------------------------------*/

    public void FadeFalse(GameObject Obj)
    {
        Obj.GetComponent<CanvasGroup>().interactable = false;
        Obj.GetComponent<CanvasGroup>().blocksRaycasts = false;
        Obj.GetComponent<CanvasGroup>().DOFade(0,1f);
    }

    /*------------------------------------------------------------------------------------------------------------*/
    public void ChangeScene(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    public void PuanKazan(string locationCheck)
    {
        int winPoint = PlayerPrefs.GetInt("Puan");
        Debug.Log("winpoint: " + winPoint);

        string[] fixedLocations = locationCheck.Split(',');
        string fixedLatitude = fixedLocations[0];
        string fixedLongitude = fixedLocations[1];

        string deviceLatitude = Input.location.lastData.latitude.ToString().Replace(',', '.');
        string deviceLongitude = Input.location.lastData.longitude.ToString().Replace(',', '.');

        //Debug.Log(locationCheck + " # " + fixedLatitude + " # " + fixedLongitude + " # " + deviceLatitude + " # " + deviceLongitude);

        if (float.TryParse(fixedLatitude, out float fixLat) && float.TryParse(fixedLongitude, out float fixLon) &&
            float.TryParse(deviceLatitude, out float devLat) && float.TryParse(deviceLongitude, out float devLon))
        {
            KodOkunduText.SetActive(true);
            Debug.Log(locationCheck + " # " + fixedLatitude + " # " + fixedLongitude + " # " + deviceLatitude + " # " + deviceLongitude);

            // if ((fixLat - 0.0005f < devLat) && (devLat < fixLat + 0.0005f) && (fixLon - 0.0005f < devLon) && (devLon < fixLon + 0.0005f))
            // {
            Debug.Log("Puan Kazanıldı.");
            winPoint += 10;
            PlayerPrefs.SetInt("Puan", winPoint);
            PuanVerildi.SetActive(true);
        }
        else
        {
            Debug.LogError("Konum verileri ayrıştırılamadı.");
            KonumOkunamadiText.SetActive(true);
        }
    }

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Konum servisleri etkinleştirilmemiş!");
            yield break;
        }

        Input.location.Start();

        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return new WaitForSeconds(1);
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Konum servisleri başlatılamadı!");
            yield break;
        }
    }

    
}
