using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#region 유틸리티 : CPrint
/*
 
- 구조화를 시키고 싶다 -> 출력에 관한것을...

- 프린트는 콘솔 프로젝트에서 사용한것 처럼 출력 규칙을 유니티 스럽게 바꾼 버전

ㆍ 유니티에서 뭔가를 만들려고 할때...

C# -> Main

유니티 -> 유니티 생성주기에 올리고 대신 호출을 하게 한다.

유니티에서는...... global using 을 권장하지 않는다.
 ㄴ 이놈 자체가 자동화가 완벽하게 안된다.
 
ㆍ 유니티에서 전역 스럽게 (느낌) 사용하고 싶다

1. 글로벌 네임스페이스 -> static
 ㄴ 별다른 using 없어도 잘 들어감

2. 네임 스페이스는 유지하되 풀네임 호출로 통일한다.
 ㄴ Commom,CPrint.Title("")
 ㄴ 어디 소속인지 명확하다.
 
3. usinr static
 ㄴ 추적이 어려움

--------------------------------------------------------------

▶ CPrint 업그레이드

- 로그 -> 구조 / 종류

- CPrint는 런타임에서도 쓸 수 있기 때문에 기본은 Editor 처리를 하지 않는다.
 ㄴ 필요하다면 전용 유틸로 분리한다.

 */
#endregion

public static class CPrint
{
    // 옵션
    // 스위치
    public static bool Enable = true;
    //서식 태그 (콘솔 컬러 태그)
    public static bool EnbleRichText = true;

    // 들여쓰기
    // 가독성을위해 -> 출력 앞에 공백을 붙여 구조적으로 분리하기가 좋다.
    //  ㄴ 묶음 / 같은 덩어리인지..? -> 트리 구조처럼 만들겠다. -> 가독성
    private static int _indentLevel = 0;
    private const int INDENT_SPACES = 2;

    // HashSet : 중복을 허용하지 않고 고유한 요소만 저장한다. (자료구조)
    //  ㄴ 일반적으로 O(1) 수준을 보인다.
    // readonly : 
    private static readonly HashSet<string> _onceSet = new HashSet<string>();

    /*
     readonly
      ㄴ 런

    - 한번 정해진 이후에는 다시 대입하지 못하게 막는다.
     ㄴ 초기화 -> 선언부에서 하거나 / 생성자에서 하거나

    - MonoBhaviour 때문에 생성자를 직접적으로 사용하는 경우는 C# 대비 많지 않다.

    ▶ 해시셋

    - 컬렉션 클래스 -> 해시 테이블 기반 -> 데이터 구조
     ㄴ 중복되지 않은 요소들의 모임을 관리 / 이럴 경우 최적화 되어 있고 탐색이 빠름 -> 추가 및 삭제도 가능

    - 비전공자 학생들은 생소한 내용
    - 전공자 학생들은 익숙 / 들어본적이 있는 내용이여야 한다.
     
     해시 테이블
    - 키 / 값이 쌍으로 데이터를 저장하는 자료구조
    - 키를 이용해 (해시 함수) 특정 인덱스로 접근 (혹은 변환) -> 데이터 저장 -> 삽입 / 삭제 / 검색이 빠르다.

    ※ 이번주 기술 노션 확인 예정

    ㆍ 내부 동작
     
    1. 해시 함수

    - 값을 -> 해시 코드로(정수) 바꾼다.
     ㄴ 같은 값이면 같은 해시 코드가 나오는게 이 자료구조의 목표이기 때문
     ㄴ 1(해시 코드) 1(해시 코드)

    2. 버킷

    - 해시 코드를 기준으로 저장 위치(버킷)를 고른다.
     ㄴ 대충 "해시코드 % 버킷개수" 같은 방식으로 인덱스를 결정한다고 생각하면 된다.

    3. 충돌

    - 서로 다른 값인데 해시 코드가 겹칠 수 있다. (+버킷)
     ㄴ 버킷 안에서 추가 비교등을 수행해서 진짜 같은 값인지 확인
    - HashSet -> 해시 + 실제 비교를 같이 쓴다.

    4. 재해싱
    - 안에 요소가 많아지면 -> 버킷이 뻑뻑해 진다. -> 성능 떨어질 수 있음
    - 더 큰 테이블(버킷)을 만들고 다시 배치한다. (재해싱)

    ※ 중복없이 저장 + 빠른 컨테이너

    ※ 내부는 해시로 위치 찾고 -> 충돌은 비교로 해결 -> 많아지면 재해싱
     
    */

    // 들여쓰기 문자열
    private static string Indent
    {
        get 
        { 
            // 레벨 * 공백수
            // 로그쪽으로..
        return new string(' ', _indentLevel * INDENT_SPACES);       
        }
    
    }

