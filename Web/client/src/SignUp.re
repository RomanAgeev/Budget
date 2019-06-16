[@react.component]
let make = () => {
    open ReactEvent.Form;

    let (username, setUsername) = React.useState(() => "");
    let (password, setPassword) = React.useState(() => "");
    let (passwordRepeat, setPasswordRepeat) = React.useState(() => "");

    let onUsenameChange = event => target(event)##value -> setUsername;
    let onPasswordChange = event => target(event)##value -> setPassword;
    let onPasswordRepeatChange = event => target(event)##value -> setPasswordRepeat;

    <div className="row">
        <form className="col s12">
            <div className="row">
                <div className="input-field col s12">
                    <input id="username" type_="text" value={ username } onChange={ onUsenameChange } />
                    <label htmlFor="username">
                        { ReasonReact.string("Username") }
                    </label>
                </div>
            </div>
            <div className="row">
                <div className="input-field col s12">
                    <input id="password" type_="password" value={ password } onChange={ onPasswordChange } />
                    <label htmlFor="password">
                        { ReasonReact.string("Password") }
                    </label>
                </div>
            </div>
            <div className="row">
                <div className="input-field col s12">
                    <input id="password_repeat" type_="password" value={ passwordRepeat } onChange={ onPasswordRepeatChange }/>
                    <label htmlFor="password_repeat">
                        { ReasonReact.string("Repeat Password") }
                    </label>
                </div>
            </div>
        </form>
        { ReasonReact.string(username) }
        <br/>
        { ReasonReact.string(password) }
        <br/>
        { ReasonReact.string(passwordRepeat) }
    </div>;
};
