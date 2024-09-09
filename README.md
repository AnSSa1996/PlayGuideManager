# PlayGuideManager



# PlayGuideManager란?
* 플레이가이드 매니자란, 튜토리얼이나 초반동선 혹은 다양한 컨텐츠의 시작 부분을 위한 가이드입니다.
* 에디터를 통해서 가이드 스텝을 직접 등록해서 __기획자가 쉽게 제작할 수 있는 시스템입니다.__



# 사용한 패키지
* UniTask를 사용해서 구현했습니다. 프로젝트에 [UniTask를 임포트 하셔야 합니다.](https://github.com/Cysharp/UniTask/tree/master)



# 특징
* 에디터를 통해, 가이드 스텝을 등록할 수 있습니다.
* 가이드의 스텝을 __스크립터블 오브젝트로 제작하여, 클라이언트가 손쉽게 제작할 수 있습니다.__(리소스 패치만으로 가이드 수정 가능)

  

# 사용 예시 영상
https://github.com/user-attachments/assets/7eed5f0a-d4fe-4cb5-b8b4-45bf6e3f4265



# 사용 방법

- 기본적으로 ResourceManager 셋팅하셔야합니다.

![image](https://github.com/user-attachments/assets/1308be0b-fce7-445d-9ac1-703fe8697a10)


1. @UIManager 프리팹을 첫 씬에 배치합니다. (프로젝트가 처음 시작되는 StartScene)

![image](https://github.com/user-attachments/assets/36db1402-3c08-4477-864b-bc88420190ed)

2. MainCamera UICamera 셋팅하기.

![image](https://github.com/user-attachments/assets/0e3fb2f5-952b-46d3-894d-ee5d15213b82)

3. UI Scripts 및 UI Prefab 작성하기.

![image](https://github.com/user-attachments/assets/8bb84f8e-6743-439a-b777-77c84344d27d)
![image](https://github.com/user-attachments/assets/2f7029ff-ab04-44a9-95d9-e70600c7e8b6)
