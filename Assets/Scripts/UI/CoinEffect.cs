using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CoinEffect : MonoBehaviour
{
    [Header("UI references")]
    [SerializeField] public Transform m_Pos_Start;
    [SerializeField] public Transform m_Parent_Holder;
    [SerializeField] public TMP_Text coinUIText;
    [SerializeField] private GameObject animatedCoinPrefab;
    [SerializeField] private Transform target;

    [Space]
    [Header("Available coins: ")]
    [SerializeField] private int maxCoins = 10;
    private System.Collections.Generic.Queue<GameObject> coinQueue = new System.Collections.Generic.Queue<GameObject>();

    [Space]
    [Header("Animation settings")]
    [SerializeField] [Range(0.5f, 100f)] private float minAnimationDuration = 0.5f;
    [SerializeField] [Range(0.9f, 100f)] private float maxAnimationDuration = 1f;
    //[SerializeField] [Range(0.5f, 0.9f)] private float minAnimationDuration;
    //[SerializeField] [Range(0.9f, 2f)] private float maxAnimationDuration;
    [SerializeField] private Ease easeType = Ease.InOutQuad;
    [SerializeField] private float spread = 1;
    [SerializeField] private float delayShowTime;

    [SerializeField] private Vector3 targetPosition;


    //private bool currentSound = true;

    private Action m_Anim_Done;

    private int coinNumber;
    public int CoinNumber
    {
        get => this.coinNumber;
        set
        {
            this.coinNumber = value;
            if (this.coinUIText != null)
            {
                this.coinUIText.text = this.coinNumber.ToString();
            }
        }
    }

    private void Awake()
    {
        if (this.target != null)
        {
            this.targetPosition = this.target.position;
        }
        PrepareCoins();
    }


    private void PrepareCoins()
    {
        GameObject coin;
        for (int i = 0; i < this.maxCoins; i++)
        {
            coin = Instantiate(this.animatedCoinPrefab, this.m_Parent_Holder == null ? transform : this.m_Parent_Holder);
            coin.SetActive(false);
            this.coinQueue.Enqueue(coin);
        }
    }

    private Vector3 Get_Position_Target()
    {
        return this.target != null ? this.target.position : this.targetPosition;
    }

    private IEnumerator Animate(Vector3 collectedCoinPosition, int startCoin, int finalCoin)
    {
        yield return new WaitForSecondsRealtime(this.delayShowTime);

        int countDownCoin = Mathf.Abs(startCoin - finalCoin) > this.maxCoins ? this.maxCoins : Mathf.Abs(startCoin - finalCoin);
        if (countDownCoin <= 0)
        {
            countDownCoin = this.maxCoins;
        }

        if (startCoin == finalCoin)
        {
            countDownCoin = 1;
        }

        int stepCoinValue = Mathf.Abs(startCoin - finalCoin) / countDownCoin;
        CoinNumber = startCoin;

        for (int i = 0; i < countDownCoin; i++)
        {
            if (this.coinQueue.Count > 0)
            {
                GameObject coin = this.coinQueue.Dequeue();
                coin.SetActive(true);

                //Move coin
                coin.transform.position = collectedCoinPosition + new Vector3(UnityEngine.Random.Range(-this.spread, this.spread), 0f, 0f);
                float duration = UnityEngine.Random.Range(this.minAnimationDuration, this.maxAnimationDuration);
                coin.transform.DOMove(Get_Position_Target(), duration).SetUpdate(true)
                    .SetEase(this.easeType)
                    .OnComplete(
                    () =>
                    {
                        coin.SetActive(false);
                        this.coinQueue.Enqueue(coin);
                        countDownCoin -= 1;
                        if (countDownCoin == 0)
                        {
                            CoinNumber = finalCoin;
                            if (this.m_Anim_Done != null)
                            {
                                this.m_Anim_Done.Invoke();
                            }
                        }
                        else
                        {
                            CoinNumber += stepCoinValue;
                        }
                    }
                    );
            }
        }

    }
    private IEnumerator AnimateNoneCoin (Vector3 collectedCoinPosition, int startCoin, int finalCoin)
    {
        yield return new WaitForSecondsRealtime(this.delayShowTime);

        int countDownCoin = Mathf.Abs(startCoin - finalCoin) > this.maxCoins ? this.maxCoins : Mathf.Abs(startCoin - finalCoin);
        if (countDownCoin <= 0)
        {
            countDownCoin = this.maxCoins;
        }

        if (startCoin == finalCoin)
        {
            countDownCoin = 1;
        }

        int stepCoinValue = Mathf.Abs(startCoin - finalCoin) / countDownCoin;
        CoinNumber = startCoin;

        for (int i = 0; i < countDownCoin; i++)
        {
            if (this.coinQueue.Count > 0)
            {
                GameObject coin = this.coinQueue.Dequeue();
                coin.SetActive(false);

                //Move coin
                coin.transform.position = collectedCoinPosition + new Vector3(UnityEngine.Random.Range(-this.spread, this.spread), 0f, 0f);
                float duration = UnityEngine.Random.Range(0f, 0.4f);
                coin.transform.DOMove(Get_Position_Target(), duration).SetUpdate(true)
                    .SetEase(this.easeType)
                    .OnComplete(
                    () =>
                    {
                        coin.SetActive(false);
                        this.coinQueue.Enqueue(coin);
                        countDownCoin -= 1;
                        if (countDownCoin == 0)
                        {
                            CoinNumber = finalCoin;
                            if (this.m_Anim_Done != null)
                            {
                                this.m_Anim_Done.Invoke();
                            }
                        }
                        else
                        {
                            CoinNumber += stepCoinValue;
                        }
                    }
                    );
            }
        }

    }
    public void AddCoins(TMP_Text _coinUIText, Vector3 collectedCoinPosition, int startCoin, int finalCoin, Action anim_Done = null)
    {
        this.coinUIText = _coinUIText;
        this.m_Anim_Done = anim_Done;
        StartCoroutine(Animate(collectedCoinPosition, startCoin, finalCoin));
    }

    //[Button]
    private void TestCoinEffect()
    {
        AddCoins(500, 1000);
    }

    public void AddCoins(int startCoin, int finalCoin, Action anim_Done = null)
    {
        this.m_Anim_Done = anim_Done;
        StartCoroutine(Animate(this.m_Pos_Start.position, startCoin, finalCoin));
    }

    public void AddCoins(Transform pos_Start, int startCoin, int finalCoin, Action anim_Done = null)
    {
        this.m_Anim_Done = anim_Done;
        StartCoroutine(Animate(pos_Start.position, startCoin, finalCoin));
    }

    public void AddCoins(Vector3 pos_End, int startCoin, int finalCoin, Action anim_Done = null)
    {
        targetPosition = pos_End;
        this.m_Anim_Done = anim_Done;
        StartCoroutine(AnimateNoneCoin(m_Pos_Start.position, startCoin, finalCoin));
    }

    public void AddCoins(bool is_UI_In_Camera, TMP_Text _coinUIText, Transform target, int startCoin, int finalCoin, Action anim_Done = null)
    {
        this.targetPosition = is_UI_In_Camera ? Camera.main.WorldToScreenPoint(target.position) : target.position;
        if (_coinUIText != null)
        {
            this.coinUIText = _coinUIText;
        }
        this.m_Anim_Done = anim_Done;
        StartCoroutine(Animate(this.m_Pos_Start.position, startCoin, finalCoin));
    }
}