    // 단계별 출력을 줄 맞춰서 읽기 쉽게 만든다.
    
    public static void IndentPush()
    {
        // 단께 올리고
    _indentLevel++;
    }

    public static void IndentPop()
    {
        // 단게 내리기
        _indentLevel--;

        // 예외처리
        if (_indentLevel < 0)
        {
            _indentLevel = 0;
        }
    }

    public static void TndentReset()
    {
        _indentLevel = 0;
    }

    private enum ELogKind
    { 
    Log,
    Warn,
    Error,
    Success
    }

    // 출력 포맷 관리를 위해
    // 들여쓰기 / 접두사 / 리치 텍스트 -> kind 분류
    private static void Emit(ELogKind kind, string msg, string tag = null, string colorHex = null)
    {
        // 지금까지 만든 문자열을 콘솔로 내보내는 출력 토어

        // 색상값 -> 헥스를 스는 이유 -> 가장 범용적인 방식 (문자열로 색을 표현하기에 가장 무난)
        //  ㄴ 1. 표준이고 무난함
        //  ㄴ 2. 문자열 -> 로그 포멧을 만들때 바로 끼워 넣기 좋음
        //  ㄴ 3. 16진수 -> 압축이 잘됨 (RGB)
        // - RGB(0, 0, 0) -> 0 ~ 255 -> FF == 255 / 00 = 0
        // EX : #FF0000 / #00FF00 / #0000FF / #FFFFFF
        // 구글 -> 헥스코드 색상표

        if (!Enable)
        {
            return;
        }
        // 접두사 만들기 -> tag가 있으면 해당되는 프리픽스를 만든다.
        // 단 , tag 가 null / 빈 문자열이면 접두사 없이 msg만 출력
        string prefix = string.Empty;

        if (!string.IsNullOrEmpty(tag))
        {
            // t / colorHex -> tag 부분만 색을 입히겠다. -> 가독성
            if (EnbleRichText && !string.IsNullOrEmpty(colorHex))
            {
                prefix = $"<color={colorHex}>[{tag}]</color>";
            }

            else 
            {
                // (리치 텍스트를 사용안하거나) 색상이 없다면 기본 형태로 만든다.
                // 공백이 있어야 msg랑 안 붙는다.
                prefix = $"[{tag}] ";
            }
        }

        string final = $"{Indent}{prefix}{msg}";

        // 로그 종류에 맞게 통일
        switch (kind)
        { 
                case ELogKind.Log:

                case ELogKind.Success:
                Debug.Log(final);
                break;

                case ELogKind.Warn:
                Debug.LogWarning(final);
                break;

                case ELogKind.Error:
                Debug.LogError(final);
                break;

                
        }

    }

    // #error version

    // Title / Section
    public static void Title(string title, char lineCh = '=')
    {
        Line(lineCh);
        Emit(ELogKind.Log, title);
        Line(lineCh);
    }

    public static void Section(string section, char lineCh = '-')
    {
        Emit(ELogKind.Log, section);
        Line(lineCh);
    }

    // Line / Blank
    // 구분선을 상황에 맞게 바꿀수 있도록 문자 / 길이를 옵션을 준것
    // 여기 다시 필기
    public static void Line(char ch = '=', int count = 10)
    {
        Emit(ELogKind.Log, new string(ch, count));
    }

    public static void Blank(int lines = 1)
    {
        // 콘솔에 빈줄만 추가 
        //  ㄴ Emit 붙이면 인덴트가 붙기 때문에 애매해 진다.

        if (!Enable)
        {
            return;
        }

        // 빈줄 여러 줄
        if (lines <= 0)
        {
            return;
        }

        Debug.Log(new string('\n', lines));
    } 

    // Log / Warn / Error
    public static void Log(string msg)
    {
        Emit(ELogKind.Log, msg);
    }

    public static void Warn(string msg)
    {
        // 주황 느낌
        Emit(ELogKind.Warn, msg, "WARN","#FF9100");
    }
    public static void Error(string msg)
    {
        // 빨간 계열
        Emit(ELogKind.Error, msg, "Error", "#FF1744");
    }

    public static void Success(string msg)
    {
        // 초록 계열
        Emit(ELogKind.Success, msg, "OK", "#00C853");
    }

    // Assert
    public static void Assert(bool condition, string msg)
    {
        // if 문이 많이 빠질수 있음 -> 매번 if문 하는것 보다 Assert롤 체크하면 흐름이 깔끔해 진다.
        // 여기 다시 필기

        if (condition)
        {
            return;
        }

        Error($"[ASSERT] {msg}");      
    }

