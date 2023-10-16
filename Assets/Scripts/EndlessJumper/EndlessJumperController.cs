using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessJumperController : MonoBehaviour
{
    public Vector2 groundSpeed;
    private float activeSpeed;

    public List<GameObject> sections;
    public GameObject textDisplayGroup;
    private List<GameObject> trackedSections = new List<GameObject>();
    private Dictionary<int, List<GameObject>> sectionPooling = new Dictionary<int, List<GameObject>>();

    public bool inGame = false;
    public Transform spawnPoint;
    public GameObject player;
    public GameObject blood;
    public float gameLength;
    private Vector3 playerStartPos;
    private bool lastTall;
    private int pitCount;
    private float progress = 0f;
    private bool win = false;
    private Vector3 playerFreeze = new Vector3();

    private void FixedUpdate()
    {

        if (!inGame) return;
        for (int i = 0; i < trackedSections.Count; i++) {
            if (i == 0) {
                trackedSections[i].transform.position += Vector3.left * Time.deltaTime * activeSpeed;
            }
            else {
                trackedSections[i].transform.position = new Vector3(trackedSections[i - 1].transform.position.x + 1f, trackedSections[i].transform.position.y, trackedSections[i].transform.position.z);
            }
        }
        //foreach (GameObject go in trackedSections) {
        //    go.transform.position += Vector3.left * Time.deltaTime * activeSpeed;
        //}
        progress += Time.deltaTime;
        activeSpeed = Mathf.Lerp(groundSpeed.x, groundSpeed.y, progress / gameLength);
        if (progress >= gameLength) {
            Win();
        }
    }

    [ContextMenu("Check Dic")]
    public void CheckDic()
    {
        foreach (KeyValuePair<int, List<GameObject>> pair in sectionPooling) {
            Debug.Log(pair.Key + " and " + pair.Value.Count);
        }
    }

    private void Awake()
    {
        for (int i = 0; i < 3; i++) {
            sectionPooling.Add(i, new List<GameObject>());
            for (int j = 0; j < 5; j++) {
                GameObject tempGo = Instantiate(sections[i],this.gameObject.transform);
                sectionPooling[i].Add(tempGo);
                tempGo.SetActive(false);
            }
        }
        playerStartPos = player.transform.position;
        GameManager.NewVictimSpawned += NewVictomSpawn;
    }

    private void OnEnable()
    {
        if (!win) {
            Reset();
        }
        
    }

    void GenerateInitialGround()
    {
        for (int i = 13; i > 0; i--) {
            
            trackedSections.Add(GetPeice(sections[0], i, true));
        }
        //inGame = true;
    }

    void GenerateNewGround()
    {
        if (progress / gameLength >= .9) {
            trackedSections.Add(GetPeice(sections[0], 0, trackedSections.Count > 0));
            return;
        }
        if (lastTall) {
            trackedSections.Add(GetPeice(sections[0], 0, trackedSections.Count > 0));
        }
        else {
            if (pitCount >= 1) {
                trackedSections.Add(GetPeice(sections[0], 0, trackedSections.Count > 0));
            }
            else {
                int pieceIndex = Random.Range(0, sections.Count);
                trackedSections.Add(GetPeice(sections[pieceIndex], 0, trackedSections.Count > 0));
                if (pieceIndex == 1) pitCount++;
                if (pieceIndex == 2) lastTall = true;
                return;
            }
        }
        pitCount = 0;
        lastTall = false;
    }

    GameObject GetPeice(GameObject type, int offset, bool spawnPointPos)
    {
        GameObject go;
        float spawnPos = spawnPoint.position.x;
        if (!spawnPointPos) {
            spawnPos = trackedSections[trackedSections.Count - 1].transform.position.x + 1;
        }
        if (sectionPooling.TryGetValue(type.GetComponent<GroundId>().index, out List<GameObject> gos)) {
            if (gos.Count > 0) {
                go = gos[0];
                gos.Remove(go);
                go.transform.position = new Vector3(spawnPos - offset, -1.35f, 0);
            }
            else {
                go = Instantiate(type, new Vector3(spawnPos - offset, -1.35f, 0), this.transform.rotation, this.gameObject.transform);
            }
        }
        else {
            go = Instantiate(type, new Vector3(spawnPos - offset, -1.35f, 0), this.transform.rotation, this.gameObject.transform);

        }
        go.SetActive(true);
        return go;
    }

    public void ReturnGround(GameObject go)
    {
        go.SetActive(false);
        if (sectionPooling.TryGetValue(go.GetComponent<GroundId>().index, out List<GameObject> gos)) {
            gos.Add(go);
        }
        else {
            sectionPooling.Add(go.GetComponent<GroundId>().index, new List<GameObject>());
            sectionPooling[go.GetComponent<GroundId>().index].Add(go);
        }
        if (trackedSections.Contains(go) && inGame){
            trackedSections.Remove(go);
            GenerateNewGround();
        }
    }

    private void OnDisable()
    {
        inGame = false;
    }

    public void Die(Vector3 playerPos)
    {
        StartCoroutine(DeathAnim(playerPos));
    }

    IEnumerator DeathAnim(Vector3 playerPos)
    {
        inGame = false;
        blood.SetActive(true);
        float elaspedTime = 0f;
        while (elaspedTime < 2f) {
            player.GetComponent<Rigidbody>().position = playerPos;
            elaspedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Reset();
    }

    void NewVictomSpawn()
    {
        win = false;
    }

    public void Reset()
    {
        blood.SetActive(false);
        inGame = false;
        player.GetComponent<Rigidbody>().useGravity = true;
        foreach (GameObject go in trackedSections) {
            ReturnGround(go);
        }
        trackedSections.Clear();
        progress = 0f;
        GenerateInitialGround();
        player.transform.position = playerStartPos;
        inGame = true;
        win = false;
        textDisplayGroup.SetActive(false);
        activeSpeed = groundSpeed.x;
    }

    public void Win()
    {
        win = true;
        textDisplayGroup.SetActive(true);
    }

    private void OnDestroy()
    {
        GameManager.NewVictimSpawned -= NewVictomSpawn;
    }
}
