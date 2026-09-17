using TMPro;
using UnityEngine;

public class Blackhole_Key_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode mKey;
    private TextMeshProUGUI mText;

    private Transform enemy;
    private Blackhole_Skill_Controller blackhole;
    private bool selected;
    public void SetupKey(KeyCode _mNewKey,Transform _mEnemy,Blackhole_Skill_Controller _mBlackhole)
    {
        sr=GetComponent<SpriteRenderer>();
        mText = GetComponentInChildren<TextMeshProUGUI>();

        mKey = _mNewKey;
        mText.text = _mNewKey.ToString();

        enemy = _mEnemy;
        blackhole = _mBlackhole;
    }

    private void Update()
    {
        if (!selected && Input.GetKeyDown(mKey))
        {
            selected = true;
            blackhole.AddEnemyToList(enemy);

            mText.color= Color.clear;
            sr.color = Color.clear;
        }

    }
}
