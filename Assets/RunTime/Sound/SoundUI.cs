using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundUI : MonoBehaviour
{
    public static SoundUI _instance { get; private set; }

    [Header("버튼 오디오 소스")]
    [SerializeField] private AudioSource _buttonSound;

    [Header("버튼 효과음 파일")]
    [SerializeField] private AudioClip _buttonClip;

    private void Awake()
    {
        if(_instance != null)
        {
            CPrint.Warn("중복 SoundUI가 존재하여 새로 생성된 오브젝트를 삭제");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnButtonClick()
    {
        if (_buttonSound != null && _buttonClip != null)
        {
            _buttonSound.PlayOneShot(_buttonClip);
        }
    }
}