    public static void CheckNull(object odj, string msg)
    {
        if (odj != null)
        {
            return;
        }

        Warn($"[NULL] {msg}");
    }
    

    // 참조 체크
    public static T Ref<T>(T obj, string msg) where T : class
    {
        //유니티에서 당연히 연결됐는지? 를 가장 빠르게 확인할수 있는 로그
        // ㄴ null 경고
        // ㄴ 아니면 이름을 출력

        // 유니티 null이 참 변신 괴물 같은 존재

        //★★★★
        // 제네릭이 뭔가? -> C# 고급 문법
        // -> 대답
        // 여러분들이 시간을 들여 사용해 봐야 함
        // 요놈 잘 쓰면 처후 디자인 패턴이 편함

        /*
         T Ref<T>
         T Ref<T>(T obj, string msg)
          ㄴ obj 가 null이면 경고 찍고 -> obj 그대로 반환
          ㄴ 검사 + 반환을 한방에 하고 싶다.

        왜 T이냐..?
         ㄴ 한줄로 끝낼려고 -> 이후 어떤 타입이 들어올지 예측이 안되지만 클래스 타입임을 명시하기 위해
         ㄴ 동작시키기 위해서

        Ex :
        _rb = GetComponent<Rigidbody>();
        if(_rb == null
        {
            CPrint.Warn(....);
        }
        
        _rb = CPrint.Ref(GetComponent<Rigidbody>, "...");
        
        - 필수 컴포넌트 체크 + 바로 대입을 해주기 위함

        ㆍ 제네릭 -> 가볍게

        - 제네릭은 -> 타입을 나중에 정하는 설계
         ㄴ 호출할때 타입이 결정된다.

        - 템플릿 / 제네릭 -> 비슷한 애기이다.
         ㄴ 클래스나 함수를 정의할때 타입을 지정하지 않고 구현할 수 있는 메커니즘

        ㆍ T

        - 타입 자리 (타입 변수)

        - 제네릭은 <T>와 같은 제네릭 타입을 명시함으로서 정의가능

        객체지향 특징 + 원칙
         ㄴ 추상화

        - 컴포넌트 기반 프로그래밍 (객체 / 구조 / 절차)

        - 제네릭 프로그래밍으로 전환이 되면 설계가 더 까다로워진다.

        where T : class

        - C#은 사용하는데 있어 조금 괜찬은 편 -> 기본적으로 모든 데이터 타입에 동작하도록 설계가 되어야 한다.

        - 제네릭 클래스 또는 함수에 어떤 데이터 타입이 지정되어도 내부 로직에 변화가 발생하면 안된다.

        - 특정 데이터 타입에 동작하도록 데이터 타입을 제한하는 것이 가능하다.

        - T는 클래스만 받겠다는 제한 (참조형)
         ㄴ 결국 우리가 만든 함수는 null 체크가 핵심인데 -> int / float 들어오면 피곤해 진다.

        - where T : class 로 null이 될수 있는 타입만 허용 하겠다. -> 실수 선행 방지

        ※ 단순한 클래스 설계가 아니고 lib + Framework 설계에서 사용된다.
         ㄴ 미니는 쓰지 말고 -> 이후 프로젝트 교과목에 잡힌 프로젝트일때 조금씩 써볼 수 있으면 써보는 걸 추천
         ㄴ 어려운데..? 개념은 알겠어 / 코드를 읽을수 있겠어.. / 근데 어떻게 써야 돼? 이걸 왜 써야 해?


        ㆍ 제네릭의 데이터 타입 제한

        Class CSomeClass<T> where T : class
         ㄴ 타입을 참조 형식으로 제한

        Class CSomeClass<T> where T : struct
         ㄴ 타입을 값 형식으로 제한

        Class CSomeClass<T> where T : SomeClass
         ㄴ 클래스 씰 != 타입을 SomeClass 직 / 간접적으로 상속하는 형식으로 제한

        Class CSomeClass<T> where T : SomeInterface
         ㄴ 타입을 SomrInterface 를 직 / 간접적으로 따르는 형식으로 제한

        Class CSomeClass<T, U> where T : U
         ㄴ 타입을 U(클래스 또는 인터페이스)로 직 / 간접적으로 상속하는 형식으로 제한

        */

        if (obj == null)
        {
            Warn($"[NULL] {msg}");
        }

        return obj;
    } 

    // 벡터3
    public static void V3(string label, Vector3 v, int digits = 2)
    { 

        // 숫자 자릿수를 줄여서 로그를 읽기 쉽게 만든다.
        //  ㄴ 반올림해서 보여주면 읽을 수 있는 형태가 됨
        float x = (float)System.Math.Round(v.x, digits);
        float y = (float)System.Math.Round(v.y, digits);
        float z = (float)System.Math.Round(v.z, digits);

        // Math.Round

        Emit(ELogKind.Log, $"{label} : ({x}, {y}, {z})");
    }

