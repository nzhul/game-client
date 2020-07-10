using System;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Data;
using Assets.Scripts.LevelManagement;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Network.Services.TCP.Interfaces;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.UI.Lobby;
using Assets.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    private string _selectedFaction = "Sanctuary";
    private string _selectedHeroClass = "Knight";

    public FactionData[] factionData;

    [Header("Faction selection:")]
    public RectTransform _factionContainer;
    public Button _factionBtnPrefab;
    public Color selectedFactionColor;
    public Color factionNormalColor;

    [Header("Hero class selection:")]
    public RectTransform _classContainer;
    public Button _classBtnPrefab;
    public Color selectedHeroColor;
    public Color heroNormalColor;

    [Header("Play Menu")]

    [SerializeField]
    private GameObject blur = default;

    [SerializeField]
    private Button playBtn = default,
        findBtn = default,
        reconnectBtn = default,
        leaveGameBtn = default,
        logoutBtn = default;

    [SerializeField]
    private GraphicMover playMenu = default;

    [Header("Search Menu")]

    [SerializeField]
    private GraphicMover searchMenu = default;

    [SerializeField]
    private Button cancelBtn = default;

    private IWorldService _worldService;

    private void Awake()
    {
        playBtn.onClick.AddListener(OnPlayGameClicked);
        findBtn.onClick.AddListener(OnFindOpponentClicked);
        cancelBtn.onClick.AddListener(OnCancelClicked);
        reconnectBtn.onClick.AddListener(OnReconnectClicked);
        leaveGameBtn.onClick.AddListener(OnLeaveGameClicked);
        logoutBtn.onClick.AddListener(OnLogoutGameClicked);
        this.AddBlurClick();
    }

    private void Start()
    {
        _worldService = new WorldService();
        InitializeFactionButtons(_selectedFaction);
        InitializePlayButtons(DataManager.Instance.ActiveGameId);
    }

    private void InitializePlayButtons(int activeGameId)
    {
        if (activeGameId == 0)
        {
            return;
        }

        playBtn.gameObject.SetActive(false);
        reconnectBtn.gameObject.SetActive(true);
        leaveGameBtn.gameObject.SetActive(true);
    }

    private void InitializeFactionButtons(string selectedFaction)
    {
        Common.Empty(_factionContainer.transform);

        int index = 0;
        foreach (FactionData faction in factionData)
        {
            Button factionBtn = GameObject.Instantiate<Button>(_factionBtnPrefab, _factionContainer);
            factionBtn.name = faction.name + "_FactionBtn";
            factionBtn.onClick.AddListener(delegate { OnFactionBtnPressed(faction, faction.name, factionBtn); });

            var factionNameText = factionBtn.transform.Find("FactionName").GetComponent<TextMeshProUGUI>();
            factionNameText.text = faction.name.ToUpper();

            if (index == 0)
            {
                factionBtn.onClick.Invoke();
            }

            index++;
        }
    }

    private void OnFactionBtnPressed(FactionData faction, string factionName, Button factionBtn)
    {
        _selectedFaction = factionName;
        Common.HighlightButton(factionBtn, selectedFactionColor, factionNormalColor, _factionContainer);
        InitializeClassButtons(factionName);
    }

    private void InitializeClassButtons(string selectedFaction)
    {
        Common.Empty(_classContainer.transform);

        FactionData faction = factionData.FirstOrDefault(f => f.name == selectedFaction);

        int index = 0;
        foreach (FactionClass @class in faction.heroes)
        {
            Button classBtn = GameObject.Instantiate<Button>(_classBtnPrefab, _classContainer);
            classBtn.name = @class.name + "_HeroBtn";
            classBtn.onClick.AddListener(delegate { OnHeroClassBtnPressed(faction, @class.name, classBtn); });

            var classNameText = classBtn.transform.Find("ClassName").GetComponent<TextMeshProUGUI>();
            classNameText.text = @class.name.ToUpper();

            if (index == 0)
            {
                classBtn.onClick.Invoke();
            }

            index++;
        }
    }

    private void OnHeroClassBtnPressed(FactionData faction, string heroClass, Button classBtn)
    {
        _selectedHeroClass = heroClass;
        FactionClass @class = faction.heroes.FirstOrDefault(x => x.name == heroClass);
        Common.HighlightButton(classBtn, selectedHeroColor, heroNormalColor, _classContainer);
    }

    private void OnFindOpponentClicked()
    {
        this.OnBlurClicked(null);
        this.playBtn.gameObject.SetActive(false);
        this.searchMenu.gameObject.SetActive(true);
        this.searchMenu.Move(MoveTarget.End);

        _worldService.FindOpponentRequest((CreatureType)Enum.Parse(typeof(CreatureType), _selectedHeroClass, true));
    }

    private void OnCancelClicked()
    {

        this.searchMenu.gameObject.transform.position = this.searchMenu.startXform.position;
        this.searchMenu.gameObject.SetActive(false);
        playBtn.gameObject.SetActive(true);
        _worldService.CancelFindOpponentRequest();
    }

    private void AddBlurClick()
    {
        var trigger = blur.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnBlurClicked((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    private void OnPlayGameClicked()
    {
        blur.SetActive(true);
        findBtn.gameObject.SetActive(true);
        playBtn.gameObject.SetActive(false);
        playMenu.Move(MoveTarget.End);
    }

    private void OnBlurClicked(PointerEventData data)
    {
        blur.SetActive(false);
        findBtn.gameObject.SetActive(false);
        playBtn.gameObject.SetActive(true);
        playMenu.Move(MoveTarget.Start);
    }

    private void OnReconnectClicked()
    {
        var gameId = DataManager.Instance.ActiveGameId;
        if (gameId == 0)
        {
            Debug.LogWarning("There is no active game to reconnect to ...");
            return;
        }

        RequestManagerTcp.GameService.Reconnect(gameId);
        GameManager.Instance.LoadScene(LevelLoader.GAME_SCENE);
    }

    private void OnLeaveGameClicked()
    {
        throw new NotImplementedException();
    }

    private void OnLogoutGameClicked()
    {
        RequestManagerTcp.UserService.SendLogoutRequest();
        GameManager.Instance.LoadScene(LevelLoader.MAIN_MENU_SCENE);
    }
}


// NOTE: How to change svg color
//var image = leaveGameBtn.gameObject.GetComponentInChildren<SVGImage>();
//image.color = Color.red;
