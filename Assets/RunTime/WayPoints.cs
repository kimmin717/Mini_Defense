using UnityEngine;

public class WayPoints : MonoBehaviour
{
    public static Transform[] _points;

    private void Awake()
    {
        // childCount : 웨이 포인트 안에 개체들이 배열의 길이가 된다. 내 경우에는 (0 ~ 15) 의 배열을 같는다.
        _points = new Transform[transform.childCount]; 


    }


}
