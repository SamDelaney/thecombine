//external modules
import * as React from "react";
import {
  Translate,
  LocalizeContextProps,
  withLocalize
} from "react-localize-redux";
import Button from "@material-ui/core/Button";
import TextField from "@material-ui/core/TextField";
import Input from "@material-ui/core/Input";
import { Grid } from "@material-ui/core";
import { asyncLogin, asyncRegister } from "./LoginActions";

export interface LoginProps {
  login?: (user: string, password: string) => void;
  register?: (user: string, password: string) => void;
}

interface LoginState {
  user: string;
  password: string;
}

class Login extends React.Component<
  LoginProps & LocalizeContextProps,
  LoginState
> {
  constructor(props: LoginProps & LocalizeContextProps) {
    super(props);
    this.state = { user: "", password: "" };
  }

  updateUser(
    evt: React.ChangeEvent<
      HTMLTextAreaElement | HTMLInputElement | HTMLSelectElement
    >
  ) {
    const user = evt.target.value;
    const password = this.state.password;
    this.setState({ user, password });
  }

  updatePassword(
    evt: React.ChangeEvent<
      HTMLTextAreaElement | HTMLInputElement | HTMLSelectElement
    >
  ) {
    const password = evt.target.value;
    const user = this.state.user;
    this.setState({ password, user });
  }

  login(e: React.FormEvent<EventTarget>) {
    e.preventDefault();
    if (this.props.login) {
      this.props.login(this.state.user, this.state.password);
    }
  }

  register() {
    if (this.props.register) {
      this.props.register(this.state.user, this.state.password);
    }
  }

  render() {
    //visual definition
    return (
      <Grid container justify="center">
        <form onSubmit={evt => this.login(evt)}>
          <TextField
            label={<Translate id="login.username" />}
            value={this.state.user}
            onChange={evt => this.updateUser(evt)}
          />
          <br />
          <TextField
            label={<Translate id="login.password" />}
            type="password"
            value={this.state.password}
            onChange={evt => this.updatePassword(evt)}
          />
          <br />
          <Button onClick={_ => this.register()}>
            <Translate id="login.register" />
          </Button>
          <Button type="submit">
            <Translate id="login.login" />
          </Button>
        </form>
      </Grid>
    );
  }
}

export default withLocalize(Login);