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

1. PlayGuideStep을 상속받은 스텝 제작.

![image](https://github.com/user-attachments/assets/238811b2-a81a-49eb-824a-4c00627bf72e)


2. PlayGuideEditor를 통해 플레이가이드 셋팅하기

![image](https://github.com/user-attachments/assets/d504eaf6-eaa7-4c28-bfcf-25f55d43e034)
