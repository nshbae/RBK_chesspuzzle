# RBK_chesspuzzle
Indie puzzle game built around chess-piece movement and interactions. This repository contains the project files for RBK: Chess Puzzle.

# RBK: Chess Puzzle

체스 기물의 이동 규칙을 퍼즐 메커니즘으로 확장한 Unity 기반 퍼즐 게임입니다.

Unity와 C#을 사용해 개인 프로젝트로 개발했으며, **기물별 이동 판정 시스템, 해답이 보장되는 절차적 퍼즐 생성, 챕터별 특수 규칙과 퍼즐 생성의 연동**을 핵심 시스템으로 구현했습니다.

> **플랫폼:** Windows / Android  
> **엔진:** Unity  
> **언어:** C#  
> **역할:** 팀장 및 개발 총괄 (3인 개발)

---

## 게임 플레이

플레이어는 8×8 보드 위에서 서로 다른 체스 기물의 이동 규칙을 이용해 이동합니다.

하나의 기물만 사용하는 것이 아니라 1~3개의 기물로 Deck을 구성하며, 매 턴 사용할 기물이 순환합니다. 따라서 현재 위치뿐 아니라 이후 턴에 사용할 이동 규칙까지 고려하여 적을 모두 제거해야 합니다.

Rook, Bishop, Knight 등의 기본 체스 기물 외에도 기존 이동 규칙을 조합하거나 변형한 기물을 구현했습니다.

---

# 주요 기술 구현

## 1. Solution-First Procedural Puzzle Generation

RBK의 핵심 시스템은 **해답 경로를 먼저 구성하는 절차적 퍼즐 생성 알고리즘**입니다.

맵을 무작위로 만든 뒤 풀 수 있는지를 검사하는 방식이 아니라 다음 순서로 퍼즐을 생성합니다.

1. 도착 위치를 선택
2. 각 턴에 사용할 기물을 결정
3. 현재 위치에 도달할 수 있는 이전 위치 계산
4. 후보 중 하나를 이전 이동 위치로 선택
5. 이를 반복하여 완전한 solution path 구성
6. 생성된 경로를 기반으로 적, 장애물, 특수 타일 배치

이 방식으로 무작위성을 유지하면서도 최소 하나 이상의 유효한 해답을 가진 퍼즐을 생성하도록 구현했습니다.

각 이동은 `Move` 데이터에 기물 종류, 시작/도착 위치, Portal 사용 여부와 이동 경로를 함께 저장합니다.

**관련 코드**
- `MapManager.cs`
  - `GenerateMap()`
  - `PickNormalMove()`
  - `requiredPathBlock()`
  - `AddExtraBlocks()`
  - `AddSpecialBlocks()`

---

## 2. 체스 기물 기반 이동 판정 시스템

기물의 이동 규칙을 Player 입력과 분리하여, 동일한 이동 판정 로직을 실제 플레이와 퍼즐 생성 과정에서 함께 사용하도록 구현했습니다.

8×8 보드는 1차원 index 배열로 관리합니다.

### 방향 탐색 기반 기물

Rook과 Bishop처럼 여러 칸을 연속 이동하는 기물은 각 방향으로 보드를 탐색하면서 다음 요소를 검사합니다.

- 장애물
- 적
- 특수 타일
- 보드 경계

### 상대 좌표 기반 기물

Knight나 King과 같이 정해진 위치로 이동하는 기물은 상대 좌표 목록을 이용해 이동 가능 위치를 계산합니다.

예를 들어 Knight는 다음과 같은 이동 벡터를 사용합니다.

