using UnityEngine;
using System.Collections.Generic;


public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private List<KeyCode> KeyCodeList;

    public float maxSize;
    public float growSpeed;
    public float shrinkSpeed;
    public bool canGrow;
    public bool canShrink;
    private float quitTimer = 1f;

    private bool canAttack;
    private int amountOfAttacks;
    private float shadowAttackCD;
    private float shadowAttackTimer;

    private List<Transform> targets=new List<Transform>();
    private List<GameObject> createdKey=new List<GameObject>();

    public void SetupBlackhole(float  _maxSize,float _growSpeed,float _shrinkSpeed,int _amountOfAttacks,float _shadowAttackCD)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        shadowAttackCD = _shadowAttackCD;
    }

    private void Update()
    {
        quitTimer -= Time.deltaTime;
        shadowAttackTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && quitTimer < 0)
        {
            canAttack = false;
            canShrink = true;
            PlayerManager.instance.player.ExitBlackhole();
            DestroyKeys();
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            if (targets.Count <= 0)
            {
                Debug.LogWarning("Press the key shown above an enemy before starting shadow attacks.");
            }
            else
            {
                DestroyKeys();
                canAttack = true;
            }
        }

        ShadowAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), growSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    private void ShadowAttackLogic()
    {
        if (targets.Count <= 0)
            return;

        if (shadowAttackTimer < 0 && canAttack)
        {
            shadowAttackTimer = shadowAttackCD;

            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;
            if (Random.Range(0, 100) > 50)
                xOffset = 2;
            else
                xOffset = -2;
            SkillManager.Instance.shadow.CreateShadowNoIai(targets[randomIndex], new Vector3(xOffset, 0));
            amountOfAttacks--;

            if (amountOfAttacks <= 0)
            {
                canAttack = false;
                canShrink = true;
                DestroyKeys();
                PlayerManager.instance.player.ExitBlackhole();
            }
        }
    }

    private void DestroyKeys()
    {
        if (createdKey.Count <= 0)
            return;
        for(int i = 0; i < createdKey.Count; i++)
        {
            Destroy(createdKey[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>()!=null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateKey(collision);

        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if(collision.GetComponent<Enemy>()!=null)
    //        collision.GetComponent<Enemy>().FreezeTime(false);
    //}

    private void OnTriggerExit2D(Collider2D collision) => collision.GetComponent<Enemy>()?.FreezeTime(false);

    private void CreateKey(Collider2D collision)
    {
        if (KeyCodeList.Count <= 0)
        {
            Debug.LogWarning("Not enough keys");
            return;
        }

        GameObject newKey = Instantiate(keyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdKey.Add(newKey);


        KeyCode choosenKey = KeyCodeList[Random.Range(0, KeyCodeList.Count)];
        KeyCodeList.Remove(choosenKey);

        Blackhole_Key_Controller newKeyScript = newKey.GetComponent<Blackhole_Key_Controller>();

        newKeyScript.SetupKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);

}
