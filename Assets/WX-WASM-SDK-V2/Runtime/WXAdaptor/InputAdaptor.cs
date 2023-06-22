using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace WeChatWASM
{
    public class InputAdaptor : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
    {
        public InputField input;
        private bool isShowKeyboad = false;
        public void OnPointerClick(PointerEventData eventData)
        {
            ShowKeyboad();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // �������뷨
            if (!input.isFocused)
            {
                HideKeyboad();
            }

        }

        public void OnInput(OnKeyboardInputListenerResult v)
        {
            Debug.Log("onInput");
            Debug.Log(v.value);
            if (input.isFocused)
            {
                input.text = v.value;
            }

        }

        public void OnConfirm(OnKeyboardInputListenerResult v)
        {
            // ���뷨confirm�ص�
            Debug.Log("onConfirm");
            Debug.Log(v.value);
            HideKeyboad();
        }

        public void OnComplete(OnKeyboardInputListenerResult v)
        {
            // ���뷨complete�ص�
            Debug.Log("OnComplete");
            Debug.Log(v.value);
            HideKeyboad();
        }

        private void ShowKeyboad()
        {
            if (!isShowKeyboad)
            {
                WX.ShowKeyboard(new ShowKeyboardOption()
                {
                    defaultValue = "",
                    maxLength = 20,
                    confirmType = "go"
                });

                //�󶨻ص�
                WX.OnKeyboardConfirm(OnConfirm);
                WX.OnKeyboardComplete(OnComplete);
                WX.OnKeyboardInput(OnInput);
                isShowKeyboad = true;
            }
        }

        private void HideKeyboad()
        {
            if (isShowKeyboad)
            {
                WX.HideKeyboard(new HideKeyboardOption());
                //ɾ��������¼�����
                WX.OffKeyboardInput(OnInput);
                WX.OffKeyboardConfirm(OnConfirm);
                WX.OffKeyboardComplete(OnComplete);
                isShowKeyboad = false;
            }
        }

    }
}