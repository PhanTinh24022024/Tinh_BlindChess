using System;
using System.Collections;
using System.Collections.Generic;
using Chess.Game;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BlindChessController : MonoBehaviour
{
    [SerializeField] private Button m_SettingBtn,
        m_PromotionBtn,
        m_StarterPackBtn,
        m_InviteBtn,
        m_LeaderBoardBtn,
        m_SupportBtn,
        m_OfflineBtn,
        m_LearnBtn,
        m_OnlineBtn,
        m_QuestBtn,
        m_IncreaseChipBtn,
        m_IncreaseDiamondBtn;

    [SerializeField] private Button m_ShopBtn,
        m_InventoryBtn,
        m_MailBtn,
        m_DailyBonusBtn,
        m_ButtonCLoseInPopupWinLose,
        m_ButtonHome,
        m_ButtonContinues,
        m_ButtonShare,
        m_ButtonWatchAds,
        m_ButtonMenu,
        m_ButtonQuit,
        m_ButtonCloseLevel,
        m_ButtonCloseOnlMode,
        m_ButtonChess,
        m_ButtonBlindChess,
        m_ButtonPlayWhite,
        m_ButtonPlayBlack,
        m_ButtonAiVsAi;

    [SerializeField] private GameObject m_UiSetting,
        m_UiStarterPack,
        m_UiInvite,
        m_UiLeaderBoard,
        m_UiSupport,
        m_PlayChessOffline,
        m_UiChessOffLine,
        m_UiLearn,
        m_PlayChessOnline,
        m_UiQuest,
        m_UiDaily,
        m_UiShop,
        m_UiInventory,
        m_UiMail,
        m_UiHome,
        m_UiWinLose,
        m_ListMenu,
        m_UiLevelPlay,
        m_UiOnlMode;

    [SerializeField] private List<Image> m_listCounEatChessInPlayer1,
        m_listCountPromotionChessInPlayer1,
        m_listCountEatChessInPlayer2;

    [SerializeField] private List<Sprite> m_listSpriteWhiteChess1,
        m_listSpriteWhiteChess2,
        m_listSpriteBlackChess1,
        m_listSpriteBlackChess2;

    [SerializeField] private TextMeshProUGUI blackCountPawnCapturedTMP,
        blackCountRookCapturedTMP,
        blackCountKnightCapturedTMP,
        blackCountBishopCapturedTMP,
        blackCountQueenCapturedTMP;

    [SerializeField] private TextMeshProUGUI whiteCountPawnCapturedTMP,
        whiteCountRookCapturedTMP,
        whiteCountKnightCapturedTMP,
        whiteCountBishopCapturedTMP,
        whiteCountQueenCapturedTMP;

    [SerializeField] private Image m_ImageWinLose;
    [SerializeField] private Sprite m_TitleWin, m_TitleLose, m_TitleDraw;
    [SerializeField] private GameManager m_Gamemanager;

    private int blackCountPawnCaptured = 0;
    private int blackCountRookCaptured = 0;
    private int blackCountKnightCaptured = 0;
    private int blackCountBishopCaptured = 0;
    private int blackCountQueenCaptured = 0;

    private int whiteCountPawnCaptured = 0;
    private int whiteCountRookCaptured = 0;
    private int whiteCountKnightCaptured = 0;
    private int whiteCountBishopCaptured = 0;
    private int whiteCountQueenCaptured = 0;
    private bool canCount = true;

    public GameObject UiHome => m_UiHome;
    public static BlindChessController instance = null;
    public bool isClickButtonIncreaseChip = false;
    public bool isClickButtonIncreaseDiamond = false;
    public bool isPlayOffline = false;
    public bool overTime = false;
    public int totalChip = 50000000;

    public int totalDiamond = 50000000;

    // Các biến lưu trạng thái di chuyển cho từng quân cờ trong BlindChessController
    public bool[] isWhiteRookFirstMove = new bool[2] { true, true };
    public bool[] isBlackRookFirstMove = new bool[2] { true, true };
    public bool[] isWhitePawnFirstMove = new bool[8] { true, true, true, true, true, true, true, true };
    public bool[] isBlackPawnFirstMove = new bool[8] { true, true, true, true, true, true, true, true };
    public bool[] isWhiteKnightFirstMove = new bool[2] { true, true };
    public bool[] isBlackKnightFirstMove = new bool[2] { true, true };
    public bool[] isWhiteBishopFirstMove = new bool[2] { true, true };
    public bool[] isBlackBishopFirstMove = new bool[2] { true, true };
    public bool isWhiteQueenFirstMove = true;
    public bool isBlackQueenFirstMove = true;
    public bool isBlindChess;
    public PieceTheme pieceTheme;
    private CoinEffect coinEffect;

    public Dictionary<int, bool> PieceSelected = new Dictionary<int, bool>();
    public List<DutBlind> dutBlinds = new List<DutBlind>();

    public bool MatchDutBlind(int file, int rank)
    {
        return dutBlinds.Exists(dutBlind => dutBlind.Match(file, rank));
    }
    

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        m_SettingBtn.onClick.AddListener(ClickButtonSetting);
        m_PromotionBtn.onClick.AddListener(ClickButtonDailyBonus);
        m_StarterPackBtn.onClick.AddListener(ClickButtonStarterPack);
        m_InviteBtn.onClick.AddListener(ClickButtonInvite);
        m_LeaderBoardBtn.onClick.AddListener(ClickButtonLeaderBoard);

        m_SupportBtn.onClick.AddListener(ClickButtonSupport);
        m_OfflineBtn.onClick.AddListener(ClickButtonOffline);
        m_LearnBtn.onClick.AddListener(ClickButtonLearn);
        m_OnlineBtn.onClick.AddListener(ClickButtonOnline);
        m_QuestBtn.onClick.AddListener(ClickButtonQuest);

        m_DailyBonusBtn.onClick.AddListener(ClickButtonDailyBonus);
        m_ShopBtn.onClick.AddListener(ClickButtonShop);
        m_IncreaseChipBtn.onClick.AddListener(ClickButtonIncreaseChip);
        m_IncreaseDiamondBtn.onClick.AddListener(ClickButtonIncreaseDiamond);
        m_InventoryBtn.onClick.AddListener(ClickButtonInventory);

        m_MailBtn.onClick.AddListener(ClickButtonMail);
        m_ButtonCLoseInPopupWinLose.onClick.AddListener(ClickButtonCLoseInPopupWinLose);
        m_ButtonHome.onClick.AddListener(ClickButtonHome);
        m_ButtonContinues.onClick.AddListener(ClickButtonContinue);
        m_ButtonShare.onClick.AddListener(ClickButtonShare);

        m_ButtonWatchAds.onClick.AddListener(ClickButtonWatchAds);
        m_ButtonMenu.onClick.AddListener(ClickButtonMenu);
        m_ButtonQuit.onClick.AddListener(ClickButtonHome);
        m_ButtonCloseLevel.onClick.AddListener(ClickButtonCloseLevel);
        m_ButtonCloseOnlMode.onClick.AddListener(ClickButtonCloseOnlMode);

        m_ButtonChess.onClick.AddListener(ClickButtonChess);
        m_ButtonBlindChess.onClick.AddListener(ClickButtonBlindChess);

        for (int i = 0; i < 16; i++)
        {
            if (i == 4) continue;
            PieceSelected.Add(i, true);
        }

        for (int i = 49; i < 64; i++)
        {
            if (i == 60) continue;
            PieceSelected.Add(i, true);
        }
    }

    private void ClickButtonWatchAds()
    {
        Debug.LogError($"CLickButtonWatchAds");
    }

    private void ClickButtonHome()
    {
        SceneManager.LoadScene(0);
        // m_PlayChessOffline.transform.localScale = Vector3.zero;
        // GameData.levelChoosing = 0;
        // isClickButtonMenu = false;
        // Debug.LogError($"ClickButtonHome");
        // m_UiHome.SetActive(true);
        // m_UiChessOffLine.SetActive(false);
        // m_UiWinLose.SetActive(false);
        // m_UiLevelPlay.SetActive(false);
        // m_UiOnlMode.SetActive(false);
        // if (m_ListMenu.activeSelf)
        // {
        //     m_ListMenu.SetActive(false);
        // }
    }

    private void ClickButtonContinue()
    {
        m_UiWinLose.SetActive(false);
        m_PlayChessOffline.transform.localScale = Vector3.one;
        m_Gamemanager.NewGame(true);
    }

    private void ClickButtonShare()
    {
        Debug.LogError($"ClickButtonShare");
    }

    private void ClickButtonCLoseInPopupWinLose()
    {
        GameData.levelChoosing = 0;
        m_UiWinLose.SetActive(false);
        m_PlayChessOffline.transform.localScale = Vector3.one;
    }

    private void ClickButtonIncreaseChip()
    {
        isClickButtonIncreaseChip = true;
        ClickButtonShop();
    }

    private void ClickButtonIncreaseDiamond()
    {
        isClickButtonIncreaseDiamond = true;
        ClickButtonShop();
    }

    private void ClickButtonSetting()
    {
        m_UiSetting.SetActive(true);
    }

    private void ClickButtonPromotion()
    {
        Debug.LogError($"ClickButtonPromotion");
    }

    private void ClickButtonStarterPack()
    {
        Debug.LogError($"ClickButtonStarterPack");
    }

    private void ClickButtonInvite()
    {
        Debug.LogError($"ClickButtonInvite");
    }

    private void ClickButtonLeaderBoard()
    {
        m_UiLeaderBoard.SetActive(true);
    }

    private void ClickButtonSupport()
    {
        Debug.LogError($"ClickButtonSupport");
    }

    private void ClickButtonOffline()
    {
        isBlindChess = false;
        isPlayOffline = true;
        m_UiLevelPlay.SetActive(true);
    }

    public void PlayOffline()
    {
        m_UiChessOffLine.SetActive(true);
        m_PlayChessOffline.SetActive(true);
        m_PlayChessOffline.transform.localScale = Vector3.one;
        if (m_PlayChessOffline.activeSelf)
        {
            m_Gamemanager.NewGame(true);
        }

        m_UiLevelPlay.SetActive(true);
    }

    private void ClickButtonLearn()
    {
        Debug.LogError($"ClickButtonLearn");
    }

    private void ClickButtonCloseLevel()
    {
        m_UiLevelPlay.SetActive(false);
    }

    private void ClickButtonOnline()
    {
        isPlayOffline = false;
        m_UiOnlMode.SetActive(true);
    }

    private void ClickButtonChess()
    {
        isBlindChess = false;
        m_UiChessOffLine.SetActive(true);
        m_PlayChessOffline.SetActive(true);
        m_PlayChessOffline.transform.localScale = Vector3.one;
        if (m_PlayChessOffline.activeSelf)
        {
            m_Gamemanager.NewGame(true);
        }
    }

    private void ClickButtonBlindChess()
    {
        isBlindChess = true;
        m_UiChessOffLine.SetActive(true);
        m_PlayChessOffline.SetActive(true);
        m_PlayChessOffline.transform.localScale = Vector3.one;
        if (m_PlayChessOffline.activeSelf)
        {
            m_Gamemanager.NewGame(true);
        }
    }

    private void ClickButtonCloseOnlMode()
    {
        m_UiOnlMode.SetActive(false);
    }

    private void ClickButtonQuest()
    {
        Debug.LogError($"ClickButtonQuest");
    }

    private void ClickButtonDailyBonus()
    {
        m_UiDaily.SetActive(true);
    }

    private void ClickButtonShop()
    {
        m_UiShop.SetActive(true);
        m_UiHome.SetActive(false);
    }

    private void ClickButtonInventory()
    {
        Debug.LogError($"ClickButtonInventory");
    }

    private void ClickButtonMail()
    {
        Debug.LogError($"ClickButtonMail");
    }

    private void ClickButtonMenu()
    {
        if (m_ListMenu.activeSelf)
        {
            m_ListMenu.SetActive(false);
        }
        else
        {
            m_ListMenu.SetActive(true);
            if (isPlayOffline)
            {
                m_ButtonPlayWhite.gameObject.SetActive(true);
                m_ButtonPlayBlack.gameObject.SetActive(true);
                m_ButtonAiVsAi.gameObject.SetActive(true);
            }
            else
            {
                m_ButtonPlayWhite.gameObject.SetActive(false);
                m_ButtonPlayBlack.gameObject.SetActive(false);
                m_ButtonAiVsAi.gameObject.SetActive(false);
            }
        }
    }

    public void ActivePopupWinLose(int isWin, float delayTime) //0:Win/ 1:lose/ 2:hoa/
    {
        if (isWin == 0)
        {
            DOVirtual.DelayedCall(delayTime, () =>
            {
                m_UiWinLose.SetActive(true);
                m_PlayChessOffline.transform.localScale = Vector3.zero;
                m_ImageWinLose.sprite = m_TitleWin;
                m_ImageWinLose.SetNativeSize();
                UnlockLevel();
            });
        }
        else if (isWin == 1)
        {
            DOVirtual.DelayedCall(delayTime, () =>
            {
                m_UiWinLose.SetActive(true);
                m_PlayChessOffline.transform.localScale = Vector3.zero;
                m_ImageWinLose.sprite = m_TitleLose;
                m_ImageWinLose.SetNativeSize();
            });
        }

        if (isWin == 2)
        {
            DOVirtual.DelayedCall(delayTime, () =>
            {
                m_UiWinLose.SetActive(true);
                m_PlayChessOffline.transform.localScale = Vector3.zero;
                m_ImageWinLose.sprite = m_TitleDraw;
                m_ImageWinLose.SetNativeSize();
            });
        }

        DOVirtual.DelayedCall(delayTime, () =>
        {
            m_Gamemanager.blackClock.isTurnToMove = false;
            m_Gamemanager.whiteClock.isTurnToMove = false;
        });
    }

    public void ClaimAndShowMoneyWithFx(int totalChipOrDiamond, int chipBonus, Button btn)
    {
        coinEffect = btn.GetComponent<CoinEffect>();
        coinEffect.AddCoins(totalChipOrDiamond, totalChipOrDiamond + chipBonus);
        totalChipOrDiamond += chipBonus;
    }

    public bool humanPlayWhite = true;

    public void UpdateNumberOfCapturedPieces(bool whiteTurn, int capturedType, bool humanPlayWhite)
    {
        //TODO: con truong hop choi onl human vs human
        canCount = !canCount;
        if (!canCount) return;
        if (whiteTurn && humanPlayWhite || !whiteTurn && !humanPlayWhite)
        {
            switch (capturedType)
            {
                case 2:
                    blackCountPawnCaptured++;
                    blackCountPawnCapturedTMP.text = $"{blackCountPawnCaptured}";
                    break;
                case 3:
                    blackCountKnightCaptured++;
                    blackCountKnightCapturedTMP.text = $"{blackCountKnightCaptured}";
                    break;
                case 5:
                    blackCountBishopCaptured++;
                    blackCountBishopCapturedTMP.text = $"{blackCountBishopCaptured}";
                    break;
                case 6:
                    blackCountRookCaptured++;
                    blackCountRookCapturedTMP.text = $"{blackCountRookCaptured}";
                    break;
                case 7:
                    blackCountQueenCaptured++;
                    blackCountQueenCapturedTMP.text = $"{blackCountQueenCaptured}";
                    break;
            }
        }
        else if (whiteTurn && !humanPlayWhite || !whiteTurn && humanPlayWhite)
        {
            switch (capturedType)
            {
                case 2:
                    whiteCountPawnCaptured++;
                    whiteCountPawnCapturedTMP.text = $"{whiteCountPawnCaptured}";
                    break;
                case 3:
                    whiteCountKnightCaptured++;
                    blackCountKnightCapturedTMP.text = $"{whiteCountKnightCaptured}";
                    break;
                case 5:
                    whiteCountBishopCaptured++;
                    whiteCountBishopCapturedTMP.text = $"{whiteCountBishopCaptured}";
                    break;
                case 6:
                    whiteCountRookCaptured++;
                    whiteCountRookCapturedTMP.text = $"{whiteCountRookCaptured}";
                    break;
                case 7:
                    whiteCountQueenCaptured++;
                    whiteCountQueenCapturedTMP.text = $"{whiteCountQueenCaptured}";
                    break;
            }
        }
    }

    public void ResetNumberCaptureType()
    {
        blackCountPawnCaptured = 0;
        blackCountKnightCaptured = 0;
        blackCountBishopCaptured = 0;
        blackCountRookCaptured = 0;
        blackCountQueenCaptured = 0;
        blackCountPawnCapturedTMP.text = $"{blackCountPawnCaptured}";
        blackCountKnightCapturedTMP.text = $"{blackCountKnightCaptured}";
        blackCountBishopCapturedTMP.text = $"{blackCountBishopCaptured}";
        blackCountRookCapturedTMP.text = $"{blackCountRookCaptured}";
        blackCountQueenCapturedTMP.text = $"{blackCountQueenCaptured}";

        whiteCountPawnCaptured = 0;
        whiteCountKnightCaptured = 0;
        whiteCountBishopCaptured = 0;
        whiteCountRookCaptured = 0;
        whiteCountQueenCaptured = 0;
        whiteCountPawnCapturedTMP.text = $"{whiteCountPawnCaptured}";
        blackCountKnightCapturedTMP.text = $"{whiteCountKnightCaptured}";
        whiteCountBishopCapturedTMP.text = $"{whiteCountBishopCaptured}";
        whiteCountRookCapturedTMP.text = $"{whiteCountRookCaptured}";
        whiteCountQueenCapturedTMP.text = $"{whiteCountQueenCaptured}";
    }

    private void UnlockLevel()
    {
        if (GameData.levelChoosing != GameData.CurrentLevel) return;
        GameData.CountUnlockNextLevel--;
        if (GameData.CountUnlockNextLevel == 0)
        {
            GameData.CurrentLevel++;
            switch (GameData.CurrentLevel)
            {
                case 2:
                    GameData.CountUnlockNextLevel = 5;
                    break;
                case 3:
                    GameData.CountUnlockNextLevel = 7;
                    break;
                case 4:
                    GameData.CountUnlockNextLevel = 7;
                    break;
                case 5:
                    GameData.CountUnlockNextLevel = 9;
                    break;
                case 6:
                    GameData.CountUnlockNextLevel = 9;
                    break;
                case 7:
                    GameData.CountUnlockNextLevel = 11;
                    break;
                case 8:
                    GameData.CountUnlockNextLevel = 15;
                    break;
                case 9:
                    GameData.CountUnlockNextLevel = 20;
                    break;
            }
        }
    }

    public void ChangeCountChessEatEndPromotionBlackOrWhite(bool humanPlaysWhite)
    {
        for (int i = 0; i < m_listSpriteBlackChess1.Count; i++)
        {
            if (humanPlaysWhite)
            {
                m_listCounEatChessInPlayer1[i].sprite = m_listSpriteBlackChess1[i];
                m_listCountEatChessInPlayer2[i].sprite = m_listSpriteWhiteChess1[i];
                m_listCountPromotionChessInPlayer1[i].sprite = m_listSpriteBlackChess2[i];
            }
            else
            {
                m_listCounEatChessInPlayer1[i].sprite = m_listSpriteWhiteChess1[i];
                m_listCountEatChessInPlayer2[i].sprite = m_listSpriteBlackChess1[i];
                m_listCountPromotionChessInPlayer1[i].sprite = m_listSpriteWhiteChess2[i];
            }
        }
    }
}

[Serializable]
public class DutBlind
{
    public int file;
    public int rank;

    public DutBlind(int file, int rank)
    {
        this.file = file;
        this.rank = rank;
    }

    public bool Match(int f, int r)
    {
        return file == f && rank == r;
    }
}