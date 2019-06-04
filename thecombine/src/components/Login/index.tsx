import Login from "./LoginComponent";
//import * as actions from "./LoginActions";
import { StoreState } from "../../types";

import { connect } from "react-redux";
import { ThunkDispatch } from "redux-thunk";
import { LoginAction, login, register } from "./LoginActions";

export function mapDispatchToProps(
  dispatch: ThunkDispatch<StoreState, any, LoginAction>
) {
  return {
    login: (user: string, password: string) => {
      //console.log('clicked test!');
      dispatch(login(user, password));
    },
    register: (user: string, password: string) => {
      //console.log('clicked test!');
      dispatch(register(user, password));
    }
  };
}

export default connect(
  null,
  mapDispatchToProps
)(Login);
