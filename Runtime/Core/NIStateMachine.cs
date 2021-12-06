using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil;

namespace Phil.FLUI {

// Non-interactive State Machine
[System.Serializable]
public struct NIStateMachine {

    public interface IChangeStateCallback {
        void DidChangeState(NIStateMachine nism);
    }

    public float priorStateTimer;
    public NonInteractiveState? priorState;
    public float currentStateTimer;
    public NonInteractiveState? currentState;

    public NIStateMachine(NonInteractiveState? initialState){
        this.priorState = initialState;
        this.currentState = initialState;
        this.priorStateTimer = 0f;
        this.currentStateTimer = 0f;
    }

    public void Reset(NonInteractiveState? resetState){
        this.priorState = resetState;
        this.currentState = resetState;
        this.priorStateTimer = 0f;
        this.currentStateTimer = 0f;
    }

    public NonInteractiveState? ChangeState(NonInteractiveState? newState){
        priorStateTimer = currentStateTimer;
        priorState = currentState;
        currentStateTimer = 0f;
        currentState = newState;
        return newState;
    }

    public NonInteractiveState? ChangeState(NonInteractiveState? newState, IChangeStateCallback callbackUser){
        this.ChangeState(newState);
        callbackUser?.DidChangeState(this);
        return newState;
    }

    public NonInteractiveState? UpdateState(float deltaTime, INIStatePeriod iStatePeriod){
        return UpdateState(deltaTime, iStatePeriod, null);
    }

    public NonInteractiveState? UpdateState(float deltaTime, INIStatePeriod iStatePeriod, IChangeStateCallback callbackUser){
        
        priorStateTimer += deltaTime;
        currentStateTimer += deltaTime;

        if(currentState.TryGetValue(out var iState) == false){
            return currentState;
        }

        float statePeriod = iStatePeriod.GetStatePeriod(iState);
        bool exceededPeriod = currentStateTimer > statePeriod;

        switch(currentState){
        case NonInteractiveState.Recede: {
            if(exceededPeriod){
                return ChangeState(null, callbackUser);
            }
        } break;

        case NonInteractiveState.Rollout: {
            if(exceededPeriod){
                return ChangeState(NonInteractiveState.Idle, callbackUser);
            }
        } break;
        
        }
        return currentState;
    }
}

}