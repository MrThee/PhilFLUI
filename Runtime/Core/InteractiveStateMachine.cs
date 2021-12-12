using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil;

namespace Phil.FLUI {

[System.Serializable]
public struct InteractiveStateMachine {

    public interface IChangeStateCallback {
        void DidChangeState(ref InteractiveStateMachine ism);
    }

    public float priorStateTimer;
    public InteractiveState? priorState;
    public float currentStateTimer;
    public InteractiveState? currentState;
    public readonly InteractiveState? postConfirmedState;

    public InteractiveStateMachine(InteractiveState? initialState, InteractiveState? postConfirmedState){
        this.priorState = initialState;
        this.currentState = initialState;
        this.priorStateTimer = 0f;
        this.currentStateTimer = 0f;
        this.postConfirmedState = postConfirmedState;
    }

    public void Reset(InteractiveState? resetState){
        this.priorState = resetState;
        this.currentState = resetState;
        this.priorStateTimer = 0f;
        this.currentStateTimer = 0f;
    }

    public void ChangeState(InteractiveState? newState){
        priorStateTimer = currentStateTimer;
        priorState = currentState;
        currentStateTimer = 0f;
        currentState = newState;
        // Debug.LogFormat("new state: {0}", newState);
    }

    public InteractiveState? ChangeState(InteractiveState? newState, IChangeStateCallback callbackUser){
        this.ChangeState(newState);
        callbackUser?.DidChangeState(ref this);
        return this.currentState;
    }

    // TODO: just pass a currentPeriod in.
    public InteractiveState? UpdateState(float deltaTime, IInteractiveStatePeriod iStatePeriod){
        return UpdateState(deltaTime, iStatePeriod, null);
    }

    public InteractiveState? UpdateState(float deltaTime, IInteractiveStatePeriod iStatePeriod, IChangeStateCallback callbackUser){

        priorStateTimer += deltaTime;
        currentStateTimer += deltaTime;

        if(currentState.TryGetValue(out var iState) == false){
            return currentState;
        }

        float statePeriod = iStatePeriod.GetStatePeriod(iState);
        bool exceededPeriod = currentStateTimer > statePeriod;
        
        switch(currentState){
        case InteractiveState.Recede: {
            if(exceededPeriod){
                ChangeState(null, callbackUser);
            }
        } break;

        case InteractiveState.Confirmed: {
            if(exceededPeriod){
                ChangeState(postConfirmedState, callbackUser);
            }
        } break;

        case InteractiveState.Rollout: {
            if(exceededPeriod){
                ChangeState(InteractiveState.Idle, callbackUser);
            }
        } break;
        
        }
        return currentState;
    }
}

}