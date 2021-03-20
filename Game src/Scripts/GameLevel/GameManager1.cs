using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager1 : MonoBehaviour
{
    [SerializeField]
    private GameObject karePrefab;

    [SerializeField]
    private Transform karelerPaneli;

    private GameObject[] karelerDizisi = new GameObject[25];

    [SerializeField]
    private Transform soruPaneli;

    [SerializeField]
    private Text soruText;

    [SerializeField]
    private Sprite[] kareSprites;

    [SerializeField]
    private GameObject sonucPanel;

    List<int> bolumDegerleriniListesi = new List<int>();

    int bolen, bolunen, kacinciSoru, butonDegeri, dogruSonuc;

    bool butonaBasildimi;

    int kalanHak;

    string zorlukDerecesi;

    HaklarManager haklarManager;

    PuanManager puanManager;

    GameObject gecerliKare;
    private void Awake()
    {
        kalanHak = 3;

        sonucPanel.GetComponent<RectTransform>().localScale = Vector3.zero;

        haklarManager = Object.FindObjectOfType<HaklarManager>();

        puanManager = Object.FindObjectOfType<PuanManager>();

        haklarManager.HaklariKontrolEt(kalanHak);
    }

    void Start()
    {
        butonaBasildimi = false;

        soruPaneli.GetComponent<RectTransform>().localScale = Vector3.zero;

        kareleriOlustur();


    }

    public void kareleriOlustur()
    {
        for (int i = 0; i < 25; i++)
        {
            GameObject kare = Instantiate(karePrefab, karelerPaneli);

            kare.transform.GetChild(1).GetComponent<Image>().sprite = kareSprites[Random.Range(0, kareSprites.Length)];

            kare.transform.GetComponent<Button>().onClick.AddListener(() => ButonaBasildi());

            karelerDizisi[i] = kare;
        }

        BolumDegerleriniYazdir();

        StartCoroutine(DoFadeRoutine());

        Invoke("SoruPaneliniAc", 3f);
    }

    void ButonaBasildi()
    {
        if (butonaBasildimi == true)
        {
            butonDegeri = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text);

            gecerliKare = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            SonucuKontrolEt();
        }

    }

    void SonucuKontrolEt()
    {
        if (butonDegeri == dogruSonuc)
        {
            gecerliKare.transform.GetChild(1).GetComponent<Image>().enabled = true;
            gecerliKare.transform.GetChild(0).GetComponent<Text>().text = "";
            gecerliKare.transform.GetComponent<Button>().interactable = false;

            puanManager.PuaniArttir(zorlukDerecesi);

            bolumDegerleriniListesi.RemoveAt(kacinciSoru);

            if (bolumDegerleriniListesi.Count>0)
            {
                SoruPaneliniAc();
            }
            else
            {
                OyunBitti();
            }

            
        }
        else
        {
            kalanHak--;
            haklarManager.HaklariKontrolEt(kalanHak);
        }


        if (kalanHak<=0)
        {
            OyunBitti();
        }
    }

    void OyunBitti()
    {
        butonaBasildimi = false; 

        sonucPanel.GetComponent<RectTransform>().DOScale(1, 0.4f).SetEase(Ease.OutBack);
    }

    IEnumerator DoFadeRoutine()
    {
        foreach (var kare in karelerDizisi)
        {
            kare.GetComponent<CanvasGroup>().DOFade(1, 0.6f);

            yield return new WaitForSeconds(0.11f);
        }
    }

    void BolumDegerleriniYazdir()
    {
        foreach (var kare in karelerDizisi)
        {
            int rastgeleDeger = Random.Range(2, 13);

            bolumDegerleriniListesi.Add(rastgeleDeger);

            kare.transform.GetChild(0).GetComponent<Text>().text = rastgeleDeger.ToString();
        }
    }

    void SoruPaneliniAc()
    {
        SoruyuSor();

        butonaBasildimi = true;

        soruPaneli.GetComponent<RectTransform>().DOScale(1, 0.4f);
    }

    void SoruyuSor()
    {
        bolen = Random.Range(2, 11);

        kacinciSoru = Random.Range(0, bolumDegerleriniListesi.Count);

        dogruSonuc = bolumDegerleriniListesi[kacinciSoru];

        bolunen = bolen * dogruSonuc;


        if (bolunen <= 40)
        {
            zorlukDerecesi = "kolay";
        }
        else if (bolunen > 40 && bolunen <= 80)
        {
            zorlukDerecesi = "orta";
        }
        else
        {
            zorlukDerecesi = "zor";
        }

        soruText.text = bolunen.ToString() + " : " + bolen.ToString();
    }
}
