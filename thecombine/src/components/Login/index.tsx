import Login from "./LoginComponent";
//import * as actions from "./LoginActions";
import { StoreState } from "../../types";

import { connect } from "react-redux";
import { ThunkDispatch } from "redux-thunk";

export default connect()(Login);
