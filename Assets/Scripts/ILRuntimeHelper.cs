using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public static class ILRuntimeHelper
{
    static ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    public static void LoadDll(byte[] dll, byte[] pdb, string entryClass, string entryMethod, Dictionary<string, string> dConfigContents)
    {
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain(ILRuntime.Runtime.ILRuntimeJITFlags.JITOnDemand);
        InitILRuntime(appdomain);
        InitializeILRuntime();
        RegisterDelegates();
        appdomain.LoadAssembly(new MemoryStream(dll), pdb == null ? null : new MemoryStream(pdb), new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
        OnHotFixLoaded(entryClass, entryMethod, dConfigContents);
    }
    static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        //������Ҫע�������ȸ�DLL���õ��Ŀ���̳�Adapter�������޷���ȷץȡ����
        domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        domain.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor());
        domain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
        domain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
        domain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
    }
    private static void RegisterDelegates()
    {
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.AsyncOperation>();

        appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.EventArgs>();
        appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<System.EventArgs>>((act) =>
        {
            return new System.EventHandler<System.EventArgs>((sender, e) =>
            {
                ((Action<System.Object, System.EventArgs>)act)(sender, e);
            });
        });

        appdomain.DelegateManager.RegisterMethodDelegate<System.Object, UnityWebSocket.OpenEventArgs>();
        appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<UnityWebSocket.OpenEventArgs>>((act) =>
        {
            return new System.EventHandler<UnityWebSocket.OpenEventArgs>((sender, e) =>
            {
                ((Action<System.Object, UnityWebSocket.OpenEventArgs>)act)(sender, e);
            });
        });

        appdomain.DelegateManager.RegisterMethodDelegate<System.Object, UnityWebSocket.CloseEventArgs>();
        appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<UnityWebSocket.CloseEventArgs>>((act) =>
        {
            return new System.EventHandler<UnityWebSocket.CloseEventArgs>((sender, e) =>
            {
                ((Action<System.Object, UnityWebSocket.CloseEventArgs>)act)(sender, e);
            });
        });
        appdomain.DelegateManager.RegisterMethodDelegate<System.Object, UnityWebSocket.ErrorEventArgs>();
        appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<UnityWebSocket.ErrorEventArgs>>((act) =>
        {
            return new System.EventHandler<UnityWebSocket.ErrorEventArgs>((sender, e) =>
            {
                ((Action<System.Object, UnityWebSocket.ErrorEventArgs>)act)(sender, e);
            });
        });
        appdomain.DelegateManager.RegisterMethodDelegate<System.Object, UnityWebSocket.MessageEventArgs>();
        appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<UnityWebSocket.MessageEventArgs>>((act) =>
        {
            return new System.EventHandler<UnityWebSocket.MessageEventArgs>((sender, e) =>
            {
                ((Action<System.Object, UnityWebSocket.MessageEventArgs>)act)(sender, e);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((Action)act)();
            });
        });

    }

    static void InitializeILRuntime()
    {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
        //����Unity��Profiler�ӿ�ֻ���������߳�ʹ�ã�Ϊ�˱�����쳣����Ҫ����ILRuntime���̵߳��߳�ID������ȷ���������к�ʱ�����Profiler
        appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
    }
    static void OnHotFixLoaded(string entryClass, string entryMethod, Dictionary<string, string> dConfigContents)
    {
        //HelloWorld����һ�η�������
        UIEntry.DebugLog($"OnHotFixLoaded {entryClass} {entryMethod}");
        appdomain.Invoke(entryClass, entryMethod, null, dConfigContents);
    }
}
