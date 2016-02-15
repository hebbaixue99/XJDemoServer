using System;
using System.Runtime.CompilerServices;

public delegate void CallBack();
public delegate void CallBack<T>(T arg1);
public delegate void CallBack<T, U>(T arg1, U arg2);
public delegate void CallBack<T, U, V>(T arg1, U arg2, V arg3);
public delegate void CallBack<T, U, V, R>(T arg1, U arg2, V arg3, R arg4);

