using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Proyecto.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace Proyecto.Manager
{
    public class UIManager : Singleton<UIManager>
{
    #region Inspector Variables
    
    [Header("Panels")]
    [SerializeField] private GameObject _menuPrincipalPanel;
    [SerializeField] private GameObject _tutorialButtonAtMenuPrincipal;
    [SerializeField] private GameObject _nivelesPanel;
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private GameObject _creditosPanel;
    [SerializeField] private GameObject _pausaPanel;
    [SerializeField] private GameObject _hudPanel;
    [SerializeField] private GameObject _hudModoLibrePanel;
    [SerializeField] private GameObject _finalScorePanel;

    [Header("FinalScore")]
    [SerializeField] private GameObject _zonaImagenes;
    [SerializeField] private GameObject _zonaTexto;
    [SerializeField] private TMP_Text[] _textoRondaMaximaFinalScore;

    [Header("HUD")]
    [SerializeField] private RectTransform _progresoActualRectTransform;
    // [SerializeField] private GameObject _textRondaGB;
    [SerializeField] private Image _barraRondaActual;
    [SerializeField] private TMP_Text[] _rondasNivelInfinitoText;
    // [SerializeField] private GameObject _textTuTurnoGB;

    [Header("Stars And Lyres")]
   [SerializeField] private List<Image> _puntuacionImage;
    [SerializeField] private List<Sprite> _starsSprites; //0 -> Empty //1 -> Full
   [SerializeField] private Vector2 _starSize;
   [SerializeField] private List<Sprite> _lyresSprites; //0 -> Empty //1 -> Full
   [SerializeField] private Vector2 _lyreSize;

    [Header("DropDown List")] [SerializeField]
    private TMP_Dropdown _instrumentsDropdownList;

    [Header("Niveles")] [SerializeField] private List<Level_Info> _level_InfoList;
    [SerializeField] private TMP_Text[] _textRondaMaximaInfinito;
    
   [Header("Tutorial")] [SerializeField] private GameObject _tutorialDialogoPanel;

    #endregion

    #region Private Variables

    private GameObject _currentPanelActive;
    private int _currentNota;
    private float _acumuladoRonda;
    private int _rondaIteraction;
    private List<RectTransform> _notasProgesoList;
    private RectTransform _currentProgresoNotas;
    private List<Image> _lastNotasImages;

    #endregion

    #region Unity Methods
    
    private void Awake()
    {
#if UNITY_EDITOR
        Debug.Log("Cargando UI Manager...");         
#endif
        _instrumentsDropdownList.value = SaveManager.Instance.Options.Instrument; //Cargar opciones desde json
        
        _instrumentsDropdownList.onValueChanged.AddListener(delegate
        {
            AudioManager.Instance.ChangeInstrument(_instrumentsDropdownList.value);
            SaveManager.Instance.Options.Instrument = _instrumentsDropdownList.value;
            StartCoroutine(playNota());
        });

        _tutorialButtonAtMenuPrincipal.SetActive(!SaveManager.Instance.PlayTutorial);
        _currentPanelActive = _menuPrincipalPanel;
        _currentPanelActive.SetActive(true);
        
        _optionsPanel.SetActive(false);

        _acumuladoRonda = 0;
        
        _notasProgesoList = new List<RectTransform>();
        _notasProgesoList.Add(_progresoActualRectTransform);
        _lastNotasImages = _progresoActualRectTransform.GetComponent<NotasUtility>().NotasImagenes;
    }

    #endregion

    #region Public Methods

    public void SetActiveFinalScorePanel(bool isActive)
    {
        _finalScorePanel.SetActive(isActive);
        if (!isActive)
        {
            RestartRondaActual();
            SetupProgresoNotas();
        }
        _hudPanel.SetActive(!isActive);
    }

    public void SetFinalScore(int stars = 0, int lyres = 0)
    {
        if (stars > 0)
        {
            for (int i = 0; i < stars; i++)
            {
                _puntuacionImage[i].sprite = _starsSprites[1]; // Estrella llena
                _puntuacionImage[i].rectTransform.sizeDelta = _starSize;
            }
        }

        if (GameplayManager.Instance.ShowVisualHelp)
        {
            for (int j = stars; j < _puntuacionImage.Count; j++)
            {
                _puntuacionImage[j].sprite = _starsSprites[0]; //Estrella vacia
                _puntuacionImage[j].rectTransform.sizeDelta = _starSize;
            }
        }

        if (lyres > 0)
        {
            for (int i = 0; i < lyres; i++)
            {
                _puntuacionImage[i].sprite = _lyresSprites[1]; // lyra llena
                _puntuacionImage[i].rectTransform.sizeDelta = _lyreSize;
            }
        }

        if (!GameplayManager.Instance.ShowVisualHelp)
        {
            for (int j = lyres; j < _puntuacionImage.Count; j++)
            {
                _puntuacionImage[j].sprite = _lyresSprites[0]; // lyra vacia
                _puntuacionImage[j].rectTransform.sizeDelta = _lyreSize;
            }
        }

        if (!GameplayManager.Instance.IsCountLimited)
        {
            _zonaImagenes.SetActive(false);
            _zonaTexto.SetActive(true);
            _textoRondaMaximaFinalScore[0].text = GameplayManager.Instance.ShowVisualHelp 
                ? SaveManager.Instance.Score.LevelList[^1].Stars.ToString() 
                : SaveManager.Instance.Score.LevelList[^1].Lyres.ToString();
            _textoRondaMaximaFinalScore[1].text = GameplayManager.Instance.ShowVisualHelp 
                ? "<sprite=1>" 
                : "<sprite=4>";
        }
        else
        {
            _zonaImagenes.SetActive(true);
            _zonaTexto.SetActive(false);
        }

        SetActiveFinalScorePanel(true);
        if(GameplayManager.Instance.IsCountLimited) DialogosFinalesManager.Instance.ShowFinalText(stars > 0 ? stars : lyres);
    }

    #region Navegacion Methods
    
    public void ShowNivelesFromMenuPrincipal()
    {
        if (TutorialManager.Instance.PlayNivelesTutorial) //Play tutorial
        {
            _menuPrincipalPanel.SetActive(false);
            _nivelesPanel.SetActive(true);
            _currentPanelActive = _nivelesPanel;
            showLevelPuntuacion();
            TutorialManager.Instance.PlayTutorial(2);
        }
        else //Ya se ha visto el tutorial
        {
            _menuPrincipalPanel.SetActive(false);
            _nivelesPanel.SetActive(true);
            _currentPanelActive = _nivelesPanel;
            showLevelPuntuacion();
        }
    }

    public void StartModoLibre()
    {
        GameplayManager.Instance.IsModoLibre = true;
        _menuPrincipalPanel.SetActive(false);
        GameplayManager.Instance.IsGameOnPause = false;
        CameraController.Instance.MoveToGameplayPosition();
    }

    public void ShowOptionFromMenuPrincipal()
    {
        if (TutorialManager.Instance.PlayOpcionesTutorial)
        {
            _menuPrincipalPanel.SetActive(false);
            _optionsPanel.SetActive(true);
            _currentPanelActive = _optionsPanel;
            TutorialManager.Instance.PlayTutorial(4);
        }
        else
        {
            _menuPrincipalPanel.SetActive(false);
            _optionsPanel.SetActive(true);
            _currentPanelActive = _optionsPanel;
        }
    }

    public void ShowTutorialFromMenuPrincipal()
    {
        TutorialManager.Instance.PlayTutorial(1);
    }

    public void ShowCreditosFromMenuPrincipal()
    {
        _menuPrincipalPanel.SetActive(false);
        _creditosPanel.SetActive(true);
        _currentPanelActive = _creditosPanel;
    }

    public void ReturnToMenuPrincipal()
    {
        if (_currentPanelActive.name.Equals(_optionsPanel.name)) SaveManager.Instance.UpdateOptions();
        _currentPanelActive.SetActive(false);
        _menuPrincipalPanel.SetActive(true);
        _currentPanelActive = _menuPrincipalPanel;
    }

    public void ReturnToMenuPrincipalFromPausa()
    {
        _pausaPanel.SetActive(false);
        _hudPanel.SetActive(false);
        _menuPrincipalPanel.SetActive(true);
        
        _currentPanelActive = _menuPrincipalPanel;
        CameraController.Instance.MoveToStartPosition();
        VisualKeyManager.Instance.RestartColors();
    }

    public void ReturnToMenuPrincipalFromModoLibre()
    {
        _hudModoLibrePanel.SetActive(false);
        _menuPrincipalPanel.SetActive(true);
        GameplayManager.Instance.IsModoLibre = false;
        GameplayManager.Instance.IsGameOnPause = true;
        _currentPanelActive = _menuPrincipalPanel;
        CameraController.Instance.MoveToStartPosition();
        VisualKeyManager.Instance.RestartColors();
    }

    public void ShowMenuNivelesFromMenuPrincipal()
    {
        GameplayManager.Instance.IsGameOnPause = true;
        CameraController.Instance.MoveToStartPosition();
        _finalScorePanel.SetActive(false);
        _nivelesPanel.SetActive(true);
        showLevelPuntuacion();
    }

    public void SetActiveTutorialPanel(bool value)
    {
        _tutorialDialogoPanel.SetActive(value);
    }

    public void SetActiveNivelesPanel(bool value)
    {
        _nivelesPanel.SetActive(value);
    }

    public void SetPausa(bool value)
    {
        _pausaPanel.SetActive(value);
        GameplayManager.Instance.IsGameOnPause = value;
        Time.timeScale = value ? 0 : 1;
    }

    public void ShowHUD()
    {
        if (GameplayManager.Instance.IsModoLibre) _hudModoLibrePanel.SetActive(true);
        else { _hudPanel.SetActive(true); }
    }

    public void SetMenuPrincipalWithTutorial()
    {
        _tutorialButtonAtMenuPrincipal.SetActive(true);
    }

    #endregion

    #region Hub informacion

    // public void ShowRondaUpdate(int ronda)
    // {
    //     Sequence sq = DOTween.Sequence();
    //     var rectTransform = _textRondaGB.GetComponent<RectTransform>();
    //     sq.Append(rectTransform.DOSizeDelta(new Vector2(1920, 1080), .8f).SetEase(Ease.OutBack)
    //         .SetLoops(2, LoopType.Yoyo));
    //     sq.Join(rectTransform.DOAnchorPos3DY(0, .8f).SetEase(Ease.OutExpo).SetLoops(2, LoopType.Yoyo));
    //     _textRondaGB.GetComponent<TMP_Text>().text = ronda.ToString();
    //     sq.OnPlay(() => _textRondaGB.SetActive(true));
    //     sq.OnComplete(() =>
    //     {
    //         _textRondaGB.SetActive(false);
    //     });
    //
    //     sq.Play();
    // }

    public void SetTutorialY(float y)
    {
        var rectT = _tutorialDialogoPanel.GetComponent<RectTransform>();
        rectT.anchoredPosition3D = new Vector3(rectT.anchoredPosition3D.x, -y / 2, 0);
    }
    
    // public void ShowTuTurnoUpdate()
    // {
    //     Sequence sq = DOTween.Sequence();
    //     var rectTransform = _textTuTurnoGB.GetComponent<RectTransform>();
    //     sq.Append(rectTransform.DOSizeDelta(new Vector2(1920, 1080), .4f).SetEase(Ease.OutBack)
    //         .SetLoops(2, LoopType.Yoyo));
    //     sq.Join(rectTransform.DOAnchorPos3DX(0, .4f).SetEase(Ease.OutBack));
    //     sq.Insert(0.4f, rectTransform.DOAnchorPos3DX(1200, .4f).SetEase(Ease.InBack));
    //
    //     sq.OnPlay(() => _textTuTurnoGB.SetActive(true));
    //     sq.OnComplete(() =>
    //     {
    //         _textTuTurnoGB.SetActive(false);
    //         rectTransform.anchoredPosition3D = new Vector3(-1200, 0, 0);
    //     });
    //
    //     sq.Play();
    // }

    /// <summary>
    /// Pone todas la notas a blancas, mueve el anchored3D anchuraTotal/7 pixeles a la izquierda y hace aparecer una nueva nota con fade
    /// </summary>
    public IEnumerator UpdateProgresoNotas()
    {
        yield return new WaitForSeconds(VisualKeyManager.Instance.TimeToHitLight);

        var oldIteration = _rondaIteraction;
        _rondaIteraction = (_currentNota % 100) / 10;

        _lastNotasImages = _notasProgesoList[_rondaIteraction].GetComponent<NotasUtility>().NotasImagenes;
        if (_rondaIteraction > 0) //hay más de 10 notas
        {
            //Todas las notas a blanco
            for (int i = 0; i < _notasProgesoList.Count-1; i++)
            {
                var imagenes = _notasProgesoList[i].GetComponent<NotasUtility>().NotasImagenes;
                foreach (var img in imagenes)
                {
                    img.DOColor(Color.white, 0.1f).SetEase(Ease.InCirc).Play();
                }
            }

            for (int i = 0; i < _currentNota % 10; i++)
            {
                _lastNotasImages[i]
                    .DOColor(Color.white, 0.3f)
                    .SetEase(Ease.InCirc).Play();
            }

            var lastRectTransform = _notasProgesoList[_rondaIteraction];
            var firstRectTransform = _notasProgesoList[0];
            var currentRectTransform = _notasProgesoList[_rondaIteraction-1];
            
            Sequence sq = DOTween.Sequence();
            if (!firstRectTransform.name.Equals(currentRectTransform.name))
            {
                //El grupo actual se oculta
                //MoveX rectransform
                var posx = currentRectTransform.anchoredPosition3D.x;
                sq.Append(currentRectTransform
                    .DOAnchorPosX(-450, 0.6f)
                    .OnComplete(()=>currentRectTransform.anchoredPosition3D = new Vector3(posx,40,0)));
                //Ocultar grupo
                sq.Join(currentRectTransform.GetComponent<CanvasGroup>()
                    .DOFade(0, 0.5f));
            
                sq.Play();
            }

            yield return new WaitForSeconds(0.4f);
            //Al ultimo grupo se añade una nueva nota
            Sequence sq2 = DOTween.Sequence();
            //Aparece nueva nota
            var index = _currentNota % 10;
            sq2.Append(_lastNotasImages[index]
                .DOFade(1, 0.4f)
                .OnComplete(()=>_currentNota = 0));
            //moveX rectransform
            sq2.Join(lastRectTransform
                .DOAnchorPos3DX(lastRectTransform.anchoredPosition3D.x - (240f/7), 0.4f));
            //mover derecha
            var finalPos = lastRectTransform.anchoredPosition3D.x;
            sq.AppendInterval(0.25f);
            sq2.Append(lastRectTransform
                .DOAnchorPosX(-450, 0.4f)
                .OnComplete(()=>lastRectTransform.anchoredPosition3D = new Vector3(finalPos,40,0)));
            //Ocultar grupo
            sq2.Join(lastRectTransform.GetComponent<CanvasGroup>()
                .DOFade(0, 0.4f));

            //Se muestra el primer grupo
            //MoveX rectransform
            sq2.Append(firstRectTransform
                .DOAnchorPosX(firstRectTransform.anchoredPosition3D.x, 0.4f)
                .OnPlay(()=>firstRectTransform.anchoredPosition3D = new Vector3(450,40,0)));
            //Ocultar grupo
            sq2.Join(firstRectTransform.GetComponent<CanvasGroup>()
                .DOFade(1, 0.4f));
            
            sq2.Play();
            
        }else
        {
            //Notas a blanco
            for (int i = 0; i < _currentNota; i++)
            {
                _lastNotasImages[i]
                    .DOColor(Color.white, 0.3f)
                    .SetEase(Ease.InCirc).Play();
            }
            //Aparece nueva nota
            Sequence sq = DOTween.Sequence();
            sq.Append(_lastNotasImages[_currentNota]
                .DOFade(1, 0.4f).SetEase(Ease.OutQuad)
                .OnComplete(()=>_currentNota = 0));
            //moveX rectransform
            sq.Join(_notasProgesoList[_rondaIteraction]
                .DOAnchorPos3DX(_notasProgesoList[_rondaIteraction].anchoredPosition3D.x - (240f/7), 0.5f)
                .SetEase(Ease.OutQuad));
            sq.Play();
        }
    }

    /// <summary>
    /// Pone nota actual al color de la nota que sonó y acerto el jugador
    /// </summary>
    /// <param name="color"></param>
    public void UpdateColorNota(Color color)
    {
        var oldIteration = _rondaIteraction; 
        _rondaIteraction = (_currentNota % 100) / 10;

        var images = _notasProgesoList[_rondaIteraction].GetComponent<NotasUtility>().NotasImagenes;
        //DOFade a la imagen with outCirc
        var index = _currentNota % _lastNotasImages.Count;
        images[index]
            .DOColor(color, .6f)
            .SetEase(Ease.OutCirc)
            .Play();
        
        if (_currentNota == (_rondaIteraction*10 + 9)) //Cambiar grupo cuando _currentNota=9,19,29,39
        {
            if (_notasProgesoList.Count == _rondaIteraction + 1)//no hay siguiente grupo => se crea
            {
                var gb = Instantiate(_progresoActualRectTransform.gameObject, _hudPanel.transform);
                gb.name = _progresoActualRectTransform.name + "_" + _rondaIteraction;
                var rectT = gb.GetComponent<RectTransform>();
                _notasProgesoList.Add(rectT);
                setNewRectTransformForNotes(rectT, gb.GetComponent<NotasUtility>().NotasImagenes);
            }

            var oldRectTransform = _notasProgesoList[_rondaIteraction];
            var newRectTransform = _notasProgesoList[_rondaIteraction+1];

            //se oculta el anterior grupo
            Sequence sq = DOTween.Sequence();
            //MoveX rectransform
            sq.AppendInterval(0.12f);
            sq.Append(oldRectTransform
                .DOAnchorPosX(-450, 0.1f)
                .OnComplete(()=>oldRectTransform.anchoredPosition3D = new Vector3(0,40,0)));
            //Ocultar grupo
            sq.Join(oldRectTransform.GetComponent<CanvasGroup>()
                .DOFade(0, 0.1f));

            sq.AppendInterval(0.1f);
            //Se muestra el siguiente grupo
            
            //MoveX rectransform
            sq.Join(newRectTransform
                .DOAnchorPosX(newRectTransform.anchoredPosition3D.x-240f/7, 0.1f)
                .OnPlay(()=>newRectTransform.anchoredPosition3D = new Vector3(450,40,0)));
            //mostrar grupo
            sq.Join(newRectTransform.GetComponent<CanvasGroup>()
                .DOFade(1, 0.1f));
            sq.Play();
        }
        _currentNota++;
    }
    
    /// <summary>
    /// Usado al sonar la secuencia
    /// </summary>
    /// <param name="color"></param>
    /// <param name="position">0-Infinity</param>
    /// <param name="timeToLight"></param>
    public void ShowNotaColorProgreso(Color color, int position, float timeToLight)
    {
        var oldIteration = _rondaIteraction; 
        _rondaIteraction = (position % 100) / 10; //0-9=>0 10-19=> 1 20-29=>2
        var images = _notasProgesoList[_rondaIteraction].GetComponent<NotasUtility>().NotasImagenes;
        if (_rondaIteraction > oldIteration) //Cambiar grupo
        {
            var newRectTransform = _notasProgesoList[_rondaIteraction];
            var oldRectTransform = _notasProgesoList[_rondaIteraction-1];
            //se oculta el anterior grupo
            Sequence sq = DOTween.Sequence();
            //MoveX rectransform
            sq.Append(oldRectTransform
                .DOAnchorPosX(-450, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(()=>oldRectTransform.anchoredPosition3D = new Vector3(0,40,0)));
            //Ocultar grupo
            sq.Join(oldRectTransform.GetComponent<CanvasGroup>()
                .DOFade(0, 0.1f)
                .SetEase(Ease.OutQuad));

            //Se muestra el siguiente grupo
            //MoveX rectransform
            sq.AppendInterval(0.03f);
            sq.Join(newRectTransform
                .DOAnchorPosX(newRectTransform.anchoredPosition3D.x, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnPlay(()=>newRectTransform.anchoredPosition3D = new Vector3(450,40,0)));
            //mostrar grupo
            sq.Join(newRectTransform.GetComponent<CanvasGroup>()
                .DOFade(1, 0.1f)
                .SetEase(Ease.OutQuad));
            sq.Play();
        }
        
        //DOFade a la imagen with outCirc
        var index = (position - (_rondaIteraction*10)) % 10;
        images[index]
            .DOColor(color, timeToLight)
            .SetEase(Ease.OutCirc)
            .SetLoops(2,LoopType.Yoyo)
            .Play();
    }

    /// <summary>
    /// Oculta todas las notas menos las tres primeras y mueve el AnchoredX a 240
    /// </summary>
    public void SetupProgresoNotas()
    {
        for (int i = 1; i < _notasProgesoList.Count; i++)//Delete all except first
        {
            Destroy(_notasProgesoList[i].gameObject);
        }
        _notasProgesoList = new List<RectTransform>();
        _notasProgesoList.Add(_progresoActualRectTransform);

        _lastNotasImages = _notasProgesoList[0].GetComponent<NotasUtility>().NotasImagenes;
        
        for (int i = _lastNotasImages.Count-1; i > 2; i--)
        {
            _lastNotasImages[i].color = new Color(1,1,1,0);
        }

        for (int j = 0; j < 3; j++)
        {
            _lastNotasImages[j].color = Color.white;
        }

        _progresoActualRectTransform.anchoredPosition3D =
            new Vector3(240, _progresoActualRectTransform.anchoredPosition3D.y, 0);
       
        _currentNota = 0;
        _rondaIteraction = 0;
        
    }
    
    public void ShowFirstGroup()
    {
        var firstRectTransform = _notasProgesoList[0];
        var lastRectTransform = _notasProgesoList[^1];
        
        if (firstRectTransform.name.Equals(lastRectTransform.name)) return;
        
        //se oculta el ultimo grupo
        Sequence sq = DOTween.Sequence();
        var startPos = lastRectTransform.anchoredPosition3D.x;
        //MoveX rectransform
        sq.Append(lastRectTransform
            .DOAnchorPosX(-450, 0.3f)
            .SetEase(Ease.OutQuad)
            .OnComplete(()=>lastRectTransform.anchoredPosition3D = new Vector3(startPos,40,0)));
        //Ocultar grupo
        sq.Join(lastRectTransform.GetComponent<CanvasGroup>()
            .DOFade(0, 0.3f)
            .SetEase(Ease.OutQuad));

        //Se muestra el primer grupo
        //MoveX rectransform
        sq.Append(firstRectTransform
            .DOAnchorPosX(0, 0.3f)
            .SetEase(Ease.OutQuad)
            .OnPlay(()=>firstRectTransform.anchoredPosition3D = new Vector3(450,40,0)));
        //mostrar grupo
        sq.Join(firstRectTransform.GetComponent<CanvasGroup>()
            .DOFade(1, 0.3f)
            .SetEase(Ease.OutQuad));
        sq.Play();
    }
    
    public void UpdateRondaActual(int actual)
    {
        if (!GameplayManager.Instance.IsCountLimited)
        {
            
            _rondasNivelInfinitoText[0].text = actual.ToString();
            return;
        }

        _acumuladoRonda += actual == 1 ? .14f : .12f;//1 -> 0.12+0.02 2-> +0.12 8 -> .98
            _barraRondaActual
                .DOFillAmount(_acumuladoRonda, .7f)
                .SetEase(Ease.OutCubic)
                .Play();
    }
    
    public void RestartRondaActual()
    {
        if (!GameplayManager.Instance.IsCountLimited)
        {
            _barraRondaActual.transform.parent.gameObject.SetActive(false);
            _rondasNivelInfinitoText[0].gameObject.SetActive(true);
            _rondasNivelInfinitoText[0].text = "0";
            _rondasNivelInfinitoText[1].text = GameplayManager.Instance.ShowVisualHelp ? "<sprite=1>" : "<sprite=4>";
            return;
        }
        
        _rondasNivelInfinitoText[0].gameObject.SetActive(false);
        _acumuladoRonda = 0;
        _barraRondaActual.fillAmount = _acumuladoRonda;
        _barraRondaActual.transform.parent.gameObject.SetActive(true);
        
    }
    
    #endregion

    public void SetIntrumentOnStart()
    {
        AudioManager.Instance.ChangeInstrument(_instrumentsDropdownList.value);
    }
    
    #endregion

    #region Private Methods

    private void showLevelPuntuacion()
    {
        int i = 0;
        foreach (var level in _level_InfoList)
        {
            var starsAndLyres = SaveManager.Instance.Score.LevelList[i];
            // Debug.Log(level.name);
            // Debug.Log(starsAndLyres.ToString());
            i++;
            if (starsAndLyres.Stars > 0)
            {
                for (int j = 0;
                     j < starsAndLyres.Stars;
                     j++) //starsAndLyres.stars es la cantidad de estrellas conseguidas anteriormente 0,1,2,3
                {
                    level.StartsImage[j].sprite = _starsSprites[1];
                }
            }

            if (starsAndLyres.Lyres > 0)
            {
                for (int k = 0;
                     k < starsAndLyres.Lyres;
                     k++) //starsAndLyres.lyres es la cantidad de liras conseguidas anteriormente 0,1,2,3
                {
                    level.LyresImage[k].sprite = _lyresSprites[1];
                }
            }
        }
        
        _textRondaMaximaInfinito[0].text = SaveManager.Instance.Score.LevelList[^1].Stars.ToString();
        _textRondaMaximaInfinito[1].text = SaveManager.Instance.Score.LevelList[^1].Lyres.ToString();
    }

    private IEnumerator playNota()
    {
        yield return new WaitForSeconds(0.15f);
        if (AudioManager.Instance.IsKid) AudioManager.Instance.PlayClip("C4_60");
        else AudioManager.Instance.PlayClip(60 - 57);
    }

    private void setNewRectTransformForNotes(RectTransform rectTransform, List<Image> images)
    {
        rectTransform.anchoredPosition3D =  new Vector3((240f/7)*10, _progresoActualRectTransform.anchoredPosition3D.y, 0);
        rectTransform.GetComponent<CanvasGroup>().alpha = 0;
        foreach (var img in images)
        {
            img.color = new Color(1,1,1,0);
        }
        images[0].color = Color.white;
    }

    #endregion
}
}