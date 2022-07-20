using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class ILRuntimeHelper
{
    static ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    public static void LoadDll(byte[] dll, byte[] pdb, string entryClass, string entryMethod)
    {
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain(ILRuntime.Runtime.ILRuntimeJITFlags.JITOnDemand);
        appdomain.LoadAssembly(new MemoryStream(dll), pdb == null ? null : new MemoryStream(pdb), new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
        InitializeILRuntime();
        OnHotFixLoaded(entryClass, entryMethod);
    }
    static void InitializeILRuntime()
    {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
        //����Unity��Profiler�ӿ�ֻ���������߳�ʹ�ã�Ϊ�˱�����쳣����Ҫ����ILRuntime���̵߳��߳�ID������ȷ���������к�ʱ�����Profiler
        appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
        //������һЩILRuntime��ע�ᣬHelloWorldʾ����ʱû����Ҫע���
    }
    static void OnHotFixLoaded(string entryClass, string entryMethod)
    {
        //HelloWorld����һ�η�������
        UIEntry.DebugLog($"OnHotFixLoaded {entryClass} {entryMethod}");
        appdomain.Invoke(entryClass, entryMethod, null, null);
    }
}
