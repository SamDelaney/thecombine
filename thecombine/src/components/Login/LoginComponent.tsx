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

export interface LoginProps {}

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
    console.log("TODO: Implement login");
  }

  register() {
    console.log("TODO: Implement register");
  }

  render() {
    //visual definition
    return (
      <Grid container justify="center">
        <form onSubmit={evt => this.login(evt)}>
          <TextField
            label={"Username"}
            value={this.state.user}
            onChange={evt => this.updateUser(evt)}
          />
          <br />
          <TextField
            label={"Password"}
            type="password"
            value={this.state.password}
            onChange={evt => this.updatePassword(evt)}
          />
          <br />
          <Button onSubmit={_ => this.register()}> Register </Button>
          <Button type="submit">Login</Button>
        </form>
      </Grid>
    );
  }
}

export default withLocalize(Login);
