using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Phil.FLUI {

[System.Serializable]
public class AspectBuffer<T> {
    public T data;
    public readonly Func<T> Get;
    public readonly Action<T> Set;

    public AspectBuffer(){
        this.data = default(T);
        this.Get = () => this.data;
        this.Set = (val)=> this.data = val;
    }

    public AspectBuffer(T value){
        this.data = value;
        this.Get = () => this.data;
        this.Set = (val)=> this.data = val;
    }
}

}