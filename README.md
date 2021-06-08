# Reality

## Scripts

### GetObject.cs
: 마우스로 객체를 클릭시 죄클릭, 우클릭에 따라 선택된 객체 클릭 확인용 변수 (leftclick, rightclick) 값 변경
##### inputs
  마우스 클릭
##### outputs
  leftclick, rightclick(좌우클릭)

### Object.cs
: leftclick, rightclick이 true 일 경우 선택된 오브젝트 확인용 변수(target)를 클릭한 오브젝트 객체로 변경
##### inputs
  leftclick, rightclick(좌우클릭)
##### outputs
  target(선택된 오브젝트)

### Cylindertest.cs
: target과 이전에 선택된 오브젝트 확인용 변수(undotarget)를 이용하여 클릭된 오브젝트 변경된다면 실시간으로 반영하고 target을 maincamera의 고정타겟 변수(Maincamera.target)으로 설정
##### inputs
  target(선택된 오브젝트)
##### outputs
  target(선택된 오브젝트)
  
### Maincamera.cs
: target의 leftclick이 true일 경우 3인칭 시점, rightclick이 true일 경우 1인칭 시점이 될 수 있도록 maincamera를 이동한다.
##### inputs
  target(선택된 오브젝트)
##### outputs
  maincamera 위치
