# Project 3 최종보고서

작성일시: 2023년 12월 17일 오후 8:08
강의 번호: 소프트웨어융합개론

**2022105441 강병훈**

# Final Video
[![Video Label](http://img.youtube.com/vi/XPFh6WQWsEY/0.jpg)](https://youtu.be/XPFh6WQWsEY)

# 문제 인식

퍼즐 게임이라는 장르는 매력적이지만 이를 선호하지 않는 사람들도 많다. 단순히 머리 쓰기를 싫어하는 사람부터, 퍼즐이 게임의 흐름을 끊고 몰입을 방해한다고 여기는 사람, 지나치게 어려운 퍼즐로 인해 수많은 실패의 트라우마를 가지고 있는 사람까지 다양한 이유가 있다. 이러한 이유를 통해서 퍼즐 게임의 문제점을 몇 가지 꼽아 볼 수 있다.

1. 퍼즐이 지나치게 어렵다.
2. 다른 장르보다 자유도가 부족하다.
3. 퍼즐을 푸는 과정에 흥미를 느끼지 못한다.


이러한 퍼즐 게임의 문제를 해결한 예시로 Untitled Goose Game을 들 수 있다.

이 게임은 플레이어가 거위를 조종하며 주어진 퀘스트(퍼즐)를 완료하기 위해서 인간을 방해하고 다음 지역으로 넘어가는 방식의 게임이다.

퍼즐을 푸는 과정에서 어느 정도의 자유도를 주었기 때문에 퍼즐에 막혔을 때 다양한 시도를 해볼 수 있어 재시도의 피로도를 줄일 수 있고, 퍼즐의 과정을 인간을 골탕 먹이는 것으로 표현해서 재미를 느끼게 했다.

이러한 게임에서 자유도를 더 높일 수 있다면 더욱 더 재미있는 게임을 만들 수 있을 것이라고 생각하여 다음과 같은 게임을 생각했다.

---

# 초기 계획

## Catch Cat

1. 게임 개요
    1. 길고양이(플레이어)가 사랑하는 집고양이를 위해 집에 잠입하여 집주인이 안 볼 때 집안을 어지르고 주의를 끌어 만나러 가는 게임.
    2. 퍼즐 요소 – 집주인에게 잡히면 실패
        1. 집주인의 위치와 시야
        2. 주변 물건과의 상호작용
            1. 물건 떨어뜨리기
            2. 소리 내기
        3. 츄르 모으기
            1. 일정 개수의 츄르를 모으지 않으면 집 고양이는 플레이어를 받아주지 않음
            2. 집안 곳곳에 랜덤 스폰
2. 재미 요소
    1. 높은 자유도 - 대부분의 물건을 떨어뜨릴 수 있다
    2. 퍼즐 과정의 흥미 - 츄르의 수집과 인간을 농락하는 재미
3. 구현 방법 - unity 이용
    1. 맵 구현
    2. 집주인과 고양이의 자연스러운 애니메이션 구현
    3. 일정 이상의 힘이 발생하면 소리가 발생하는 시스템 구현
    4. 소리에 반응하는 집주인(navigation mesh)
    5. 츄르 스폰과 수집 구현
    6. 기타 디테일 구현

---

# 구체적인 수행 과정 및 내역과 코드 및 구현 설명

자세한 코드는 하단의 깃허브 링크를 참조해주세요.

1. 맵 구현
    1. 먼저 유니티의 terrain을 사용하여 월드맵을 제작했습니다.
    2. 이후 lowpoly asset들을 찾아 추가해 주변 배경을 구성했습니다.
    3. 실질적인 플레이를 하는 집을 만들었습니다.
    
    ![Untitled](https://github.com/dot-mario/swcon101_termprojrct/assets/74451418/336f1e5a-ca56-423b-a75b-ea4972392beb)
    
    ![Untitled 1](https://github.com/dot-mario/swcon101_termprojrct/assets/74451418/ebd310ef-74b4-4e90-b8ae-4e87fefea72a)

    ![Untitled 2](https://github.com/dot-mario/swcon101_termprojrct/assets/74451418/04dd1010-47d2-404a-a409-446db551cfd4)

    
2. 플레이어 캐릭터 구현
    1. 3인칭으로 구현하기 위해서 cinemachine의 freelook camera를 사용했습니다.

![Untitled 3](https://github.com/dot-mario/swcon101_termprojrct/assets/74451418/b6ea6d82-68c1-45b4-9858-bf99e64402b0)

`GetInput()` : 키 입력을 받습니다.

`Move()`: 카메라가 바라보는 방향을 기준으로 움직이도록 하였습니다.

일정 시간 이상 뛰면 구르도록 하였습니다.

`Turn()`: 움직이는 방향에 따라 캐릭터가 회전하도록 하였습니다.

`Jump()`: 땅에 있을 때 점프 할 수 있게 하였습니다.

점프키를 오래 누르면 더 높이 뛰도록 하였습니다.

`GroundCheck()`: Raycast를 통해 플레이어가 공중에 떠 있는지 체크합니다.

`FlyCheck()`: 공중에 특정 시간 동안 떠 있다면 Fly 애니메이션을 실행합니다.

![Untitled 4](https://github.com/dot-mario/swcon101_termprojrct/assets/74451418/909b4437-1495-4c37-91b8-b8db3943857c)

스페이스바를 오래 누른다면 Jump Charging 상태가 되어 기를 모으다가 일정 시간 이후에는 Jump Charging End가 됩니다. 이후 키를 땠을 때 Jump 모션이 보여지고 Fly 상태가 되어 하강합니다. 이를 통해 플레이어가 원하는 만큼만 점프 할 수 있습니다.

일반적으로 wasd를 눌러 이동 할 때는 Walk, shift를 누른 상태로 다니면 Run 상태가 되었다가 일정 시간 이후엔 Roll 상태가 됩니다.

https://youtu.be/V0cdd3L4A30

플레이어 움직임

1. 적 AI 구현
    
    플레이어 캐릭터는 입력에 따른 상태를 한번만 재생하면 되지만 AI는 상태를 유지할 필요가 있다. 또한 상태끼리의 순환 또한 필요해서 앞서 고양이에서 그러했던 것처럼 Update 하나에서 전부 처리하기에 무리가 있었습니다.
    
    따라서 코루틴을 사용하여 statemachine 효과를 냈습니다.
    

![%EA%B3%A0%EC%96%91%EC%9D%B4%EC%BD%94%EB%93%9C4](https://github.com/dot-mario/swcon101_termprojrct/assets/74451418/9d842c1b-4b10-4a55-af47-9d6e36b2bc19)

### 타겟 감지

3가지 상태가 있습니다. `Idle`, `Walk`, `Chase` 

`Idle`: 제자리에서 고개를 돌려 타겟을 찾습니다

`Walk`: 정해진 위치를 랜덤으로 배회합니다.

`Chase`: 플레이어를 쫒아갑니다.

AI 행동 패턴은 그림과 같은 방식으로 작성했습니다.

![Untitled 5](https://github.com/dot-mario/swcon101_termprojrct/assets/74451418/43f7ac92-a7f3-4034-a613-846b37c02adb)

특정 범위 내에 들어온 특정 레이어를 얻은 뒤

```csharp
float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;
```

시야각을 표현하기 위해 내적 후 acos을 계산하여 라디안 각도를 구하고 이를 변환했습니다.

```csharp
if (targetAngle <= ViewAngle * 0.5f && !Physics.Raycast(myPos, targetDir, Vector3.Distance(myPos, targetPos), ObstacleMask))
```

이후 raycast를 하여 사이에 장애물이 있는지 판단하였습니다.

### Walk 상태에서 발생한 문제

![Untitled 6](https://github.com/dot-mario/swcon101_termprojrct/assets/74451418/cb15eac0-9af1-4866-981a-38534ac16911)

Walk 반복에서 navigation mesh의 remainDistance가 항상 0 이 나오는 문제가 있어 다음 글을 참고하여 `pathComplete()` 함수를 추가했습니다.

[How can I tell when a navmeshagent has reached its destination?](https://discussions.unity.com/t/how-can-i-tell-when-a-navmeshagent-has-reached-its-destination/52403/6)

### 시야의 시각적 표현

![Untitled 7](https://github.com/dot-mario/swcon101_termprojrct/assets/74451418/b760fae7-a9d2-40ea-bf87-837be2f3718e)

AI시야를 시각적으로 표현하기 위해서 `OnDrawGizmos()` 함수를 통해 시야 범위의 구 겉에 점을 찍고, 이 점이 시야각 안에 있다면 선을 그어 표현했습니다.

또한 RaycastHit을 사용하여 raycast가 장애물에 닿으면 그 지점부터 선을 그어, 시야가 장애물에 방해 받는 모습 또한 표현했습니다.

![Untitled 8](https://github.com/dot-mario/swcon101_termprojrct/assets/74451418/bf27fd50-10d3-401a-b51e-0b1e408a762e)

1. 츄르 스폰과 수집 구현
    1. 적절한 모델을 구하지 못해 선물 상자로 대체했습니다.
    2. 플레이어가 선물 상자에 닿으면 선물 상자가 사라집니다.
    3. 일정 개수의 선물 상자를 모으면 문이 열립니다.
    4. 문을 통과하면 게임은 클리어 됩니다.
    

---

# 계획 대비 최종 결과물

### 기존 계획

1. 맵 구현
2. 집주인과 고양이의 자연스러운 애니메이션 구현
3. 일정 이상의 힘이 발생하면 소리가 발생하는 시스템 구현
4. 소리에 반응하는 집주인(navigation mesh)
5. 츄르 스폰과 수집 구현

### 최종 결과물

1. (완료) 맵 구현
2. (완료) 집주인과 고양이의 자연스러운 애니메이션 구현
3. (미완료) 일정 이상의 힘이 발생하면 소리가 발생하는 시스템 구현
4. (미완료) 소리에 반응하는 집주인(navigation mesh)
5. (완료) 츄르 스폰과 수집 구현

이번 학기 동안에 계획했던 소리에 반응하는 시스템은 만들지 못했습니다. 앞으로 추가할 예정입니다!

---

# 프로젝트 소감

막연히 이론적으로 알고 있던 ai시야와 플레이어의 움직임을 직접 만들어 보니 공부했던 내용을 다시 복습할 수 있었고, 제 생각대로 작동하는 프로그램을 확인할 수 있어 뿌듯했습니다.

퍼즐 장르의 한계를 깨고 플레이어에게 자유도를 주고 싶었지만 정작 퍼즐 구성에는 집중하지 못하고 게임의 완성을 목표로 진행한 것 같아 아쉬웠습니다.

학기가 시작할 때는 당연히 완성 할 수 있을 줄 알았지만, 다른 과목 과제에 매달리다 보니 이 프로젝트에 점점 소홀해지는 자신을 발견 할 수 있었습니다. 이에 대해 반성하고, 완료하지 못했던 계획들을 완수하고 싶습니다.

감사합니다.

---

# Githup Link

[https://github.com/dot-mario/swcon101_termprojrct](https://github.com/dot-mario/swcon101_termprojrct)