```text
(+1, -2)
(+1, +2)
(-1, -2)
(-1, +2)
...

또한 기존 이동 규칙을 조합하여 새로운 기물을 구현했습니다.

- **Queen** = Rook + Bishop
- **Crow** = Bishop + Knight
- **Dragon** = Rook + Knight
- **Unicorn** = Queen + Knight

게임에 변주를 주기 위해 완전히 새로운 이동 규칙을 가지는 기물들도 있습니다.

- **Deer**: 상하좌우 대각선으로 1~2칸 이동할 수 있습니다. 나이트처럼 중간에 장애물이 있어도 이동이 가능합니다.
- **Jumper**: 상하좌우 대각선으로 2칸 떨어진 칸과 나이트처럼 이동할 수 있습니다. 나이트처럼 중간에 장애물이 있어도 이동이 가능합니다.

이동 가능 여부를 계산할 때 단순한 좌표뿐 아니라 적과의 상성, 장애물, 챕터별 특수 지형도 함께 판정합니다.

**관련 코드**
- `playerPieceManager.cs`
  - `PieceList()`
  - `checkBlock()`
  - `RookList()`
  - `BishopList()`
  - `pointList()`
  - `QueenList()`
  - `CrowList()`
  - `DragonList()`
  - `UnicornList()`

---

## 3. 챕터별 특수 퍼즐 규칙

각 챕터에는 서로 다른 보드 기믹이 존재합니다.

특수 타일을 단순한 런타임 효과로 구현하는 데 그치지 않고, **이동 가능 영역 계산과 퍼즐 생성 과정에도 해당 규칙을 반영**했습니다.

### Volcano — 턴에 따라 변화하는 위험 지형

용암 특수 타일이 매 턴 여러 상태를 순환하며 4턴마다 위험한 지형으로 바뀝니다.

맵 생성 과정에서는 solution path가 각 타일을 어느 턴에 사용하는지 분석하고, 생성된 해답을 방해하지 않는 상태에 특수 타일을 배치합니다.

### Library — 이동에 따라 변화하는 지형

기물이 지나간 횟수에 따라 타일이 조금씩 깨져나가고, 일정 횟수 이상 밟으면 파괴되어 지나갈 수 없습니다.

생성된 solution에서 각 타일을 통과하는 횟수를 계산하고 이를 특수 타일 배치에 이용합니다.

### Space — Wormhole

한 쌍의 특수 타일을 Portal로 연결합니다.

기물의 이동 경로가 Wormhole에 도달하면:

1. 연결된 출구 탐색
2. 출구로 이동
3. 기존 이동 방향을 유지한 채 탐색 계속
4. 출구 이후의 이동 가능 위치까지 reachable area에 포함

하도록 구현했습니다.

Portal의 입구·출구와 이동 가능 위치를 별도 데이터로 관리하여 실제 플레이와 퍼즐 생성 양쪽에서 동일하게 사용할 수 있도록 했습니다.

**관련 코드**
- `MapManager.cs`
  - `AddSpecialBlocks()`
  - `UpdateVolcanoSP()`
  - `UpdateLibrarySP()`
- `playerPieceManager.cs`
  - `checkBlock()`
  - `wormHoleList()`

---

## 4. 기물 Deck 및 턴 순환 시스템

플레이어가 1~3개의 기물로 Deck을 구성할 수 있도록 구현했습니다.

Deck에 포함된 기물이 많을수록 난이도는 증가하지만, 클리어 할 경우 획득 보상도 증가합니다.

선택된 기물은 매 턴 순서대로 교체되므로, 플레이어는 현재 이동뿐 아니라 다음 턴에 사용할 이동 규칙까지 고려해야 합니다.

Deck 편집 화면에서는 다음 기능을 제공합니다.

- Drag & Drop 기반 기물 편집
- 기물 순서 변경
- 획득한 기물 관리
- Random 기물 (매 스테이지마다 획득한 기물 중 1개가 무작위로 결정됩니다.)
- Deck 구성 저장

동일한 Deck 정보는 실제 플레이뿐 아니라 절차적 퍼즐 생성 과정에서도 사용됩니다.

**관련 코드**
- `DeckEditManager.cs`
- `PlayerManager.cs`

---

## 5. Snapshot 기반 Undo / Redo

매 이동이 완료될 때 퍼즐의 현재 상태를 snapshot으로 저장합니다.

저장되는 데이터에는 다음이 포함됩니다.

- 전체 보드 상태
- 적 배치
- 플레이어 위치
- 현재 기물 순서
- 이동 횟수
- 획득한 재화
- Tutorial 상태

Undo / Redo 시 이전 상태를 다시 계산하는 대신 저장된 snapshot을 복원합니다.

Undo 이후 새로운 이동을 수행한 경우 기존의 이후 history를 제거하여 새로운 history branch를 생성하도록 구현했습니다.

**관련 코드**
- `HIstoryManager.cs`
- `MapManager.LoadHistory()`

---

## 6. 생성된 Solution을 활용한 Hint 시스템

절차적 퍼즐 생성 과정에서 구성된 solution은 맵 생성 이후에도 유지됩니다.

따라서 Hint를 요청했을 때 별도의 Solver를 다시 실행하지 않고 기존 solution 데이터를 활용할 수 있습니다.

저장된 이동 경로는 다음과 같은 보드 좌표 형식으로 변환되어 플레이어에게 제공됩니다.

```text
A3 C5 F4 ...
```

이를 통해 퍼즐 생성 시스템의 결과를 실제 gameplay assistance 기능까지 재사용했습니다.

---

# 기타 구현 시스템

그 외 다음과 같은 게임 시스템을 구현했습니다.

- Story Mode / Procedural Custom Mode
- Stage progression 및 별점 평가
- Coin / Ruby 보상
- 기물·난이도·퍼즐 길이 해금
- `PlayerPrefs` 기반 진행 데이터 저장
- 데이터 기반 Dialogue 시스템
- 캐릭터 표정 및 배경 전환
- BGM / 환경음 / 이동음 / 효과음 관리
- 음악·효과음 음량 설정
- 30 / 60 / 120 FPS 설정
- 화면 비율에 대응하는 UI Scaling

---

# 시스템 구조

주요 gameplay 기능을 Manager 단위로 나누어 구성했습니다.

```text
MainManager
├── MapManager
│   ├── Board state
│   ├── Puzzle generation
│   ├── Special tile mechanics
│   └── Reward calculation
│
├── playerPieceManager
│   └── Movement / reachability rules
│
├── PlayerManager
│   ├── Input
│   ├── Turn cycle
│   └── Player movement
│
├── HIstoryManager
│   └── Undo / Redo
│
├── DeckEditManager
│   └── Deck configuration
│
├── DialogueManager
├── SaveManager
├── SoundManager
└── UImanager
```

---

# 다운로드

Windows 및 Android용 플레이 가능한 빌드는 GitHub **Releases**에서 확인할 수 있습니다.

- Windows build
- Android build

[Releases](../../releases)