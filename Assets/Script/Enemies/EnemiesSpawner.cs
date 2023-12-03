using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemiesSpawner : MonoBehaviour
{
    public PlayerObj _playerObj;

    public static EnemiesSpawner Instance;

    public GameObject SpawnRect;
    public GameObject FightRect;

    public AnimationCurve _numberOfEnemy1PerWave;
    public AnimationCurve _numberOfEnemy2PerWave;
    public AnimationCurve _numberOfEnemy3PerWave;

    private EnemyController _currentEnemy;

    private Vector3 UpLeft = new();
    private Vector3 DownRight = new();

    private Vector3 RandPos = new Vector3 (0, 0, 0);

    private float _timerInWave = 0;
    private float _timeInWave = 10;
    private float _timeBetweenWave = 5;

    [SerializeField] private int numberOfObjectActive = 0;
    private int newActive = 0;
    [SerializeField] private int[] numberOfEnemiesInWave;
    [SerializeField] private int numberOfEnemiesInWaves = 0;

    private bool _inNewWave = false;

    public TextMeshProUGUI _scoreTxt;
    public TextMeshProUGUI _waveTxt;

    [SerializeField] private List<int> _EnemiesCanBeInvoke = new List<int>(); 

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        numberOfEnemiesInWave = new int[3] { 0, 0, 0 };
        _playerObj.wave = 0;
        _playerObj.score = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if(numberOfObjectActive <= 0 && numberOfEnemiesInWaves <= 0 && !_inNewWave)
        {
            numberOfObjectActive = 0;
            numberOfEnemiesInWaves = 0;
            StartCoroutine(NewWave());
        }
        else if(numberOfObjectActive <= 1 /*|| _timerInWave < Time.time*/) 
        {
            ContinueWave();
        }

        _scoreTxt.text = "Score : " + _playerObj.score;
        _waveTxt.text = "Wave : " + _playerObj.wave;
    }

    private IEnumerator NewWave()
    {
        _inNewWave = true;

        yield return new WaitForSecondsRealtime(_timeBetweenWave);

        _playerObj.wave++;
        _EnemiesCanBeInvoke.Clear();
        for (int i = 0; i < numberOfEnemiesInWave.Length; i++)
        {
            switch(i)
            {
                case 0:
                    numberOfEnemiesInWave[i] = Mathf.CeilToInt(_numberOfEnemy1PerWave.Evaluate(_playerObj.wave));
                    _EnemiesCanBeInvoke.Add(i);
                    break; 
                case 1:
                    numberOfEnemiesInWave[i] = Mathf.CeilToInt(_numberOfEnemy2PerWave.Evaluate(_playerObj.wave));
                    _EnemiesCanBeInvoke.Add(i);
                    break; 
                case 2:
                    numberOfEnemiesInWave[i] = Mathf.CeilToInt(_numberOfEnemy3PerWave.Evaluate(_playerObj.wave));
                    _EnemiesCanBeInvoke.Add(i);
                    break;
            }
        }
        numberOfEnemiesInWaves = Mathf.CeilToInt(_numberOfEnemy1PerWave.Evaluate(_playerObj.wave)) + Mathf.CeilToInt(_numberOfEnemy2PerWave.Evaluate(_playerObj.wave)) + Mathf.CeilToInt(_numberOfEnemy3PerWave.Evaluate(_playerObj.wave));
        _inNewWave = false;
    }

    private void ContinueWave()
    {
        newActive = 0;
        if (numberOfEnemiesInWaves >= 10)
        {
            numberOfEnemiesInWaves -= 10;
            numberOfObjectActive += 10;
            newActive += 10;
        }
        else if(numberOfEnemiesInWaves < 10 && numberOfEnemiesInWaves > 0 ) 
        {
            numberOfObjectActive += numberOfEnemiesInWaves;
            newActive += numberOfEnemiesInWaves;
            numberOfEnemiesInWaves = 0;
        }
        else 
        {
            return;
        }

        for (int i = 0; i < newActive; i++)
        {
            SpawnEnemy();
        }
        _timerInWave = Time.time + _timeInWave;
    }

    private void GetRandPosInRect(GameObject rect)
    {
        UpLeft = rect.transform.GetChild(0).position;
        DownRight = rect.transform.GetChild(1).position;

        RandPos.x = Random.Range(UpLeft.x, DownRight.x);
        RandPos.y = Random.Range(DownRight.y, UpLeft.y);
    }

    private void SpawnEnemy()
    {
        GetRandPosInRect(SpawnRect);
        _currentEnemy = PoolController.Instance.GetNew(GetRandomEnemy(), RandPos)?.GetComponent<EnemyController>();

        if (_currentEnemy != null)
        {
            GetRandPosInRect(FightRect);
            if (_currentEnemy._type != Type.Boss)
                _currentEnemy.SetTargetPosition = RandPos;
            else
                _currentEnemy.SetTargetPosition = new Vector3(-2.5f, 3.8f, 0);

        }
        else
        {
            numberOfObjectActive -= 1;
        }



    }

    private Type GetRandomEnemy()
    {
        if (_EnemiesCanBeInvoke.Count > 0)
        {
            int rand = Random.Range(0, _EnemiesCanBeInvoke.Count);
            if (numberOfEnemiesInWave[_EnemiesCanBeInvoke[rand]] > 0)
            {
                numberOfEnemiesInWave[_EnemiesCanBeInvoke[rand]]--;
                return GetEnemy(_EnemiesCanBeInvoke[rand]);
            }
            else
            {
                _EnemiesCanBeInvoke.Remove(_EnemiesCanBeInvoke[rand]);
                return GetRandomEnemy();
            }
        }
        return 0;
    }

    private Type GetEnemy(int i )
    {
        switch(i) 
        {
            case 0:
                return Type.Ennemies1;
            case 1:
                return Type.Ennemies2;
            case 2:
                return Type.Boss;
        }
        return Type.Ennemies1;
    }

    public void EnemyDead() => numberOfObjectActive -= 1;
    public void AddScore(int score) => _playerObj.score += score;
}
