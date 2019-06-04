import { Dispatch } from "react";
import { string } from "prop-types";

export const LOGIN = "LOGIN";
export type LOGIN = typeof LOGIN;

export const REGISTER = "REGISTER";
export type REGISTER = typeof REGISTER;

export interface LoginData {
  user: string;
  password: string;
}

//action types
export interface Login {
  type: LOGIN;
  payload: LoginData;
}

export interface Register {
  type: REGISTER;
  payload: LoginData;
}

//thunk action creator
export function asyncLogin(user: string, password: string) {
  return async (dispatch: Dispatch<Login>) => {
    //console.log('asyncPressButton called');
    dispatch(login(user, password));
  };
}

//pure action creator. LEAVE PURE!
export function login(user: string, password: string): Login {
  //console.log('PressButton called');
  return {
    type: LOGIN,
    payload: { user, password }
  };
}

//thunk action creator
export function asyncRegister(user: string, password: string) {
  return async (dispatch: Dispatch<Register>) => {
    //console.log('asyncPressButton called');
    dispatch(register(user, password));
  };
}

//pure action creator. LEAVE PURE!
export function register(user: string, password: string): Register {
  //console.log('PressButton called');
  return {
    type: REGISTER,
    payload: { user, password }
  };
}

export type LoginAction = Login | Register; // | OtherAction