    public static void KV(string key, object value)
    {
        // KV = key = value 형태로 값을 찍는 표준 포맷 헬퍼
        // 우리가 디버깅 할때 -> 가장 자주 직는 형태 -> 여기서 포맷 통일하겠다.

        /*
         EX:
        CPrint.Group("Spawn Check", () =>
        {
        CPrint,KV("PlayerPos", transform.position)
         CPrint.KV("HP",hp)
         });
         */

        Log($"{key} = {value}");
    }

    // 로그 규모가 커진다 -> 섹션을 만든다. (로그 덩어리)
    public static void Group(string title, Action body, char lineCh = '=', int LineCount = 20)
    {
        if (!Enable)
        {
            return;
        }

        // 타이틀 찍고 / 들여쓰기 올리고 / 그안의 본문 실행 하고 / 들여쓰기 내리고 / 구분선으로 마무리

        /*
         ▶ 델리게이트 -> 간단 버전
         
         - 이 또한 고급 문법이니..

        - 델리게이트는 함수를 변수처럼 다룰 수 있게 해주는 타입
         ㄴ 특정 함수를 대신 호출해주는 대리자
        
        - 프로그래밍에서 델리게이트는 콜백 함수를 의미한다.
         ㄴ 델리게이트를 이용하면 특정 이벤트가 발생하는 시점에 해당 이벤트를 처리하는 것이 가능하다.

        - 대리자는 자기가 가르키고 있는 함수를 호출하는 역활을 한다.
         ㄴ 함수에 대한 참조를 갖고 있다.

        [요약]
        1. 실행을 위임한다.
        2. 호출 주체와 실행 주체가 같지 않다.
        3. 콜백의 시작점
        
        ※ 함수 폴링 방식? -> 열심히 조사
         

        ▶ Action 

        - Action은 델리게이트의 미리 만들어진 형태 (표준)
         ㄴ 3총사 : Action <T> / Func<T, TResult> / Predicate<T>
        
        - 델리게이트는 함수를 담을 수 잇는 타입

        - Action -> 매개 변수 없고 / 반환값 없는 형태 -> C# 기본 제공
         ㄴ 실행할 코드 덩어리를 -> 변수처럼 전달한다.

        EX :
        CPrint.Group("프리셋, () =>
        {
        CPrint.Log("색상 교체");
        CPrint.Log("재질 교체");
        CPrint.Log("라이트 교체");
        });

        - Action은 코드를 데이터처럼 넘긴다 라는 느낌 -> Group은 로그 꾸러미를 함수로 받는것

        */

        // 1. 그룹 제목 출력
        Title(title, lineCh);
        // 2. 그룹 내부 -> 한 단게 들여쓰기
        IndentPush();
        // 실행할 코드 블록을 호출(Action)
        // ?.Invoke() : body 가 null이면 실행하지 않겠다. -> 에외 방지
        body?.Invoke();
        // 다시 복구 (들여쓰기)
        IndentPop();
        // 구분선 -> 스타일 튜닝
        Line(lineCh, LineCount);
    }

    public static void Once(string key, string msg)
    {

        if (!Enable)
        {
            return;
        }

        // 이미 키가 있는 경우 -> 재출력 금지
        if (_onceSet.Contains(key))
        {
            return;
        }

        _onceSet.Add(key);

        Warn($"[ONCE] {msg}");

        /*
         EX :
        CPrint.Once("NoRB","Rigidbody 가 없어 물리 이동이 안됨 (혹은 동작 하지 않음)")      
        */
    }

    public static void OnceClear()
    { 
        // 등록된 키 전부 비운다.
        //  ㄴ 보통 씬 재시작 -> 테스트 반복 환경에서 사용할 수 있음
    _onceSet.Clear();
    }

    // 에디터 / 개발 빌드에서만 남기고 싶은 함수 모음
    //  ㄴ 규모가 작으면 즉성으로 처리를 하고 -> 함수가 많아지면 -> 선택적 컴파일로 처리하면 된다.
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Ray(Vector3 origin, Vector3 direction, Color color, float duration = 0f)
    {
        if (!Enable)
        {
            return;
        }

        Debug.DrawRay(origin, direction, color, duration);
    }

   
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Line3D(Vector3 a, Vector3 b, Color color, float duration = 0f)
    {
        if (!Enable)
        {
            return;
        }

        Debug.DrawLine(a, b, color, duration);
    }

    // 이후에는 필요하면 추가 예정

}