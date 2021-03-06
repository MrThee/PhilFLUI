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
    public InteractiveState? priorState {
        get => m_priorStateValid ? m_priorState : (InteractiveState?)null;
        set {
            m_priorStateValid = value.HasValue;
            if(value.HasValue){
                m_priorState = value.Value;
            }
        }
    }
    public float currentStateTimer;
    public InteractiveState? currentState {
        get => m_currentStateValid ? m_currentState : (InteractiveState?)null;
        set {
            m_currentStateValid = value.HasValue;
            if(value.HasValue){
                m_currentState = value.Value;
            }
        }
    }
    private bool m_currentStateValid;
    private InteractiveState m_currentState;
    private bool m_priorStateValid;
    private InteractiveState m_priorState;
    public readonly InteractiveState? postConfirmedState;

    public InteractiveStateMachine(InteractiveState? initialState, InteractiveState? postConfirmedState){
        this.m_priorStateValid = initialState.HasValue;
        this.m_priorState = initialState.HasValue ? initialState.Value : InteractiveState.Rollout;
        this.priorStateTimer = 0f;
        this.m_currentStateValid = initialState.HasValue;
        this.m_currentState = initialState.HasValue ? initialState.Value : InteractiveState.Rollout;
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