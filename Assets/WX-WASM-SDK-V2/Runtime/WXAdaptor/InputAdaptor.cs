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
            // 隐藏输入法
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
            // 输入法confirm回调
            Debug.Log("onConfirm");
            Debug.Log(v.value);
            HideKeyboad();
        }

        public void OnComplete(OnKeyboardInputListenerResult v)
        {
            // 输入法complete回调
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

                //绑定回调
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
                //删除掉相关事件监听
                WX.OffKeyboardInput(OnInput);
                WX.OffKeyboardConfirm(OnConfirm);
                WX.OffKeyboardComplete(OnComplete);
                isShowKeyboad = false;
            }
        }

    }
}